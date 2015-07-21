using System.Collections.Generic;
using System.Linq;
using Ansj.Net.Domain;
using Ansj.Net.SplitWord.Analysis;
using Lucene.Net.Support;

namespace Ansj.Net.App.Keyword
{
    public class KeyWordComputer
    {
        private static readonly HashMap<string, double> PosScore = new HashMap<string, double>();
        private readonly int _keywordAmount = 5;

        static KeyWordComputer()
        {
            PosScore.Add("null", 0.0);
            PosScore.Add("w", 0.0);
            PosScore.Add("en", 0.0);
            PosScore.Add("num", 0.0);
            PosScore.Add("nr", 3.0);
            PosScore.Add("nrf", 3.0);
            PosScore.Add("nw", 3.0);
            PosScore.Add("nt", 3.0);
            PosScore.Add("l", 0.2);
            PosScore.Add("a", 0.2);
            PosScore.Add("nz", 3.0);
            PosScore.Add("v", 0.2);
        }

        public KeyWordComputer()
        {
        }

        /// <summary>
        ///     返回关键词个数
        /// </summary>
        /// <param name="keywordAmount"></param>
        public KeyWordComputer(int keywordAmount)
        {
            _keywordAmount = keywordAmount;
        }

        private List<Keyword> ComputeArticleTfidf(string content, int titleLength)
        {
            var tm = new HashMap<string, Keyword>();

            var parse = NlpAnalysis.Parse(content);
            foreach (var term in parse)
            {
                var weight = getWeight(term, content.Length, titleLength);
                if (weight == 0)
                    continue;
                var keyword = tm[term.Name];
                if (keyword == null)
                {
                    keyword = new Keyword(term.Name, term.Nature.allFrequency, weight);
                    tm[term.Name] = keyword;
                }
                else
                {
                    keyword.UpdateWeight(1);
                }
            }

            var treeSet = new SortedSet<Keyword>(tm.Values);

            var arrayList = new List<Keyword>(treeSet);
            if (treeSet.Count <= _keywordAmount)
            {
                return arrayList;
            }
            return arrayList.Take(_keywordAmount).ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="content">正文</param>
        /// <returns></returns>
        public List<Keyword> ComputeArticleTfidf(string title, string content)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                title = "";
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                content = "";
            }
            return ComputeArticleTfidf(title + "\t" + content, title.Length);
        }

        /// <summary>
        ///     只有正文
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public List<Keyword> ComputeArticleTfidf(string content)
        {
            return ComputeArticleTfidf(content, 0);
        }

        private double getWeight(Term term, int length, int titleLength)
        {
            if (term.Name.Trim().Length < 2)
            {
                return 0;
            }

            var pos = term.Nature.natureStr;

            var posScore = PosScore[pos];

            if (posScore == 0)
            {
                return 0;
            }

            if (titleLength > term.Offe)
            {
                return 5*posScore;
            }
            return (length - term.Offe)*posScore/length;
        }
    }
}