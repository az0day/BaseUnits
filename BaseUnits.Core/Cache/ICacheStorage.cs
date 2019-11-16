using System;
using System.Collections.Generic;

namespace BaseUnits.Core.Cache
{
    /// <summary>
    /// ICacheStorage
    /// </summary>
    public interface ICacheStorage : IDisposable
    {
        bool InsertItem<TKey, TValue>(string table, TKey key, TValue value) where TKey : IComparable<TKey>;
        bool InsertItems<TKey, TValue>(string table, IDictionary<TKey, TValue> list) where TKey : IComparable<TKey>;

        TValue QueryItem<TKey, TValue>(string table, TKey key) where TKey : IComparable<TKey>;

        /// <summary>
        /// 获取不重复数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="table"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        IDictionary<TKey, TValue> QueryItems<TKey, TValue>(string table, IList<TKey> keys) where TKey : IComparable<TKey>;

        /// <summary>
        /// (不重复数据)获取前几条
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="table"></param>
        /// <param name="desc"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IDictionary<TKey, TValue> TopValues<TKey, TValue>(string table, bool desc, int take) where TKey : IComparable<TKey>;

        /// <summary>
        /// 获取前几条 Keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="table"></param>
        /// <param name="desc"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IList<TKey> TopKeys<TKey, TValue>(string table, bool desc, int take) where TKey : IComparable<TKey>;

        bool ExistsItem<TKey, TValue>(string table, TKey key) where TKey : IComparable<TKey>;

        /// <summary>
        /// 给定顺序分页
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="table"></param>
        /// <param name="from">起始 Key (包含在结果中)</param>
        /// <param name="take"></param>
        /// <returns></returns>
        IList<KeyValuePair<TKey, TValue>> BackwardValues<TKey, TValue>(string table, TKey from, int take) where TKey : IComparable<TKey>;

        /// <summary>
        /// 给定顺序分页
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="table"></param>
        /// <param name="from">起始 Key (包含在结果中)</param>
        /// <param name="take"></param>
        /// <returns></returns>
        IList<KeyValuePair<TKey, TValue>> ForwardValues<TKey, TValue>(string table, TKey from, int take) where TKey : IComparable<TKey>;

        long Count<TKey, TValue>(string table) where TKey : IComparable<TKey>;
        TKey Min<TKey, TValue>(string table) where TKey : IComparable<TKey>;
        TKey Max<TKey, TValue>(string table) where TKey : IComparable<TKey>;

        void Export<TKey, TValue>(string table, string fileName) where TKey : IComparable<TKey>;
        void Restore<TKey, TValue>(string table, string fileName) where TKey : IComparable<TKey>;

        bool Remove<TKey, TValue>(string table, TKey key, out TValue value) where TKey : IComparable<TKey>;
        void Remove<TKey>(string table, IList<TKey> keys) where TKey : IComparable<TKey>;
    }
}
