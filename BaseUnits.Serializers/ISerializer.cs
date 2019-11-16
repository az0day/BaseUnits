namespace BaseUnits.Serializers
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// 序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        byte[] ToBytes<T>(T data);

        /// <summary>
        /// 转换回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes"></param>
        /// <returns></returns>
        T FromBytes<T>(byte[] bytes);
    }
}
