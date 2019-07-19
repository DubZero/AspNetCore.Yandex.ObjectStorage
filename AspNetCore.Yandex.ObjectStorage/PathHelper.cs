using System;
using System.Text.RegularExpressions;

namespace AspNetCore.Yandex.ObjectStorage
{
    public static class PathHelper
    {
        public static string RemoveProtocol(this string path, string protocol)
        {
            return path.Replace($"{protocol}://", "");
        }

        public static string RemoveEndPoint(this string path, string endpoint)
        {
            return path.Replace($"{endpoint}/", "");
        }

        public static string RemoveBucket(this string path, string bucket)
        {
            var regex = new Regex(Regex.Escape($"{bucket}/"));
            return regex.Replace(path, $"",1);
        }

        public static int IndexOfFile(this string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            var formatIndex = path.LastIndexOf('.');

            if (formatIndex > 0) // Then file exist in path
            {
                return path.Contains('/') ? path.LastIndexOf('/') : 0;
            }

            return -1;
        }

        public static bool IsFileWithPath(this string path)
        {
            return IndexOfFile(path) > 0;
        }
    }
}