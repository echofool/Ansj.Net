using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Nlpcn.Net.Commons.Lang.Tire
{
    public class SmartGetWord<T>
    {
        private const string Emptystring = "";
        private readonly SmartForest<T> _forest;
        private SmartForest<T> _branch;
        private char[] _chars;
        private int _i;
        private bool _isBack;
        public int Offe;
        /// <summary>
        /// 参数
        /// </summary>
        public T Param { get; private set; }
        private int _root;
        private byte _status;
        private string _str;
        private int _tempOffe;

        public SmartGetWord(SmartForest<T> forest, string content)
        {
            _chars = content.ToCharArray();
            _forest = forest;
            _branch = forest;
        }

        public SmartGetWord(SmartForest<T> forest, char[] chars)
        {
            _chars = chars;
            _forest = forest;
            _branch = forest;
        }

        public string GetAllWords()
        {
            var temp = AllWords();
            while (Emptystring.Equals(temp))
            {
                temp = AllWords();
            }
            return temp;
        }

        public string GetFrontWords()
        {
            var temp = FrontWords();
            while (Emptystring.Equals(temp))
            {
                temp = FrontWords();
            }
            return temp;
        }

        private string AllWords()
        {
            if ((!_isBack) || (_i == _chars.Length - 1))
            {
                _i = (_root - 1);
            }
            for (_i += 1; _i < _chars.Length; _i = (_i + 1))
            {
                _branch = _branch.GetBranch(_chars[_i]);
                if (_branch == null)
                {
                    _root += 1;
                    _branch = _forest;
                    _i = (_root - 1);
                    _isBack = false;
                }
                else
                {
                    switch (_branch.Status)
                    {
                        case 2:
                            _isBack = true;
                            Offe = (_tempOffe + _root);
                            Param = _branch.Param;
                            return new string(_chars, _root, _i - _root + 1);
                        case 3:
                            Offe = (_tempOffe + _root);
                            _str = new string(_chars, _root, _i - _root + 1);
                            Param = _branch.Param;
                            _branch = _forest;
                            _isBack = false;
                            _root += 1;
                            return _str;
                    }
                }
            }
            _tempOffe += _chars.Length;
            return null;
        }

        private string FrontWords()
        {
            for (; _i < _chars.Length + 1; _i++)
            {
                if (_i == _chars.Length)
                {
                    _branch = null;
                }
                else
                {
                    _branch = _branch.GetBranch(_chars[_i]);
                }
                if (_branch == null)
                {
                    _branch = _forest;
                    if (_isBack)
                    {
                        Offe = _root;
                        _str = new string(_chars, _root, _tempOffe);
                        if ((_root > 0) && (IsE(_chars[(_root - 1)])) && (IsE(_str[0])))
                        {
                            _str = Emptystring;
                        }

                        if ((_str.Length != 0) && (_root + _tempOffe < _chars.Length) && (IsE(_str[_str.Length - 1]))
                            && (IsE(_chars[(_root + _tempOffe)])))
                        {
                            _str = Emptystring;
                        }
                        if (_str.Length == 0)
                        {
                            _root += 1;
                            _i = _root;
                        }
                        else
                        {
                            _i = (_root + _tempOffe);
                            _root = _i;
                        }
                        _isBack = false;

                        if (Emptystring.Equals(_str))
                        {
                            return Emptystring;
                        }
                        return _str;
                    }
                    _i = _root;
                    _root += 1;
                }
                else
                {
                    switch (_branch.Status)
                    {
                        case 2:
                            _isBack = true;
                            _tempOffe = (_i - _root + 1);
                            Param = _branch.Param;
                            break;
                        case 3:
                            Offe = _root;
                            _str = new string(_chars, _root, _i - _root + 1);
                            var temp = _str;

                            if ((_root > 0) && (IsE(_chars[(_root - 1)])) && (IsE(_str[0])))
                            {
                                _str = Emptystring;
                            }

                            if ((_str.Length != 0) && (_i + 1 < _chars.Length) && (IsE(_str[_str.Length - 1]))
                                && (IsE(_chars[(_i + 1)])))
                            {
                                _str = Emptystring;
                            }
                            Param = _branch.Param;
                            _branch = _forest;
                            _isBack = false;
                            if (temp.Length > 0)
                            {
                                _i += 1;
                                _root = _i;
                            }
                            else
                            {
                                _i = (_root + 1);
                            }
                            if (Emptystring.Equals(_str))
                            {
                                return Emptystring;
                            }
                            return _str;
                    }
                }
            }
            _tempOffe += _chars.Length;
            return null;
        }

        public bool IsE(char c)
        {
            if (c == '.' || ((c >= 'a') && (c <= 'z')))
            {
                return true;
            }
            return false;
        }

        public void Reset(string content)
        {
            Offe = 0;
            _status = 0;
            _root = 0;
            _i = _root;
            _isBack = false;
            _tempOffe = 0;
            _chars = content.ToCharArray();
            _branch = _forest;
        }
    }
}