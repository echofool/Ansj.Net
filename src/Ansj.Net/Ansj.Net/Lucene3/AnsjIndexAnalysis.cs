using System.Collections.Generic;
using System.IO;
using Ansj.Net.Lucene.Util;
using Ansj.Net.SplitWord.Analysis;
using Lucene.Net.Analysis;

namespace Ansj.Net.Lucene3
{
    public class AnsjIndexAnalysis : Analyzer
    {
        /// <summary>
        ///     如果需要停用词就传入停用词的hashmap value0
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="pstemming">是否分析词干</param>
        public AnsjIndexAnalysis(HashSet<string> filter, bool pstemming)
        {
            Filter = filter;
            Pstemming = pstemming;
        }

        public AnsjIndexAnalysis(bool pstemming)
        {
            Pstemming = pstemming;
        }

        public AnsjIndexAnalysis()
        {
        }

        public HashSet<string> Filter { get; set; }
        public bool Pstemming { get; set; }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            return new AnsjTokenizer(new IndexAnalysis(reader), reader, Filter, Pstemming);
        }
    }
}