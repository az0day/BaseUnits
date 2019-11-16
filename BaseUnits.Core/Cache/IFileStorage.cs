using System;
using System.Collections.Generic;
using System.IO;

namespace BaseUnits.Core.Cache
{
    /// <summary>
    /// IFileStorage
    /// </summary>
    public interface IFileStorage : IDisposable
    {
        /// <summary>
        /// Add Or Update by tag
        /// </summary>
        /// <param name="tag">tag-filename</param>
        /// <param name="stream"></param>
        /// <returns></returns>
        CacheFileEntity AddOrUpdate(string tag, Stream stream);

        /// <summary>
        /// Query by did
        /// </summary>
        /// <param name="did"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        CacheFileEntity QueryItem(Guid did, Stream stream);

        /// <summary>
        /// Top Items
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        IList<CacheFileEntity> TopItems(int take);
    }
}
