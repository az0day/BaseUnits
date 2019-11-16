using BaseUnits.Core.Cache;
using BaseUnits.CacheEngine.Implements;

namespace BaseUnits.CacheEngine
{
    public sealed class Factory
    {
        public static ICacheStorage CreateDBreeze(string folder)
        {
            return new CacheStorageDBreeze(folder);
        }
    }
}
