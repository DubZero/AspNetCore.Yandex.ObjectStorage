using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Builders;
using AspNetCore.Yandex.ObjectStorage.Builders.ObjectRequestBuilders;
using AspNetCore.Yandex.ObjectStorage.Multipart;

using Microsoft.Extensions.Options;

namespace AspNetCore.Yandex.ObjectStorage
{
	public class YandexStorageService
	{
		private YandexStorageOptions _options;

		public YandexStorageService(IOptions<YandexStorageOptions> options)
		{
			_options = options.Value;
		}

		public YandexStorageService(YandexStorageOptions options)
		{
			_options = options;
		}

		/// <summary>
		/// Test connection to storage
		/// </summary>
		/// <returns>Retruns true if all credentials correct</returns>
		public async Task<S3GetResponse> TryGetAsync()
		{
			var builder = new ObjectGetRequestBuilder(_options);

			var requestMessage = builder.Build().GetResult();

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3GetResponse(response);
		}

		public async Task<byte[]> GetAsByteArrayAsync(string filename)
		{
			var builder = new ObjectGetRequestBuilder(_options);
			var formattedPath = FormatPath(filename);

			var requestMessage = builder.Build(formattedPath).GetResult();

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsByteArrayAsync();
			}

			throw new Exception(await response.Content.ReadAsStringAsync());
		}

		/// <summary>
		/// Return object as Stream
		/// </summary>
		/// <param name="filename">full URL or filename if it is in root folder</param>
		/// <returns>Stream</returns>
		/// <exception cref="Exception"></exception>
		public async Task<Stream> GetAsStreamAsync(string filename)
		{
			var builder = new ObjectGetRequestBuilder(_options);
			var formattedPath = FormatPath(filename);

			var requestMessage = builder.Build(formattedPath).GetResult();

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			if (response.IsSuccessStatusCode)
			{
				return await response.Content.ReadAsStreamAsync();
			}

			throw new Exception(await response.Content.ReadAsStringAsync());
		}

		public async Task<S3PutResponse> PutObjectAsync(Stream stream, string filename)
		{
			var builder = new ObjectPutRequestBuilder(_options);
			var formattedPath = FormatPath(filename);
			
			var requestMessage = builder.Build(stream, formattedPath).GetResult();

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3PutResponse(response, GetObjectUri(formattedPath));
		}

		public async Task<S3PutResponse> PutObjectAsync(byte[] byteArr, string filename)
		{
			var builder = new ObjectPutRequestBuilder(_options);
			var formattedPath = FormatPath(filename);

			var requestMessage = builder.Build(byteArr, formattedPath).GetResult();

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3PutResponse(response, GetObjectUri(formattedPath));
		}

		private string FormatPath(string path)
		{
			return path.RemoveProtocol(_options.Protocol)
				.RemoveEndPoint(_options.Endpoint)
				.RemoveBucket(_options.BucketName);
		}

		public async Task<S3DeleteResponse> DeleteObjectAsync(string filename)
		{
			var formattedPath = FormatPath(filename);
			var builder = new ObjectDeleteRequestBuilder(_options);
			var requestMessage = builder.Build(formattedPath).GetResult();

			using var client = new HttpClient();
			var response = await client.SendAsync(requestMessage);

			return new S3DeleteResponse(response);
		}

		#region Multipart
		
		/// <summary>
		/// Multipart upload for files more than 100Mb
		/// </summary>
		/// <param name="stream">Stream with file</param>
		/// <param name="filename">Filename</param>
		/// <param name="partSize">Size of 1 part in Kb, must be more than 5120 Kb</param>
		/// <returns></returns>
		private async Task<string> MutipartAsync(Stream stream, string filename, int partSize = 6000)
		{
			var startResult = await StartUpload(filename);

			throw new NotImplementedException();
		}

		/// <summary>
		/// Multipart upload for files more than 100Mb
		/// </summary>
		/// <param name="byteArr">File in byte array</param>
		/// <param name="filename">Filename</param>
		/// <param name="partSize">Size of 1 part in Kb, must be more than 5120 Kb</param>
		/// <returns></returns>
		private async Task<string> MutipartAsync(byte[] byteArr, string filename, int partSize = 6000)
		{
			// https://cloud.yandex.ru/docs/storage/s3/api-ref/multipart
			// Общий алгоритм

			// Ининциализируем начало загрузки - отправляем метаданные - получаем индефикатор загрузки

			// Разбиваем файл на части и указываем номера частей, а также присваеваем айди загрузки

			// Загружаем все части (по 1ой) асинхронно

			// Отправляем запрос на завершение загрузки

			var startResult = await StartUpload(filename);

			throw new NotImplementedException();
		}

		private async Task<bool> FileToParts(Stream stream, InitiateMultipartUploadResult startResponse, int partSize)
		{
			var part = new byte[partSize];
			var offset = 0;
			var partNumber = 0;
			while (stream.Position != stream.Length)
			{
				var position = await stream.ReadAsync(part, offset, partSize);
				offset += partSize;
				partNumber += 1;
				await UploadPart(part, partNumber, startResponse.UploadId);
				part = new byte[partSize];
			}

			return true;
		}

		private async Task<InitiateMultipartUploadResult> StartUpload(string filename)
		{
			throw new NotImplementedException();
		}

		private async Task<string> UploadPart(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> CopyPart(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> ListParts(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> AbortUpload(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> CompleteUpload(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		private async Task<string> ListUploads(byte[] filePart, int partNumber, string uploadId)
		{
			throw new NotImplementedException();
		}

		#endregion Multipart

		#region Bucket methods

		public Task<S3PutResponse> CreateBucket()
		{
			throw new NotImplementedException();
		}
		
		public Task<S3GetResponse> GetBucketMeta()
		{
			throw new NotImplementedException();
		}
		
		public Task<S3GetResponse> GetBucketListObjects()
		{
			throw new NotImplementedException();
		}
		
		public Task<S3GetResponse> GetBucketList()
		{
			throw new NotImplementedException();
		}
		
		public Task<S3DeleteResponse> DeleteBucket()
		{
			throw new NotImplementedException();
		}

		#endregion Bucket methods

		#region PRIVATE

		private string GetObjectUri(string filename)
		{
			return $"{_options.HostName}/{filename}";
		}

		#endregion PRIVATE
	}
}