using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Tire;
using Nlpcn.Net.Commons.Lang.Tire.Domain;
using Nlpcn.Net.Commons.Lang.Util;

namespace Ansj.Net.Library
{
    /**
     * 用户自定义词典操作类
     * 
     * @author ansj
     */

    public class UserDefineLibrary
    {
        public static readonly string DefaultNature = "userDefine";
        public static readonly int DefaultFreq = 1000;
        public static readonly string DefaultFreqStr = "1000";
        public static Forest Forest;
        public static Forest AmbiguityForest;

        static UserDefineLibrary()
        {
            InitUserLibrary();
            InitAmbiguityLibrary();
        }

        /// <summary>
        ///     关键词增加
        /// </summary>
        /// <param name="keyword">所要增加的关键词</param>
        /// <param name="nature">关键词的词性</param>
        /// <param name="freq">关键词的词频</param>
        public static void InsertWord(string keyword, string nature, int freq)
        {
            var paramers = new string[2];
            paramers[0] = nature;
            paramers[1] = freq.ToString();
            var value = new Value(keyword, paramers);
            StaticLibrary.InsertWord(Forest, value);
        }

        /// <summary>
        ///     加载纠正词典
        /// </summary>
        private static void InitAmbiguityLibrary()
        {
            var ambiguityLibrary = MyStaticValue.AmbiguityLibrary;
            if (string.IsNullOrWhiteSpace(ambiguityLibrary))
            {
                MyStaticValue.Librarylog.Warn("init ambiguity  warning :" + ambiguityLibrary +
                                              " because : file not found or failed to read !");
                return;
            }
            ambiguityLibrary = MyStaticValue.AmbiguityLibrary;
            var file = new FileInfo(ambiguityLibrary);
            if (file.Exists)
            {
                try
                {
                    AmbiguityForest = StaticLibrary.MakeForest(ambiguityLibrary);
                }
                catch (Exception e)
                {
                    MyStaticValue.Librarylog.Warn("init ambiguity  error :" + new FileInfo(ambiguityLibrary).FullName +
                                                  " because : not find that file or can not to read !");
                    Trace.WriteLine(e);
                }
                MyStaticValue.Librarylog.Info("init ambiguityLibrary ok!");
            }
            else
            {
                MyStaticValue.Librarylog.Warn("init ambiguity  warning :" + new FileInfo(ambiguityLibrary).FullName +
                                              " because : file not found or failed to read !");
            }
        }

        /// <summary>
        ///     加载用户自定义词典和补充词典
        /// </summary>
        private static void InitUserLibrary()
        {
            try
            {
                Forest = new Forest();
                // 加载用户自定义词典
                var userLibrary = MyStaticValue.UserLibrary;
                LoadLibrary(Forest, userLibrary);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        ///     单个文件加载词典
        /// </summary>
        /// <param name="forest"></param>
        /// <param name="file"></param>
        public static void LoadFile(Forest forest, FileInfo file)
        {
            if (!file.Exists)
            {
                MyStaticValue.Librarylog.Warn("file in path " + file.FullName + " can not to read!");
                return;
            }
            TextReader br = null;
            try
            {
                br = IOUtil.GetReader(new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read),
                    Encoding.UTF8);
                string temp;
                while ((temp = br.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(temp))
                    {
                    }
                    var strs = temp.Split('\t');

                    strs[0] = strs[0].ToLower();

                    // 如何核心辞典存在那么就放弃
                    if (MyStaticValue.IsSkipUserDefine && DatDictionary.GetId(strs[0]) > 0)
                    {
                        continue;
                    }

                    Value value;
                    if (strs.Length != 3)
                    {
                        value = new Value(strs[0], DefaultNature, DefaultFreqStr);
                    }
                    else
                    {
                        value = new Value(strs[0], strs[1], strs[2]);
                    }
                    StaticLibrary.InsertWord(forest, value);
                }
                MyStaticValue.Librarylog.Info("init user userLibrary ok path is : " + file.FullName);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {
                IOUtil.Close(br);
            }
        }

        /// <summary>
        ///     加载词典,传入一本词典的路径.或者目录.词典后缀必须为.dic
        /// </summary>
        /// <param name="forest"></param>
        /// <param name="path"></param>
        public static void LoadLibrary(Forest forest, string path)
        {
            // 加载用户自定义词典
            if (path != null)
            {
                path = "Resources/" + path;
                var file = new FileInfo(path);
                if (!File.Exists(path) && !Directory.Exists(path))
                {
                    MyStaticValue.Librarylog.Warn("init userLibrary  warning :" + file.FullName +
                                                  " because : file not found or failed to read !");
                    return;
                }
                if (file.Exists)
                {
                    LoadFile(forest, file);
                }
                else if (Directory.Exists(path))
                {
                    var files = new DirectoryInfo(path).GetFiles();
                    for (var i = 0; i < files.Length; i++)
                    {
                        if (files[i].Name.Trim().EndsWith(".dic"))
                        {
                            LoadFile(forest, files[i]);
                        }
                    }
                }
                else
                {
                    MyStaticValue.Librarylog.Warn("init user library  error :" + path +
                                                  " because : not find that file !");
                }
            }
        }

        /// <summary>
        ///     删除关键词
        /// </summary>
        /// <param name="word"></param>
        public static void RemoveWord(string word)
        {
            StaticLibrary.RemoveWord(Forest, word);
        }

        public static string[] GetParams(string word)
        {
            IWoodInterface temp = Forest;
            for (var i = 0; i < word.Length; i++)
            {
                temp = temp.Get(word[i]);
                if (temp == null)
                {
                    return null;
                }
            }
            if (temp.Status > 1)
            {
                return temp.Param;
            }
            return null;
        }

        public static string[] GetParams(Forest forest, string word)
        {
            IWoodInterface temp = forest;
            for (var i = 0; i < word.Length; i++)
            {
                temp = temp.Get(word[i]);
                if (temp == null)
                {
                    return null;
                }
            }
            if (temp.Status > 1)
            {
                return temp.Param;
            }
            return null;
        }

        public static bool Contains(string word)
        {
            return GetParams(word) != null;
        }

        /// <summary>
        ///     将用户自定义词典清空
        /// </summary>
        public static void Clear()
        {
            Forest.clear();
        }
    }
}