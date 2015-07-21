using System;

namespace org.ansj.app.crf.pojo
{
    [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
    public class Element
    {
        private static readonly double MIN = int.MinValue;
        public int[] from;
        public int len = 1;
        public char name;
        public string nature;
        private int tag = -1;
        public double[] tagScore;

        public Element(char name)
        {
            this.name = name;
        }

        public Element(char name, int tag)
        {
            this.name = name;
            this.tag = tag;
        }

        public int getTag()
        {
            return tag;
        }

        public Element updateTag(int tag)
        {
            this.tag = tag;
            return this;
        }

        public Element updateNature(string nature)
        {
            this.nature = nature;
            return this;
        }

        public void IncrementLen()
        {
            len++;
        }

        public override string ToString()
        {
            return name + "/" + tag;
        }

        public void maxFrom(Model model, Element element)
        {
            if (from == null)
            {
                from = new int[tagScore.Length];
            }
            var pTagScore = element.tagScore;
            double rate = 0;
            for (var i = 0; i < tagScore.Length; i++)
            {
                var maxValue = MIN;
                for (var j = 0; j < pTagScore.Length; j++)
                {
                    if ((rate = model.tagRate(j, i)) == double.MinValue)
                    {
                        continue;
                    }
                    var value = (pTagScore[j] + tagScore[i]) + rate;
                    if (value > maxValue)
                    {
                        maxValue = value;
                        from[i] = j;
                    }
                }
                tagScore[i] = maxValue;
            }
        }

        public static char getTagName(int tag)
        {
            // TODO Auto-generated method stub
            switch (tag)
            {
                case 0:
                    return 'S';
                case 1:
                    return 'B';
                case 2:
                    return 'M';
                case 3:
                    return 'E';
                default:
                    return '?';
            }
        }
    }
}