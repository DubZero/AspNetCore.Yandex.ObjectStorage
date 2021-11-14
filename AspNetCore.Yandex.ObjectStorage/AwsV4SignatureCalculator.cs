using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using AspNetCore.Yandex.ObjectStorage.Helpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace AspNetCore.Yandex.ObjectStorage
{
	public class AwsV4SignatureCalculator
	{
		private const string Iso8601DateTimeFormat = "yyyyMMddTHHmmssZ";
		private const string Iso8601DateFormat = "yyyyMMdd";

		private readonly string _awsSecretKey;
		private readonly string _service;
		private readonly string _region;

		public AwsV4SignatureCalculator(string awsSecretKey, string service = null, string region = null)
		{
			_awsSecretKey = awsSecretKey;
			_service = service ?? "s3";
			_region = region ?? YandexStorageDefaults.Location;
		}

		/// <summary>
		/// Calculates request signature string using Signature Version 4.
		/// http://docs.aws.amazon.com/general/latest/gr/sigv4_signing.html
		/// </summary>
		/// <param name="request">Request</param>
		/// <param name="signedHeaders">Canonical headers that are a part of a signing process</param>
		/// <param name="requestDate">Date and time when request takes place</param>
		/// <returns>Signature</returns>
		public string CalculateSignature(HttpRequestMessage request, string[] signedHeaders, DateTime requestDate)
		{
			signedHeaders = signedHeaders.Select(x => x.Trim().ToLowerInvariant()).OrderBy(x => x).ToArray();

			var canonicalRequest = GetCanonicalRequest(request, signedHeaders);
			var stringToSign = GetStringToSign(requestDate, canonicalRequest);
			return GetSignature(requestDate, stringToSign);
		}

		public static string GetPayloadHash(HttpRequestMessage request)
		{
			if (request.Content is ByteArrayContent || request.Content is MultipartContent)
			{
				var bytes = request.Content.ReadAsByteArrayAsync().Result;
				return Utils.ToHex(HashHelper.GetSha256(bytes));
			}

			var payload = request.Content != null ? request.Content.ReadAsStringAsync().Result : "";
			return Utils.ToHex(HashHelper.GetSha256(payload));
		}

		/// <summary>
		/// http://docs.aws.amazon.com/general/latest/gr/sigv4-create-canonical-request.html
		/// </summary>
		/// <param name="request"></param>
		/// <param name="signedHeaders"></param>
		/// <returns></returns>
		private static string GetCanonicalRequest(HttpRequestMessage request, string[] signedHeaders)
		{
			var canonicalRequest = new StringBuilder();
			canonicalRequest.AppendFormat("{0}\n", request.Method.Method);
			canonicalRequest.AppendFormat("{0}\n", request.RequestUri.AbsolutePath);
			canonicalRequest.AppendFormat("{0}\n", GetCanonicalQueryParameters(QueryHelpers.ParseQuery(request.RequestUri.Query)));
			canonicalRequest.AppendFormat("{0}\n", GetCanonicalHeaders(request, signedHeaders));
			canonicalRequest.AppendFormat("{0}\n", string.Join(";", signedHeaders));
			canonicalRequest.Append(GetPayloadHash(request));
			return canonicalRequest.ToString();
		}

		private static string GetCanonicalQueryParameters(Dictionary<string, StringValues> queryParameters)
		{
			var canonicalQueryParameters = new StringBuilder();
			foreach (var key in queryParameters.Keys)
			{
				canonicalQueryParameters.AppendFormat("{0}={1}&", Utils.UrlEncode(key),
													  Utils.UrlEncode(queryParameters[key]));
			}

			// remove trailing '&'
			if (canonicalQueryParameters.Length > 0)
				canonicalQueryParameters.Remove(canonicalQueryParameters.Length - 1, 1);

			return canonicalQueryParameters.ToString();
		}

		private static string GetCanonicalHeaders(HttpRequestMessage request, IEnumerable<string> signedHeaders)
		{
			var headers = request.Headers.ToDictionary(x => x.Key.Trim().ToLowerInvariant(),
													   x => string.Join(" ", x.Value).Trim());

			if (request.Content != null)
			{
				var contentHeaders = request.Content.Headers.ToDictionary(x => x.Key.Trim().ToLowerInvariant(),
																		  x => string.Join(" ", x.Value).Trim());
				foreach (var contentHeader in contentHeaders)
				{
					headers.Add(contentHeader.Key, contentHeader.Value);
				}
			}

			var sortedHeaders = new SortedDictionary<string, string>(headers);

			var canonicalHeaders = new StringBuilder();
			foreach (var header in sortedHeaders.Where(header => signedHeaders.Contains(header.Key)))
			{
				canonicalHeaders.AppendFormat("{0}:{1}\n", header.Key, header.Value);
			}
			return canonicalHeaders.ToString();
		}

		/// <summary>
		/// http://docs.aws.amazon.com/general/latest/gr/sigv4-create-string-to-sign.html
		/// </summary>
		/// <param name="requestDate"></param>
		/// <param name="canonicalRequest"></param>
		/// <returns></returns>
		private string GetStringToSign(DateTime requestDate, string canonicalRequest)
		{
			var dateStamp = requestDate.ToString(Iso8601DateFormat, CultureInfo.InvariantCulture);
			var scope = $"{dateStamp}/{_region}/{_service}/aws4_request";

			var stringToSign = new StringBuilder();
			stringToSign.AppendFormat("AWS4-HMAC-SHA256\n{0}\n{1}\n",
									  requestDate.ToString(Iso8601DateTimeFormat, CultureInfo.InvariantCulture),
									  scope);
			stringToSign.Append(Utils.ToHex(HashHelper.GetSha256(canonicalRequest)));
			return stringToSign.ToString();
		}

		/// <summary>
		/// http://docs.aws.amazon.com/general/latest/gr/sigv4-calculate-signature.html
		/// </summary>
		/// <param name="requestDate"></param>
		/// <param name="stringToSign"></param>
		/// <returns></returns>
		private string GetSignature(DateTime requestDate, string stringToSign)
		{
			var kSigning = GetSigningKey(requestDate);
			return Utils.ToHex(HashHelper.GetKeyedHash(kSigning, stringToSign));
		}

		private byte[] GetSigningKey(DateTime requestDate)
		{
			var dateStamp = requestDate.ToString(Iso8601DateFormat, CultureInfo.InvariantCulture);
			var kDate = HashHelper.GetKeyedHash("AWS4" + _awsSecretKey, dateStamp);
			var kRegion = HashHelper.GetKeyedHash(kDate, _region);
			var kService = HashHelper.GetKeyedHash(kRegion, _service);
			return HashHelper.GetKeyedHash(kService, "aws4_request");
		}

		private static class Utils
		{
			private const string ValidUrlCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

			public static string UrlEncode(string data)
			{
				var encoded = new StringBuilder();
				foreach (char symbol in Encoding.UTF8.GetBytes(data))
				{
					if (ValidUrlCharacters.IndexOf(symbol) != -1)
					{
						encoded.Append(symbol);
					}
					else
					{
						encoded.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)symbol));
					}
				}
				return encoded.ToString();
			}



			public static string ToHex(byte[] data)
			{
				var sb = new StringBuilder();
				for (var i = 0; i < data.Length; i++)
				{
					sb.Append(data[i].ToString("x2", CultureInfo.InvariantCulture));
				}
				return sb.ToString();
			}
		}
	}
}