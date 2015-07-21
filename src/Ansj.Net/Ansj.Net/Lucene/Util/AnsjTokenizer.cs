using System;
using System.Collections.Generic;
using System.IO;
using Ansj.Net.Domain;
using Ansj.Net.SplitWord;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace Ansj.Net.Lucene.Util
{
    public sealed class AnsjTokenizer : Tokenizer
    {
        private readonly AbstractAnalysis _analysis;
        private readonly HashSet<string> _filter;

        /// <summary>
        ///     偏移量
        /// </summary>
        private readonly OffsetAttribute _offsetAtt;

        /// <summary>
        ///     距离
        /// </summary>
        private readonly PositionIncrementAttribute _positionAttr;

        private readonly bool _pstemming;
        private readonly PorterStemmer _stemmer = new PorterStemmer();

        /// <summary>
        ///     当前词
        /// </summary>
        private readonly TermAttribute _termAtt;

        public AnsjTokenizer(AbstractAnalysis analysis, TextReader input, HashSet<string> filter, bool pstemming)
            : base(input)
        {
            _analysis = analysis;
            _termAtt = AddAttribute<TermAttribute>();
            _offsetAtt = AddAttribute<OffsetAttribute>();
            _positionAttr = AddAttribute<PositionIncrementAttribute>();
            _filter = filter;
            _pstemming = pstemming;
        }

        #region Overrides of TokenStream

        /// <summary>
        ///     Consumers (i.e., <see cref="T:Lucene.Net.Index.IndexWriter" />) use this method to advance the stream to
        ///     the next token. Implementing classes must implement this method and update
        ///     the appropriate <see cref="T:Lucene.Net.Util.Attribute" />s with the attributes of the next
        ///     token.
        ///     The producer must make no assumptions about the attributes after the
        ///     method has been returned: the caller may arbitrarily change it. If the
        ///     producer needs to preserve the state for subsequent calls, it can use
        ///     <see cref="M:Lucene.Net.Util.AttributeSource.CaptureState" /> to create a copy of the current attribute state.
        ///     This method is called for every token of a document, so an efficient
        ///     implementation is crucial for good performance. To avoid calls to
        ///     <see cref="M:Lucene.Net.Util.AttributeSource.AddAttribute``1" /> and
        ///     <see cref="M:Lucene.Net.Util.AttributeSource.GetAttribute``1" />,
        ///     references to all <see cref="T:Lucene.Net.Util.Attribute" />s that this stream uses should be
        ///     retrieved during instantiation.
        ///     To ensure that filters and consumers know which attributes are available,
        ///     the attributes must be added during instantiation. Filters and consumers
        ///     are not required to check for availability of attributes in
        ///     <see cref="M:Lucene.Net.Analysis.TokenStream.IncrementToken" />.
        /// </summary>
        /// <returns>
        ///     false for end of stream; true otherwise
        /// </returns>
        public override bool IncrementToken()
        {
            // TODO Auto-generated method stub
            ClearAttributes();
            var position = 0;
            Term term;
            var length = 0;
            do
            {
                term = _analysis.Next();
                if (term == null)
                {
                    break;
                }
                length = term.Name.Length;
                if (_pstemming && term.TermNatures.Natures[0] == TermNature.EN)
                {
                    Console.WriteLine(_pstemming);
                    var name = _stemmer.Stem(term.Name);
                    term.Name = name;
                }
                position++;
            } while (_filter != null && (_filter.Contains(term.Name) || term.Name.Length > 30));

            if (term != null)
            {
                _positionAttr.PositionIncrement = position;
                _termAtt.SetTermBuffer(term.Name.ToCharArray(), 0, term.Name.Length);
                _offsetAtt.SetOffset(term.Offe, term.Offe + length);
                return true;
            }
            End();
            return false;
        }

        #endregion
    }
}