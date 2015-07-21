using System.Collections.Generic;
using System.Diagnostics;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.SplitWord;

namespace Ansj.Net.Util
{
    /// <summary>
    ///     最短路径
    /// </summary>
    public class Graph
    {
        protected static readonly string E = "末##末";
        protected static readonly string B = "始##始";
        public char[] Chars;
        protected Term End;

        /// <summary>
        ///     是否有数字
        /// </summary>
        public bool HasNum;

        /// <summary>
        ///     是否有人名
        /// </summary>
        public bool HasPerson;

        public string RealStr;
        protected Term Root;
        public Term[] Terms;

        /// <summary>
        ///     是否需有歧异
        /// </summary>
        /// <param name="str"></param>
        public Graph(string str)
        {
            RealStr = str;
            Chars = str.ToCharArray();
            Terms = new Term[Chars.Length + 1];
            End = new Term(E, Chars.Length, AnsjItem.End);
            Root = new Term(B, -1, AnsjItem.Begin);
            Terms[Chars.Length] = End;
        }

        /// <summary>
        ///     构建最优路径
        /// </summary>
        /// <param name="merger"></param>
        /// <returns></returns>
        public List<Term> GetResult(AbstractAnalysis.Merger merger)
        {
            return merger.Execute();
        }

        /// <summary>
        ///     增加一个词语到图中
        /// </summary>
        /// <param name="term"></param>
        public void AddTerm(Term term)
        {
            // 是否有数字
            if (!HasNum && term.TermNatures.NumAttr.NumFreq > 0)
            {
                HasNum = true;
            }
            // 是否有人名
            if (!HasPerson && term.TermNatures.PersonAttr.Flag)
            {
                HasPerson = true;
            }
            // 将词放到图的位置
            if (Terms[term.Offe] == null)
            {
                Terms[term.Offe] = term;
            }
            else
            {
                Terms[term.Offe] = term.SetNext(Terms[term.Offe]);
            }
        }

        /// <summary>
        ///     取得最优路径的root Term
        /// </summary>
        /// <returns></returns>
        protected Term OptimalRoot()
        {
            var to = End;
            to.ClearScore();
            Term from;
            while ((from = to.From) != null)
            {
                for (var i = from.Offe + 1; i < to.Offe; i++)
                {
                    Terms[i] = null;
                }
                if (from.Offe > -1)
                {
                    Terms[from.Offe] = from;
                }
                // 断开横向链表.节省内存
                from.SetNext(null);
                @from.To = to;
                from.ClearScore();
                to = from;
            }
            return Root;
        }

        /// <summary>
        ///     删除最短的节点
        /// </summary>
        public void RemoveLittlePath()
        {
            // 是否有交叉
            var flag = false;
            var length = Terms.Length - 1;
            for (var i = 0; i < length; i++)
            {
                var maxTerm = GetMaxTerm(i);

                if (maxTerm == null)
                    continue;

                var maxTo = maxTerm.ToValue();

                /**
                 * 对字数进行优化.如果一个字.就跳过..两个字.且第二个为null则.也跳过.从第二个后开始
                 */
                switch (maxTerm.Name.Length)
                {
                    case 1:
                        continue;
                    case 2:
                        if (Terms[i + 1] == null)
                        {
                            i = i + 1;
                            continue;
                        }
                        break;
                }

                /**
                 * 判断是否有交叉
                 */
                for (var j = i + 1; j < maxTo; j++)
                {
                    var temp = GetMaxTerm(j);
                    if (temp == null)
                    {
                        continue;
                    }
                    if (maxTo < temp.ToValue())
                    {
                        maxTo = temp.ToValue();
                        flag = true;
                    }
                }

                if (flag)
                {
                    i = maxTo - 1;
                    flag = false;
                }
                else
                {
                    maxTerm.SetNext(null);
                    Terms[i] = maxTerm;
                    for (var j = i + 1; j < maxTo; j++)
                    {
                        Terms[j] = null;
                    }
                    // FIXME: 这里理论上得设置。但是跑了这么久，还不发生错误。应该是不依赖于双向链接。需要确认下。这段代码是否有用
                    // //将下面的to的from设置回来
                    // temp = terms[i+maxTerm.getName().Length] ;
                    // do{
                    // temp.setFrom(maxTerm) ;
                    // }while((temp=temp.getNext())!=null) ;
                }
            }
        }

        /// <summary>
        ///     得到最到本行最大term
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private Term GetMaxTerm(int i)
        {
            // TODO Auto-generated method stub
            var maxTerm = Terms[i];
            if (maxTerm == null)
            {
                return null;
            }
            var maxTo = maxTerm.ToValue();
            var term = maxTerm;
            while ((term = term.GetNext()) != null)
            {
                if (maxTo < term.ToValue())
                {
                    maxTo = term.ToValue();
                    maxTerm = term;
                }
            }
            return maxTerm;
        }

        /// <summary>
        ///     删除无意义的节点,防止viterbi太多
        /// </summary>
        public void RemoveLittleSinglePath()
        {
            for (var i = 0; i < Terms.Length; i++)
            {
                if (Terms[i] == null)
                    continue;
                var maxTo = Terms[i].ToValue();
                if (maxTo - i == 1 || i + 1 == Terms.Length)
                    continue;
                for (var j = i; j < maxTo; j++)
                {
                    var temp = Terms[j];
                    if (temp != null && temp.ToValue() <= maxTo && temp.Name.Length == 1)
                    {
                        Terms[j] = null;
                    }
                }
            }
        }

        /// <summary>
        ///     删除小节点。保证被删除的小节点的单个分数小于等于大节点的分数
        /// </summary>
        public void RemoveLittlePathByScore()
        {
            for (var i = 0; i < Terms.Length; i++)
            {
                if (Terms[i] == null)
                {
                    continue;
                }
                Term maxTerm = null;
                double maxScore = 0;
                var term = Terms[i];
                // 找到自身分数对大最长的

                do
                {
                    if (maxTerm == null || maxScore > term.Score)
                    {
                        maxTerm = term;
                    }
                    else if (maxScore == term.Score && maxTerm.Name.Length < term.Name.Length)
                    {
                        maxTerm = term;
                    }
                } while ((term = term.GetNext()) != null);
                term = maxTerm;
                do
                {
                    var maxTo = term.ToValue();
                    maxScore = term.Score;
                    if (maxTo - i == 1 || i + 1 == Terms.Length)
                        continue;
                    var flag = true; // 可以删除
                    outLable:
                    for (var j = i; j < maxTo; j++)
                    {
                        var temp = Terms[j];
                        if (temp == null)
                        {
                            continue;
                        }
                        do
                        {
                            if (temp.ToValue() > maxTo || temp.Score < maxScore)
                            {
                                flag = false;
                                goto outLable;
                            }
                        } while ((temp = temp.GetNext()) != null);
                    }
                    // 验证通过可以删除了
                    if (flag)
                    {
                        for (var j = i + 1; j < maxTo; j++)
                        {
                            Terms[j] = null;
                        }
                    }
                } while ((term = term.GetNext()) != null);
            }
        }

        public void WalkPathByScore()
        {
            // BEGIN先行打分
            MergerByScore(Root, 0);
            // 从第一个词开始往后打分
            for (var i = 0; i < Terms.Length; i++)
            {
                var term = Terms[i];
                while (term != null && term.From != null && term != End)
                {
                    var to = term.ToValue();
                    MergerByScore(term, to);
                    term = term.GetNext();
                }
            }
            OptimalRoot();
        }

        public void WalkPath()
        {
            // BEGIN先行打分
            Merger(Root, 0);
            // 从第一个词开始往后打分
            for (var i = 0; i < Terms.Length; i++)
            {
                var term = Terms[i];
                while (term != null && term.From != null && term != End)
                {
                    var to = term.ToValue();
                    Merger(term, to);
                    term = term.GetNext();
                }
            }
            OptimalRoot();
        }

        /// <summary>
        ///     具体的遍历打分方法
        /// </summary>
        /// <param name="fromTerm">起始位置</param>
        /// <param name="to">起始属性</param>
        private void Merger(Term fromTerm, int to)
        {
            if (Terms[to] != null)
            {
                var term = Terms[to];
                while (term != null)
                {
                    // 关系式to.set(from)
                    term.SetPathScore(fromTerm);
                    term = term.GetNext();
                }
            }
            else
            {
                var c = Chars[to];
                var tn = DatDictionary.GetItem(c).Natures;
                if (tn == null || tn == TermNatures.Null)
                {
                    tn = TermNatures.Null;
                }
                Terms[to] = new Term(c.ToString(), to, tn);
                Terms[to].SetPathScore(fromTerm);
            }
        }

        /// <summary>
        ///     根据分数
        ///     起始位置
        ///     起始属性
        /// </summary>
        /// <param name="fromTerm"></param>
        /// <param name="to"></param>
        private void MergerByScore(Term fromTerm, int to)
        {
            if (Terms[to] != null)
            {
                var term = Terms[to];
                while (term != null)
                {
                    // 关系式to.set(from)
                    term.SetPathSelfScore(fromTerm);
                    term = term.GetNext();
                }
            }
        }

        /// <summary>
        ///     对graph进行调试用的
        /// </summary>
        public void PrintGraph()
        {
            for (var i = 0; i < Terms.Length; i++)
            {
                var term = Terms[i];
                if (term == null)
                {
                    continue;
                }
                Trace.WriteLine(term.Name + "\t" + term.SelfScore + " ,");
                if ((term = term.GetNext()) != null)
                {
                    Trace.WriteLine(term + "\t" + term.SelfScore + " ,");
                }
                Trace.WriteLine("");
            }
        }
    }
}