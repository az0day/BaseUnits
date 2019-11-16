using System;

namespace BaseUnits.Core.Cache
{
    /// <summary>
    /// Cache Defined Base
    /// </summary>
    [Serializable]
    public abstract class CacheDefinedBase
    {
        /// <summary>
        /// Ticks
        /// </summary>
        public long Ticks { get; set; } = DateTime.Now.Ticks;

        /// <summary>
        /// Data Cached Type
        /// </summary>
        public abstract CacheDefinedEnum Cached { get; }
    }
}
