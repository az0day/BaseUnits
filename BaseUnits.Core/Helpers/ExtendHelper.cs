using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BaseUnits.Core.Entities;

namespace BaseUnits.Core.Helpers
{
    public static class ExtendHelper
    {
        /// <summary>
        /// 实现 Contains 的模糊查询
        /// </summary>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ContainsKey(this string source, string key)
        {
            return source.IndexOf(key, StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        /// <summary>
        /// Parse Ip
        /// </summary>
        /// <param name="empty"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string EmptyWithReplace(this string empty, string target)
        {
            return string.IsNullOrEmpty(empty) ? target : empty;
        }

        private static Tuple<PageInfo, IList<T>> PagerTuple<T>(
            this IEnumerable<T> query,
            int pageNow,
            int pageSize,
            int order = 0,
            Func<T, dynamic> sorting = null,
            bool descending = false
        )
        {
            var list = query as IList<T> ?? query.ToList();
            var count = list.Count();
            var page = DefaultValues.CalculatePageInfo(count, pageNow, pageSize, order);
            var start = (page.PageNow - 1) * pageSize;

            if (count <= 0)
            {
                return new Tuple<PageInfo, IList<T>>(page, new List<T>());
            }

            if (sorting != null)
            {
                if (descending)
                {
                    // 降序
                    var d = list.OrderByDescending(sorting)
                        .Skip(start)
                        .Take(pageSize
                    ).ToList();

                    return new Tuple<PageInfo, IList<T>>(page, d);
                }
                else
                {
                    // 升序
                    var d = list.OrderBy(sorting)
                        .Skip(start)
                        .Take(pageSize
                    ).ToList();

                    return new Tuple<PageInfo, IList<T>>(page, d);
                }
            }

            return new Tuple<PageInfo, IList<T>>(page, list.Skip(start).Take(pageSize).ToList());
        }

        public static PagedItems<T> PagedItems<T>(
            this IEnumerable<T> query,
            int pageNow,
            int pageSize,
            int order = 0,
            Func<T, dynamic> sorting = null,
            bool isDescending = false
            )
        {
            var tpl = query.PagerTuple(pageNow, pageSize, order, sorting, isDescending);
            return new PagedItems<T>
            {
                Page = tpl.Item1,
                Items = tpl.Item2,
            };
        }

        private static string ToMd5(this Stream stream)
        {
            return StaticHelper.Md5(stream);
        }

        private static string ToSha1(this Stream stream)
        {
            return StaticHelper.Sha1(stream);
        }

        /// <summary>
        /// 生成唯一标识
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ToUniqueHash(this Stream stream)
        {
            var md5 = stream.ToMd5();
            var sha1 = stream.ToSha1();
            return string.Format("{0}{1}", md5, sha1.Substring(0, 8));
        }

        #region .JSON.
        /// <summary>
        /// 转为Json
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ToJson(this object data)
        {
            return StaticHelper.ToJson(data);
        }

        /// <summary>
        /// 将字符串转为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ParseByJson<T>(this string json)
        {
            return StaticHelper.ParseByJson<T>(json);
        }

        #endregion
    }
}
