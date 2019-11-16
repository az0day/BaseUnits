using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BaseUnits.Core.Entities
{
    /// <summary>
    /// 当前域名及一些设置
    /// </summary>
    [Serializable]
    public class DomainSetting
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="folder"></param>
        internal DomainSetting(string folder)
        {
            CalculateFolder(folder);
            Setting = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        }

        #region .Privates.
        private bool _domainParsed;
        private string _trueFolder = string.Empty;
        private HashSet<string> _domainsList = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// 读取文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static string ReadStaticContent(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (var sr = new StreamReader(fileName, Encoding.UTF8))
                {
                    return sr.ReadToEnd();
                }
            }

            return string.Empty;
        }

        private IEnumerable<string> ReadDomains()
        {
            if (_domainParsed)
            {
                return _domainsList;
            }

            // domainlist
            _domainParsed = true;

            var text = this["domainlist"];
            var list = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            if (string.IsNullOrWhiteSpace(text))
            {
                return _domainsList;
            }

            var domains = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in domains)
            {
                var domain = item.Trim();
                if (string.IsNullOrEmpty(domain))
                {
                    continue;
                }

                var v = $".{domain}.";
                list.Add(v);
            }

            lock (_domainsList)
            {
                _domainsList = list;
            }

            return _domainsList;
        }

        /// <summary>
        /// 设置目录和起始目录
        /// </summary>
        /// <param name="path"></param>
        private void CalculateFolder(string path)
        {
            var folder = new DirectoryInfo(path);

            Folder = folder.Name;
            _trueFolder = folder.FullName;
        }
        #endregion

        #region .Public.
        /// <summary>
        /// 精确匹配域名
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool Equals(string domain)
        {
            var list = ReadDomains();
            return list.Any(domain.Equals);
        }

        /// <summary>
        /// 域名包含
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool Contains(string domain)
        {
            var list = ReadDomains();
            return list.Any(domain.Contains);
        }
        #endregion

        #region .Properties.
        /// <summary>
        /// 针对该域名的一些设置(显示,资源)
        /// </summary>
        public Dictionary<string, string> Setting { get; private set; }

        /// <summary>
        /// 是否为默认
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 目录地址
        /// </summary>
        public string Folder { get; private set; }

        /// <summary>
        /// 品牌名字
        /// </summary>
        public string BrandCode { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title
        {
            get
            {
                return this["Title"];
            }
        }
        #endregion

        #region .Settings.
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="settings"></param>
        public void Update(Dictionary<string, string> settings)
        {
            if (settings != null)
            {
                foreach (var item in settings)
                {
                    Setting[item.Key] = item.Value;
                }
            }
        }

        public IDictionary<string, string> QueryTags()
        {
            var query = from p in Setting.Keys where p.StartsWith("{") && p.EndsWith("}") select p;
            return query.ToDictionary(key => key, key => Setting[key]);
        }

        public string TagValue(string key)
        {
            var k = string.Format("{{{0}}}", key);
            return this[k];
        }

        public bool ValueBool(string key)
        {
            return Boolean(key);
        }

        public int ValueInt(string key)
        {
            return Int(key);
        }

        public decimal ValueDecimal(string key)
        {
            return Decimal(key);
        }

        /// <summary>
        /// 根据键值获取对应设置信息, 不存在则0长字符
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Empty;
                }

                string value;
                Setting.TryGetValue(key, out value);

                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }

                return value;
            }
        }

        /// <summary>
        /// 是否
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool Boolean(string key)
        {
            var v = this[key];
            if (string.IsNullOrEmpty(v))
            {
                return false;
            }

            if (v.Equals("true", StringComparison.OrdinalIgnoreCase) || v.Equals("1"))
            {
                return true;
            }

            bool ret;
            bool.TryParse(v, out ret);
            return ret;
        }

        /// <summary>
        /// Integer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private int Int(string key)
        {
            var v = this[key];

            int ret;
            int.TryParse(v, out ret);

            return ret;
        }

        /// <summary>
        /// Decimal
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private decimal Decimal(string key)
        {
            var v = this[key];

            decimal ret;
            decimal.TryParse(v, out ret);

            return ret;
        }
        #endregion

        #region .Template.
        /// <summary>
        /// 读取模板内容
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string TemplateHtml(string key)
        {
            key = key.Trim('/', '\\');
            var paths = key.Split('/', '\\').ToList();

            paths.Insert(0, _trueFolder);
            paths[paths.Count - 1] += ".html";

            var file = Path.Combine(paths.ToArray());
            var html = ReadStaticContent(file);
            return html;
        }
        #endregion
    }
}
