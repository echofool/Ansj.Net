using System.Linq;
using System.Text;

namespace Nlpcn.Net.Commons.Lang.Tire.Domain
{
    public class Value
    {
        private const char Tab = '\t';

        public Value(string keyword, params string[] paramers)
        {
            Keyword = keyword;
            if (paramers != null)
            {
                Paramers = paramers;
            }
        }

        public Value(string temp)
        {
            var strs = temp.Split(Tab);
            Keyword = strs[0];
            if (strs.Length > 1)
            {
                Paramers = strs.Skip(1).ToArray();
            }
        }

        public string Keyword { get; set; }
        public string[] Paramers { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Keyword);
            for (var i = 0; i < Paramers.Length; i++)
            {
                sb.Append(Tab);
                sb.Append(Paramers[i]);
            }
            return sb.ToString();
        }
    }
}