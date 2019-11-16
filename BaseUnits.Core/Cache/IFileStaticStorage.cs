using System;
using System.IO;

namespace BaseUnits.Core.Cache
{
    /// <summary>
    /// IFileStorage
    /// </summary>
    public interface IFileStaticStorage : IDisposable
    {
        /// <summary>
        /// Add Or Update
        /// </summary>
        /// <param name="file"></param>
        /// <param name="tag"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        CacheFileEntity AddOrUpdate(string file, string tag, Stream stream);

        /// <summary>
        /// Query Item
        /// </summary>
        /// <param name="file"></param>
        /// <param name="did"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        CacheFileEntity QueryItem(string file, Guid did, Stream stream);
    }
}
