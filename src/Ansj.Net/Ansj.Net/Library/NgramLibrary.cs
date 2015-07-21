using System;
using System.Diagnostics;
using Ansj.Net.Domain;
using Ansj.Net.Util;

namespace Ansj.Net.Library
{
    /// <summary>
    ///     两个词之间的关联
    /// </summary>
    public class NgramLibrary
    {
        static NgramLibrary()
        {
            try
            {
                MyStaticValue.initBigramTables();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        ///     查找两个词与词之间的频率
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int GetTwoWordFreq(Term from, Term to)
        {
            if (from.Item.BigramEntryMap == null)
            {
                return 0;
            }
            var freq = from.Item.BigramEntryMap[to.Item.Index];

            return freq;
        }
    }
}