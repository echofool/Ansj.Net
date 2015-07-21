using System.Collections.Generic;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.Util;
using WordAlert = Nlpcn.Net.Commons.Lang.Util.WordAlert;

namespace Ansj.Net.Recognition
{
    public class NatureRecognition
    {
        private readonly NatureTerm[] _end = {new NatureTerm(TermNature.END)};
        private readonly NatureTerm[][] _natureTermTable;
        private readonly NatureTerm _root = new NatureTerm(TermNature.BEGIN);
        private readonly List<Term> _terms;

        /// <summary>
        ///     构造方法.传入分词的最终结果
        /// </summary>
        /// <param name="terms"></param>
        public NatureRecognition(List<Term> terms)
        {
            _terms = terms;
            _natureTermTable = new NatureTerm[terms.Count + 1][];
            _natureTermTable[terms.Count] = _end;
        }

        /// <summary>
        ///     进行最佳词性查找,引用赋值.所以不需要有返回值
        /// </summary>
        public void Recognition()
        {
            var length = _terms.Count;
            for (var i = 0; i < length; i++)
            {
                _natureTermTable[i] = GetNatureTermArr(_terms[i].TermNatures.Natures);
            }
            Walk();
        }

        /// <summary>
        ///     传入一组。词对词语进行。词性标注
        /// </summary>
        /// <param name="words"></param>
        /// <param name="offe"></param>
        /// <returns></returns>
        public static List<Term> Recognition(List<string> words, int offe)
        {
            var terms = new List<Term>(words.Count);
            var tempOffe = 0;
            foreach (var word in words)
            {
                // 获得词性 ， 先从系统辞典。在从用户自定义辞典
                var ansjItem = DatDictionary.GetItem(word);
                TermNatures tn;
                if (ansjItem.Natures != TermNatures.Null)
                {
                    tn = ansjItem.Natures;
                }
                else
                {
                    string[] @params;
                    if ((@params = UserDefineLibrary.GetParams(word)) != null)
                    {
                        tn = new TermNatures(new TermNature(@params[0], 1));
                    }
                    else if (WordAlert.IsEnglish(word))
                    {
                        tn = TermNatures.En;
                    }
                    else if (WordAlert.IsNumber(word))
                    {
                        tn = TermNatures.M;
                    }
                    else
                    {
                        tn = TermNatures.Null;
                    }
                }

                terms.Add(new Term(word, offe + tempOffe, tn));
                tempOffe += word.Length;
            }
            new NatureRecognition(terms).Recognition();
            return terms;
        }

        public void Walk()
        {
            var length = _natureTermTable.Length - 1;
            SetScore(_root, _natureTermTable[0]);
            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < _natureTermTable[i].Length; j++)
                {
                    SetScore(_natureTermTable[i][j], _natureTermTable[i + 1]);
                }
            }
            OptimalRoot();
        }

        private void SetScore(NatureTerm natureTerm, NatureTerm[] natureTerms)
        {
            for (var i = 0; i < natureTerms.Length; i++)
            {
                natureTerms[i].SetScore(natureTerm);
            }
        }

        private static NatureTerm[] GetNatureTermArr(TermNature[] termNatures)
        {
            var natureTerms = new NatureTerm[termNatures.Length];
            for (var i = 0; i < natureTerms.Length; i++)
            {
                natureTerms[i] = new NatureTerm(termNatures[i]);
            }
            return natureTerms;
        }

        /// <summary>
        ///     获得最优路径
        /// </summary>
        private void OptimalRoot()
        {
            var to = _end[0];
            NatureTerm from;
            var index = _natureTermTable.Length - 1;
            while ((from = to.From) != null && index > 0)
            {
                _terms[--index].Nature = from.TermNature.nature;
                to = from;
            }
        }

        /// <summary>
        ///     关于这个term的词性
        /// </summary>
        public class NatureTerm
        {
            public double Score;
            public double SelfScore;
            public TermNature TermNature;

            public NatureTerm(TermNature termNature)
            {
                TermNature = termNature;
                SelfScore = termNature.frequency + 1;
            }

            public NatureTerm From { get; private set; }

            public void SetScore(NatureTerm natureTerm)
            {
                var tempScore = MathUtil.CompuNatureFreq(natureTerm, this);
                if (From == null || Score < tempScore)
                {
                    Score = tempScore;
                    From = natureTerm;
                }
            }

            public override string ToString()
            {
                return TermNature.nature.natureStr + "/" + SelfScore;
            }
        }
    }
}