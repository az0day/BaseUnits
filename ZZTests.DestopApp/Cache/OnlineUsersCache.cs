using BaseUnits.Core.Service;
using ZZTests.DestopApp.Entities;

namespace ZZTests.DestopApp.Cache
{
    class OnlineUsersCache : BaseServiceWithMessage
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static readonly OnlineUsersCache Instance = new OnlineUsersCache();

        private IRunningCache _cache = new DefaultRunningCache();

        public int Count => _cache.Count;

        public void AddOrUpdate(OnlineUser user)
        {
            _cache.AddOrUpdate(user.LoginName, user);
        }

        public bool TryRemove(string loginName)
        {
            return _cache.TryRemove(loginName);
        }
    }
}
