using System.Collections.Generic;
using System.IO;
using Ansj.Net.Domain;
using Ansj.Net.Util;

namespace Ansj.Net.SplitWord.Analysis
{
    /// <summary>
    ///     基本的分词.只做了.ngram模型.和数字发现.其他一律不管
    /// </summary>
    public class BaseAnalysis : AbstractAnalysis
    {
        private BaseAnalysis()
        {
        }

        public BaseAnalysis(TextReader reader)
        {
            ResetContent(new AnsjReader(reader));
        }

        protected override List<Term> GetResult(Graph graph)
        {
            var merger = Merger.Create(() =>
            {
                graph.WalkPath();
                var result = new List<Term>();
                var length = graph.Terms.Length - 1;
                for (var i = 0; i < length; i++)
                {
                    if (graph.Terms[i] != null)
                    {
                        result.Add(graph.Terms[i]);
                    }
                }

                SetRealName(graph, result);
                return result;
            });
            return merger.Execute();
        }

        public static List<Term> Parse(string str)
        {
            return new BaseAnalysis().ParseStr(str);
        }
    }
}