namespace Nlpcn.Net.Commons.Lang.Dat
{
    public abstract class Item
    {
        private int _base = 65536;
        public int Check { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }

        public int Base
        {
            get { return _base; }
            set { _base = value; }
        }

        /// <summary>
        /// 从词典中加载如果又特殊需求可重写此构造方法
        /// </summary>
        /// <param name="split"></param>
        public abstract void Init(string[] split);
        /// <summary>
        /// 从生成的词典中加载。应该和toText方法对应
        /// </summary>
        /// <param name="split"></param>

        public abstract void InitValue(string[] split);
        /// <summary>
        /// 以文本格式序列化的显示
        /// </summary>
        /// <returns></returns>
        public abstract string ToText();

        public override string ToString()
        {
            return ToText();
        }
    }
}