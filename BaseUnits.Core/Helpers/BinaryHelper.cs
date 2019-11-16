using System;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BaseUnits.Core.Helpers
{
    /// <summary>
    /// 流
    /// </summary>
    public abstract class BinaryHelper
    {
        /// <summary>
        /// 数据备份
        /// </summary>
        /// <param name="source"></param>
        /// <param name="path"></param>
        /// <param name="gzip"></param>
        public static void Backup(object source, string path, bool gzip = false)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                if (source == null)
                {
                    return;
                }

                var bytes = ToBytes(source, gzip);
                File.WriteAllBytes(path, bytes);
            }
            catch (Exception ex)
            {
                LogsHelper.Error("Backup", ex);
            }
        }

        /// <summary>
        /// 数据还原
        /// </summary>
        /// <param name="path"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static T Restore<T>(string path, bool gzip = false)
        {
            if (File.Exists(path))
            {
                try
                {
                    var bytes = File.ReadAllBytes(path);
                    return FromBytes<T>(bytes, gzip);
                }
                catch (Exception ex)
                {
                    LogsHelper.Error("BinaryHelper.RestoreT", ex);
                }
            }

            return default(T);
        }

        /// <summary>
        /// 获取文本文件内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string RestoreText(string path, string encoding = "UTF-8")
        {
            if (File.Exists(path))
            {
                try
                {
                    var encode = Encoding.GetEncoding(encoding);
                    using (var stream = File.OpenRead(path))
                    {
                        var sdata = new byte[stream.Length];
                        stream.Read(sdata, 0, sdata.Length);
                        stream.Close();
                        return encode.GetString(sdata);
                    }
                }
                catch (Exception ex)
                {
                    LogsHelper.Error("RestoreText", ex);
                }
            }

            return null;
        }

        #region .Bytes Transfer.
        //private static readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="data"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static byte[] ToBytes(object data, bool gzip = false)
        {
            if (data == null)
            {
                return new byte[0];
            }

            try
            {
                using (var ms = new MemoryStream())
                {
                    var binaryFormatter = new BinaryFormatter
                    {
                        AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                        Binder = new DeserializationAppDomainBinder()
                    };
                    binaryFormatter.Serialize(ms, data);

                    var bytes = ms.ToArray();
                    if (gzip)
                    {
                        using (var gms = new MemoryStream())
                        {
                            using (var gz = new GZipStream(gms, CompressionMode.Compress))
                            {
                                gz.Write(bytes, 0, bytes.Length);
                                gz.Close();
                            }

                            return gms.ToArray();
                        }
                    }

                    return bytes;
                }
            }
            catch (Exception ex)
            {
                LogsHelper.Error("ToBytes", ex);
            }

            return new byte[0];
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="gzip"></param>
        /// <returns></returns>
        public static T FromBytes<T>(byte[] bytes, bool gzip = false)
        {
            try
            {
                if (bytes == null || bytes.Length == 0)
                {
                    return default(T);
                }

                if (gzip)
                {
                    using (var gz = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress))
                    {
                        var buffer = new byte[4096];
                        using (var ms = new MemoryStream())
                        {
                            int offset;
                            while ((offset = gz.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                // 解压后的数据写入内存流
                                ms.Write(buffer, 0, offset);
                            }

                            var bf = new BinaryFormatter();
                            ms.Position = 0;
                            return (T)bf.Deserialize(ms);
                        }
                    }
                }

                // Create a MemoryStream to convert bytes to a stream
                using (var ms = new MemoryStream(bytes))
                {
                    // Go to head of the stream
                    ms.Position = 0;

                    // Deserialize the message
                    var bf = new BinaryFormatter
                    {
                        AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                        Binder = new DeserializationAppDomainBinder()
                    };

                    // Return the deserialized message
                    return (T)bf.Deserialize(ms);
                }
            }
            catch (Exception ex)
            {
                LogsHelper.Error("BinaryHelper.FromBytesT", ex);
            }
            return default(T);
        }

        /// <summary>
        /// DeserializationAppDomainBinder
        /// </summary>
        private sealed class DeserializationAppDomainBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                var toAssemblyName = assemblyName.Split(',')[0];
                return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                        where assembly.FullName.Split(',')[0] == toAssemblyName
                        select assembly.GetType(typeName)).FirstOrDefault();
            }
        }

        public static byte[] ToBytesDeflate(object data)
        {
            if (data == null)
            {
                return new byte[0];
            }

            try
            {
                var bytes = ToBytes(data);
                using (var gms = new MemoryStream())
                {
                    using (var gz = new DeflateStream(gms, CompressionMode.Compress))
                    {
                        gz.Write(bytes, 0, bytes.Length);
                        gz.Close();
                    }

                    return gms.ToArray();
                }
            }
            catch (Exception ex)
            {
                LogsHelper.Error("ToBytesDeflate", ex);
            }

            return new byte[0];
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static T FromBytesDeflate<T>(byte[] bytes)
        {
            try
            {
                if (bytes == null || bytes.Length == 0)
                {
                    return default(T);
                }

                var gz = new DeflateStream(new MemoryStream(bytes), CompressionMode.Decompress);
                var buffer = new byte[4096];
                using (var ms = new MemoryStream())
                {
                    int offset;
                    while ((offset = gz.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        // 解压后的数据写入内存流
                        ms.Write(buffer, 0, offset);
                    }

                    var bf = new BinaryFormatter();
                    ms.Position = 0;
                    return (T)bf.Deserialize(ms);
                }
            }
            catch (Exception ex)
            {
                LogsHelper.Error("BinaryHelper.FromBytesDeflateT", ex);
            }
            return default(T);
        }
        #endregion
    }
}
