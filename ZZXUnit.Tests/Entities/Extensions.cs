using System;
using System.Security.Cryptography;
using System.Text;
using BaseUnits.Core.Helpers;

namespace ZZXUnit.Tests.Entities
{
    static class Extensions
    {
        public static long CreateFastHashCode(params object[] texts)
        {
            return GenerateHashCode(texts);
        }
        
        private static long GenerateHashCode(params object[] texts)
        {
            var text = string.Join(":", texts);
            return Get128HashCode(text);
        }

        /// <summary>
        /// Normal Md5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string Md5(string source)
        {
            var hash = CreateMd5(source);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static string Sha1(string source)
        {
            var hash = CreateSha1(source);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private static string Hash(string md5, string sha1)
        {
            return string.Format("{0}{1}", md5, sha1.Substring(16, 8));
        }

        public static string CreateUniqueHash(params object[] texts)
        {
            var text = string.Join(":", texts);
            var md5 = Md5(text);
            var sha1 = Sha1(text);
            return Hash(md5, sha1);
        }

        public static Int64 Get256HashCode(string text)
        {
            // https://www.codeproject.com/Articles/34309/Convert-String-to-64bit-Integer
            if (!string.IsNullOrEmpty(text))
            {
                // sha256 length = 32; md5 length = 16;
                var s256 = CreateSha256(text);
                var md5 = CreateMd5(text);

                // target length = 40
                var bytes = new byte[40];
                s256.CopyTo(bytes, 0);
                Array.Copy(md5, 4, bytes, 32, 8);

                var lefta = BitConverter.ToInt64(bytes, 0);
                var leftb = BitConverter.ToInt64(bytes, 8);
                var righta = BitConverter.ToInt64(bytes, 24);
                var rightb = BitConverter.ToInt64(bytes, 32);
                var code = lefta ^ leftb ^ righta ^ rightb;
                return code;
            }

            return 0;
        }

        public static Int64 Get128HashCode(string text)
        {
            // https://www.codeproject.com/Articles/34309/Convert-String-to-64bit-Integer
            if (!string.IsNullOrEmpty(text))
            {
                // sha1 length = 20; md5 length = 16;
                var sha1 = CreateSha1(text);
                var md5 = CreateMd5(text);

                var bytes = new byte[32];
                md5.CopyTo(bytes, 0);
                Array.Copy(sha1, 2, bytes, 16, 16);

                var start = BitConverter.ToInt64(bytes, 0);
                var middle = BitConverter.ToInt64(bytes, 8);
                var end = BitConverter.ToInt64(bytes, 24);
                var code = start ^ middle ^ end;
                return code;
            }

            return 0;
        }

        private static byte[] CreateSha256(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            using (var hash = new SHA256CryptoServiceProvider())
            {
                var bts = hash.ComputeHash(bytes);
                return bts;
            }
        }

        private static byte[] CreateMd5(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            using (var md5 = new MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(bytes);
                return hash;
            }
        }

        private static byte[] CreateSha1(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var sha1 = new SHA1CryptoServiceProvider();
            var hash = sha1.ComputeHash(bytes);
            return hash;
        }





        public static string ToJsonString(this object data)
        {
            return StaticHelper.ToJson(data);
        }


        public static byte[] ToBinaryBytes(this object data)
        {
            return BinaryHelper.ToBytes(data);
        }

        public static T FromBinaryBytes<T>(this byte[] bytes)
        {
            return BinaryHelper.FromBytes<T>(bytes);
        }

    }
}
