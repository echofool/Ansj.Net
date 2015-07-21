using Ansj.Net.Library;

namespace Ansj.Net.Domain
{
    /// <summary>
    ///     这里面封装了一些基本的词性.
    /// </summary>
    public class Nature
    {
        public static readonly Nature NW = NatureLibrary.GetNature("nw");
        public static readonly Nature NRF = NatureLibrary.GetNature("nrf");
        public static readonly Nature NR = NatureLibrary.GetNature("nr");
        public static readonly Nature NULL = NatureLibrary.GetNature("null");
        // 词性的频率
        public readonly int allFrequency;
        // 词性对照表的位置
        public readonly int index;
        // 词性的下标值
        public readonly int natureIndex;
        // 词性的名称
        public readonly string natureStr;

        public Nature(string natureStr, int index, int natureIndex, int allFrequency)
        {
            this.natureStr = natureStr;
            this.index = index;
            this.natureIndex = natureIndex;
            this.allFrequency = allFrequency;
        }

        public Nature(string natureStr)
        {
            this.natureStr = natureStr;
            index = 0;
            natureIndex = 0;
            allFrequency = 0;
        }

        public override string ToString()
        {
            return natureStr + ":" + index + ":" + natureIndex;
        }
    }
}