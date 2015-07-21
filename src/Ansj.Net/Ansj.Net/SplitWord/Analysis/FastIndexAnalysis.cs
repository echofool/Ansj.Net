using System.Collections.Generic;
using System.IO;
using Ansj.Net.Domain;
using Ansj.Net.Util;

namespace Ansj.Net.SplitWord.Analysis
{
    /// <summary>
    /// </summary>
    public class FastIndexAnalysis : AbstractAnalysis
    {
        public FastIndexAnalysis(TextReader br)
        {
            ResetContent(br);
        }

        public FastIndexAnalysis()
        {
        }

        public static List<Term> Parse(string str)
        {
            return new FastIndexAnalysis().ParseStr(str);
        }

        protected override List<Term> GetResult(Graph graph)
        {
            var result = new LinkedList<Term>();
            var length = graph.Terms.Length - 1;
            for (var i = 0; i < length; i++)
            {
                Term term;
                if ((term = graph.Terms[i]) != null)
                {
                    result.Add(term);
                    while ((term = term.GetNext()) != null)
                    {
                        result.Add(term);
                    }
                }
            }

            return result;
        }
    }
}