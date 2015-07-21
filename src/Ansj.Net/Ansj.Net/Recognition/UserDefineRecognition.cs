using System;
using System.Diagnostics;
using System.Text;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.Util;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.Recognition
{
    /// <summary>
    ///     用户自定义词典.又称补充词典
    /// </summary>
    public class UserDefineRecognition
    {
        private readonly IWoodInterface[] _forests = {UserDefineLibrary.Forest};
        private readonly Term[] _terms;
        private IWoodInterface _branch;
        private int _endOffe = -1;
        private IWoodInterface _forest;
        private int _offe = -1;
        private int _tempFreq = 50;
        private string _tempNature;

        public UserDefineRecognition(Term[] terms, params IWoodInterface[] forests)
        {
            _terms = terms;
            if (forests != null && forests.Length > 0)
            {
                _forests = forests;
            }
        }

        public void Recognition()
        {
            foreach (var forest in _forests)
            {
                if (forest == null)
                {
                    continue;
                }
                Reset();
                _forest = forest;

                _branch = forest;

                var length = _terms.Length - 1;

                for (var i = 0; i < length; i++)
                {
                    if (_terms[i] == null)
                        continue;
                    bool flag;
                    if (Equals(_branch, forest))
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }

                    _branch = termStatus(_branch, _terms[i]);
                    if (_branch == null)
                    {
                        if (_offe != -1)
                        {
                            i = _offe;
                        }
                        Reset();
                    }
                    else if (_branch.Status == 3)
                    {
                        _endOffe = i;
                        _tempNature = _branch.Param[0];
                        _tempFreq = getInt(_branch.Param[1], 50);
                        if (_offe != -1 && _offe < _endOffe)
                        {
                            i = _offe;
                            MakeNewTerm();
                            Reset();
                        }
                        else
                        {
                            Reset();
                        }
                    }
                    else if (_branch.Status == 2)
                    {
                        _endOffe = i;
                        if (_offe == -1)
                        {
                            _offe = i;
                        }
                        else
                        {
                            _tempNature = _branch.Param[0];
                            _tempFreq = getInt(_branch.Param[1], 50);
                            if (flag)
                            {
                                MakeNewTerm();
                            }
                        }
                    }
                    else if (_branch.Status == 1)
                    {
                        if (_offe == -1)
                        {
                            _offe = i;
                        }
                    }
                }
                if (_offe != -1 && _offe < _endOffe)
                {
                    MakeNewTerm();
                }
            }
        }

        private int getInt(string str, int def)
        {
            try
            {
                return int.Parse(str);
            }
            catch (FormatException e)
            {
                Trace.WriteLine(e);
                return def;
            }
        }

        private void MakeNewTerm()
        {
            var sb = new StringBuilder();
            for (var j = _offe; j <= _endOffe; j++)
            {
                if (_terms[j] != null)
                {
                    sb.Append(_terms[j].Name);
                }
            }
            var termNatures = new TermNatures(new TermNature(_tempNature, _tempFreq));
            var term = new Term(sb.ToString(), _offe, termNatures) {SelfScore = -1*_tempFreq};
            TermUtil.InsertTerm(_terms, term);
        }

        /// <summary>
        ///     重置
        /// </summary>
        private void Reset()
        {
            _offe = -1;
            _endOffe = -1;
            _tempFreq = 50;
            _tempNature = null;
            _branch = _forest;
        }

        /// <summary>
        ///     传入一个term 返回这个term的状态
        /// </summary>
        /// <param name="branch"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        private IWoodInterface termStatus(IWoodInterface branch, Term term)
        {
            var name = term.Name;
            for (var j = 0; j < name.Length; j++)
            {
                branch = branch.Get(name.charAt(j));
                if (branch == null)
                {
                    return null;
                }
            }
            return branch;
        }
    }
}