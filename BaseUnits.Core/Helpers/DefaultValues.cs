using BaseUnits.Core.Entities;

namespace BaseUnits.Core.Helpers
{
    /// <summary>
    /// 默认值
    /// </summary>
    public sealed class DefaultValues : StaticHelper
    {
        #region .Languages.
        /// <summary>
        /// 解析为语言
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static LanguagesEnum ParseLanguagesEnum(string code)
        {
            switch (code.ToLower())
            {
                case "zh-cn":
                    return LanguagesEnum.ChineseSimplified;
                case "zh-tw":
                    return LanguagesEnum.ChineseTraditional;
                case "id-id":
                    return LanguagesEnum.Indonesia;
                case "ja-jp":
                    return LanguagesEnum.Japanese;
                case "th-th":
                    return LanguagesEnum.Thai;
                case "vi-vn":
                    return LanguagesEnum.Vietnam;
                default:
                    return LanguagesEnum.English;
            }
        }

        /// <summary>
        /// 转换
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string ParseLanguagesCode(LanguagesEnum language)
        {
            switch (language)
            {
                case LanguagesEnum.ChineseSimplified:
                    return "zh-CN";
                case LanguagesEnum.ChineseTraditional:
                    return "zh-TW";
                case LanguagesEnum.Indonesia:
                    return "id-ID";
                case LanguagesEnum.Japanese:
                    return "ja-JP";
                case LanguagesEnum.Thai:
                    return "th-TH";
                case LanguagesEnum.Vietnam:
                    return "vi-VN";
                default:
                    return "en-US";
            }
        }

        /// <summary>
        /// Calculate Page Info
        /// </summary>
        /// <param name="count"></param>
        /// <param name="pageNow"></param>
        /// <param name="pageSize"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static PageInfo CalculatePageInfo(int count, int pageNow, int pageSize, int order = 0)
        {
            return PageInfo.Calculate(count, pageNow, pageSize, order);
        }
        #endregion

        #region .Hash.
        /// <summary>
        /// HASH SALT
        /// </summary>
        private const string HASH_SALT = "f4f02b037ae207ab-{0}-2cb3acf1de99921a";

        /// <summary>
        /// 加盐的Md5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5(string source)
        {
            var text = string.Format(HASH_SALT, source);
            return Md5Origin(text);
        }

        /// <summary>
        /// Normal Md5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Md5Origin(string source)
        {
            return Md5Normal(source);
        }

        /// <summary>
        /// 加盐的Sha1
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Sha1(string source)
        {
            var text = string.Format(HASH_SALT, source);
            return Sha1Origin(text);
        }

        /// <summary>
        /// Normal Md5
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Sha1Origin(string source)
        {
            return Sha1Normal(source);
        }

        private static string Hash(string md5, string sha1)
        {
            return string.Format("{0}{1}", md5, sha1.Substring(0, 8));
        }

        public static string HashTagNormal(string text)
        {
            var md5 = Md5Normal(text);
            var sha1 = Sha1Normal(text);
            return Hash(md5, sha1);
        }

        public static string HashTagSalt(string text)
        {
            var md5 = Md5(text);
            var sha1 = Sha1(text);
            return Hash(md5, sha1);
        }
        #endregion

        #region .Base64.
        public static string Base64Encode(string text)
        {
            return CreateBase64(text);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            return FromBase64(base64EncodedData);
        }
        #endregion
    }
}
