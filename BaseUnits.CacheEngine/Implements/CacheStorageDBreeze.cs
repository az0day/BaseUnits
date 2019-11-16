using System;
using System.Collections.Generic;

using DBreeze;
using BaseUnits.CacheEngine.Inner;
using BaseUnits.Core.Cache;

namespace BaseUnits.CacheEngine.Implements
{
    class CacheStorageDBreeze : HelperBase, ICacheStorage
    {
        private readonly DBreezeEngine _engine;
        private readonly object _lock = new object();
        private readonly Type _btype = (new byte[0]).GetType();

        public CacheStorageDBreeze(string folder)
        {
            _engine = new DBreezeEngine(folder);
        }

        protected override string Title
        {
            get
            {
                return "CacheDBreeze";
            }
        }

        private bool IsValueType<TValue>()
        {
            var type = typeof(TValue);
            if (_btype == type)
            {
                return true;
            }

            return type.IsValueType;
        }

        #region .Implements.

        public bool InsertItem<TKey, TValue>(string table, TKey key, TValue value) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        tran.Insert(table, key, value);
                        tran.Commit();
                    }
                    return true;
                }
            }

            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    tran.Insert(table, key, ToZipBytes(value));
                    tran.Commit();
                }
                return true;
            }
        }

        public bool InsertItems<TKey, TValue>(string table, IDictionary<TKey, TValue> list) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        foreach (var item in list)
                        {
                            tran.Insert(table, item.Key, item.Value);
                        }

                        tran.Commit();
                    }
                    return true;
                }
            }

            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var item in list)
                    {
                        tran.Insert(table, item.Key, ToZipBytes(item.Value));
                    }

                    tran.Commit();
                }
                return true;
            }
        }

        public TValue QueryItem<TKey, TValue>(string table, TKey key) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        var row = tran.Select<TKey, TValue>(table, key);
                        if (row.Exists)
                        {
                            return row.Value;
                        }
                    }

                    return default(TValue);
                }
            }

            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<TKey, byte[]>(table, key);
                    if (row.Exists)
                    {
                        return FromZipBytes<TValue>(row.Value);
                    }
                }

                return default(TValue);
            }
        }

        public IDictionary<TKey, TValue> QueryItems<TKey, TValue>(string table, IList<TKey> keys) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    var list = new Dictionary<TKey, TValue>();
                    using (var tran = _engine.GetTransaction())
                    {
                        foreach (var key in keys)
                        {
                            var row = tran.Select<TKey, TValue>(table, key);
                            if (row.Exists)
                            {
                                list[key] = row.Value;
                                continue;
                            }

                            list[key] = default(TValue);
                        }
                    }

                    return list;
                }
            }

            lock (_lock)
            {
                var list = new Dictionary<TKey, TValue>();
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var key in keys)
                    {
                        var row = tran.Select<TKey, byte[]>(table, key);
                        if (row.Exists)
                        {
                            list[key] = FromZipBytes<TValue>(row.Value);
                            continue;
                        }

                        list[key] = default(TValue);
                    }
                }

                return list;
            }
        }

        public IDictionary<TKey, TValue> TopValues<TKey, TValue>(string table, bool desc, int take) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    var list = new Dictionary<TKey, TValue>();
                    var index = 0;
                    using (var tran = _engine.GetTransaction())
                    {
                        if (desc)
                        {
                            foreach (var d in tran.SelectBackward<TKey, TValue>(table))
                            {
                                list[d.Key] = d.Value;
                                index++;
                                if (take > 0 && index >= take)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var d in tran.SelectForward<TKey, TValue>(table))
                            {
                                list[d.Key] = d.Value;
                                index++;
                                if (take > 0 && index >= take)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    return list;
                }
            }

            lock (_lock)
            {
                var list = new Dictionary<TKey, TValue>();
                var index = 0;
                using (var tran = _engine.GetTransaction())
                {
                    if (desc)
                    {
                        foreach (var d in tran.SelectBackward<TKey, byte[]>(table))
                        {
                            list[d.Key] = FromZipBytes<TValue>(d.Value);
                            index++;
                            if (take > 0 && index >= take)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var d in tran.SelectForward<TKey, byte[]>(table))
                        {
                            list[d.Key] = FromZipBytes<TValue>(d.Value);
                            index++;
                            if (take > 0 && index >= take)
                            {
                                break;
                            }
                        }
                    }
                }

                return list;
            }
        }

        public IList<TKey> TopKeys<TKey, TValue>(string table, bool desc, int take) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    var list = new List<TKey>();
                    var index = 0;

                    using (var tran = _engine.GetTransaction())
                    {
                        if (desc)
                        {
                            foreach (var d in tran.SelectBackward<TKey, TValue>(table))
                            {
                                list.Add(d.Key);
                                index++;
                                if (take > 0 && index >= take)
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (var d in tran.SelectForward<TKey, TValue>(table))
                            {
                                list.Add(d.Key);
                                index++;
                                if (take > 0 && index >= take)
                                {
                                    break;
                                }
                            }
                        }
                    }

                    return list;
                }
            }

            lock (_lock)
            {
                var list = new List<TKey>();
                var index = 0;

                using (var tran = _engine.GetTransaction())
                {
                    if (desc)
                    {
                        foreach (var d in tran.SelectBackward<TKey, byte[]>(table))
                        {
                            list.Add(d.Key);
                            index++;
                            if (take > 0 && index >= take)
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        foreach (var d in tran.SelectForward<TKey, byte[]>(table))
                        {
                            list.Add(d.Key);
                            index++;
                            if (take > 0 && index >= take)
                            {
                                break;
                            }
                        }
                    }
                }

                return list;
            }
        }

        public bool ExistsItem<TKey, TValue>(string table, TKey key) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        var row = tran.Select<TKey, TValue>(table, key);
                        return row.Exists;
                    }
                }

            }

            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<TKey, byte[]>(table, key);
                    return row.Exists;
                }
            }
        }

        public IList<KeyValuePair<TKey, TValue>> BackwardValues<TKey, TValue>(string table, TKey from, int take) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    var list = new List<KeyValuePair<TKey, TValue>>();
                    var index = 0;
                    using (var tran = _engine.GetTransaction())
                    {
                        foreach (var d in tran.SelectBackwardStartFrom<TKey, TValue>(table, from, true))
                        {
                            var kv = new KeyValuePair<TKey, TValue>(d.Key, d.Value);
                            list.Add(kv);
                            index++;
                            if (take > 0 && index >= take)
                            {
                                break;
                            }
                        }
                    }

                    return list;
                }
            }

            lock (_lock)
            {
                var list = new List<KeyValuePair<TKey, TValue>>();
                var index = 0;
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var d in tran.SelectBackwardStartFrom<TKey, byte[]>(table, from, true))
                    {
                        var kv = new KeyValuePair<TKey, TValue>(d.Key, FromZipBytes<TValue>(d.Value));
                        list.Add(kv);
                        index++;
                        if (take > 0 && index >= take)
                        {
                            break;
                        }
                    }
                }

                return list;
            }
        }

        public IList<KeyValuePair<TKey, TValue>> ForwardValues<TKey, TValue>(string table, TKey from, int take) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    var list = new List<KeyValuePair<TKey, TValue>>();
                    var index = 0;
                    using (var tran = _engine.GetTransaction())
                    {
                        foreach (var d in tran.SelectForwardStartFrom<TKey, TValue>(table, from, true))
                        {
                            var kv = new KeyValuePair<TKey, TValue>(d.Key, d.Value);
                            list.Add(kv);
                            index++;
                            if (take > 0 && index >= take)
                            {
                                break;
                            }
                        }
                    }

                    return list;
                }
            }

            lock (_lock)
            {
                var list = new List<KeyValuePair<TKey, TValue>>();
                var index = 0;
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var d in tran.SelectForwardStartFrom<TKey, byte[]>(table, from, true))
                    {
                        var kv = new KeyValuePair<TKey, TValue>(d.Key, FromZipBytes<TValue>(d.Value));
                        list.Add(kv);
                        index++;
                        if (take > 0 && index >= take)
                        {
                            break;
                        }
                    }
                }

                return list;
            }
        }

        public long Count<TKey, TValue>(string table) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        return Convert.ToInt64(tran.Count(table));
                    }
                }
            }

            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    return Convert.ToInt64(tran.Count(table));
                }
            }
        }

        public TKey Min<TKey, TValue>(string table) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        var row = tran.Min<TKey, TValue>(table);
                        if (row.Exists)
                        {
                            return row.Key;
                        }

                        return default(TKey);
                    }
                }
            }

            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Min<TKey, byte[]>(table);
                    if (row.Exists)
                    {
                        return row.Key;
                    }

                    return default(TKey);
                }
            }
        }

        public TKey Max<TKey, TValue>(string table) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        var row = tran.Max<TKey, TValue>(table);
                        if (row.Exists)
                        {
                            return row.Key;
                        }

                        return default(TKey);
                    }
                }
            }

            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Max<TKey, byte[]>(table);
                    if (row.Exists)
                    {
                        return row.Key;
                    }

                    return default(TKey);
                }
            }
        }

        public void Export<TKey, TValue>(string table, string fileName) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            var items = new SortedDictionary<TKey, TValue>();

            if (isValue)
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        foreach (var d in tran.SelectForward<TKey, TValue>(table))
                        {
                            items.Add(d.Key, d.Value);
                        }
                    }
                }
            }
            else
            {
                lock (_lock)
                {
                    using (var tran = _engine.GetTransaction())
                    {
                        foreach (var d in tran.SelectForward<TKey, byte[]>(table))
                        {
                            items.Add(d.Key, FromZipBytes<TValue>(d.Value));
                        }
                    }
                }
            }

            Backup(items, fileName);
        }

        public void Restore<TKey, TValue>(string table, string fileName) where TKey : IComparable<TKey>
        {
            var items = Restore<TKey, TValue>(fileName);
            if (items.Count > 0)
            {
                InsertItems(table, items);
            }
        }

        public bool Remove<TKey, TValue>(string table, TKey key, out TValue value) where TKey : IComparable<TKey>
        {
            var isValue = IsValueType<TValue>();
            if (isValue)
            {
                lock (_lock)
                {
                    value = default(TValue);
                    using (var tran = _engine.GetTransaction())
                    {
                        var row = tran.Select<TKey, TValue>(table, key);
                        if (row.Exists)
                        {
                            value = row.Value;
                            tran.RemoveKey(table, key);
                            tran.Commit();
                            return true;
                        }

                        return false;
                    }
                }
            }

            lock (_lock)
            {
                value = default(TValue);
                using (var tran = _engine.GetTransaction())
                {
                    var row = tran.Select<TKey, byte[]>(table, key);
                    if (row.Exists)
                    {
                        value = FromZipBytes<TValue>(row.Value);
                        tran.RemoveKey(table, key);
                        tran.Commit();
                        return true;
                    }

                    return false;
                }
            }
        }

        public void Remove<TKey>(string table, IList<TKey> keys) where TKey : IComparable<TKey>
        {
            lock (_lock)
            {
                using (var tran = _engine.GetTransaction())
                {
                    foreach (var key in keys)
                    {
                        tran.RemoveKey(table, key);
                    }

                    tran.Commit();
                }
            }
        }

        public void Dispose()
        {
            _engine?.Dispose();
        }
        #endregion
    }
}
