using System.Collections.Generic;
using System.Text;
using Nlpcn.Net.Commons.Lang.Tire;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.App.Summary
{
    /// <summary>
    ///     关键字标红
    /// </summary>
    public class TagContent
    {
        private readonly string _beginTag;
        private readonly string _endTag;

        public TagContent(string beginTag, string endTag)
        {
            _beginTag = beginTag;
            _endTag = endTag;
        }

        public string GetContent(Pojo.Summary summary)
        {
            return GetContent(summary.GetKeyWords(), summary.GetSummary());
        }

        public string GetContent(List<Keyword.Keyword> keyWords, string content)
        {
            var sf = new SmartForest<double>();
            foreach (var keyWord in keyWords)
            {
                sf.Add(keyWord.Name.ToLower(), keyWord.Score);
            }

            var sgw = new SmartGetWord<double>(sf, content.ToLower());

            var beginOffe = 0;
            string temp;
            var sb = new StringBuilder();
            while ((temp = sgw.GetFrontWords()) != null)
            {
                sb.Append(content.Substring(beginOffe, sgw.Offe));
                sb.Append(_beginTag);
                sb.Append(content.Substring(sgw.Offe, sgw.Offe + temp.Length));
                sb.Append(_endTag);
                beginOffe = sgw.Offe + temp.Length;
            }

            if (beginOffe < content.Length - 1)
            {
                sb.Append(content.Substring(beginOffe, content.Length));
            }

            return sb.ToString();
        }
    }
}