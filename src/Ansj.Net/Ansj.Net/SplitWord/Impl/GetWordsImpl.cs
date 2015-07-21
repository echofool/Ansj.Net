using Ansj.Net.Domain;
using Ansj.Net.Library;

namespace Ansj.Net.SplitWord.Impl
{
    public class GetWordsImpl : IGetWords
    {
        private int _baseValue;
        private int _charHashCode;
        private int _charsLength;
        private int _checkValue;
        private int _start;
        private string _str;
        private int _tempBaseValue;
        public char[] Chars;
        public int End;
        public int I;

        /// <summary>
        ///     当前词的偏移量
        /// </summary>
        public int Offe;

        /// <summary>
        ///     同时加载词典,传入词语相当于同时调用了setStr()
        /// </summary>
        /// <param name="str"></param>
        public GetWordsImpl(string str)
        {
            SetStr(str);
        }

        /// <summary>
        ///     同时加载词典
        /// </summary>
        public GetWordsImpl()
        {
        }

        public void SetStr(string str)
        {
            SetChars(str.ToCharArray(), 0, str.Length);
        }

        public void SetChars(char[] chars, int start, int end)
        {
            Chars = chars;
            I = start;
            _start = start;
            _charsLength = end;
            _checkValue = 0;
        }

        public string AllWords()
        {
            for (; I < _charsLength; I++)
            {
                _charHashCode = Chars[I];
                End++;
                switch (GetStatement())
                {
                    case 0:
                        if (_baseValue == Chars[I])
                        {
                            _str = Chars[I].ToString();
                            Offe = I;
                            _start = ++I;
                            End = 0;
                            _baseValue = 0;
                            _tempBaseValue = _baseValue;
                            return _str;
                        }
                        I = _start;
                        _start++;
                        End = 0;
                        _baseValue = 0;
                        break;
                    case 2:
                        I++;
                        Offe = _start;
                        _tempBaseValue = _baseValue;
                        return DatDictionary.GetItem(_tempBaseValue).Name;
                    case 3:
                        Offe = _start;
                        _start++;
                        I = _start;
                        End = 0;
                        _tempBaseValue = _baseValue;
                        _baseValue = 0;
                        return DatDictionary.GetItem(_tempBaseValue).Name;
                }
            }
            if (_start++ != I)
            {
                I = _start;
                _baseValue = 0;
                return AllWords();
            }
            End = 0;
            _baseValue = 0;
            I = 0;
            return null;
        }

        public int GetOffe()
        {
            return Offe;
        }

        /// <summary>
        ///     根据用户传入的c得到单词的状态. 0.代表这个字不在词典中 1.继续 2.是个词但是还可以继续 3.停止已经是个词了
        /// </summary>
        /// <returns></returns>
        private int GetStatement()
        {
            _checkValue = _baseValue;
            _baseValue = DatDictionary.GetItem(_checkValue).Base + _charHashCode;
            if (_baseValue < DatDictionary.ArrayLength &&
                (DatDictionary.GetItem(_baseValue).Check == _checkValue || DatDictionary.GetItem(_baseValue).Check == -1))
            {
                return DatDictionary.GetItem(_baseValue).Status;
            }
            return 0;
        }

        public AnsjItem GetItem()
        {
            return DatDictionary.GetItem(_tempBaseValue);
        }
    }
}