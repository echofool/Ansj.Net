using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Lucene.Net.Support;

namespace org.ansj.app.crf.pojo
{
    /**
     * 解析crf++模板
     * 
     * @author ansj
     * 
     */

    [Serializable]
    public class Template
    {
        private static readonly long serialVersionUID = 8265350854930361325L;

        public int[][] ft =
        {
            new[] {-2}, new[] {-1}, new[] {0}, new[] {1}, new[] {2}, new[] {-2, -1}, new[] {-1, 0},
            new[] {0, 1}, new[] {1, 2}, new[] {-1, 1}
        };

        public int left = 2;
        public int right = 2;
        public HashMap<string, int> statusMap;
        public int tagNum;
        /**
         * 解析配置文件
         * 
         * @param templatePath
         * @return
         * @throws IOException
         */

        public static Template parse(string templateStr)
        {
            // TODO Auto-generated method stub
            return parse(new StringReader(templateStr));
        }

        public static Template parse(TextReader br)
        {
            var t = new Template();

            string temp = null;

            var lists = new List<string>();
            while ((temp = br.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(temp) || temp.StartsWith("#"))
                {
                    continue;
                }
                lists.Add(temp);
            }
            br.Close();

            t.ft = new int[lists.Count - 1][];
            for (var i = 0; i < lists.Count - 1; i++)
            {
                temp = lists[i];
                var split = temp.Split(':');

                var index = int.Parse(split[0].Substring(1));

                split = split[1].Split(' ');
                var ints = new int[split.Length];

                for (var j = 0; j < ints.Length; j++)
                {
                    ints[j] = int.Parse(split[j].Substring(split[j].IndexOf('[') + 1, split[j].IndexOf(',')));
                }
                t.ft[index] = ints;
            }
            t.left = 0;
            t.right = 0;
            // find max and min
            foreach (var ints in t.ft)
            {
                foreach (var j in ints)
                {
                    t.left = t.left > j ? j : t.left;
                    t.right = t.right < j ? j : t.right;
                }
            }
            t.left = t.left;

            return t;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("left:" + left);
            sb.Append("\t");
            sb.Append("rightr:" + right);
            sb.Append("\n");
            foreach (var ints in ft)
            {
                sb.Append(string.Join(",", ints));
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}