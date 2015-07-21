namespace Ansj.Net.Domain
{
    public class NumNatureAttr
    {
        public static readonly NumNatureAttr Null = new NumNatureAttr();
        private int _numEndFreq = -1;
        private int _numFreq = -1;

        /// <summary>
        /// </summary>
        public NumNatureAttr()
        {
            Flag = false;
        }

        /// <summary>
        ///     数字的结尾
        /// </summary>
        public int NumFreq
        {
            get { return _numFreq; }
            set { _numFreq = value; }
        }

        /// <summary>
        ///     是有可能是一个数字
        /// </summary>
        public int NumEndFreq
        {
            get { return _numEndFreq; }
            set { _numEndFreq = value; }
        }

        /// <summary>
        ///     最大词性是否是数字
        /// </summary>
        public bool Flag { get; set; }
    }
}