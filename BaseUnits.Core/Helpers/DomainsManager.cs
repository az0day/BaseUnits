using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BaseUnits.Core.Entities;
using Newtonsoft.Json;

namespace BaseUnits.Core.Helpers
{
    /// <summary>
    /// 当前域名及一些设置
    /// </summary>
    public sealed class DomainsManager : IDisposable
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="folder"></param>
        public DomainsManager(string folder)
        {
            Home = new DirectoryInfo(folder);
            Folder = Home.Name;
            StartRoot = Home.FullName;

            // 监控文件更改
            StartTimer();
        }

        #region .private.
        private readonly object _lock = new object();
        /// <summary>
        /// 模板
        /// </summary>
        private Dictionary<string, DomainSetting> _templates = new Dictionary<string, DomainSetting>(StringComparer.InvariantCultureIgnoreCase);
        private bool _running;

        private string ToJson(object data)
        {
            try
            {
                return JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                var content = string.Format("json: {0}, error: {1}", data.GetType(), ex);
                LogsHelper.Error("JsonDomainsManager", content);
                return string.Empty;
            }
        }

        private DateTime _lastUpdate = DateTime.Now;
        private const string CONFIG_TXT = "config.exclude";
        private const string HOME_HTML = "home.html";
        private const string DEFAULT_BRAND_KEY = "*default*";
        private readonly string[] _monitorList = new[] { CONFIG_TXT, HOME_HTML };

        /// <summary>
        /// 计时器
        /// </summary>
        private void StartTimer()
        {
            UpdateSetting();

            _running = true;
            _lastUpdate = DateTime.Now;

            // 定时检查更改
            Task.Factory.StartNew(ProcessMonitorTimer);
        }

        private void ProcessMonitorTimer()
        {
            while (_running)
            {
                var check = MonitorWriteTime();
                if (check)
                {
                    UpdateSetting();
                }

                Thread.Sleep(10000);
            }
        }

        /// <summary>
        /// 检查更改
        /// </summary>
        /// <returns></returns>
        private bool MonitorWriteTime()
        {
            var d = new DirectoryInfo(StartRoot);
            if (d.Exists)
            {
                var dd = d.GetDirectories();
                return (
                    from dir in dd
                    from cf in _monitorList
                    from f in dir.GetFiles(cf)
                    select f
                ).Any(f => f.LastWriteTime > _lastUpdate);
            }

            return false;
        }

        /// <summary>
        /// 更新设定
        /// </summary>
        private void UpdateSetting()
        {
            var setDefault = false;
            var settings = new Dictionary<string, DomainSetting>(StringComparer.InvariantCultureIgnoreCase);
            var d = new DirectoryInfo(StartRoot);
            var dd = d.GetDirectories();
            foreach (var folder in dd)
            {
                var folderName = folder.Name;

                DomainSetting setting;
                if (!settings.TryGetValue(folderName, out setting))
                {
                    var fn = Path.Combine(StartRoot, folderName);
                    setting = new DomainSetting(fn);
                    settings.Add(folderName, setting);
                }

                var sfile = Path.Combine(StartRoot, folderName, CONFIG_TXT);
                var sts = ReadFileSettings(sfile);
                setting.Update(sts);

                #region .clean up.
                // 默认模板，只检测一次
                // IsDefault
                // IsDefaultBrand
                if (!setDefault)
                {
                    if (setting.ValueBool("IsDefault") || setting.ValueBool("IsDefaultBrand"))
                    {
                        setDefault = true;
                        setting.IsDefault = true;
                        settings[DEFAULT_BRAND_KEY] = setting;
                    }
                }

                // 品牌代码
                // 查找 BrandCode 配置
                // 若没有则使用 Title 设置
                var brand = setting["BrandCode"];
                if (string.IsNullOrEmpty(brand))
                {
                    brand = setting["Title"];
                }

                // 大写
                if (!string.IsNullOrEmpty(brand))
                {
                    setting.BrandCode = brand;
                }
                #endregion
            }

            // set default
            if (!setDefault)
            {
                var temp = settings[settings.Keys.ToArray()[0]];
                temp.IsDefault = true;
                settings[DEFAULT_BRAND_KEY] = temp;
            }

            lock (_lock)
            {
                _templates = settings;
            }
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        private static Dictionary<string, string> ReadFileSettings(string fullName)
        {
            var settings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (!File.Exists(fullName))
            {
                return settings;
            }

            using (var stream = File.OpenRead(fullName))
            {
                var sr = new StreamReader(stream);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    // # 开头表示注释
                    if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    {
                        continue;
                    }

                    // 检查
                    var seg = line.Split(new[] { ':' }, 2);
                    var key = seg[0].Trim();

                    if (string.IsNullOrEmpty(key))
                    {
                        continue;
                    }

                    #region .backup sites.
                    // 备份网址放在最后
                    // 优先处理
                    if (key.Equals("backupsites", StringComparison.OrdinalIgnoreCase))
                    {
                        var sites = sr.ReadToEnd().Trim();
                        settings["backupsites"] = sites;
                        break;
                    }
                    #endregion

                    // 键配置
                    // 后面的配置覆盖前面
                    var value = (seg.ElementAtOrDefault(1) ?? string.Empty).Trim();
                    if (!string.IsNullOrEmpty(value))
                    {
                        settings[key] = value;
                    }
                }
            }

            return settings;
        }
        #endregion

        #region .public.
        /// <summary>
        /// 目录地址
        /// </summary>
        public string Folder { get; private set; }

        /// <summary>
        /// 主目录
        /// </summary>
        public DirectoryInfo Home { get; }

        /// <summary>
        /// 起始目录物理地址
        /// </summary>
        public string StartRoot { get; private set; }

        /// <summary>
        /// 查找配置
        /// </summary>
        /// <param name="host"></param>
        /// <returns></returns>
        public DomainSetting QueryDomainAddressFrom(string host)
        {
            lock (_lock)
            {
                host = string.Format(".{0}.", host);

                // 精确匹配
                foreach (var item in _templates.Values)
                {
                    if (item.Equals(host))
                    {
                        return item;
                    }
                }

                // 模糊匹配
                foreach (var item in _templates.Values)
                {
                    if (item.Contains(host))
                    {
                        return item;
                    }
                }

                return _templates[DEFAULT_BRAND_KEY];
            }
        }

        public DomainSetting QueryByBrand(string code)
        {
            lock (_lock)
            {
                foreach (var item in _templates.Values)
                {
                    if (item.BrandCode.Equals(code, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return item;
                    }
                }

                return _templates[DEFAULT_BRAND_KEY];
            }
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            lock (_lock)
            {
                return ToJson(_templates);
            }
        }
        #endregion

        public void Dispose()
        {
            _running = false;
            //LogsWrite("LogsDomainDispose", "");
        }
    }
}
