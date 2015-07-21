using System.Collections.Generic;
using System.IO;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.Recognition;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.SplitWord.Analysis
{
    /// <summary>
    ///     默认用户自定义词性优先
    /// </summary>
    public class UserDefineAnalysis : AbstractAnalysis
    {
        private UserDefineAnalysis()
        {
        }

        /// <summary>
        ///     用户自己定义的词典
        /// </summary>
        /// <param name="forests"></param>
        public UserDefineAnalysis(params IWoodInterface[] forests)
        {
            if (forests == null)
            {
                forests = new IWoodInterface[] {UserDefineLibrary.Forest};
            }
            Forests = forests;
        }

        public UserDefineAnalysis(TextReader reader, params IWoodInterface[] forests)
        {
            Forests = forests;
            ResetContent(new AnsjReader(reader));
        }

        protected override List<Term> GetResult(Graph graph)
        {
            return new UserDefineAnalysisMerger {Graph = graph, Analysis = this}.Execute();
        }

        public static List<Term> Parse(string str)
        {
            return new UserDefineAnalysis().ParseStr(str);
        }

        public static List<Term> Parse(string str, params IWoodInterface[] forests)
        {
            return new UserDefineAnalysis(forests).ParseStr(str);
        }

        private class UserDefineAnalysisMerger : Merger
        {
            public UserDefineAnalysis Analysis;
            public Graph Graph;

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
                userDefineRecognition(Graph, Analysis.Forests);

                return GetResult();
            }

            private void userDefineRecognition(Graph graph, params IWoodInterface[] forests)
            {
                new UserDefineRecognition(graph.Terms, forests).Recognition();
                graph.RemoveLittlePath();
                graph.WalkPathByScore();
            }

            private List<Term> GetResult()
            {
                var result = new List<Term>();
                var length = Graph.Terms.Length - 1;
                for (var i = 0; i < length; i++)
                {
                    if (Graph.Terms[i] != null)
                    {
                        result.Add(Graph.Terms[i]);
                    }
                }
                Analysis.SetRealName(Graph, result);

                FilterModifWord.ModifResult(result);
                return result;
            }

            #endregion
        }
    }
}