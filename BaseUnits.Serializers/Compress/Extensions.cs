using System.Linq;
using Snappy.Sharp;
using BaseUnits.Core.Helpers;

namespace BaseUnits.Serializers.Compress
{
    public static class Extensions
    {
        #region .Binary Bytes.
        public static byte[] ToBinaryBytes(this object data)
        {
            return BinaryHelper.ToBytes(data);
        }

        public static T FromBinaryBytes<T>(this byte[] bytes)
        {
            return BinaryHelper.FromBytes<T>(bytes);
        }
        #endregion

        #region .Snappy Sharp Bytes.
        public static byte[] ToSnappySharpBytes(this object data)
        {
            var bytes = data.ToBinaryBytes();
            return bytes.ToSnappyCompress();
        }

        public static T FromSnappySharpBytes<T>(this byte[] bytes)
        {
            var target = bytes.FromSnappyCompress();
            return target.FromBinaryBytes<T>();
        }

        private static byte[] ToSnappyCompress(this byte[] bytes)
        {
            var originMax = bytes.Length;
            var snappy = new SnappyCompressor();
            var result = new byte[snappy.MaxCompressedLength(originMax)];
            var sized = snappy.Compress(bytes, 0, originMax, result);
            return result.Take(sized).ToArray();
        }

        private static byte[] FromSnappyCompress(this byte[] bytes)
        {
            var length = bytes.Length;
            var decomperssor = new SnappyDecompressor();
            return decomperssor.Decompress(bytes, 0, length);
        }
        #endregion

        #region .Snappy Sharp Bytes.
        public static byte[] ToObjectSerializerBytes(this object data)
        {
            var serializer = new ObjectSerialization.ObjectSerializer();
            var bytes = serializer.Serialize(data);
            return bytes.ToSnappyCompress();
        }

        public static T FromObjectSerializerBytes<T>(this byte[] bytes)
        {
            var serializer = new ObjectSerialization.ObjectSerializer();
            var target = bytes.FromSnappyCompress();
            return serializer.Deserialize<T>(target);
        }
        #endregion
    }
}
