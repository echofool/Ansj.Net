using System.Collections.Generic;
using System.Text;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.Library.Company;
using Ansj.Net.Recognition;
using Lucene.Net.Support;

namespace Ansj.Net.Util
{
    /// <summary>
    ///     term的操作类
    /// </summary>
    public class TermUtil
    {
        private static readonly HashMap<string, int[]> CompanyMap = CompanyAttrLibrary.GetCompanyMap();

        /// <summary>
        ///     将两个term合并为一个全新的term
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="termNatures"></param>
        /// <returns></returns>
        public static Term MakeNewTermNum(Term from, Term to, TermNatures termNatures)
        {
            var term = new Term(from.Name + to.Name, from.Offe, termNatures);
            term.TermNatures.NumAttr = @from.TermNatures.NumAttr;
            TermLink(term, to.To);
            TermLink(term.From, term);
            return term;
        }

        public static void TermLink(Term from, Term to)
        {
            if (from == null || to == null)
                return;
            from.To = to;
            to.From = from;
        }

        /// <summary>
        ///     将一个term插入到链表中的对应位置中,应该是词长由大到小
        /// </summary>
        /// <param name="terms"></param>
        /// <param name="term"></param>
        public static void InsertTerm(Term[] terms, Term term)
        {
            var temp = terms[term.Offe];
            //插入到最右面
            var last = temp;
            while ((temp = temp.GetNext()) != null)
            {
                last = temp;
            }
            last.SetNext(term);
        }

        public static void InsertTermNum(Term[] terms, Term term)
        {
            terms[term.Offe] = term;
        }

        public static void InsertTerm(Term[] terms, List<Term> tempList, TermNatures nr)
        {
            var sb = new StringBuilder();
            var offe = tempList[0].Offe;
            foreach (var item in tempList)
            {
                sb.Append(item.Name);
                terms[item.Offe] = null;
            }
            var term = new Term(sb.ToString(), offe, TermNatures.Nr);
            InsertTermNum(terms, term);
        }

        protected static Term SetToAndfrom(Term to, Term from)
        {
            from.To = to;
            to.From = from;
            return from;
        }

        /// <summary>
        ///     得到细颗粒度的分词，并且确定词性
        ///     返回是null说明已经是最细颗粒度
        /// </summary>
        /// <param name="term"></param>
        public static void ParseNature(Term term)
        {
            if (!Nature.NW.Equals(term.Nature))
            {
                return;
            }

            var name = term.Name;

            if (name.Length <= 3)
            {
                return;
            }

            // 是否是外国人名
            if (ForeignPersonRecognition.IsFName(name))
            {
                term.Nature = NatureLibrary.GetNature("nrf");
                return;
            }

            var subTerm = term.SubTerm;

            // 判断是否是机构名
            term.SubTerm = subTerm;
            var last = subTerm[subTerm.Count - 1];
            var all = 0;

            var item = CompanyMap[last.Name];
            if (item != null)
            {
                all += item[1];
            }

            if (all > 1000)
            {
                term.Nature = NatureLibrary.GetNature("nt");
            }
        }

        /// <summary>
        ///     从from到to生成subterm
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static List<Term> GetSubTerm(Term from, Term to)
        {
            var subTerm = new List<Term>(3);

            while ((from = @from.To) != to)
            {
                subTerm.Add(from);
            }

            return subTerm;
        }
    }
}