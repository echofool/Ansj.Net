using System.Collections.Generic;
using System.IO;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.Recognition;
using Ansj.Net.SplitWord.Impl;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.SplitWord.Analysis
{
    /// <summary>
    ///     标准分词
    /// </summary>
    public class ToAnalysis : AbstractAnalysis
    {
        private ToAnalysis()
        {
        }

        /// <summary>
        ///     用户自己定义的词典
        /// </summary>
        /// <param name="forests"></param>
        public ToAnalysis(params IWoodInterface[] forests)
        {
            if (forests == null)
            {
                forests = new IWoodInterface[] {UserDefineLibrary.Forest};
            }
            Forests = forests;
        }

        public ToAnalysis(TextReader reader, params IWoodInterface[] forests)
        {
            Forests = forests;
            ResetContent(new AnsjReader(reader));
        }

        protected override List<Term> GetResult(Graph graph)
        {
            return new ToAnalysisMerger {Graph = graph, ToAnalysis = this}.Execute();
        }

        public static List<Term> Parse(string str)
        {
            return new ToAnalysis().ParseStr(str);
        }

        public static List<Term> Parse(string str, params IWoodInterface[] forests)
        {
            return new ToAnalysis(forests).ParseStr(str);
        }

        private class ToAnalysisMerger : Merger
        {
            public Graph Graph;
            public ToAnalysis ToAnalysis;

            #region Overrides of Merger

            public override List<Term> Execute()
            {
                Graph.WalkPath();

                // 数字发现
                if (MyStaticValue.IsNumRecognition && Graph.HasNum)
                {
                    NumRecognition.Recognition(Graph.Terms);
                }

                // 姓名识别
                if (Graph.HasPerson && MyStaticValue.IsNameRecognition)
                {
                    // 亚洲人名识别
                    new AsianPersonRecognition(Graph.Terms).Recognition();
                    Graph.WalkPathByScore();
                    NameFix.NameAmbiguity(Graph.Terms);
                    // 外国人名识别
                    new ForeignPersonRecognition(Graph.Terms).Recognition();
                    Graph.WalkPathByScore();
                }

                // 用户自定义词典的识别
                userDefineRecognition(Graph, ToAnalysis.Forests);

                return Result();
            }

            private void userDefineRecognition(Graph graph, params IWoodInterface[] forests)
            {
                new UserDefineRecognition(graph.Terms, forests).Recognition();
                graph.RemoveLittlePath();
                graph.WalkPathByScore();
            }


            /// <summary>
            ///     检索的分词
            /// </summary>
            /// <returns></returns>
            private List<Term> Result()
            {
                var result = new LinkedList<Term>();
                var length = Graph.Terms.Length - 1;
                for (var i = 0; i < length; i++)
                {
                    if (Graph.Terms[i] != null)
                    {
                        result.Add(Graph.Terms[i]);
                    }
                }

                var last = new LinkedList<Term>();
                foreach (var term in result)
                {
                    if (term.Name.Length >= 3)
                    {
                        var gwi = new GetWordsImpl(term.Name);
                        string temp;
                        while ((temp = gwi.AllWords()) != null)
                        {
                            if (temp.Length < term.Name.Length && temp.Length > 1)
                            {
                                last.Add(new Term(temp, gwi.Offe + term.Offe, TermNatures.Null));
                            }
                        }
                    }
                }

                result.AddAll(last);

                ToAnalysis.SetRealName(Graph, result);
                return result;
            }

            #endregion
        }
    }
}