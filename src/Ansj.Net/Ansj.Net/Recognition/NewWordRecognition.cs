using System.Diagnostics;
using System.Text;
using Ansj.Net.Dic;
using Ansj.Net.Domain;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.Recognition
{
    /// <summary>
    ///     新词识别
    /// </summary>
    public class NewWordRecognition
    {
        private readonly SmartForest<NewWord> _forest;
        private readonly Term[] _terms;
        private SmartForest<NewWord> _branch;
        private Term _from;

        /// <summary>
        ///     偏移量
        /// </summary>
        private int _offe;

        private double _score;
        private StringBuilder _stringBuilder = new StringBuilder();
        private Nature _tempNature;
        private Term _to;

        public NewWordRecognition(Term[] terms, LearnTool learn)
        {
            _terms = terms;
            _forest = learn.GetForest();
            _branch = learn.GetForest();
        }

        public void Recognition()
        {
            if (_branch == null)
            {
                return;
            }
            var length = _terms.Length - 1;

            for (var i = 0; i < length; i++)
            {
                if (_terms[i] == null)
                {
                    continue;
                }
                _from = _terms[i].From;
                _terms[i].Score = 0;
                _terms[i].SelfScore = 0;

                if (_branch != null)
                {
                    _branch = _branch.GetBranch(_terms[i].Name);

                    if (_branch == null || _branch.Status == 3)
                    {
                        Reset();
                        continue;
                    }

                    _offe = i;

                    // 循环查找添加
                    var term = _terms[i];
                    _stringBuilder.Append(term.Name);
                    if (_branch.Status == 2)
                    {
                        term.SelfScore = _branch.Param.Score;
                    }
                    var flag = true;
                    while (flag)
                    {
                        term = term.To;
                        _branch = _branch.GetBranch(term.Name);
                        // 如果没有找到跳出
                        if (_branch == null)
                        {
                            break;
                        }

                        switch (_branch.Status)
                        {
                            case 1:
                                _stringBuilder.Append(term.Name);
                                continue;
                            case 2:
                                _stringBuilder.Append(term.Name);
                                _score = _branch.Param.Score;
                                _tempNature = _branch.Param.Nature;
                                _to = term.To;
                                MakeNewTerm();
                                continue;
                            case 3:
                                _stringBuilder.Append(term.Name);
                                _score = _branch.Param.Score;
                                _tempNature = _branch.Param.Nature;
                                _to = term.To;
                                MakeNewTerm();
                                flag = false;
                                break;
                            default:
                                Trace.WriteLine("怎么能出现0呢?");
                                break;
                        }
                    }
                }
                Reset();
            }
        }

        private void MakeNewTerm()
        {
            var term = new Term(_stringBuilder.ToString(), _offe, _tempNature.natureStr, 1)
            {
                SelfScore = _score,
                Nature = _tempNature
            };
            if (_stringBuilder.Length > 3)
            {
                term.SubTerm = TermUtil.GetSubTerm(_from, _to);
            }
            TermUtil.TermLink(_from, term);
            TermUtil.TermLink(term, _to);
            TermUtil.InsertTerm(_terms, term);
            TermUtil.ParseNature(term);
        }

        /// <summary>
        ///     重置
        /// </summary>
        private void Reset()
        {
            _offe = -1;
            _tempNature = null;
            _branch = _forest;
            _score = 0;
            _stringBuilder = new StringBuilder();
        }
    }
}