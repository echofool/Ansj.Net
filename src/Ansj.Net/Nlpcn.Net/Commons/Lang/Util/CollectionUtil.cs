using System.Collections.Generic;

namespace Nlpcn.Net.Commons.Lang.Util
{
    public class CollectionUtil
    {
        /// <summary>
        /// map 按照value排序
        /// </summary>
        /// <typeparam name="TK"></typeparam>
        /// <typeparam name="TV"></typeparam>
        /// <param name="map"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static List<KeyValuePair<TK, TV>> SortMapByValue<TK, TV>(IEnumerable<KeyValuePair<TK, TV>> map, int sort)
        {
            var orderList = new List<KeyValuePair<TK, TV>>(map);
            orderList.Sort((x, y) => Comparer<TV>.Default.Compare(x.Value, y.Value)*sort);
            return orderList;
        }
    }
}