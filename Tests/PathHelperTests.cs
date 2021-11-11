using System;

using AspNetCore.Yandex.ObjectStorage;
using AspNetCore.Yandex.ObjectStorage.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class PathHelperTests
	{
		public const string SimpleUrl = "https://storage.yandexcloud.net/scouting/23D919258BF03F640BB745274F41A1BF.pdf";
		public const string WithPathUrl = "https://storage.yandexcloud.net/scouting/path1/23D919258BF03F640BB745274F41A1BF.pdf";
		public const string HardWithPathUrl = "https://storage.yandexcloud.net/scouting/scouting/23D919258BF03F640BB745274F41A1BF.pdf";

		[TestMethod]
		public void TestIndexSearch()
		{
			var url = SimpleUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
				.RemoveBucket("scouting");

			var index1 = url.IndexOfFile();
			Assert.AreEqual(0, index1, "Wrong index of File (SimpleUrl)");
		}

		[TestMethod]
		public void TestMultipleBucketNames()
		{
			var url = HardWithPathUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
				.RemoveBucket("scouting");

			var index1 = url.IndexOfFile();
			Assert.AreEqual("scouting".Length, index1, "Wrong index of File (HardWithPathUrl)");
		}

		[TestMethod]
		public void TestUrlWithPath()
		{
			var url = WithPathUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
				.RemoveBucket("scouting");

			var index1 = url.IndexOfFile();
			Assert.AreEqual("path1".Length, index1, "Wrong index of File (HardWithPathUrl)");
		}

		[TestMethod]
		public void TestPathExist()
		{
			var trueUrl = WithPathUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
				.RemoveBucket("scouting");
			var falseUrl = SimpleUrl.RemoveProtocol("https").RemoveEndPoint("storage.yandexcloud.net")
				.RemoveBucket("scouting");

			Assert.IsTrue(trueUrl.IsFileWithPath());
			Assert.IsFalse(falseUrl.IsFileWithPath());
		}

		[TestMethod]
		public void EmptyTest()
		{
			Assert.ThrowsException<ArgumentNullException>(() => "".IndexOfFile());
		}
	}
}