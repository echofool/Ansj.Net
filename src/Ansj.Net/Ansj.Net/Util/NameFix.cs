using Ansj.Net.Domain;
using Nlpcn.Net.Commons.Lang.Util;

namespace Ansj.Net.Util
{
    public class NameFix
    {
        /// <summary>
        ///     人名消歧,比如.邓颖超生前->邓颖 超生 前 fix to 丁颖超 生 前! 规则的方式增加如果两个人名之间连接是- ， ·，•则连接
        /// </summary>
        /// <param name="terms"></param>
        public static void NameAmbiguity(Term[] terms)
        {
            Term term;
            Term next;
            for (var i = 0; i < terms.Length - 1; i++)
            {
                term = terms[i];
                if (term != null && term.TermNatures == TermNatures.Nr && term.Name.Length == 2)
                {
                    next = terms[i + 2];
                    if (next.TermNatures.PersonAttr.Split > 0)
                    {
                        term.Name = term.Name + next.Name[0];
                        terms[i + 2] = null;
                        terms[i + 3] = new Term(next.Name.Substring(1), next.Offe, TermNatures.Nw);
                        TermUtil.TermLink(term, terms[i + 3]);
                        TermUtil.TermLink(terms[i + 3], next.To);
                    }
                }
            }

            // 外国人名修正
            for (var i = 0; i < terms.Length; i++)
            {
                term = terms[i];
                if (term != null && term.Name.Length == 1 && i > 0 && WordAlert.CharCover(term.Name[0]) == '·')
                {
                    var from = term.From;
                    next = term.To;

                    if (from.Nature.natureStr.StartsWith("nr") && next.Nature.natureStr.StartsWith("nr"))
                    {
                        from.Name = from.Name + term.Name + next.Name;
                        TermUtil.TermLink(from, next.To);
                        terms[i] = null;
                        terms[i + 1] = null;
                    }
                }
            }
        }
    }
}