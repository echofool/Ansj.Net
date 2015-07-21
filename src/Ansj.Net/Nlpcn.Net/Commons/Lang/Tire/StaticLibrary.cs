using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Nlpcn.Net.Commons.Lang.Tire
{
    public static class StaticLibrary
    {
        public static Forest MakeForest(string path)
        {
            return MakeForest(File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
        }

        public static Forest MakeForest(Stream inputStream)
        {
            return MakeForest(new StreamReader(inputStream, Encoding.UTF8));
        }

        public static Forest MakeForest(StreamReader br)
        {
            return MakeLibrary(br, new Forest());
        }

        /// <summary>
        /// 传入value数组.构造树
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>

        public static Forest MakeForest(List<Value> values)
        {
            var forest = new Forest();
            foreach (var value in values)
            {
                InsertWord(forest, value.ToString());
            }
            return forest;
        }

        /// <summary>
        /// 词典树的构造方法
        /// </summary>
        /// <param name="br"></param>
        /// <param name="forest"></param>
        /// <returns></returns>

        private static Forest MakeLibrary(StreamReader br, Forest forest)
        {
            if (br == null) return forest;
            try
            {
                string temp = null;
                while ((temp = br.ReadLine()) != null)
                {
                    InsertWord(forest, temp);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                br.Close();
            }
            return forest;
        }

        public static void InsertWord(Forest forest, Value value)
        {
            InsertWord(forest, value.Keyword, value.Paramers);
        }

        /// <summary>
        /// 插入一个词
        /// </summary>
        /// <param name="forest"></param>
        /// <param name="temp"></param>

        public static void InsertWord(IWoodInterface forest, string temp)
        {
            var param = temp.Split('\t');

            temp = param[0];

            var resultParams = new string[param.Length - 1];
            for (var j = 1; j < param.Length; j++)
            {
                resultParams[j - 1] = param[j];
            }

            InsertWord(forest, temp, resultParams);
        }

        private static void InsertWord(IWoodInterface forest, string temp, string[] param)
        {
            var branch = forest;
            var chars = temp.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (chars.Length == i + 1)
                {
                    branch.Add(new Branch(chars[i], 3, param));
                }
                else
                {
                    branch.Add(new Branch(chars[i], 1, null));
                }
                branch = branch.Get(chars[i]);
            }
        }

        /// <summary>
        /// 删除一个词
        /// </summary>
        /// <param name="forest"></param>
        /// <param name="word"></param>
        public static void RemoveWord(Forest forest, string word)
        {
            IWoodInterface branch = forest;
            var chars = word.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (branch == null)
                    return;
                if (chars.Length == i + 1)
                {
                    branch.Add(new Branch(chars[i], -1, null));
                }
                branch = branch.Get(chars[i]);
            }
        }
    }
}