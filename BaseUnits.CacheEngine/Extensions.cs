using BaseUnits.Core.Helpers;

namespace BaseUnits.CacheEngine
{
    public static class Extensions
    {
        public static byte[] ToGzipBytes<T>(this T data)
        {
            return BinaryHelper.ToBytes(data, true);
        }

        public static T FromGzipBytes<T>(this byte[] bytes)
        {
            return BinaryHelper.FromBytes<T>(bytes, true);
        }
    }
}
