using System;
using System.Collections.Generic;
using System.Linq;
using Ansj.Net.Util;
using org.ansj.app.crf;
using org.ansj.app.crf.pojo;

namespace Ansj.Net.App.Crf
{
    /**
     * 分词
     * 
     * @author ansj
     * 
     */

    [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
    public class SplitWord
    {
        private readonly Model model;
        private readonly int[] revTagConver;
        private readonly int[] tagConver;
        /**
	 * 这个对象比较重。支持多线程，请尽量重复使用
	 * 
	 * @param model
	 * @throws Exception
	 */

        public SplitWord(Model model)
        {
            this.model = model;
            tagConver = new int[model.template.tagNum];
            revTagConver = new int[model.template.tagNum];
            var entrySet = model.template.statusMap;

            // case 0:'S';case 1:'B';case 2:'M';3:'E';
            foreach (var entry in entrySet)
            {
                if ("S".Equals(entry.Key))
                {
                    tagConver[entry.Value] = 0;
                    revTagConver[0] = entry.Value;
                }
                else if ("B".Equals(entry.Key))
                {
                    tagConver[entry.Value] = 1;
                    revTagConver[1] = entry.Value;
                }
                else if ("M".Equals(entry.Key))
                {
                    tagConver[entry.Value] = 2;
                    revTagConver[2] = entry.Value;
                }
                else if ("E".Equals(entry.Key))
                {
                    tagConver[entry.Value] = 3;
                    revTagConver[3] = entry.Value;
                }
            }

            model.end1 = model.template.statusMap["S"];
            model.end2 = model.template.statusMap["E"];
        }

        public List<string> cut(char[] chars)
        {
            return cut(new string(chars));
        }

        public List<string> cut(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return Enumerable.Empty<string>().ToList();
            }

            var elements = vterbi(line);

            var result = new LinkedList<string>();

            Element e = null;
            var begin = 0;
            var end = 0;

            for (var i = 0; i < elements.Count; i++)
            {
                e = elements[i];
                switch (fixTag(e.getTag()))
                {
                    case 0:
                        end += e.len;
                        result.Add(line.Substring(begin, end));
                        begin = end;
                        break;
                    case 1:
                        end += e.len;
                        while (fixTag((e = elements[++i]).getTag()) != 3)
                        {
                            end += e.len;
                        }
                        end += e.len;
                        result.Add(line.Substring(begin, end));
                        begin = end;
                        break;
                    default:
                        break;
                }
            }
            return result;
        }

        private List<Element> vterbi(string line)
        {
            var elements = WordAlert.Str2Elements(line);

            var length = elements.Count;
            if (length == 0)
            {
                // 避免空list，下面get(0)操作越界
                return elements;
            }
            if (length == 1)
            {
                elements[0].updateTag(revTagConver[0]);
                return elements;
            }

            /**
		 * 填充图
		 */
            for (var i = 0; i < length; i++)
            {
                computeTagScore(elements, i);
            }

            // 如果是开始不可能从 m，e开始 ，所以将它设为一个很小的值
            elements[0].tagScore[revTagConver[2]] = -1000;
            elements[0].tagScore[revTagConver[3]] = -1000;
            for (var i = 1; i < length; i++)
            {
                elements[i].maxFrom(model, elements[i - 1]);
            }

            // 末位置只能从S,E开始
            var next = elements[elements.Count - 1];
            Element self = null;
            var maxStatus = next.tagScore[model.end1] > next.tagScore[model.end2] ? model.end1 : model.end2;
            next.updateTag(maxStatus);
            maxStatus = next.from[maxStatus];
            // 逆序寻找
            for (var i = elements.Count - 2; i > 0; i--)
            {
                self = elements[i];
                self.updateTag(maxStatus);
                maxStatus = self.from[self.getTag()];
                next = self;
            }
            elements[0].updateTag(maxStatus);
            return elements;
        }

        private void computeTagScore(List<Element> elements, int index)
        {
            var tagScore = new double[model.template.tagNum];

            var t = model.template;
            char[] chars = null;
            for (var i = 0; i < t.ft.Length; i++)
            {
                chars = new char[t.ft[i].Length];
                for (var j = 0; j < chars.Length; j++)
                {
                    chars[j] = getElement(elements, index + t.ft[i][j]).name;
                }
                MatrixUtil.Dot(tagScore, model.getFeature(i, chars));
            }
            elements[index].tagScore = tagScore;
        }

        private Element getElement(List<Element> elements, int i)
        {
            // TODO Auto-generated method stub
            if (i < 0)
            {
                return new Element((char) ('B' + i));
            }
            if (i >= elements.Count)
            {
                return new Element((char) ('B' + i - elements.Count + 1));
            }
            return elements[i];
        }

        public int fixTag(int tag)
        {
            return tagConver[tag];
        }

        /**
	 * 随便给一个词。计算这个词的内聚分值，可以理解为计算这个词的可信度
	 * 
	 * @param word
	 */

        public double cohesion(string word)
        {
            if (word.Length == 0)
            {
                return int.MinValue;
            }

            var elements = WordAlert.Str2Elements(word);

            for (var i = 0; i < elements.Count; i++)
            {
                computeTagScore(elements, i);
            }

            var value = elements[0].tagScore[revTagConver[1]];

            var len = elements.Count - 1;

            for (var i = 1; i < len; i++)
            {
                value += elements[i].tagScore[revTagConver[2]];
            }

            value += elements[len].tagScore[revTagConver[3]];

            if (value < 0)
            {
                return 1;
            }
            value += 1;

            return value;
        }
    }
}