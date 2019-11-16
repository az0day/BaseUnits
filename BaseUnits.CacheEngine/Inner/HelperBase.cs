using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

using BaseUnits.Core.Helpers;

namespace BaseUnits.CacheEngine.Inner
{
    internal abstract class HelperBase
    {
        protected abstract string Title { get; }

        #region .Logs.
        protected void LogsError(string message)
        {
            LogsHelper.Error(Title, message);
        }

        protected void LogsWrite(string message)
        {
            LogsWrite(Title, message);
        }

        private void LogsWrite(string title, string message)
        {
            LogsHelper.Info(title, message);
        }
        #endregion

        #region .Bytes Transfer.
        /// <summary>
        /// 永久序列化的场合压缩保存
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected static byte[] ToZipBytes(object data)
        {
            return BinaryHelper.ToBytes(data, true);
        }

        /// <summary>
        /// 永久序列化的场合压缩保存
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        protected T FromZipBytes<T>(byte[] bytes)
        {
            return BinaryHelper.FromBytes<T>(bytes, true);
        }
        #endregion

        /// <summary>
        /// 获取应用下的完整路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected static string CreateFullPath(string path)
        {
            var root = StaticHelper.GetFullRootPath();
            var list = new List<string>
            {
                root
            };

            var folders = path.Trim('\\', '/').Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            list.AddRange(folders);
            return Path.Combine(list.ToArray());
        }

        private const int BUFFER_SIZE = 4096;
        protected void Backup<TKey, TValue>(SortedDictionary<TKey, TValue> source, string filename)
        {
            try
            {
                if (source == null)
                {
                    return;
                }

                if (File.Exists(filename))
                {
                    File.Delete(filename);
                }

                using (var stream = new MemoryStream())
                {
                    var bfm = new BinaryFormatter();
                    bfm.Serialize(stream, source);
                    stream.Seek(0, SeekOrigin.Begin);

                    var length = (int)stream.Length;
                    using (var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        var read = 0;
                        while (read < length)
                        {
                            var next = length - read < BUFFER_SIZE ? length - read : BUFFER_SIZE;
                            var wd = new byte[next];

                            stream.Read(wd, 0, next);
                            fs.Write(wd, 0, next);
                            fs.Flush();

                            read += next;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogsWrite("ErrorBackup", ex.ToString());
            }
        }

        protected SortedDictionary<TKey, TValue> Restore<TKey, TValue>(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    var fs = File.OpenRead(path);
                    var count = fs.Length;
                    var bytes = new byte[count];

                    fs.Read(bytes, 0, (int)count);
                    fs.Close();

                    using (var ms = new MemoryStream(bytes))
                    {
                        var transfer = new BinaryFormatter();

                        var target = (SortedDictionary<TKey, TValue>)transfer.Deserialize(ms);
                        return target;
                    }
                }
                catch (Exception ex)
                {
                    LogsWrite("ErrorRestore", ex.ToString());
                }
            }

            return new SortedDictionary<TKey, TValue>();
        }
    }
}
