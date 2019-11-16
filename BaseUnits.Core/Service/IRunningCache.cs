using System;
using System.Collections.Generic;

namespace BaseUnits.Core.Service
{
    /// <summary>
    /// 运行时缓存接口
    /// </summary>
    public interface IRunningCache
    {
        /// <summary>
        /// 个数
        /// </summary>
        int Count { get; }

        long Hits { get; }
        long Miss { get; }

        IDictionary<string, Type> GetTypes();

        /// <summary>
        /// 永久缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void Forever<T>(string key, T data);

        /// <summary>
        /// 读取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TryGet<T>(string key, out T data);

        /// <summary>
        /// 删除一项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool TryRemove(string key);

        /// <summary>
        /// 完整复制
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        bool TryClone<T>(string key, out T data);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="seconds">默认值 10s</param>
        void AddOrUpdate<T>(string key, T data, int seconds = 10);
    }
}
