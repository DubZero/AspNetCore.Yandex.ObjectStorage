using System;
using System.IO;
using System.Net.Http;
using AspNetCore.Yandex.ObjectStorage.Configuration;
using Xunit;

namespace AspNetCore.Yandex.ObjectStorage.UnitTest
{
    public class PayloadHashTest
    {
        [Fact]
        public void TestHash()
        {
            HttpRequestMessage requestMessage = new(HttpMethod.Put, new Uri($"{YandexStorageDefaults.Protocol}://{YandexStorageDefaults.EndPoint}/test"));

            using (var ms = new MemoryStream())
            {
                TextWriter tw = new StreamWriter(ms);
                tw.Write("test file");
                tw.Flush();
                ms.Position = 0;

                var content = new ByteArrayContent(ms.ToArray());
                requestMessage.Content = content;
            }

            var result = AwsV4SignatureCalculator.GetPayloadHash(requestMessage);

            Assert.Equal("9a30a503b2862c51c3c5acd7fbce2f1f784cf4658ccf8e87d5023a90c21c0714", result);
        }
    }
}