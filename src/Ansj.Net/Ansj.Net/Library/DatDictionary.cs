using System;
using System.Diagnostics;
using Ansj.Net.Dic;
using Ansj.Net.Domain;
using Ansj.Net.Library.Name;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Dat;

namespace Ansj.Net.Library
{
    public class DatDictionary
    {
        /// <summary>
        ///     所有在词典中出现的词,并且承担简繁体转换的任务.
        /// </summary>
        public static readonly char[] InSystem = new char[65536];

        /// <summary>
        ///     核心词典
        /// </summary>
        private static readonly DoubleArrayTire Dat = LoadDat();

        /// <summary>
        ///     数组长度
        /// </summary>
        public static int ArrayLength = Dat.ArrayLength;

        /// <summary>
        ///     加载词典
        /// </summary>
        /// <returns></returns>
        private static DoubleArrayTire LoadDat()
        {
            var start = DateTime.Now.CurrentTimeMillis();

            try
            {
                var dat = DoubleArrayTire.LoadText<AnsjItem>(DicReader.GetInputStream("core.dic"));

                /**
                 * 人名识别必备的
                 */
                PersonNameFull(dat);

                /**
                 * 记录词典中的词语，并且清除部分数据
                 */

                foreach (var item in dat.GetDat())
                {
                    if (item == null || item.Name == null)
                    {
                        continue;
                    }

                    if (item.Status < 4)
                    {
                        for (var i = 0; i < item.Name.Length; i++)
                        {
                            InSystem[item.Name[i]] = item.Name[i];
                        }
                    }
                    if (item.Status < 2)
                    {
                        item.Name = null;
                    }
                }
                // 特殊字符标准化
                InSystem['％'] = '%';

                MyStaticValue.Librarylog.Info("init core library ok use time :" +
                                              (DateTime.Now.CurrentTimeMillis() - start));

                return dat;
            }
            catch (Exception e)
            {
                // TODO Auto-generated catch block
                Trace.WriteLine(e);
            }

            return null;
        }

        private static void PersonNameFull(DoubleArrayTire dat)
        {
            var personMap = new PersonAttrLibrary().GetPersonMap();

            // 人名词性补录
            var entrySet = personMap;
            var c = default(char);
            string temp = null;
            foreach (var entry in entrySet)
            {
                temp = entry.Key;

                AnsjItem ansjItem;
                if (temp.Length == 1 && ((AnsjItem) dat.GetDat()[temp[0]]) == null)
                {
                    ansjItem = new AnsjItem
                    {
                        Base = c,
                        Check = -1,
                        Status = 3,
                        Name = temp
                    };
                    dat.GetDat()[temp[0]] = ansjItem;
                }
                else
                {
                    ansjItem = dat.GetItem<AnsjItem>(temp);
                }

                if (ansjItem == null)
                {
                    continue;
                }

                if ((ansjItem.Natures) == null)
                {
                    ansjItem.Natures = new TermNatures(TermNature.NR);
                }
                ansjItem.Natures.SetPersonNatureAttr(entry.Value);
            }
        }

        public static int Status(char c)
        {
            Item item = (AnsjItem) Dat.GetDat()[c];
            if (item == null)
            {
                return 0;
            }
            return item.Status;
        }

        /// <summary>
        ///     判断一个词语是否在词典中
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool IsInSystemDic(string word)
        {
            var item = Dat.GetItem<Item>(word);
            return item != null && item.Status > 1;
        }

        public static AnsjItem GetItem(int index)
        {
            var item = Dat.GetItem<AnsjItem>(index);
            if (item == null)
            {
                return AnsjItem.Null;
            }

            return item;
        }

        public static AnsjItem GetItem(string str)
        {
            var item = Dat.GetItem<AnsjItem>(str);
            if (item == null)
            {
                return AnsjItem.Null;
            }

            return item;
        }

        public static int GetId(string str)
        {
            return Dat.GetId(str);
        }
    }
}