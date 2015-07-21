using System.Collections.Generic;
using System.IO;
using Ansj.Net.Dic;
using Ansj.Net.Domain;
using Ansj.Net.Recognition;
using Ansj.Net.SplitWord.Impl;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.SplitWord.Analysis
{
    /// <summary>
    ///     自然语言分词,具有未登录词发现功能。建议在自然语言理解中用。搜索中不要用
    /// </summary>
    public class NlpAnalysis : AbstractAnalysis
    {
        private LearnTool _learn;

        private NlpAnalysis()
        {
        }

        /// <summary>
        ///     用户自己定义的词典
        /// </summary>
        /// <param name="forests"></param>
        public NlpAnalysis(params IWoodInterface[] forests)
        {
            Forests = forests;
        }

        public NlpAnalysis(LearnTool learn, params IWoodInterface[] forests)
        {
            Forests = forests;
            _learn = learn;
        }

        public NlpAnalysis(TextReader reader, params IWoodInterface[] forests)
        {
            Forests = forests;
            ResetContent(new AnsjReader(reader));
        }

        public NlpAnalysis(TextReader reader, LearnTool learn, params IWoodInterface[] forests)
        {
            Forests = forests;
            _learn = learn;
            ResetContent(new AnsjReader(reader));
        }

        protected override List<Term> GetResult(Graph graph)
        {
            return new NlpAnalysisMerger {Graph = graph, NlpAnalysis = this}.Execute();
        }

        public static List<Term> Parse(string str)
        {
            return new NlpAnalysis().ParseStr(str);
        }

        public static List<Term> Parse(string str, params IWoodInterface[] forests)
        {
            return new NlpAnalysis(forests).ParseStr(str);
        }

        public static List<Term> Parse(string str, LearnTool learn, params IWoodInterface[] forests)
        {
            return new NlpAnalysis(learn, forests).ParseStr(str);
        }

        private class NlpAnalysisMerger : Merger
        {
            public Graph Graph;
            public NlpAnalysis NlpAnalysis;

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
                userDefineRecognition(Graph, NlpAnalysis.Forests);

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

                NlpAnalysis.SetRealName(Graph, result);
                return result;
            }
        }
    }
}