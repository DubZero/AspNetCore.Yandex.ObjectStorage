using System;

using AspNetCore.Yandex.ObjectStorage.Helpers;

using Xunit;

namespace AspNetCore.Yandex.ObjectStorage.UnitTest
{
    public class PathHelperTests
    {
        private const string SimpleUrl = "https://storage.yandexcloud.net/scouting/23D919258BF03F640BB745274F41A1BF.pdf";
        private const string WithPathUrl = "https://storage.yandexcloud.net/scouting/path1/23D919258BF03F640BB745274F41A1BF.pdf";
        private const string HardWithPathUrl = "https://storage.yandexcloud.net/scouting/scouting/23D919258BF03F640BB745274F41A1BF.pdf";

        [Fact]
        public void TestIndexSearch()
        {
            var url = SimpleUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
                .RemoveBucket("scouting");

            var index1 = url.IndexOfFile();
            Assert.Equal(0, index1);
        }

        [Fact]
        public void TestMultipleBucketNames()
        {
            var url = HardWithPathUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
                .RemoveBucket("scouting");

            var index1 = url.IndexOfFile();
            Assert.Equal("scouting".Length, index1);
        }

        [Fact]
        public void TestUrlWithPath()
        {
            var url = WithPathUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
                .RemoveBucket("scouting");

            var index1 = url.IndexOfFile();
            Assert.Equal("path1".Length, index1);
        }

        [Fact]
        public void TestPathExist()
        {
            var trueUrl = WithPathUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
                .RemoveBucket("scouting");
            var falseUrl = SimpleUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
                .RemoveBucket("scouting");

            Assert.True(trueUrl.IsFileWithPath());
            Assert.False(falseUrl.IsFileWithPath());
        }

        [Fact]
        public void EmptyTest()
        {
            Assert.Throws<ArgumentNullException>(() => "".IndexOfFile());
        }
    }
}