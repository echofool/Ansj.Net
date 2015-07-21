using System;
using System.Collections.Generic;
using Ansj.Net.Util;

namespace Ansj.Net.Domain
{
    public class Term : IComparable<Term>
    {
        /// <summary>
        ///     词性列表
        /// </summary>
        private AnsjItem _item = AnsjItem.Null;

        /// <summary>
        ///     本身这个term的词性.需要在词性识别之后才会有值,默认是空
        /// </summary>
        private Nature _nature = Nature.NULL;

        /// <summary>
        ///     同一行内数据
        /// </summary>
        private Term _next;

        private string _realName;

        /// <summary>
        ///     本身分数
        /// </summary>
        private double _selfScore = 1;

        /// <summary>
        ///     分数
        /// </summary>
        public double Score;

        public Term(string name, int offe, AnsjItem item)
        {
            Name = name;
            Offe = offe;
            _item = item;
            if (item.Natures != null)
            {
                TermNatures = item.Natures;
                if (TermNatures.Nature != null)
                {
                    _nature = TermNatures.Nature;
                }
            }
            else
            {
                TermNatures = TermNatures.Null;
            }
        }

        public Term(string name, int offe, TermNatures termNatures)
        {
            Name = name;
            Offe = offe;
            TermNatures = termNatures;
            if (termNatures.Nature != null)
            {
                _nature = termNatures.Nature;
            }
        }

        public Term(string name, int offe, string natureStr, int natureFreq)
        {
            Name = name;
            Offe = offe;
            var termNature = new TermNature(natureStr, natureFreq);
            _nature = termNature.nature;
            TermNatures = new TermNatures(termNature);
        }

        /// <summary>
        ///     起始位置
        /// </summary>
        public Term From { get; set; }

        /// <summary>
        ///     当前词
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     当前词的起始位置
        /// </summary>
        public int Offe { get; private set; }

        public string RealName
        {
            get { return _realName ?? Name; }
            set { _realName = value; }
        }

        public List<Term> SubTerm { get; set; }

        /// <summary>
        ///     到达位置
        /// </summary>
        public Term To { get; set; }

        /// <summary>
        ///     获得这个term的所有词性
        /// </summary>
        public TermNatures TermNatures { get; private set; }

        /// <summary>
        ///     词性列表
        /// </summary>
        public AnsjItem Item
        {
            get { return _item; }
            set { _item = value; }
        }

        /// <summary>
        ///     本身分数
        /// </summary>
        public double SelfScore
        {
            get { return _selfScore; }
            set { _selfScore = value; }
        }

        /// <summary>
        ///     本身这个term的词性.需要在词性识别之后才会有值,默认是空
        /// </summary>
        public Nature Nature
        {
            get { return _nature; }
            set { _nature = value; }
        }

        #region Implementation of IComparable<in Term>

        /// <summary>
        ///     比较当前对象和同一类型的另一对象。
        /// </summary>
        /// <returns>
        ///     一个值，指示要比较的对象的相对顺序。
        ///     返回值的含义如下：
        ///     值含义小于零此对象小于  <paramref name="other" /> 参数。
        ///     零此对象等于 <paramref name="other" />。
        ///     大于零此对象大于 <paramref name="other" />。
        /// </returns>
        /// <param name="other">与此对象进行比较的对象。</param>
        public int CompareTo(Term other)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        ///     可以到达的位置
        /// </summary>
        /// <returns></returns>
        public int ToValue()
        {
            return Offe + Name.Length;
        }

        /// <summary>
        ///     核心构建最优的路径
        /// </summary>
        /// <param name="from"></param>
        public void SetPathScore(Term from)
        {
            // 维特比进行最优路径的构建
            var score = MathUtil.CompuScore(from, this);
            if (From == null || Score >= score)
            {
                SetFromAndScore(from, score);
            }
        }

        /// <summary>
        ///     核心分数的最优的路径,越小越好
        /// </summary>
        /// <param name="from"></param>
        public void SetPathSelfScore(Term from)
        {
            var score = _selfScore + from.Score;
            // 维特比进行最优路径的构建
            if (From == null || Score > score)
            {
                SetFromAndScore(from, score);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="from"></param>
        /// <param name="score"></param>
        private void SetFromAndScore(Term from, double score)
        {
            // TODO Auto-generated method stub
            From = from;
            Score = score;
        }

        /// <summary>
        ///     进行term合并
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public Term Merage(Term to)
        {
            Name = Name + to.Name;
            return this;
        }

        /// <summary>
        ///     更新偏移量
        /// </summary>
        /// <param name="offe"></param>
        public void UpdateOffe(int offe)
        {
            Offe += offe;
        }

        public Term GetNext()
        {
            return _next;
        }

        /// <summary>
        ///     返回他自己
        /// </summary>
        /// <param name="next">设置他的下一个</param>
        /// <returns></returns>
        public Term SetNext(Term next)
        {
            _next = next;
            return this;
        }

        public string GetNatureStr()
        {
            return _nature.natureStr;
        }

        public override string ToString()
        {
            if ("null".Equals(_nature.natureStr))
            {
                return Name;
            }
            return RealName + "/" + _nature.natureStr;
        }

        /// <summary>
        ///     将term的所有分数置为0
        /// </summary>
        public void ClearScore()
        {
            Score = 0;
            _selfScore = 0;
        }
    }
}