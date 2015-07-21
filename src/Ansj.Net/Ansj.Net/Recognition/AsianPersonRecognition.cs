using System;
using System.Collections.Generic;
using System.Text;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.Util;

namespace Ansj.Net.Recognition
{
    /// <summary>
    ///     人名识别工具类
    /// </summary>
    public class AsianPersonRecognition
    {
        private static readonly double[] Factory = {0.16271366224044456, 0.8060521860870434, 0.031234151672511947};
        private readonly Term[] _terms;
        private bool _skip;
        // 名称是否有歧异
        // public int B = -1;//0 姓氏
        // public int C = -1;//1 双名的首字
        // public int D = -1;//2 双名的末字
        // public int E = -1;//3 单名
        // public int N = -1; //4任意字
        // public int L = -1;//11 人名的下文
        // public int M = -1;//12 两个中国人名之间的成分
        // public int m = -1;//44 可拆分的姓名
        // double[] factory = {"BC", "BCD", "BCDE"}

        public AsianPersonRecognition(Term[] terms)
        {
            _terms = terms;
        }

        public void Recognition()
        {
            var termList = GetNewTerms();
            foreach (var term2 in termList)
            {
                TermUtil.InsertTerm(_terms, term2);
            }
        }

        public List<Term> GetNewTerms()
        {
            var termList = new List<Term>();
            var beginFreq = 10;
            for (var i = 0; i < _terms.Length; i++)
            {
                var term = _terms[i];
                if (term == null || !term.TermNatures.PersonAttr.Flag)
                {
                    continue;
                }
                term.Score = 0;
                term.SelfScore = 0;
                for (var j = 2; j > -1; j--)
                {
                    var freq = term.TermNatures.PersonAttr.GetFreq(j, 0);
                    if ((freq > 10) || (term.Name.Length == 2 && freq > 10))
                    {
                        var tempTerm = NameFind(i, beginFreq, j);
                        if (tempTerm != null)
                        {
                            termList.Add(tempTerm);
                            // 如果是无争议性识别
                            if (_skip)
                            {
                                for (var j2 = i; j2 < tempTerm.ToValue(); j2++)
                                {
                                    if (_terms[j2] != null)
                                    {
                                        _terms[j2].Score = 0;
                                        _terms[j2].SelfScore = 0;
                                    }
                                }
                                i = tempTerm.ToValue() - 1;
                                break;
                            }
                        }
                    }
                }
                beginFreq = term.TermNatures.PersonAttr.Begin + 1;
            }
            return termList;
        }

        /// <summary>
        ///     人名识别
        /// </summary>
        /// <param name="offe"></param>
        /// <param name="beginFreq"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private Term NameFind(int offe, int beginFreq, int size)
        {
            var sb = new StringBuilder();
            var undefinite = 0;
            _skip = false;
            PersonNatureAttr pna;
            var index = 0;
            double allFreq = 0;
            Term term = null;
            var i = offe;
            for (; i < _terms.Length; i++)
            {
                // 走到结尾处识别出来一个名字.
                if (_terms[i] == null)
                {
                    continue;
                }
                term = _terms[i];
                pna = term.TermNatures.PersonAttr;
                // 在这个长度的这个位置的词频,如果没有可能就干掉,跳出循环
                int freq;
                if ((freq = pna.GetFreq(size, index)) == 0)
                {
                    return null;
                }

                if (pna.AllFreq > 0)
                {
                    undefinite++;
                }
                sb.Append(term.Name);
                allFreq += Math.Log(term.TermNatures.AllFreq + 1);
                allFreq += -Math.Log((freq));
                index++;

                if (index == size + 2)
                {
                    break;
                }
            }

            var score = -Math.Log(Factory[size]);
            score += allFreq;
            double endFreq = 0;
            // 开始寻找结尾词
            var flag = true;
            while (flag)
            {
                i++;
                if (i >= _terms.Length)
                {
                    endFreq = 10;
                    flag = false;
                }
                else if (_terms[i] != null)
                {
                    var twoWordFreq = NgramLibrary.GetTwoWordFreq(term, _terms[i]);
                    if (twoWordFreq > 3)
                    {
                        return null;
                    }
                    endFreq = _terms[i].TermNatures.PersonAttr.End + 1;
                    flag = false;
                }
            }

            score -= Math.Log(endFreq);
            score -= Math.Log(beginFreq);

            if (score > -3)
            {
                return null;
            }

            if (allFreq > 0 && undefinite > 0)
            {
                return null;
            }

            _skip = undefinite == 0;
            term = new Term(sb.ToString(), offe, TermNatures.Nr);
            term.SelfScore = score;

            return term;
        }

        public List<NewWord> GetNewWords()
        {
            var all = new List<NewWord>();
            var termList = GetNewTerms();
            foreach (var term2 in termList)
            {
                all.Add(new NewWord(term2.Name, Nature.NR));
            }
            return all;
        }
    }
}