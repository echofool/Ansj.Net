using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Ansj.Net.Dic;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using log4net;
using Lucene.Net.Support;
using nlpcn.net.commons;
using org.ansj.app.crf;

namespace Ansj.Net.Util
{
    /// <summary>
    ///     这个类储存一些公用变量
    /// </summary>
    public class MyStaticValue
    {
        public static readonly ILog Librarylog = LogManager.GetLogger("LIBRARYLOG");

        /// <summary>
        ///     是否开启人名识别
        /// </summary>
        public static bool IsNameRecognition = true;

        private static readonly ReaderWriterLock Lock = new ReaderWriterLock();

        /// <summary>
        ///     是否开启数字识别
        /// </summary>
        public static bool IsNumRecognition = true;

        /// <summary>
        ///     是否数字和量词合并
        /// </summary>
        public static bool IsQuantifierRecognition = true;

        // crf 模型

        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)] private static
            App.Crf.SplitWord
            _crfSplitWord;

        public static bool IsRealName = false;

        /// <summary>
        ///     用户自定义词典的加载,如果是路径就扫描路径下的dic文件
        /// </summary>
        public static string UserLibrary = "library/default.dic";

        public static string AmbiguityLibrary = "library/ambiguity.dic";

        /// <summary>
        ///     是否用户辞典不加载相同的词
        /// </summary>
        public static bool IsSkipUserDefine = false;

        /// <summary>
        ///     人名词典
        /// </summary>
        /// <returns></returns>
        public static TextReader GetPersonReader()
        {
            return DicReader.GetReader("person/person.dic");
        }

        /// <summary>
        ///     机构名词典
        /// </summary>
        /// <returns></returns>
        public static TextReader GetCompanReader()
        {
            return DicReader.GetReader("company/company.data");
        }

        /// <summary>
        ///     机构名词典
        /// </summary>
        /// <returns></returns>
        public static TextReader GetNewWordReader()
        {
            return DicReader.GetReader("newWord/new_word_freq.dic");
        }

        /// <summary>
        ///     核心词典
        /// </summary>
        /// <returns></returns>
        public static TextReader GetArraysReader()
        {
            return DicReader.GetReader("arrays.dic");
        }

        /// <summary>
        ///     数字词典
        /// </summary>
        /// <returns></returns>
        public static TextReader GetNumberReader()
        {
            return DicReader.GetReader("numberLibrary.dic");
        }

        /// <summary>
        ///     英文词典
        /// </summary>
        /// <returns></returns>
        public static TextReader GetEnglishReader()
        {
            return DicReader.GetReader("englishLibrary.dic");
        }

        /// <summary>
        ///     词性表
        /// </summary>
        /// <returns></returns>
        public static TextReader GetNatureMapReader()
        {
            // TODO Auto-generated method stub
            return DicReader.GetReader("nature/nature.map");
        }

        /// <summary>
        ///     词性关联表
        /// </summary>
        /// <returns></returns>
        public static TextReader GetNatureTableReader()
        {
            // TODO Auto-generated method stub
            return DicReader.GetReader("nature/nature.table");
        }

        /// <summary>
        ///     得到姓名单字的词频词典
        /// </summary>
        /// <returns></returns>
        public static TextReader GetPersonFreqReader()
        {
            // TODO Auto-generated method stub
            return DicReader.GetReader("person/name_freq.dic");
        }

        /// <summary>
        ///     名字词性对象反序列化
        /// </summary>
        /// <returns></returns>
        public static HashMap<string, int[][]> GetPersonFreqMap()
        {
            Stream inputStream = null;
            ObjectInputStream objectInputStream = null;
            var map = new HashMap<string, int[][]>(0);
            try
            {
                inputStream = DicReader.GetInputStream("person/asian_name_freq.data");
                objectInputStream = new ObjectInputStream(inputStream);
                map = (HashMap<string, int[][]>) objectInputStream.ReadObject();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "GetPersonFreqMap");
            }
            finally
            {
                try
                {
                    if (objectInputStream != null)
                        objectInputStream.Close();
                    if (inputStream != null)
                        inputStream.Close();
                }
                catch (IOException e)
                {
                    Trace.WriteLine(e, "GetPersonFreqMap");
                }
            }
            return map;
        }

        /// <summary>
        ///     词与词之间的关联表数据
        /// </summary>
        public static void initBigramTables()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(DicReader.GetInputStream("bigramdict.dic"), Encoding.UTF8);
                string temp;
                while ((temp = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(temp))
                    {
                        continue;
                    }
                    var strs = temp.Split('\t');
                    var freq = int.Parse(strs[1]);
                    strs = strs[0].Split('@');
                    var fromItem = DatDictionary.GetItem(strs[0]);

                    var toItem = DatDictionary.GetItem(strs[1]);

                    if (fromItem == AnsjItem.Null && strs[0].Contains("#"))
                    {
                        fromItem = AnsjItem.Begin;
                    }

                    if (toItem == AnsjItem.Null && strs[1].Contains("#"))
                    {
                        toItem = AnsjItem.End;
                    }

                    if (fromItem == AnsjItem.Null || toItem == AnsjItem.Null)
                    {
                        continue;
                    }

                    if (fromItem.BigramEntryMap == null)
                    {
                        fromItem.BigramEntryMap = new HashMap<int, int>();
                    }

                    fromItem.BigramEntryMap.Add(toItem.Index, freq);
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        ///     得到默认的模型
        /// </summary>
        /// <returns></returns>
        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
        public static App.Crf.SplitWord GetCrfSplitWord()
        {
            // TODO Auto-generated method stub
            if (_crfSplitWord != null)
            {
                return _crfSplitWord;
            }
            Lock.AcquireReaderLock(60*1000);
            if (_crfSplitWord != null)
            {
                return _crfSplitWord;
            }

            try
            {
                _crfSplitWord = new App.Crf.SplitWord(Model.loadModel(DicReader.GetInputStream("crf/crf.model")));
            }
            finally
            {
                Lock.ReleaseLock();
            }

            return _crfSplitWord;
        }
    }
}