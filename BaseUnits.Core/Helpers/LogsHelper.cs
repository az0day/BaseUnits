using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace BaseUnits.Core.Helpers
{
    /// <summary>
    /// 单例日志打印工具
    /// </summary>
    public class LogsHelper
    {
        /// <summary>
        /// 默认保留 90 天
        /// </summary>
        public static int ExpireDays = 90;

        /// <summary>
        /// 目录名
        /// </summary>
        public static string LogsFolder = "_Logs";

        #region .Writer.
        /// <summary>
        /// 确保单线程执行
        /// </summary>
        internal class LogFileWriter
        {
            private DateTime _lastClean = DateTime.Now;

            private readonly ConcurrentDictionary<string, LogAppender> _writers = new ConcurrentDictionary<string, LogAppender>(StringComparer.InvariantCultureIgnoreCase);

            public void Write(LogItem item)
            {
                // 不区分大小写
                var key = $"{item.Level}-{item.Title}";

                LogAppender appender;
                if (!_writers.TryGetValue(key, out appender))
                {
                    // 初始化
                    appender = new LogAppender
                    {
                        LastTime = DateTime.Now
                    };
                    _writers[key] = appender;
                }

                // 写入日志
                appender.Append(item);

                // 先写再清理
                CleanAppenders();
            }

            private void CleanAppenders()
            {
                var last = DateTime.Now.AddMinutes(-25);
                if (_lastClean < last)
                {
                    _lastClean = DateTime.Now;

                    // 清理时间已过的
                    var before = DateTime.Now.AddMinutes(-60);
                    var query = (
                        from p in _writers
                        where p.Value.LastTime < before
                        select p.Key
                    ).ToArray();

                    foreach (var key in query)
                    {
                        LogAppender p;
                        if (_writers.TryRemove(key, out p))
                        {
                            p.Dispose();
                        }
                    }

                    // 清理旧目录
                    CleanExpiredLogs();
                }
            }

            private void CleanExpiredLogs()
            {
                // 最长保留 90 天
                var days = ExpireDays;
                if (days <= 0)
                {
                    return;
                }

                var now = DateTime.Now;
                days = Math.Min(days, 90);

                var keeps = new HashSet<string>();
                for (var i = 0; i < days; i++)
                {
                    keeps.Add(now.AddDays(-i).ToString("yyyyMMdd", new CultureInfo("en-US")));
                }

                var pathes = new[]
                {
                    StaticHelper.GetFullRootPath(),
                    LogsFolder
                };

                var baseFolder = Path.Combine(pathes);
                if (!Directory.Exists(baseFolder))
                {
                    return;
                }

                var directory = new DirectoryInfo(baseFolder);
                foreach (var child in directory.GetDirectories())
                {
                    if (!keeps.Contains(child.Name))
                    {
                        try
                        {
                            child.Delete(true);
                        }
                        catch
                        {
                            //
                        }
                    }
                }
            }
        }
        #endregion

        #region .Appender.
        internal class LogAppender : IDisposable
        {
            public DateTime LastTime { get; set; }

            private StreamWriter _writer = StreamWriter.Null;

            private string _lastFileName = string.Empty;

            private void InitWriter(LogItem item)
            {
                var fileName = CreateFileName(item);
                if (!fileName.Equals(_lastFileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    // 释放上一个
                    if (_writer != StreamWriter.Null)
                    {
                        _writer.Dispose();
                        _writer = StreamWriter.Null;
                    }

                    // 自动创建目录
                    var directory = Path.GetDirectoryName(fileName);
                    if (!string.IsNullOrEmpty(directory))
                    {
                        if (!Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }
                    }

                    _writer = CreateWrite(fileName);
                    _lastFileName = fileName;
                }
            }

            private string CreateFileName(LogItem item)
            {
                var time = item.Time;
                var fileName = $"{time:HH}.[{item.Level}].{item.Title}.log";
                var pathes = new[]
                {
                    StaticHelper.GetFullRootPath(),
                    LogsFolder,
                    time.ToString("yyyyMMdd", new CultureInfo("en-US")),
                    ToSafeFileName(fileName)
                };

                return Path.Combine(pathes);
            }

            private static string ToSafeFileName(string s)
            {
                return s
                    .Replace("\\", "")
                    .Replace("/", "")
                    .Replace("\"", "")
                    .Replace("*", "")
                    .Replace(":", "")
                    .Replace("?", "")
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("|", "");
            }

            private StreamWriter CreateWrite(string fileName)
            {
                return new StreamWriter(fileName, true, Encoding.UTF8, 8192);
            }

            public void Append(LogItem item)
            {
                // 设置最后时间
                LastTime = DateTime.Now;

                // 检查并初始化
                InitWriter(item);

                if (_writer != null)
                {
                    try
                    {
                        _writer.Write($"[{item.Time:HH:mm:ss.fff}] {item.Content}");
                        _writer.Write(Environment.NewLine);
                        _writer.Flush();
                    }
                    catch
                    {
                        //
                    }
                }
            }

            public void Dispose()
            {
                if (_writer != StreamWriter.Null)
                {
                    _writer.Dispose();
                    _writer = StreamWriter.Null;
                }
            }
        }
        #endregion

        #region .LogItem.
        [Serializable]
        internal class LogItem
        {
            public string Title { get; set; }

            public string Content { get; set; }


            public LogLevelEnum Level { get; set; }


            public DateTime Time { get; set; }
        }
        #endregion

        #region .LogLevelEnum.
        /// <summary>
        /// 日志级别
        /// </summary>
        [Serializable]
        public enum LogLevelEnum
        {
            Info = 1,

            Debug = 2,

            Trace = 3,

            Warn = 4,

            Error = 5,

            Fatal = 6
        }

        /// <summary>
        /// 消息级别
        /// </summary>
        [Serializable]
        public enum MessageLevelEnum
        {
            None,
            LogOnly,
            MsgOnly,
            Both,
        }
        #endregion

        #region .Singleton.
        private static readonly Lazy<LogsHelper> instance = new Lazy<LogsHelper>(() => new LogsHelper());

        internal static LogsHelper Helper { get; } = instance.Value;

        /// <summary>
        /// 使用单例模式
        /// </summary>
        private LogsHelper()
        {
            _worker = new ThreadQueueConsumer<LogItem>(ConsumeWrite);
        }
        #endregion

        #region .Internal Methods.
        private readonly ThreadQueueConsumer<LogItem> _worker;
        private readonly LogFileWriter _fileWriter = new LogFileWriter();

        private void ConsumeWrite(LogItem item)
        {
            _fileWriter.Write(item);
        }

        internal void WriteContent(string title, string content, LogLevelEnum level)
        {
            _worker.Push(new LogItem
            {
                Title = title,
                Content = content,
                Time = DateTime.Now,
                Level = level
            });
        }
        #endregion

        #region .Static Methods.
        /// <summary>
        /// 追加日志(以日期-时间 文件名记录)
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        public static void Append(string filename, string content)
        {
            Info(filename, content);
        }

        public static void Write(string filename, string content, LogLevelEnum level)
        {
            Helper.WriteContent(filename, content, level);
        }

        public static void Debug(string title, string content)
        {
            Helper.WriteContent(title, content, LogLevelEnum.Debug);
        }

        public static void Info(string title, string content)
        {
            Helper.WriteContent(title, content, LogLevelEnum.Info);
        }

        public static void Error(string title, string content)
        {
            Helper.WriteContent(title, content, LogLevelEnum.Error);
        }

        public static void Error(string title, Exception ex)
        {
            Helper.WriteContent(title, ex.ToString(), LogLevelEnum.Error);
        }

        public static void Fatal(string title, string content)
        {
            Helper.WriteContent(title, content, LogLevelEnum.Fatal);
        }

        public static void Warning(string title, string content)
        {
            Helper.WriteContent(title, content, LogLevelEnum.Warn);
        }

        public static void Trace(string title, string content)
        {
            Helper.WriteContent(title, content, LogLevelEnum.Trace);
        }
        #endregion
    }
}
