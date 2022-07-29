using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Yandex.ObjectStorage.Models;
using AspNetCore.Yandex.ObjectStorage.Object.Models;
using Bogus;
using Xunit;

namespace AspNetCore.Yandex.ObjectStorage.IntegrationTests
{
    public class ObjectServiceTests
    {
        private readonly Faker _faker;
        private readonly IYandexStorageService _yandexStorageService;
        private readonly IYandexStorageService _anotherLocationService;

        public ObjectServiceTests()
        {
            _faker = new Faker("en");

            _yandexStorageService = new YandexStorageService(EnvironmentOptions.GetFromEnvironment());
            _anotherLocationService = new YandexStorageService(EnvironmentOptions.GetFromEnvironmentWithNotDefaultLocation());
        }

        [Fact(DisplayName = "[001] PutObjectAsync - put object as byte array")]
        public async Task PutObject_AsByteArray_Success()
        {
            var fakeObject = _faker.Random.Bytes(100);

            var filename = _faker.Random.String2(15);

            var result = await _yandexStorageService.ObjectService.PutAsync(fakeObject, filename);

            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var getResult = await GetObjectAsync(filename);

            Assert.Equal(fakeObject, getResult);

            await ClearUploadedAsync(filename);
        }

        [Fact(DisplayName = "[002] PutObjectAsync - put object as stream")]
        public async Task PutObject_AsStream_Success()
        {
            var fakeObject = _faker.Random.Bytes(100);
            var filename = _faker.Random.String2(15);

            await using (var ms = new MemoryStream(fakeObject))
            {
                ms.Position = 0;
                _ = await _yandexStorageService.ObjectService.PutAsync(ms, filename);
            }

            var getResult = await _yandexStorageService.ObjectService.GetAsByteArrayAsync(filename);

            Assert.Equal(fakeObject, getResult);

            await ClearUploadedAsync(filename);
        }

        [Fact(DisplayName = "[003] GetObjectAsync - GetAsByteArray testing")]
        public async Task GetObject_GetAsByteArray_Success()
        {
            var fakeObject = _faker.Random.Bytes(100);
            var filename = _faker.Random.String2(15);

            await UploadObjectAsync(fakeObject, filename);

            var getResult = await GetObjectAsync(filename);

            Assert.Equal(fakeObject, getResult);

            await ClearUploadedAsync(filename);
        }

        [Fact(DisplayName = "[004] GetObjectAsync - GetAsStream testing")]
        public async Task GetObject_GetAsStream_Success()
        {
            const int size = 100;

            var fakeObject = _faker.Random.Bytes(size);
            var filename = _faker.Random.String2(15);

            await UploadObjectAsync(fakeObject, filename);

            var streamResult = await _yandexStorageService.ObjectService.GetAsStreamAsync(filename);

            Assert.True(streamResult.IsSuccess);

            byte[] byteArr;
            await using (MemoryStream ms = new())
            {
                await streamResult.Value.CopyToAsync(ms);
                byteArr = ms.ToArray();
            }

            Assert.Equal(fakeObject, byteArr);

            await ClearUploadedAsync(filename);
        }

        [Fact(DisplayName = "[005] DeleteObjectAsync - DeleteObject testing")]
        public async Task DeleteObject_Success()
        {
            const int size = 100;

            var fakeObject = _faker.Random.Bytes(size);
            var filename = _faker.Random.String2(15);

            await UploadObjectAsync(fakeObject, filename);

            var getResult = await GetObjectAsync(filename);

            Assert.NotEmpty(getResult);

            await _yandexStorageService.ObjectService.DeleteAsync(filename);

            var getResultAfterDelete = await TryGetObjectAsync(filename);

            Assert.False(getResultAfterDelete.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResultAfterDelete.StatusCode);
        }

        [Fact(DisplayName = "[006] PutObjectAsync - put big object as stream")]
        public async Task PutObject_BigObjectAsStream_Success()
        {
            var fakeObject = _faker.Random.Bytes(3_000_000);
            var filename = _faker.Random.String2(15);

            await using (var ms = new MemoryStream(fakeObject))
            {
                ms.Position = 0;
                _ = await _yandexStorageService.ObjectService.PutAsync(ms, filename);
            }

            var getResult = await _yandexStorageService.ObjectService.GetAsByteArrayAsync(filename);

            Assert.Equal(fakeObject, getResult);

            await ClearUploadedAsync(filename);
        }

        [Fact(DisplayName = "[007] PutObjectAsync - not default location")]
        public async Task PutObject_NotDefaultLocation_Success()
        {
            var fakeObject = _faker.Random.Bytes(100);

            var filename = _faker.Random.String2(15);

            var result = await _anotherLocationService.ObjectService.PutAsync(fakeObject, filename);

            Assert.True(result.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);

            var getResult = await _anotherLocationService.ObjectService.GetAsByteArrayAsync(filename);

            Assert.Equal(fakeObject, getResult);

            await ClearUploadedAsync(filename);
        }

        #region Private

        private async Task<S3ObjectGetResponse> TryGetObjectAsync(string filename)
        {
            return await _yandexStorageService.TryGetAsync(filename);
        }

        private async Task<byte[]> GetObjectAsync(string filename)
        {
            return (await _yandexStorageService.ObjectService.GetAsByteArrayAsync(filename)).Value;
        }

        private async Task ClearUploadedAsync(params string[] filenames)
        {
            var deleteTasks = filenames.Select(async filename =>
            {
                await _yandexStorageService.ObjectService.DeleteAsync(filename);
            });

            await Task.WhenAll(deleteTasks);
        }

        private async Task UploadObjectAsync(byte[] file, string filename)
        {
            await _yandexStorageService.ObjectService.PutAsync(file, filename);
        }

        #endregion
    }
}