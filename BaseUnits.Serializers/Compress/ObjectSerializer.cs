namespace BaseUnits.Serializers.Compress
{
    public class ObjectSerializer : ISerializer
    {
        public T FromBytes<T>(byte[] bytes)
        {
            return bytes.FromObjectSerializerBytes<T>();
        }

        public byte[] ToBytes<T>(T data)
        {
            return data.ToObjectSerializerBytes();
        }
    }
}
