using System.IO;
using Ansj.Net.Domain;
using Ansj.Net.Util;
using Lucene.Net.Support;

namespace Ansj.Net.Library.Name
{
    /// <summary>
    ///     人名标注所用的词典就是简单的hashmap简单方便谁用谁知道,只在加载词典的时候用
    /// </summary>
    public class PersonAttrLibrary
    {
        private HashMap<string, PersonNatureAttr> _personNatureAttrs;

        public HashMap<string, PersonNatureAttr> GetPersonMap()
        {
            if (_personNatureAttrs != null)
            {
                return _personNatureAttrs;
            }
            Init1();
            Init2();
            return _personNatureAttrs;
        }

        /// <summary>
        ///     name_freq
        /// </summary>
        private void Init2()
        {
            var personFreqMap = MyStaticValue.GetPersonFreqMap();
            foreach (var entry in personFreqMap)
            {
                var pna = _personNatureAttrs[entry.Key];
                if (pna == null)
                {
                    pna = new PersonNatureAttr();
                    pna.SetlocFreq(entry.Value);
                    _personNatureAttrs.Add(entry.Key, pna);
                }
                else
                {
                    pna.SetlocFreq(entry.Value);
                }
            }
        }

        /// <summary>
        ///     person.dic
        /// </summary>
        private void Init1()
        {
            TextReader br = null;
            try
            {
                _personNatureAttrs = new HashMap<string, PersonNatureAttr>();
                br = MyStaticValue.GetPersonReader();
                string temp;
                while ((temp = br.ReadLine()) != null)
                {
                    var strs = temp.Split('\t');
                    var pna = _personNatureAttrs[strs[0]];
                    if (pna == null)
                    {
                        pna = new PersonNatureAttr();
                    }
                    pna.AddFreq(int.Parse(strs[1]), int.Parse(strs[2]));
                    _personNatureAttrs.Add(strs[0], pna);
                }
            }
            finally
            {
                if (br != null)
                    br.Close();
            }
        }
    }
}