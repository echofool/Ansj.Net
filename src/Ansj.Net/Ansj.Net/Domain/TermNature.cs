using Ansj.Net.Library;

namespace Ansj.Net.Domain
{
    public class TermNature
    {
        public static readonly TermNature M = new TermNature("m", 1);
        public static readonly TermNature EN = new TermNature("en", 1);
        public static readonly TermNature BEGIN = new TermNature("始##始", 1);
        public static readonly TermNature END = new TermNature("末##末", 1);
        public static readonly TermNature USER_DEFINE = new TermNature("userDefine", 1);
        public static readonly TermNature NR = new TermNature("nr", 1);
        public static readonly TermNature NT = new TermNature("nt", 1);
        public static readonly TermNature NW = new TermNature("nw", 1);
        public static readonly TermNature NULL = new TermNature("null", 1);
        public int frequency;
        public Nature nature;

        public TermNature(string natureStr, int frequency)
        {
            nature = NatureLibrary.GetNature(natureStr);
            this.frequency = frequency;
        }

        public static TermNature[] setNatureStrToArray(string natureStr)
        {
            // TODO Auto-generated method stub
            natureStr = natureStr.Trim('{', '}');
            var split = natureStr.Split(',');
            var all = new TermNature[split.Length];
            for (var i = 0; i < split.Length; i++)
            {
                var strs = split[i].Split('=');
                var frequency = int.Parse(strs[1]);
                all[i] = new TermNature(strs[0].Trim(), frequency);
            }
            return all;
        }

        public override string ToString()
        {
            return nature.natureStr + "/" + frequency;
        }
    }
}