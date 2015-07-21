using System;
using System.Collections.Generic;
using System.Linq;
using Ansj.Net.Domain;
using Ansj.Net.Recognition;
using Ansj.Net.Util;
using Lucene.Net.Support;
using Nlpcn.Net.Commons.Lang.Tire.Domain;
using Nlpcn.Net.Commons.Lang.Util;

namespace Ansj.Net.Dic
{
    /// <summary>
    ///     新词发现,这是个线程安全的.所以可以多个对象公用一个
    /// </summary>
    public class LearnTool
    {
        /// <summary>
        ///     新词发现的结果集.可以序列化到硬盘.然后可以当做训练集来做.
        /// </summary>
        private readonly SmartForest<NewWord> _smartForest = new SmartForest<NewWord>();

        private bool _isAsianName = true;
        private bool _isForeignName = true;

        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)] private App.Crf.SplitWord
            splitWord;

        /// <summary>
        ///     告诉大家你学习了多少个词了
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///     是否开启学习机
        /// </summary>
        public bool IsAsianName
        {
            get { return _isAsianName; }
            set { _isAsianName = value; }
        }

        public bool IsForeignName
        {
            get { return _isForeignName; }
            set { _isForeignName = value; }
        }

        /// <summary>
        ///     公司名称学习
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="splitWord"></param>
        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
        public void Learn(Graph graph, App.Crf.SplitWord splitWord)
        {
            this.splitWord = splitWord;

            // 亚洲人名识别
            if (_isAsianName)
            {
                FindAsianPerson(graph);
            }

            // 外国人名识别
            if (_isForeignName)
            {
                FindForeignPerson(graph);
            }
        }

        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
        private void FindAsianPerson(Graph graph)
        {
            var newWords = new AsianPersonRecognition(graph.Terms).GetNewWords();
            AddListToTerm(newWords);
        }

        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
        private void FindForeignPerson(Graph graph)
        {
            var newWords = new ForeignPersonRecognition(graph.Terms).GetNewWords();
            AddListToTerm(newWords);
        }

        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
        // 批量将新词加入到词典中
        private void AddListToTerm(List<NewWord> newWords)
        {
            if (newWords.Count == 0)
                return;
            foreach (var newWord in newWords)
            {
                AddTerm(newWord);
            }
        }

        /// <summary>
        ///     增加一个新词到树中
        /// </summary>
        /// <param name="newWord"></param>
        [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
        public void AddTerm(NewWord newWord)
        {
            SmartForest<NewWord> smartForest;
            if ((smartForest = _smartForest.GetBranch(newWord.Name)) != null && smartForest.Param != null)
            {
                var temp = smartForest.Param;
                temp.Update(newWord.Nature, newWord.AllFreq);
            }
            else
            {
                Count++;
                newWord.Score = -splitWord.cohesion(newWord.Name);
                lock (_smartForest)
                {
                    _smartForest.Add(newWord.Name, newWord);
                }
            }
        }

        public SmartForest<NewWord> GetForest()
        {
            return _smartForest;
        }

        /// <summary>
        ///     返回学习到的新词
        /// </summary>
        /// <param name="num">返回数目.0为全部返回</param>
        /// <returns></returns>
        public List<KeyValuePair<string, double>> GetTopTree(int num)
        {
            return GetTopTree(num, null);
        }

        /// <summary>
        /// </summary>
        /// <param name="num"></param>
        /// <param name="nature"></param>
        /// <returns></returns>
        public List<KeyValuePair<string, double>> GetTopTree(int num, Nature nature)
        {
            if (_smartForest.Branches == null)
            {
                return null;
            }
            var hm = new HashMap<string, double>();
            for (var i = 0; i < _smartForest.Branches.Length; i++)
            {
                ValueResult(_smartForest.Branches[i], hm, nature);
            }
            var sortMapByValue = CollectionUtil.SortMapByValue(hm, -1);
            if (num == 0)
            {
                return sortMapByValue;
            }
            num = Math.Min(num, sortMapByValue.Count);
            return sortMapByValue.Take(num).ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="smartForest"></param>
        /// <param name="hm"></param>
        /// <param name="nature"></param>
        private void ValueResult(SmartForest<NewWord> smartForest, HashMap<string, double> hm, Nature nature)
        {
            // TODO Auto-generated method stub
            if (smartForest == null || smartForest.Branches == null)
            {
                return;
            }
            for (var i = 0; i < smartForest.Branches.Length; i++)
            {
                var param = smartForest.Branches[i].Param;
                if (smartForest.Branches[i].Status == 3)
                {
                    if (param.IsActive && (nature == null || param.Nature.Equals(nature)))
                    {
                        hm.Add(param.Name, param.Score);
                    }
                }
                else if (smartForest.Branches[i].Status == 2)
                {
                    if (param.IsActive && (nature == null || param.Nature.Equals(nature)))
                    {
                        hm.Add(param.Name, param.Score);
                    }
                    ValueResult(smartForest.Branches[i], hm, nature);
                }
                else
                {
                    ValueResult(smartForest.Branches[i], hm, nature);
                }
            }
        }

        /// <summary>
        ///     尝试激活，新词
        /// </summary>
        /// <param name="name"></param>
        public void Active(string name)
        {
            var branch = _smartForest.GetBranch(name);
            if (branch != null && branch.Param != null)
            {
                branch.Param.IsActive = true;
            }
        }
    }
}