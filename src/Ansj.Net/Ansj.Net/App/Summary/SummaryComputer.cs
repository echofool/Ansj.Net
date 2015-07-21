using System;
using System.Collections.Generic;
using System.Text;
using Ansj.Net.App.Keyword;
using Ansj.Net.SplitWord.Analysis;
using Nlpcn.Net.Commons.Lang.Tire;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.App.Summary
{
    /// <summary>
    ///     自动摘要,同时返回关键词
    /// </summary>
    public class SummaryComputer
    {
        private static readonly HashSet<string> FilterSet = new HashSet<string>();
        private readonly string _content;
        private readonly bool _isSplitSummary = true;

        /// <summary>
        ///     summaryLength
        /// </summary>
        private readonly int _len = 300;

        private readonly string _title;

        static SummaryComputer()
        {
            FilterSet.Add("w");
            FilterSet.Add("null");
        }

        public SummaryComputer(string title, string content)
        {
            _title = title;
            _content = content;
        }

        public SummaryComputer(int len, string title, string content)
        {
            _len = len;
            _title = title;
            _content = content;
        }

        public SummaryComputer(int len, bool isSplitSummary, string title, string content)
        {
            _len = len;
            _title = title;
            _content = content;
            _isSplitSummary = isSplitSummary;
        }

        /// <summary>
        ///     计算摘要，利用关键词抽取计算
        /// </summary>
        /// <returns></returns>
        public Pojo.Summary ToSummary()
        {
            return ToSummary(new List<Keyword.Keyword>());
        }

        /// <summary>
        ///     根据用户查询串计算摘要
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Pojo.Summary ToSummary(string query)
        {
            var parse = NlpAnalysis.Parse(query);

            var keywords = new List<Keyword.Keyword>();
            foreach (var term in parse)
            {
                if (FilterSet.Contains(term.Nature.natureStr))
                {
                    continue;
                }
                keywords.Add(new Keyword.Keyword(term.Name, term.TermNatures.AllFreq, 1));
            }

            return ToSummary(keywords);
        }

        /// <summary>
        ///     计算摘要，传入用户自己算好的关键词
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public Pojo.Summary ToSummary(List<Keyword.Keyword> keywords)
        {
            if (keywords == null)
            {
                keywords = new List<Keyword.Keyword>();
            }

            if (keywords.Count == 0)
            {
                var kc = new KeyWordComputer(10);
                keywords = kc.ComputeArticleTfidf(_title, _content);
            }
            return Explan(keywords, _content);
        }

        /// <summary>
        ///     计算摘要
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private Pojo.Summary Explan(List<Keyword.Keyword> keywords, string content)
        {
            var sf = new SmartForest<double>();

            foreach (var keyword in keywords)
            {
                sf.Add(keyword.Name, keyword.Score);
            }

            // 先断句
            var sentences = ToSentenceList(content.ToCharArray());

            foreach (var sentence in sentences)
            {
                ComputeScore(sentence, sf);
            }

            double maxScore = 0;
            var maxIndex = 0;

            for (var i = 0; i < sentences.Count; i++)
            {
                var tempScore = sentences[i].Score;
                var tempLength = sentences[i].Value.Length;

                if (tempLength >= _len)
                {
                    if (maxScore < tempScore)
                    {
                        maxScore = tempScore;
                        maxIndex = i;
                        continue;
                    }
                }
                for (var j = i + 1; j < sentences.Count; j++)
                {
                    tempScore += sentences[j].Score;
                    tempLength += sentences[j].Value.Length;
                    if (tempLength >= _len)
                    {
                        if (maxScore < tempScore)
                        {
                            maxScore = tempScore;
                            maxIndex = i;
                            break;
                        }
                    }
                }

                if (tempLength < _len)
                {
                    if (maxScore < tempScore)
                    {
                        maxIndex = i;
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            var sb = new StringBuilder();
            for (var i = maxIndex; i < sentences.Count; i++)
            {
                sb.Append(sentences[i].Value);
                if (sb.Length > _len)
                {
                    break;
                }
            }

            var summaryStr = sb.ToString();

            /**
             * 是否强制文本长度。对于abc这种字符算半个长度
             */

            if (_isSplitSummary && sb.Length > _len)
            {
                double value = _len;

                var newSummary = new StringBuilder();
                char c;
                for (var i = 0; i < sb.Length; i++)
                {
                    c = sb[i];
                    if (c < 256)
                    {
                        value -= 0.5;
                    }
                    else
                    {
                        value -= 1;
                    }

                    if (value < 0)
                    {
                        break;
                    }

                    newSummary.Append(c);
                }

                summaryStr = newSummary.ToString();
            }

            return new Pojo.Summary(keywords, summaryStr);
        }

        /// <summary>
        ///     计算一个句子的分数
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="forest"></param>
        private static void ComputeScore(Sentence sentence, SmartForest<double> forest)
        {
            var sgw = new SmartGetWord<double>(forest, sentence.Value);
            while (sgw.GetFrontWords() != null)
            {
                sentence.Score += sgw.Param;
            }
            if (sentence.Score == 0)
            {
                sentence.Score = sentence.Value.Length*-0.005;
            }
            else
            {
                sentence.Score /= Math.Log(sentence.Value.Length + 3);
            }
        }

        public List<Sentence> ToSentenceList(char[] chars)
        {
            var sb = new StringBuilder();

            var sentences = new List<Sentence>();

            for (var i = 0; i < chars.Length; i++)
            {
                if (sb.Length == 0 && (char.IsWhiteSpace(chars[i]) || chars[i] == ' '))
                {
                    continue;
                }

                sb.Append(chars[i]);
                switch (chars[i])
                {
                    case '.':
                        if (i < chars.Length - 1 && chars[i + 1] > 128)
                        {
                            insertIntoList(sb, sentences);
                            sb = new StringBuilder();
                        }
                        break;
                    case ' ':
                    case '	':
                    case ' ':
                    case '。':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case ';':
                    case '；':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case '!':
                    case '！':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case '?':
                    case '？':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                    case '\n':
                    case '\r':
                        insertIntoList(sb, sentences);
                        sb = new StringBuilder();
                        break;
                }
            }

            if (sb.Length > 0)
            {
                insertIntoList(sb, sentences);
            }

            return sentences;
        }

        private void insertIntoList(StringBuilder sb, List<Sentence> sentences)
        {
            var content = sb.ToString().Trim();
            if (content.Length > 0)
            {
                sentences.Add(new Sentence(content));
            }
        }

        /// <summary>
        ///     句子对象
        /// </summary>
        public class Sentence
        {
            public double Score;
            public string Value;

            public Sentence(string value)
            {
                Value = value.Trim();
            }

            public override string ToString()
            {
                return Value;
            }
        }
    }
}