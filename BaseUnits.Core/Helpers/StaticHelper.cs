using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography;

using Newtonsoft.Json;

namespace BaseUnits.Core.Helpers
{
    /// <summary>
    /// BaseStaticHelper
    /// </summary>
    public abstract class StaticHelper
    {
        #region .Properties.
        /// <summary>
        /// 北京时间 ，UTC时间+8
        /// </summary>
        public static readonly int UtcTimeZone = 8;

        /// <summary>
        /// 执行成功，提示代码：0
        /// </summary>
        public static readonly int SuccessCode = 0;
        #endregion

        /// <summary>
        /// 默认货币小数位
        /// </summary>
        public static readonly int MoneyDigit = 6;

        /// <summary>
        /// 系统默认货币
        /// </summary>
        public static readonly string Currency = "MYR";

        #region .DateTime.
        /// <summary>
        /// 默认时间 (最小)
        /// </summary>
        public static readonly DateTime DateTimeMin = new DateTime(1900, 1, 1);

        /// <summary>
        /// 默认时间 (最大)
        /// </summary>
        public static readonly DateTime DateTimeMax = new DateTime(2099, 12, 31);

        /// <summary>
        /// 当前时间
        /// </summary>
        public static DateTime NowTime
        {
            get
            {
                return DateTime.UtcNow.AddHours(8);
            }
        }

        /// <summary>
        /// 下周一中午12点以前
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static DateTime ParseNextMonday12(DateTime now)
        {
            // 从现在开始到下一个星期一的中午12点以前
            // 如果今天就是星期一且小于中午12点，应该返回今天中午12点以前
            var date = now.Date;
            var today = now.Hour < 12 ? date.AddHours(-12) : date.AddHours(12);
            var nextWeek = today.AddDays(7);
            var days = (int)today.AddDays(6).DayOfWeek;
            var monday = nextWeek.AddDays(-days);
            return monday.Date.AddHours(12);
        }

        /// <summary>
        /// WorkingDate
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWorkingDate()
        {
            var hour = DateTime.Now.Hour;
            DateTime newDate;
            if (hour < 12)
            {
                newDate = DateTime.Today.AddDays(-1);
            }
            else
            {
                newDate = DateTime.Today;
            }

            return newDate;
        }

        private static readonly long startTicks = new DateTime(1970, 1, 1).Ticks;

        /// <summary>
        /// Get Timestamp
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetTimestamp(DateTime date)
        {
            return (int)((date.ToUniversalTime().Ticks - startTicks) / 10000000);
        }

        /// <summary>
        /// Date From
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime DateFrom(int timestamp)
        {
            return new DateTime(startTicks + timestamp * 10000000L).ToLocalTime();
        }

        /// <summary>
        /// GET Working Date Time
        /// </summary>
        /// <returns></returns>
        public static DateTime GetWorkingDateStart()
        {
            var hour = DateTime.Now.Hour;
            var today = DateTime.Today;
            DateTime newDate;
            if (hour < 12)
            {
                newDate = today.AddHours(-12);
            }
            else
            {
                newDate = today.AddHours(12);
            }

            return newDate;
        }

        /// <summary>
        /// Get WorkingDate Now
        /// </summary>
        /// <returns></returns>
        public static int GetWorkingDateNow()
        {
            return ParseWorkingDate(DateTime.Now);
        }

        /// <summary>
        /// Parse Working Date
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int ParseWorkingDate(DateTime date)
        {
            var hh = int.Parse(date.ToString("HH"));
            if (hh < 12)
            {
                var d = date.AddDays(-1);
                return int.Parse(string.Format(d.ToString("{0}MMdd"), d.Year));
            }

            return int.Parse(string.Format(date.ToString("{0}MMdd"), date.Year));
        }
        #endregion

        #region .Transfer.
        /// <summary>
        /// 将UTC时间转换为北京时间，并格式化为("yyyy-MM-dd HH:mm:ss"),格式
        /// </summary>
        /// <param name="utcTime"></param>
        /// <returns></returns>
        public static string TransUtcToStringTime(DateTime utcTime)
        {
            return utcTime.AddHours(UtcTimeZone).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 检测值是否存在
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckExists(int source, params int[] target)
        {
            return target.Any(t => source == t);
        }

        /// <summary>
        /// 是否有空参数
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CheckExistsEmpty(params string[] source)
        {
            return source.Any(string.IsNullOrWhiteSpace);
        }

        /// <summary>
        /// 检测是否含有空对象
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CheckExistsNull(params object[] source)
        {
            return source.Any(tmp => tmp == null);
        }

        /// <summary>
        /// 检查是否所有参数都为正整数
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool CheckPositive(params int[] source)
        {
            return source.All(tmp => tmp > 0);
        }

        /// <summary>
        /// 按格式转换日期，失败则返回最小时间
        /// </summary>
        /// <param name="d"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static DateTime ParseDateFormat(string d, string format)
        {
            try
            {
                DateTime dt;
                if (DateTime.TryParseExact(d, format, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dt))
                {
                    return dt;
                }
            }
            catch (Exception ex)
            {
                LogsHelper.Error("ParseDateFormat", $"string: {d} - {ex}");
            }

            return DateTime.MinValue;
        }
        #endregion

        #region .Md5 Hash.
        /// <summary>
        /// Normal Md5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5Normal(string source)
        {
            var bytes = CreateMd5(source);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Normal Md5
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Md5File(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return Md5(stream);
            }
        }

        /// <summary>
        /// Normal Md5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Md5(Stream input)
        {
            var pos = input.Position;
            if (pos > 0)
            {
                input.Position = 0;
            }

            var md5 = new MD5CryptoServiceProvider();
            var bytes = md5.ComputeHash(input);
            if (pos > 0)
            {
                // reset
                input.Position = pos;
            }
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        #endregion

        #region .Sha1 Hash.
        /// <summary>
        /// Normal Sha1
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Sha1Normal(string data)
        {
            var bytes = CreateSha1(data);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

        /// <summary>
        /// Normal Sha1
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string Sha1File(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return Sha1(stream);
            }
        }

        /// <summary>
        /// Normal Sha1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Sha1(Stream input)
        {
            var pos = input.Position;
            if (pos > 0)
            {
                input.Position = 0;
            }

            var sha1 = new SHA1CryptoServiceProvider();
            var bytes = sha1.ComputeHash(input);

            if (pos > 0)
            {
                // reset
                input.Position = pos;
            }

            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
        #endregion

        #region .Hash Code.
        /// <summary>
        /// Slower (1X) -vs- CreateFastHashCode (10X)
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static long CreateStrongHashCode(params object[] texts)
        {
            var text = string.Join(":", texts);
            return Get256HashCode(text);
        }

        /// <summary>
        /// Faster (10X) -vs- GetStrongHashCode (1X)
        /// </summary>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static long CreateFastHashCode(params object[] texts)
        {
            var text = string.Join(":", texts);
            return Get128HashCode(text);
        }

        public static long Get256HashCode(string text)
        {
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

        public static long Get128HashCode(string text)
        {
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
            var bytes = Encoding.Unicode.GetBytes(text);
            var hash = new SHA256CryptoServiceProvider();
            var bts = hash.ComputeHash(bytes);
            return bts;
        }

        private static byte[] CreateMd5(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(bytes);
            return hash;
        }

        private static byte[] CreateSha1(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var sha1 = new SHA1CryptoServiceProvider();
            var hash = sha1.ComputeHash(bytes);
            return hash;
        }
        #endregion

        #region .Encode.
        public static string CreateBase64(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            var base64 = Convert.ToBase64String(bytes);
            return base64;
        }

        public static string FromBase64(string text)
        {
            var bytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string CreateSafeBase64(string text)
        {
            return CreateBase64(text)
                .Replace("/", "_")
                .Replace("+", "-")
                .TrimEnd('=');
        }

        public static string FromSafeBase64(string text)
        {
            return FromBase64(ToBase64(text));
        }

        private static string ToBase64(string text)
        {
            var length = text.Length;
            return text
                .PadRight(length + (4 - length % 4) % 4, '=')
                .Replace("_", "/")
                .Replace("-", "+");
        }
        #endregion

        #region .随机数序列.
        /// <summary>
        /// 随机的字串
        /// </summary>
        private static readonly string[] randomChars = new[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

        /// <summary>
        /// 长度
        /// </summary>
        private static readonly int randomCharsLength = randomChars.Length;
        //private static Random random = new Random(int.Parse(DateTime.Now.ToString("smHd")));

        /// <summary>
        /// 种子
        /// </summary>
        private static readonly int randomSeed = Guid.NewGuid().GetHashCode();

        /// <summary>
        /// 随机初始化
        /// </summary>
        private static readonly Random random = new Random(randomSeed);

        /// <summary>
        /// 产生固定长度的随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomKey(int length)
        {
            if (length <= 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            lock (random)
            {
                var len = randomCharsLength;
                for (var i = 0; i < length; i++)
                {
                    var idx = random.Next(0, len);
                    sb.Append(randomChars[idx]);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// 随机长度的随机数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string RandomKey(int start, int end)
        {
            int length;
            lock (random)
            {
                length = random.Next(start, end);
            }

            return RandomKey(length);
        }

        /// <summary>
        /// 随机数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static int Random(int start, int end)
        {
            lock (random)
            {
                return random.Next(start, end);
            }
        }

        /// <summary>
        /// 随机数据下标
        /// </summary>
        /// <param name="max"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] RandomIndexs(int max, int count)
        {
            if (count <= 0)
            {
                return new int[0];
            }

            if (count > max)
            {
                throw new ArgumentOutOfRangeException("count", @"""count"" can't be bigger than ""max"".");
            }

            lock (random)
            {
                // init
                // 不重复随机数
                var sequence = new int[max];
                for (var i = 0; i < max; i++)
                {
                    sequence[i] = i;
                }

                var end = max - 1;
                var output = new int[count];
                for (var i = 0; i < count; i++)
                {
                    var num = random.Next(0, end + 1);
                    output[i] = sequence[num];
                    sequence[num] = sequence[end];
                    end--;
                }

                return output;
            }
        }
        #endregion

        #region .可逆加解密.
        /// <summary>
        /// 简单的可逆加密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="secretKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptSimple(string source, string secretKey, string encoding = "UTF-8")
        {
            var sb = new StringBuilder();
            var sourceBytes = Encoding.GetEncoding(encoding).GetBytes(source);
            var keyBytes = Encoding.GetEncoding(encoding).GetBytes(secretKey);
            var j = 0;

            foreach (var b in sourceBytes)
            {
                if (j == keyBytes.Length)
                {
                    j = 0;
                }
                sb.AppendFormat("{0:X2}", b ^ keyBytes[j]);
                j++;
            }

            return sb.ToString();
        }

        /// <summary>
        /// 简单的可逆解密
        /// </summary>
        /// <param name="source"></param>
        /// <param name="secretKey"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptSimple(string source, string secretKey, string encoding = "UTF-8")
        {
            try
            {
                var sourceLength = source.Length / 2;
                var sourceBytes = new byte[sourceLength];
                var keyBytes = Encoding.GetEncoding(encoding).GetBytes(secretKey);
                var length = keyBytes.Length;
                var j = 0;

                for (var i = 0; i < sourceLength; i++)
                {
                    if (j == length)
                    {
                        j = 0;
                    }

                    var b = (byte)Convert.ToInt16(source.Substring(i * 2, 2), 16);
                    sourceBytes[i] = (byte)(b ^ keyBytes[j]);
                    j++;
                }

                return Encoding.GetEncoding(encoding).GetString(sourceBytes, 0, sourceBytes.Length);
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region .Encrypt.
        /// <summary>
        /// AES
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesEncrypt(string source, string key)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Encoding.UTF8.GetBytes(source);

            var rDel = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rDel.CreateEncryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary>
        /// AES
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string AesDecrypt(string source, string key)
        {
            var keyArray = Encoding.UTF8.GetBytes(key);
            var toEncryptArray = Encoding.UTF8.GetBytes(source);

            var rm = new RijndaelManaged
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            var cTransform = rm.CreateDecryptor();
            var resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }
        #endregion

        #region .Parser.
        /// <summary>
        /// 解析根域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParseDomain(string url)
        {
            try
            {
                if (!url.Contains(Uri.SchemeDelimiter))
                {
                    url = string.Concat(Uri.UriSchemeHttp, Uri.SchemeDelimiter, url);
                }

                var uri = new Uri(url);
                return uri.Host;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 解析根域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ParseRootDomain(string url)
        {
            var d = ParseDomain(url);
            if (d.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
            {
                return d.Substring(4);
            }

            return d;
        }

        public static string SubString(string input, int length)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            if (length <= 0)
            {
                return input;
            }

            int j = 0, k = 0;
            using (var ce = input.GetEnumerator())
            {
                while (ce.MoveNext())
                {
                    j += (ce.Current > 0 && ce.Current < 255) ? 1 : 2;

                    if (j <= length)
                    {
                        k++;
                    }
                    else
                    {
                        return input.Substring(0, k);
                    }
                }
            }

            return input;
        }


        #endregion

        #region .bytes compress.
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] Compress(object data)
        {
            return BinaryHelper.ToBytes(data, true);
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T Decompression<T>(object bytes)
        {
            return BinaryHelper.FromBytes<T>((byte[])bytes, true);
        }
        #endregion

        #region .IP.
        /// <summary>
        /// IP 验证
        /// </summary>
        private const string IP_VALID = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(25[0-5]|2[0-4]\d|1\d\d|\d{1,2})$";
        private const string IP4_CONTAINS = @"(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(25[0-5]|2[0-4]\d|1\d\d|\d{1,2})";

        /// <summary>
        /// 一个段
        /// </summary>
        private const string IP_LOTS = @"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.\*$";

        /// <summary>
        /// IP验证（包含 0.0.0.0）
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool ValidIp4(string ip)
        {
            return (!string.IsNullOrEmpty(ip)) && Regex.IsMatch(ip, IP_VALID);
        }

        /// <summary>
        /// IP验证（包含 0.0.0.*）
        /// </summary>
        /// <param name="ips"></param>
        /// <returns></returns>
        public static bool ValidLotsIp4(string ips)
        {
            return (!string.IsNullOrEmpty(ips)) && Regex.IsMatch(ips, IP_LOTS);
        }

        /// <summary>
        /// 解析用户IP
        /// </summary>
        /// <param name="svs">ServerVariables</param>
        /// <returns></returns>
        public static string ParseClientIp(NameValueCollection svs)
        {
            var hxip = ParseIp(svs, "HTTP_X_FORWARDED_FOR", true);
            var bip = ParseIp(svs, "X-Forwarded-For", true);
            var xip = ParseIp(svs, "X_FORWARDED_FOR", true);
            var rip = ParseIp(svs, "REMOTE_ADDR", true);
            var hip = ParseIp(svs, "REMOTE_HOST", true);

            return hxip.EmptyWithReplace(bip).EmptyWithReplace(xip).EmptyWithReplace(rip).EmptyWithReplace(hip);
        }

        /// <summary>
        /// 返回IP
        /// </summary>
        /// <param name="svs">ServerVariables</param>
        /// <param name="key"></param>
        /// <param name="first"></param>
        /// <returns></returns>
        private static string ParseIp(NameValueCollection svs, string key, bool first)
        {
            var val = svs[key];
            if (!string.IsNullOrEmpty(val))
            {
                // 取第1个
                var ms = Regex.Matches(val, IP4_CONTAINS);
                var count = ms.Count - 1;
                if (count >= 0)
                {
                    return ms[first ? 0 : count].Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 返回最后一个IP
        /// </summary>
        /// <param name="vals"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ParseLastIp(NameValueCollection vals, string key)
        {
            var val = vals[key];
            if (!string.IsNullOrEmpty(val))
            {
                // 取第1个
                var ms = Regex.Matches(val, IP4_CONTAINS);
                var count = ms.Count;
                if (count > 0)
                {
                    return ms[count - 1].Value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取客户端IP和端口号
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static string ParseAddress(Socket socket)
        {
            try
            {
                if (socket != null)
                {
                    var remote = (IPEndPoint)socket.RemoteEndPoint;
                    var address = string.Format("{0}:{1}", remote.Address, remote.Port);
                    return address;
                }
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch
            // ReSharper restore EmptyGeneralCatchClause
            { }

            return string.Empty;
        }
        #endregion

        #region .JSON.
        /// <summary>
        /// 转为Json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(object data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                LogsHelper.Error("ToJson", string.Format("data: {0}, error: {1}", data.GetType(), ex));
                return string.Empty;
            }
        }

        /// <summary>
        /// 将字符串转为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ParseByJson<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                LogsHelper.Error("StaticHelper.ParseByJsonT", string.Format("json: {0}, error: {1}", json, ex));
                return default(T);
            }
        }

        #endregion

        #region .Browser.
        /// <summary>
        /// bots
        /// </summary>
        private static readonly List<string> bots = new List<string>
        {
            "baiduspider","bspider","cydralspider","360spider","haosouspider","yisouspider",
            "gammaspider","portalbspider","infospiders","pjspider","spider_monkey",
            "spiderline","wapspider","webspider","spiderbot",
            "googlebot","bingbot","yandexbot","ahrefsbot","msnbot","linkedinbot","exabot","compspybot",
            "yesupbot","paperlibot","tweetmemebot","semrushbot","gigabot","voilabot","adsbot-google",
            "botlink","alkalinebot","araybot","undrip bot","borg-bot","boxseabot","yodaobot","admedia bot",
            "ezooms.bot","confuzzledbot","coolbot","internet cruiser robot","yolinkbot","diibot","musobot",
            "dragonbot","elfinbot","wikiobot","twitterbot","contextad bot","hambot","iajabot","news bot",
            "irobot","socialradarbot","ko_yappo_robot","skimbot","psbot","rixbot","seznambot","careerbot",
            "simbot","solbot","mail.ru_bot","blekkobot","bitlybot","techbot","void-bot",
            "vwbot_k","diffbot","friendfeedbot","archive.org_bot","woriobot","crystalsemanticsbot","wepbot",
            "spbot","tweetedtimes bot","mj12bot","who.is bot","dotbot",
            "psbot","robot","jbot","bbot","google",
            "spider","bot"
        };

        /// <summary>
        /// crawlers
        /// </summary>
        private static readonly List<string> crawlers = new List<string>
        {
            "google","baidu","sogou","coccoc","digext","yandex",
            "80legs","yahoo! slurp","ia_archiver","mediapartners-google","lwp-trivial",
            "nederland.zoek","ahoy","anthill","appie","arale","araneo","ariadne","atn_worldwide","atomz",
            "bjaaland","ukonline","calif","christcrawler","combine","cosmos","cusco","cyberspyder",
            "digger","grabber","downloadexpress","ecollector","ebiness","esculapio","esther",
            "fastcrawler","felix ide","hamahakki","kit-fireball","fouineur","freecrawl","desertrealm",
            "gcreep","golem","griffon","gromit","gulliver","gulper","whowhere",
            "havindex","hotwired","htdig","ingrid","informant","inspectorwww","iron33",
            "jcrawler","teoma","ask jeeves","jeeves","image.kapsi.net","kdd-explorer","label-grabber",
            "larbin","linkidator","linkwalker","lockon","logo_gif_crawler","marvin","mattie","mediafox",
            "merzscope","nec-meshexplorer","mindcrawler","udmsearch","moget","motor","muncher","muninn",
            "muscatferret","mwdsearch","sharp-info-agent","webmechanic","netscoop","newscan-online",
            "objectssearch","orbsearch","packrat","pageboy","parasite","patric","pegasus","perlcrawler",
            "phpdig","piltdownman","pimptrain","plumtreewebaccessor","getterrobo-plus","raven",
            "roadrunner","robbie","robocrawl","robofox","webbandit","scooter","search-au","searchprocess",
            "senrigan","shagseeker","site valet","skymob","slcrawler","slurp","snooper","speedy",
            "curl_image_client","suke","www.sygol.com","tach_bw","templeton",
            "titin","topiclink","udmsearch","urlck","valkyrie libwww-perl","verticrawl","victoria",
            "webscout","voyager","crawlpaper","webcatcher","t-h-u-n-d-e-r-s-t-o-n-e",
            "webmoose","pagesinventory","webquest","webreaper","webwalker","winona","occam",
            "robi","fdse","jobo","rhcs","gazz","dwcp","yeti","crawler","fido","wlm","wolp","wwwc","xget",
            "legs","curl","webs","wget","sift","cmc"
        };

        /// <summary>
        /// Parse Browser [--, ----]
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="bot"></param>
        /// <returns></returns>
        public static string ParseBrowserBot(string userAgent, out bool bot)
        {
            bot = false;
            if (string.IsNullOrEmpty(userAgent))
            {
                //return "USER-AGENT-NULL";
                return "--";
            }

            var ua = userAgent.ToLower();
            var botExists = ua.Contains("bot") || ua.Contains("spider");

            var match = botExists ? bots.FirstOrDefault(ua.Contains) : crawlers.FirstOrDefault(ua.Contains);
            var found = !string.IsNullOrEmpty(match);
            if (found && match.Length < 5)
            {
                var log = string.Format("Possible new crawler found: {0} - {1}", match, userAgent);
                LogsHelper.Warning("ParseBrowser.NewCrawler", log);
            }

            if (found)
            {
                bot = true;
                return match;
            }

            //return "BOT-NOT-FOUND";
            return "----";
        }

        private static readonly Regex oss = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od|ad)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        private static readonly Regex devices = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// Parse Mobile [--, ----]
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        private static string ParseMobile(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                //return "USER-AGENT-NULL";
                return "--";
            }

            var ua = userAgent.ToLower();
            var mobile = string.Empty;
            var mo = oss.Match(ua);
            if (mo.Success)
            {
                mobile = mo.Value;
            }

            if (ua.Length >= 4)
            {
                mo = devices.Match(ua.Substring(0, 4));
                if (mo.Success)
                {
                    mobile += mo.Value;
                }
            }

            if (string.IsNullOrEmpty(mobile))
            {
                return "----";
            }

            return mobile;
        }

        /// <summary>
        /// mobile
        /// </summary>
        /// <param name="userAgent"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool TryParseMobile(string userAgent, out string mobile)
        {
            mobile = ParseMobile(userAgent);
            return !(mobile.Equals("--") || mobile.Equals("----"));
        }
        #endregion

        /// <summary>
        /// 根据标识计算负载均衡的下标
        /// </summary>
        /// <param name="length"></param>
        /// <param name="texts"></param>
        /// <returns></returns>
        public static int NextArrayIndex(int length, params string[] texts)
        {
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), @"should be positive.");
            }

            var hashCode = string.Join("%", texts).GetHashCode();
            var andValue = (hashCode & 0x7fffffff);
            return andValue % length;
        }

        #region .File & Folder.
        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="relates">相对程序所在目录的路径：文件名或目录</param>
        /// <param name="removeLast">是否移除最后一截</param>
        /// <returns>返回组合程序所在目录之后的完整路径</returns>
        private static string CalculatePath(string relates, bool removeLast)
        {
            // Web 目录 DLL 运行在 bin 目录时，取上级目录作为根目录
            var root = GetFullRootPath();
            var list = new List<string>
            {
                root
            };

            var folders = relates.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // 是否移除最后一截
            // 使用场合：当需要文件所在目录的时候
            if (removeLast && folders.Count > 0)
            {
                folders.RemoveAt(folders.Count - 1);
            }

            list.AddRange(folders);

            return Path.Combine(list.ToArray());
        }

        /// <summary>
        /// 获取完整路径
        /// </summary>
        /// <param name="relates">相对程序所在目录的路径：文件名或目录</param>
        /// <returns>返回组合程序所在目录之后的完整路径</returns>
        public static string GetFullPath(string relates)
        {
            return CalculatePath(relates, false);
        }

        /// <summary>
        /// 获取文件所在目录
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetFileFullFolder(string filename)
        {
            if (filename.EndsWith(@"\") || filename.EndsWith(@"/"))
            {
                return GetFullPath(filename);
            }

            return CalculatePath(filename, true);
        }

        /// <summary>
        /// 获取完整路径，如果所在目录不存在则自动创建
        /// </summary>
        /// <param name="relates">相对路径的文件或目录路径</param>
        /// <param name="isFolder">是否为目录</param>
        /// <returns>isFolder = true 识别 relates 为目录；isFolder = false 识别 relates 为目录和文件，将创建到参数的上一层路径，如传入 a/b.txt，只创建到 a 目录，不会创建 b.txt 目录。</returns>
        public static string CreateAndGetFullPath(string relates, bool isFolder)
        {
            var path = isFolder ? GetFullPath(relates) : GetFileFullFolder(relates);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return GetFullPath(relates);
        }

        public static string GetFullRootPath()
        {
            // Web 目录 DLL 运行在 bin 目录时，取上级目录作为根目录
            //var root = AppDomain.CurrentDomain.BaseDirectory;
            var root = Directory.GetCurrentDirectory();
            if (root.EndsWith(@"\bin\") || root.EndsWith("/bin/"))
            {
                var dir = new DirectoryInfo(root);
                if (dir.Parent != null)
                {
                    root = dir.Parent.FullName;
                }
            }

            return root;
        }
        #endregion
    }
}
