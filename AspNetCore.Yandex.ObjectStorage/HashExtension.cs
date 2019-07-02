using System;
using System.Security.Cryptography;
using System.Text;

namespace AspNetCore.Yandex.ObjectStorage
{
    public static class HashExtension
    {
        public static string GetMD5(this string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public static int GetMD5Int32(this string input)
        {
            MD5 md5 = MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);
            var result = BitConverter.ToInt32(hash, 0);

            return result;
        }


        public static string GetSHA1(this string input)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha1.ComputeHash(inputBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}