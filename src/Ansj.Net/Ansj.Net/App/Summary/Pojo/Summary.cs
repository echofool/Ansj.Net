using System.Collections.Generic;

namespace Ansj.Net.App.Summary.Pojo
{
    /// <summary>
    ///     摘要结构体封装
    /// </summary>
    public class Summary
    {
        /// <summary>
        ///     关键词
        /// </summary>
        private readonly List<Keyword.Keyword> _keyWords;

        /// <summary>
        ///     摘要
        /// </summary>
        private readonly string _summary;

        public Summary(List<Keyword.Keyword> keyWords, string summary)
        {
            _keyWords = keyWords;
            _summary = summary;
        }

        public List<Keyword.Keyword> GetKeyWords()
        {
            return _keyWords;
        }

        public string GetSummary()
        {
            return _summary;
        }
    }
}