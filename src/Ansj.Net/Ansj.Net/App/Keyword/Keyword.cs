using System;

namespace Ansj.Net.App.Keyword
{
    public class Keyword : IComparable<Keyword>
    {
        private readonly double _idf;

        public Keyword(string name, int docFreq, double weight)
        {
            Name = name;
            _idf = Math.Log(10000 + 10000.0/(docFreq + 1));
            Score = _idf*weight;
            Freq++;
        }

        public Keyword(string name, double score)
        {
            Name = name;
            Score = score;
            _idf = score;
            Freq++;
        }

        public int Freq { get; private set; }
        public string Name { get; set; }
        public double Score { get; set; }

        /// <summary>
        ///     比较当前对象和同一类型的另一对象。
        /// </summary>
        /// <returns>
        ///     一个值，指示要比较的对象的相对顺序。
        ///     返回值的含义如下：
        ///     值含义小于零此对象小于 <paramref name="other" /> 参数。
        ///     零此对象等于 <paramref name="other" />。
        ///     大于零此对象大于 <paramref name="other" />。
        /// </returns>
        /// <param name="other">与此对象进行比较的对象。</param>
        public int CompareTo(Keyword other)
        {
            if (Score < other.Score)
            {
                return 1;
            }
            return -1;
        }

        public void UpdateWeight(int weight)
        {
            Score += weight*_idf;
            Freq++;
        }

        #region Overrides of Object

        /// <summary>
        ///     确定指定的
        ///     <see cref="T:System.Object" /> 是否等于当前的 <see cref="T:Ansj.Net.App.Keyword.Keyword" />。
        /// </summary>
        /// <returns>
        ///     如果指定的
        ///     <see cref="T:System.Object" /> 等于当前的 <see cref="T:Ansj.Net.App.Keyword.Keyword" />，则为 true；否则为 false。
        /// </returns>
        /// <param name="obj">
        ///     与当前的
        ///     <see cref="T:Ansj.Net.App.Keyword.Keyword" /> 进行比较的 <see cref="T:System.Object" />。
        /// </param>
        /// <filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            var keyword = obj as Keyword;
            if (keyword != null)
            {
                var k = keyword;
                return k.Name.Equals(Name);
            }
            return false;
        }


        /// <summary>
        ///     用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        ///     当前 <see cref="T:System.Object" /> 的哈希代码。
        /// </returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        /// <summary>
        ///     返回表示当前
        ///     <see cref="T:Ansj.Net.App.Keyword.Keyword" /> 的 <see cref="T:System.String" />。
        /// </summary>
        /// <returns>
        ///     <see cref="T:System.String" />，表示当前的 <see cref="T:Ansj.Net.App.Keyword.Keyword" />。
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Name + "/" + Score; // "="+score+":"+freq+":"+idf;
        }

        #endregion
    }
}