using System.Security.Cryptography;
using System.Text;

namespace AspNetCore.Yandex.ObjectStorage.Helpers
{
    internal class HashHelper
    {
        internal static byte[] GetSha256(string value)
        {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value));
        }

        internal static byte[] GetSha256(byte[] value)
        {
            return SHA256.Create().ComputeHash(value);
        }

        internal static byte[] GetKeyedHash(string key, string value)
        {
            return GetKeyedHash(Encoding.UTF8.GetBytes(key), value);
        }

        internal static byte[] GetKeyedHash(byte[] key, string value)
        {
            KeyedHashAlgorithm mac = new HMACSHA256(key);
            mac.Initialize();
            return mac.ComputeHash(Encoding.UTF8.GetBytes(value));
        }
    }
}