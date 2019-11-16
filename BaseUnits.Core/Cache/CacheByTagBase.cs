using System;

namespace BaseUnits.Core.Cache
{
    /// <summary>
    /// Cache Tag Base
    /// </summary>
    [Serializable]
    public abstract class CacheByTagBase : CacheDefinedBase
    {
        /// <summary>
        /// Tag
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Cached Key Type
        /// </summary>
        public override CacheDefinedEnum Cached
        {
            get
            {
                return CacheDefinedEnum.String;
            }
        }
    }
}
