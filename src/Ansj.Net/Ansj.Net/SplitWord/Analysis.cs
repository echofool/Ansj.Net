using System;
using System.Collections.Generic;
using System.IO;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.SplitWord.Impl;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Tire;
using Nlpcn.Net.Commons.Lang.Tire.Domain;
using WordAlert = Nlpcn.Net.Commons.Lang.Util.WordAlert;

namespace Ansj.Net.SplitWord
{
    /// <summary>
    /// 基本分词+人名识别
    /// </summary>
    public abstract class AbstractAnalysis
    {
        /// <summary>
        /// 分词的类
        /// </summary>
        private readonly GetWordsImpl _getWordsImpl = new GetWordsImpl();

        private readonly LinkedList<Term> _terms = new LinkedList<Term>();
        private Forest _ambiguityForest = UserDefineLibrary.AmbiguityForest;

        /// <summary>
        /// 文档读取流
        /// </summary>
        private AnsjReader _ansjReader;

        protected IWoodInterface[] Forests = null;

        /// <summary>
        /// 用来记录偏移量
        /// </summary>
        public int Offe;

        public Forest AmbiguityForest
        {
            get { return _ambiguityForest; }
            set { _ambiguityForest = value; }
        }

        /// <summary>
        /// while 循环调用.直到返回为null则分词结束
        /// </summary>
        /// <returns></returns>
        public Term Next()
        {
            Term term;
            if (_terms.Count != 0)
            {
                term = _terms.Poll();
                term.UpdateOffe(Offe);
                return term;
            }

            var temp = _ansjReader.ReadLine();
            Offe = _ansjReader.Start;
            while (string.IsNullOrWhiteSpace(temp))
            {
                if (temp == null)
                {
                    return null;
                }
                temp = _ansjReader.ReadLine();
            }

            // 歧异处理字符串

            AnalysisStr(temp);

            if (!_terms.IsEmpty())
            {
                term = _terms.Poll();
                term.UpdateOffe(Offe);
                return term;
            }

            return null;
        }

        /// <summary>
        ///     一整句话分词,用户设置的歧异优先
        /// </summary>
        /// <param name="temp"></param>
        private void AnalysisStr(string temp)
        {
            var gp = new Graph(temp);
            var startOffe = 0;

            if (_ambiguityForest != null)
            {
                var gw = new GetWord(_ambiguityForest, gp.Chars);
                while ((gw.GetFrontWords()) != null)
                {
                    if (gw.Offe > startOffe)
                    {
                        Analysis(gp, startOffe, gw.Offe);
                    }
                    var @params = gw.GetParams();
                    startOffe = gw.Offe;
                    for (var i = 0; i < @params.Length; i += 2)
                    {
                        gp.AddTerm(new Term(@params[i], startOffe, new TermNatures(new TermNature(@params[i + 1], 1))));
                        startOffe += @params[i].Length;
                    }
                }
            }
            if (startOffe < gp.Chars.Length - 1)
            {
                Analysis(gp, startOffe, gp.Chars.Length);
            }
            var result = GetResult(gp);

            _terms.AddAll(result);
        }

        private void Analysis(Graph gp, int startOffe, int endOffe)
        {
            var chars = gp.Chars;

            for (var i = startOffe; i < endOffe; i++)
            {
                int start;
                int end;
                string str;
                switch (DatDictionary.Status(chars[i]))
                {
                    case 0:
                        gp.AddTerm(new Term(chars[i].ToString(), i, TermNatures.Null));
                        break;
                    case 4:
                        start = i;
                        end = 1;
                        while (++i < endOffe && DatDictionary.Status(chars[i]) == 4)
                        {
                            end++;
                        }
                        str = WordAlert.AlertEnglish(chars, start, end);
                        gp.AddTerm(new Term(str, start, TermNatures.En));
                        i--;
                        break;
                    case 5:
                        start = i;
                        end = 1;
                        while (++i < endOffe && DatDictionary.Status(chars[i]) == 5)
                        {
                            end++;
                        }
                        str = WordAlert.AlertNumber(chars, start, end);
                        gp.AddTerm(new Term(str, start, TermNatures.M));
                        i--;
                        break;
                    default:
                        start = i;
                        end = i;
                        var c = chars[start];
                        while (DatDictionary.InSystem[c] > 0)
                        {
                            end++;
                            if (++i >= endOffe)
                                break;
                            c = chars[i];
                        }

                        if (start == end)
                        {
                            gp.AddTerm(new Term(c.ToString(), i, TermNatures.Null));
                            continue;
                        }

                        _getWordsImpl.SetChars(chars, start, end);
                        while ((str = _getWordsImpl.AllWords()) != null)
                        {
                            gp.AddTerm(new Term(str, _getWordsImpl.Offe, _getWordsImpl.GetItem()));
                        }

                        /**
                         * 如果未分出词.以未知字符加入到gp中
                         */
                        if (DatDictionary.InSystem[c] > 0 || DatDictionary.Status(c) > 3)
                        {
                            i -= 1;
                        }
                        else
                        {
                            gp.AddTerm(new Term(c.ToString(), i, TermNatures.Null));
                        }

                        break;
                }
            }
        }

        /// <summary>
        ///     将为标准化的词语设置到分词中
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="result"></param>
        protected void SetRealName(Graph graph, List<Term> result)
        {
            if (!MyStaticValue.IsRealName)
            {
                return;
            }

            var str = graph.RealStr;

            foreach (var term in result)
            {
                term.RealName = str.Substring(term.Offe, term.Offe + term.Name.Length);
            }
        }

        protected List<Term> ParseStr(string temp)
        {
            AnalysisStr(temp);
            return _terms;
        }

        protected abstract List<Term> GetResult(Graph graph);

        /// <summary>
        ///     重置分词器
        /// </summary>
        /// <param name="br"></param>
        public void ResetContent(AnsjReader br)
        {
            Offe = 0;
            _ansjReader = br;
        }

        public void ResetContent(TextReader reader)
        {
            Offe = 0;
            _ansjReader = new AnsjReader(reader);
        }

        public void ResetContent(TextReader reader, int buffer)
        {
            Offe = 0;
            _ansjReader = new AnsjReader(reader, buffer);
        }

        public abstract class Merger
        {
            public abstract List<Term> Execute();

            public static Merger Create(Func<List<Term>> mergerFunc)
            {
                return new MergerImpl { MererFunc = mergerFunc };
            }

            private class MergerImpl : Merger
            {
                public Func<List<Term>> MererFunc { get; set; }

                #region Overrides of Merger

                public override List<Term> Execute()
                {
                    return MererFunc();
                }

                #endregion
            }
        }
    }
}