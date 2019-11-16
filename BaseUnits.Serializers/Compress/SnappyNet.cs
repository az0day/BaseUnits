namespace BaseUnits.Serializers.Compress
{
    /// <summary>
    /// Snappy.Net
    /// </summary>
    public sealed class SnappyNet : ISerializer
    {
        public T FromBytes<T>(byte[] bytes)
        {
            return bytes.FromSnappySharpBytes<T>();
        }

        public byte[] ToBytes<T>(T data)
        {
            return data.ToSnappySharpBytes();
        }
    }
}
