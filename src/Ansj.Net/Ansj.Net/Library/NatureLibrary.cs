using System;
using Ansj.Net.Domain;
using Ansj.Net.Util;
using Lucene.Net.Support;

namespace Ansj.Net.Library
{
    /// <summary>
    ///     这里封装了词性和词性之间的关系.以及词性的索引.这是个好东西. 里面数组是从ict里面找来的. 不是很新.没有预料无法训练
    /// </summary>
    public class NatureLibrary
    {
        private static readonly int One = 1;
        private static readonly int MinusOne = -1;

        /// <summary>
        ///     词性的字符串对照索引位的hashmap(我发现我又效率狂了.不能这样啊)
        /// </summary>
        private static readonly HashMap<string, Nature> Naturemap = new HashMap<string, Nature>();

        /// <summary>
        ///     词与词之间的关系.对照natureARRAY,natureMap
        /// </summary>
        private static int[][] _naturetable;

        /// <summary>
        ///     初始化对照表
        /// </summary>
        static NatureLibrary()
        {
            try
            {
                Init();
            }
            catch (Exception e)
            {
                throw new Exception("词性列表加载失败!", e);
            }
        }

        private static void Init()
        {
            var split = '\t';
            // 加载词对照性表
            var reader = MyStaticValue.GetNatureMapReader();
            string temp;
            string[] strs;
            var maxLength = 0;
            while ((temp = reader.ReadLine()) != null)
            {
                strs = temp.Split(split);
                if (strs.Length != 4)
                    continue;

                var p0 = int.Parse(strs[0]);
                var p1 = int.Parse(strs[1]);
                var p2 = int.Parse(strs[3]);
                Naturemap.Add(strs[2], new Nature(strs[2], p0, p1, p2));
                maxLength = Math.Max(maxLength, p1);
            }
            reader.Close();
            _naturetable = new int[maxLength + 1][];
            for (var i = 0; i < maxLength + 1; i++)
            {
                _naturetable[i] = new int[maxLength + 1];
            }
            // 加载词性关系
            //NATURETABLE = new int[maxLength + 1][maxLength + 1];
            reader = MyStaticValue.GetNatureTableReader();
            var j = 0;
            while ((temp = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(temp))
                    continue;
                strs = temp.Split(split);
                for (var i = 0; i < strs.Length; i++)
                {
                    _naturetable[j][i] = int.Parse(strs[i]);
                }
                j++;
            }
            reader.Close();
        }

        /// <summary>
        ///     获得两个词性之间的频率
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int GetTwoNatureFreq(Nature from, Nature to)
        {
            if (from.index < 0 || to.index < 0)
            {
                return 0;
            }
            return _naturetable[from.index][to.index];
        }

        /// <summary>
        ///     获得两个term之间的频率
        /// </summary>
        /// <param name="fromTerm"></param>
        /// <param name="toTerm"></param>
        /// <returns></returns>
        public static int GetTwoTermFreq(Term fromTerm, Term toTerm)
        {
            var from = fromTerm.Nature;
            var to = toTerm.Nature;
            if (from.index < 0 || to.index < 0)
            {
                return 0;
            }
            return _naturetable[from.index][to.index];
        }

        /// <summary>
        ///     根据字符串得到词性.没有就创建一个
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Nature GetNature(string input)
        {
            var nature = Naturemap[input];
            if (nature == null)
            {
                nature = new Nature(input, MinusOne, MinusOne, One);
                Naturemap.Add(input, nature);
                return nature;
            }
            return nature;
        }
    }
}