using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using LZ4;
using Snappy;
using Snappy.Sharp;
using ZZXUnit.Tests.Entities;

namespace ZZXUnit.Tests.Serializers
{
    static class Extensions
    {
        public static byte[] ToLz4Bytes(this object data)
        {
            var bytes = data.ToBinaryBytes();
            using (var ms = new MemoryStream())
            {
                using (var lz4 = new LZ4Stream(ms, LZ4StreamMode.Compress))
                {
                    lz4.Write(bytes, 0, bytes.Length);
                    lz4.Close();

                    return ms.ToArray();
                }
            }
        }

        public static T FromLz4Bytes<T>(this byte[] bytes)
        {
            using (var gz = new LZ4Stream(new MemoryStream(bytes), LZ4StreamMode.Decompress))
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

                    ms.Position = 0;

                    // Deserialize the message
                    var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
                    {
                        AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                        Binder = new DeserializationAppDomainBinder()
                    };

                    return (T)bf.Deserialize(ms);
                }
            }
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

        public static byte[] ToSnappySharpBytes(this object data)
        {
            var bytes = data.ToBinaryBytes();
            var originMax = bytes.Length;
            var snappy = new SnappyCompressor();
            var result = new byte[snappy.MaxCompressedLength(originMax)];
            var sized = snappy.Compress(bytes, 0, originMax, result);
            return result.Take(sized).ToArray();
        }

        public static T FromSnappySharpBytes<T>(this byte[] bytes)
        {
            var length = bytes.Length;
            var decomperssor = new SnappyDecompressor();
            var target = decomperssor.Decompress(bytes, 0, length);
            return target.FromBinaryBytes<T>();
        }

        public static byte[] ToSnappyStandardBytes(this object data)
        {
            var bytes = data.ToBinaryBytes();
            return SnappyCodec.Compress(bytes);
        }

        public static T FromSnappyStandardBytes<T>(this byte[] bytes)
        {
            var target = SnappyCodec.Uncompress(bytes);
            return target.FromBinaryBytes<T>();
        }


        public static byte[] ToObjectSerializerBytes(this object data)
        {
            var serializer = new ObjectSerialization.ObjectSerializer();
            var bytes = serializer.Serialize(data);
            return SnappyCodec.Compress(bytes);
        }

        public static T FromObjectSerializerBytes<T>(this byte[] bytes)
        {
            var serializer = new ObjectSerialization.ObjectSerializer();
            var target = SnappyCodec.Uncompress(bytes);
            return serializer.Deserialize<T>(target);
        }
    }
}
