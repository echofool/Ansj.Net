using Lucene.Net.Support;
using Nlpcn.Net.Commons.Lang.Dat;

namespace Ansj.Net.Domain
{
    public class AnsjItem : Item
    {
        private static readonly long SerialVersionUid = 1L;
        public static readonly AnsjItem Null = new AnsjItem();
        public static readonly AnsjItem Begin = new AnsjItem();
        public static readonly AnsjItem End = new AnsjItem();

        static AnsjItem()
        {
            Null.Base = 0;
            Begin.Index = 0;
            Begin.Natures = TermNatures.Begin;

            End.Index = -1;
            End.Natures = TermNatures.End;
        }

        public AnsjItem()
        {
            BigramEntryMap = null;
        }

        public string Param { get; set; }

        /// <summary>
        ///     词性词典,以及词性的相关权重
        /// </summary>
        public TermNatures Natures { get; set; }

        public HashMap<int, int> BigramEntryMap { get; set; }

        #region Overrides of Item

        public override void Init(string[] split)
        {
            Name = split[0];
            Param = split[1];
        }

        public override void InitValue(string[] split)
        {
            Index = int.Parse(split[0]);
            Base = int.Parse(split[2]);
            Check = int.Parse(split[3]);
            Status = byte.Parse(split[4]);
            if (Status > 1)
            {
                Name = split[1];
                Natures = new TermNatures(TermNature.setNatureStrToArray(split[5]), Index);
            }
            else
            {
                Natures = new TermNatures(TermNature.NULL);
            }
        }

        public override string ToText()
        {
            return Index + "\t" + Name + "\t" + Base + "\t" + Check + "\t" + Status + "\t" + Param;
        }

        #endregion
    }
}