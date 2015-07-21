namespace Nlpcn.Net.Commons.Lang.Dat
{
    public class BasicItem : Item
    {
        /// <summary>
        /// 从词典中加载如果又特殊需求可重写此构造方法
        /// </summary>
        /// <param name="split"></param>

        public override void Init(string[] split)
        {
            Name = split[0];
        }

        /// <summary>
        /// 从生成的词典中加载。应该和toString方法对应
        /// </summary>
        /// <param name="split"></param>

        public override void InitValue(string[] split)
        {
            Index = int.Parse(split[0]);
            Name = split[1];
            Base = int.Parse(split[2]);
            Check = int.Parse(split[3]);
            Status = byte.Parse(split[4]);
        }

        public override string ToText()
        {
            return Index + "\t" + Name + "\t" + Base + "\t" + Check + "\t" + Status;
        }

        public override string ToString()
        {
            return ToText();
        }
    }
}