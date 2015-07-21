using System;

namespace Ansj.Net.Domain
{
    /// <summary>
    ///     每一个term都拥有一个词性集合
    /// </summary>
    public class TermNatures
    {
        public static readonly TermNatures M = new TermNatures(TermNature.M);
        public static readonly TermNatures Nr = new TermNatures(TermNature.NR);
        public static readonly TermNatures En = new TermNatures(TermNature.EN);
        public static readonly TermNatures End = new TermNatures(TermNature.END, 50610, -1);
        public static readonly TermNatures Begin = new TermNatures(TermNature.BEGIN, 50610, 0);
        public static readonly TermNatures Nt = new TermNatures(TermNature.NT);
        public static readonly TermNatures Nw = new TermNatures(TermNature.NW);
        public static readonly TermNatures Null = new TermNatures(TermNature.NULL);

        /// <summary>
        ///     数字属性
        /// </summary>
        private NumNatureAttr _numAttr = NumNatureAttr.Null;

        /// <summary>
        ///     人名词性
        /// </summary>
        private PersonNatureAttr _personAttr = PersonNatureAttr.Null;

        /// <summary>
        ///     所有的词频
        /// </summary>
        public int AllFreq;

        /// <summary>
        ///     词的id
        /// </summary>
        public int ID = -2;

        /// <summary>
        ///     构造方法.一个词对应这种玩意
        /// </summary>
        /// <param name="termNatures"></param>
        /// <param name="id"></param>
        public TermNatures(TermNature[] termNatures, int id)
        {
            ID = id;
            Natures = termNatures;
            // find maxNature
            var maxFreq = -1;
            TermNature termNature = null;
            for (var i = 0; i < termNatures.Length; i++)
            {
                if (maxFreq < termNatures[i].frequency)
                {
                    maxFreq = termNatures[i].frequency;
                    termNature = termNatures[i];
                }
            }

            if (termNature != null)
            {
                Nature = termNature.nature;
            }

            SerAttribute();
        }

        public TermNatures(TermNature termNature)
        {
            Natures = new TermNature[1];
            Natures[0] = termNature;
            Nature = termNature.nature;
            SerAttribute();
        }

        public TermNatures(TermNature termNature, int allFreq, int id)
        {
            ID = id;
            Natures = new TermNature[1];
            termNature.frequency = allFreq;
            Natures[0] = termNature;
            AllFreq = allFreq;
        }

        /// <summary>
        ///     默认词性
        /// </summary>
        public Nature Nature { get; set; }

        /// <summary>
        ///     关于这个term的所有词性
        /// </summary>
        public TermNature[] Natures { get; set; }

        /// <summary>
        ///     人名词性
        /// </summary>
        public PersonNatureAttr PersonAttr
        {
            get { return _personAttr; }
            set { _personAttr = value; }
        }

        /// <summary>
        ///     数字属性
        /// </summary>
        public NumNatureAttr NumAttr
        {
            get { return _numAttr; }
            set { _numAttr = value; }
        }

        private void SerAttribute()
        {
            var max = 0;
            NumNatureAttr numNatureAttr = null;
            for (var i = 0; i < Natures.Length; i++)
            {
                var termNature = Natures[i];
                AllFreq += termNature.frequency;
                max = Math.Max(max, termNature.frequency);
                switch (termNature.nature.index)
                {
                    case 18:
                        if (numNatureAttr == null)
                        {
                            numNatureAttr = new NumNatureAttr();
                        }
                        numNatureAttr.NumFreq = termNature.frequency;
                        break;
                    case 29:
                        if (numNatureAttr == null)
                        {
                            numNatureAttr = new NumNatureAttr();
                        }
                        numNatureAttr.NumEndFreq = termNature.frequency;
                        break;
                }
            }
            if (numNatureAttr != null)
            {
                if (max == numNatureAttr.NumFreq)
                {
                    numNatureAttr.Flag = true;
                }
                _numAttr = numNatureAttr;
            }
        }

        public void SetPersonNatureAttr(PersonNatureAttr personAttr)
        {
            _personAttr = personAttr;
        }
    }
}