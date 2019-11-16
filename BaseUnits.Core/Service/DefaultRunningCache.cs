using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using BaseUnits.Core.Helpers;

namespace BaseUnits.Core.Service
{
    /// <summary>
    /// 运行时缓存
    /// </summary>
    public class DefaultRunningCache : IDisposable, IRunningCache
    {
        protected struct CacheContext
        {
            public string Key;

            public object Data;

            public DateTime ExpireTime;
        }

        public DefaultRunningCache()
        {
            Enable = true;
            StartCleanTimer();
        }

        #region .Privates.
        private readonly ConcurrentDictionary<string, CacheContext> _cached = new ConcurrentDictionary<string, CacheContext>();

        private long _hits;

        private long _miss;
        #endregion

        #region .Timer.
        /// <summary>
        /// 用来等待及退出
        /// </summary>
        private readonly ManualResetEvent _waitEvent = new ManualResetEvent(false);
        private Thread _cleanThread;

        /// <summary>
        /// 每 5 分钟清理一次
        /// </summary>
        private const int EXPIRE_CLEAN_INTVAL = 1000 * 60 * 5;

        private void CleanTimer()
        {
            bool ret;
            do
            {
                ret = _waitEvent.WaitOne(EXPIRE_CLEAN_INTVAL);
                if (!ret)
                {
                    RemoveExpiredItems();
                }
            } while (!ret);
        }

        private void RemoveExpiredItems()
        {
            var items = _cached.Values;
            var now = DateTime.Now;

            foreach (var item in items)
            {
                if (now > item.ExpireTime)
                {
                    _cached.TryRemove(item.Key, out var _);
                }
            }
        }

        private void StartCleanTimer()
        {
            _cleanThread = new Thread(CleanTimer)
            {
                IsBackground = true
            };
            _cleanThread.Start();
        }

        private void StopCleanTimer()
        {
            if (_cleanThread != null)
            {
                _waitEvent.Set();
                _cleanThread.Join();
                _cleanThread = null;
            }
        }
        #endregion

        #region .Properties.
        public int Count
        {
            get
            {
                return _cached.Count;
            }
        }

        public long Hits
        {
            get
            {
                return _hits;
            }
        }

        public long Miss
        {
            get
            {
                return _miss;
            }
        }

        public bool Enable { get; set; }
        #endregion

        #region .Dispose.
        private bool _disposed;

        ~DefaultRunningCache()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                StopCleanTimer();
            }

            _disposed = true;
        }
        #endregion

        #region .Implements.
        /// <summary>
        /// 缓存秒数, &lt;=0: 50 年, 0: default seconds
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        private DateTime CreateExpireTime(int seconds)
        {
            var now = DateTime.Now;
            if (seconds <= 0)
            {
                return now.AddYears(50);
            }

            return now.AddSeconds(seconds);
        }

        public IDictionary<string, Type> GetTypes()
        {
            return _cached.ToDictionary(v => v.Key, v => v.Value.Data.GetType());
        }

        public void Forever<T>(string key, T data)
        {
            AddOrUpdate(key, data, -1);
        }

        /// <summary>
        /// 新增或更新缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="seconds">缓存秒数, -1: forever, 0: default seconds</param>
        public void AddOrUpdate<T>(string key, T data, int seconds = 10)
        {
            if (!Enable)
            {
                return;
            }

            var expire = CreateExpireTime(seconds);
            var cache = new CacheContext
            {
                Key = key,
                Data = data,
                ExpireTime = expire
            };

            _cached.AddOrUpdate(key, cache, (okey, value) =>
            {
                value.Data = cache.Data;
                value.ExpireTime = cache.ExpireTime;
                return value;
            });
        }        

        /// <summary>
        /// 删除一项
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryRemove(string key)
        {
            return _cached.TryRemove(key, out var _);
        }

        /// <summary>
        /// 取出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryGet<T>(string key, out T data)
        {
            data = default(T);
            if (!Enable)
            {
                return false;
            }

            //var removed = false;
            var found = _cached.TryGetValue(key, out var cache);
            if (found)
            {
                if (DateTime.Now > cache.ExpireTime)
                {
                    //removed = _cached.TryRemove(key, out cache);
                    found = false;
                }
            }

            if (found)
            {
                data = (T)cache.Data;
                Interlocked.Increment(ref _hits);
            }
            else
            {
                Interlocked.Increment(ref _miss);
            }

            return found;
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool TryClone<T>(string key, out T data)
        {
            if(TryGet(key, out data))
            {
                var bytes = BinaryHelper.ToBytes(data);
                data = BinaryHelper.FromBytes<T>(bytes);
                return true;
            }

            data = default(T);
            return false;
        }
        #endregion
    }
}
