using Ansj.Net.Domain;
using Ansj.Net.Util;

namespace Ansj.Net.Recognition
{
    public class NumRecognition
    {
        /// <summary>
        ///     数字+数字合并,zheng
        /// </summary>
        /// <param name="terms"></param>
        public static void Recognition(Term[] terms)
        {
            var length = terms.Length - 1;
            for (var i = 0; i < length; i++)
            {
                if (terms[i] == null)
                {
                    continue;
                }
                if (".".Equals(terms[i].Name) || "．".Equals(terms[i].Name))
                {
                    // 如果是.前后都为数字进行特殊处理
                    var to = terms[i].To;
                    var from = terms[i].From;
                    if (@from.TermNatures.NumAttr.Flag && to.TermNatures.NumAttr.Flag)
                    {
                        @from.Name = @from.Name + "." + to.Name;
                        TermUtil.TermLink(@from, to.To);
                        terms[to.Offe] = null;
                        terms[i] = null;
                        i = @from.Offe - 1;
                    }
                    continue;
                }
                if (!terms[i].TermNatures.NumAttr.Flag)
                {
                    continue;
                }

                var temp = terms[i];
                // 将所有的数字合并
                while ((temp = temp.To).TermNatures.NumAttr.Flag)
                {
                    terms[i].Name = terms[i].Name + temp.Name;
                }
                // 如果是数字结尾
                if (MyStaticValue.IsQuantifierRecognition && temp.TermNatures.NumAttr.NumEndFreq > 0)
                {
                    terms[i].Name = terms[i].Name + temp.Name;
                    temp = temp.To;
                }

                // 如果不等,说明terms[i]发生了改变
                if (terms[i].To != temp)
                {
                    TermUtil.TermLink(terms[i], temp);
                    // 将中间无用元素设置为null
                    for (var j = i + 1; j < temp.Offe; j++)
                    {
                        terms[j] = null;
                    }
                    i = temp.Offe - 1;
                }
            }
        }
    }
}