using System;

namespace BaseUnits.Core.Cache
{
    /// <summary>
    /// Cache Int Base
    /// </summary>
    [Serializable]
    public abstract class CacheByIntBase : CacheDefinedBase
    {
        /// <summary>
        /// Tag
        /// </summary>
        public int Tag { get; set; }

        public override CacheDefinedEnum Cached
        {
            get
            {
                return CacheDefinedEnum.Int;
            }
        }
    }
}
