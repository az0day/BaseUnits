using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseUnits.Core.Entities
{
    /// <summary>
    /// 语言信息
    /// </summary>
    [Serializable]
    public class LanguagesField
    {
        /// <summary>
        /// 个数
        /// </summary>
        private const int COUNT = 7;

        /// <summary>
        /// 资源信息
        /// </summary>
        private readonly string[] _languages = new string[COUNT];

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="language"></param>
        /// <param name="value"></param>
        public LanguagesField(LanguagesEnum language, string value)
            : this()
        {
            _languages[(int)language] = value;
        }

        /// <summary>
        /// 无参数构造函数
        /// </summary>
        public LanguagesField()
        {
            var values = new string[COUNT];
            for (var i = 0; i < COUNT; i++)
            {
                values[i] = string.Empty;
            }

            InitLanguageField(values);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="values"></param>
        public LanguagesField(string[] values)
        {
            InitLanguageField(values);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="values"></param>
        private void InitLanguageField(IList<string> values)
        {
            var count = Math.Min(COUNT, values.Count);
            for (var i = 0; i < count; i++)
            {
                _languages[i] = values[i];
            }
        }

        /// <summary>
        /// 获取某一语言对应值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[LanguagesEnum key]
        {
            get
            {
                return _languages[(int)key] ?? string.Empty;
            }

            set
            {
                _languages[(int)key] = value;
            }
        }

        /// <summary>
        /// 将各语言的资源组成以某字符分隔的字符串
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public string CreateString(string separator)
        {
            var text = string.Join(separator, Values);
            return text;
        }

        /// <summary>
        /// 获取所有语言组成的数组
        /// </summary>
        public string[] Values
        {
            get
            {
                return _languages;
            }
            set
            {
                if (value != null)
                {
                    InitLanguageField(value);
                }
            }
        }

        /// <summary>
        /// 产生以某字符隔的字符串
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string LanguagesValues(string separator)
        {
            var query = Enum.GetValues(typeof(LanguagesEnum)).Cast<int>();
            var text = string.Join(separator, query);
            return text;
        }

        /// <summary>
        /// 产生以某字符隔的字符串
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string LanguagesNames(string separator)
        {
            var query = Enum.GetNames(typeof(LanguagesEnum));
            var text = string.Join(separator, query);
            return text;
        }

        /// <summary>
        /// 将特定字符复制并用分隔符连接(个数为语言总数)
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="text">每种语言固定的值</param>
        /// <returns></returns>
        public static string CreateLanguagesText(string separator, string text)
        {
            var array = new string[COUNT];
            for (var i = 0; i < COUNT; i++)
            {
                array[i] = text;
            }

            return string.Join(separator, array);
        }
    }
}
