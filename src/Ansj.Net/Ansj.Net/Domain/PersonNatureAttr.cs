using System.Text;

namespace Ansj.Net.Domain
{
    /// <summary>
    ///     人名标注pojo类
    /// </summary>
    public class PersonNatureAttr
    {
        // public int B = -1;//0 姓氏
        // public int C = -1;//1 双名的首字
        // public int D = -1;//2 双名的末字
        // public int E = -1;//3 单名
        // public int N = -1; //4任意字
        // public int L = -1;//11 人名的下文
        // public int M = -1;//12 两个中国人名之间的成分
        // public int m = -1;//44 可拆分的姓名
        // String[] parretn = {"BC", "BCD", "BCDE", "BCDEN"}
        // double[] factory = {"BC", "BCD", "BCDE", "BCDEN"}

        public static readonly PersonNatureAttr Null = new PersonNatureAttr();
        private int[][] _locFreq;
        public int AllFreq { get; set; }
        // 12
        public int Begin { get; set; }
        // 11+12
        public int End { get; set; }
        // 是否有可能是名字的第一个字
        public bool Flag { get; set; }
        public int Split { get; set; }

        /// <summary>
        ///     设置
        /// </summary>
        /// <param name="index"></param>
        /// <param name="freq"></param>
        public void AddFreq(int index, int freq)
        {
            switch (index)
            {
                case 11:
                    End += freq;
                    AllFreq += freq;
                    break;
                case 12:
                    End += freq;
                    Begin += freq;
                    AllFreq += freq;
                    break;
                case 44:
                    Split += freq;
                    AllFreq += freq;
                    break;
            }
        }

        /// <summary>
        ///     得到某一个位置的词频
        /// </summary>
        /// <param name="length"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        public int GetFreq(int length, int loc)
        {
            if (_locFreq == null)
                return 0;
            if (length > 3)
                length = 3;
            if (loc > 4)
                loc = 4;
            return _locFreq[length][loc];
        }

        /// <summary>
        ///     词频记录表
        /// </summary>
        /// <param name="ints"></param>
        public void SetlocFreq(int[][] ints)
        {
            for (var i = 0; i < ints.Length; i++)
            {
                if (ints[i][0] > 0)
                {
                    Flag = true;
                    break;
                }
            }
            _locFreq = ints;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("begin=" + Begin);
            sb.Append(",");
            sb.Append("end=" + End);
            sb.Append(",");
            sb.Append("split=" + Split);
            return sb.ToString();
        }
    }
}