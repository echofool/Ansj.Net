using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Lucene.Net.Util;

namespace Ansj.Net.Lucene.Util
{
    /// <summary>
    ///     抄袭lucene的英文处理
    ///     Stemmer, implementing the Porter Stemming Algorithm
    ///     The Stemmer class transforms a word into its root form. The input word can be
    ///     provided a character at time (by calling add()), or at once by calling one of
    ///     the various stem(something) methods.
    /// </summary>
    public class PorterStemmer
    {
        private const int InitialSize = 50;

        /// <summary>
        ///     lucene.net没有这个RamUsageEstimator.NUM_BYTES_CHAR
        /// </summary>
        //TODO echofool lucene.net没有这个RamUsageEstimator.NUM_BYTES_CHAR
        private static readonly int NumBytesChar;

        private char[] _b;
        private bool _dirty;

        private int _i,
            /* offset into b */
            _j,
            _k,
            _k0;

        public PorterStemmer()
        {
            _b = new char[InitialSize];
            _i = 0;
        }

        /// <summary>
        ///     reset() resets the stemmer so it can stem another word. If you invoke the
        ///     stemmer by calling add(char) and then stem(), you must call reset()
        ///     before starting another word.
        /// </summary>
        public void Reset()
        {
            _i = 0;
            _dirty = false;
        }

        /// <summary>
        ///     Add a character to the word being stemmed. When you are finished adding
        ///     characters, you can call stem(void) to process the word.
        /// </summary>
        /// <param name="ch"></param>
        public void Add(char ch)
        {
            if (_b.Length <= _i)
            {
                //TODO:echofool 这个地方不知道到底要转成什么类型才合适
                _b = ArrayUtil.Grow(_b.Select(t => (byte) t).ToArray(), _i + 1).Select(t => (char) t).ToArray();
            }
            _b[_i++] = ch;
        }

        /// <summary>
        ///     After a word has been stemmed, it can be retrieved by toString(), or a
        ///     reference to the internal buffer can be retrieved by getResultBuffer and
        ///     getResultLength (which is generally more efficient.)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return new string(_b, 0, _i);
        }

        /// <summary>
        ///     Returns the length of the word resulting from the stemming process.
        /// </summary>
        /// <returns></returns>
        public int GetResultLength()
        {
            return _i;
        }

        /// <summary>
        ///     Returns a reference to a character buffer containing the results of the
        ///     stemming process. You also need to consult getResultLength() to determine
        ///     the length of the result.
        /// </summary>
        /// <returns></returns>
        public char[] GetResultBuffer()
        {
            return _b;
        }

        /// <summary>
        ///     cons(i) is true &lt;=&gt; b[i] is a consonant.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool Cons(int i)
        {
            switch (_b[i])
            {
                case 'a':
                case 'e':
                case 'i':
                case 'o':
                case 'u':
                    return false;
                case 'y':
                    return (i == _k0) || !Cons(i - 1);
                default:
                    return true;
            }
        }

        /// <summary>
        ///     m() measures the number of consonant sequences between k0 and j. if c is
        ///     a consonant sequence and v a vowel sequence, and &lt;..&gt; indicates arbitrary
        ///     presence,
        ///     &lt;c&gt;&lt;v&gt; gives 0 &lt;c&gt;vc&lt;v&gt;  gives 1 &lt;c&gt;vcvc&lt;v&gt;  gives 2 &lt;c&gt;vcvcvc&lt;v&gt;
        ///     gives 3
        ///     ....
        /// </summary>
        /// <returns></returns>
        private int M()
        {
            var n = 0;
            var i = _k0;
            while (true)
            {
                if (i > _j)
                    return n;
                if (!Cons(i))
                    break;
                i++;
            }
            i++;
            while (true)
            {
                while (true)
                {
                    if (i > _j)
                        return n;
                    if (Cons(i))
                        break;
                    i++;
                }
                i++;
                n++;
                while (true)
                {
                    if (i > _j)
                        return n;
                    if (!Cons(i))
                        break;
                    i++;
                }
                i++;
            }
        }

        /// <summary>
        ///     vowelinstem() is true &lt;=&gt; k0,...j contains a vowel
        /// </summary>
        /// <returns></returns>
        private bool Vowelinstem()
        {
            int i;
            for (i = _k0; i <= _j; i++)
                if (!Cons(i))
                    return true;
            return false;
        }

        /// <summary>
        ///     doublec(j) is true &lt;=&gt; j,(j-1) contain a double consonant.
        /// </summary>
        /// <param name="j"></param>
        /// <returns></returns>
        private bool Doublec(int j)
        {
            if (j < _k0 + 1)
                return false;
            if (_b[j] != _b[j - 1])
                return false;
            return Cons(j);
        }

        /// <summary>
        ///     cvc(i) is true &lt;=&gt; i-2,i-1,i has the form consonant - vowel - consonant
        ///     and also if the second c is not w,x or y. this is used when trying to
        ///     restore an e at the end of a short word. e.g.
        ///     cav(e), lov(e), hop(e), crim(e), but snow, box, tray.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private bool Cvc(int i)
        {
            if (i < _k0 + 2 || !Cons(i) || Cons(i - 1) || !Cons(i - 2))
                return false;
            int ch = _b[i];
            if (ch == 'w' || ch == 'x' || ch == 'y')
                return false;
            return true;
        }

        private bool Ends(string s)
        {
            var l = s.Length;
            var o = _k - l + 1;
            if (o < _k0)
                return false;
            for (var i = 0; i < l; i++)
                if (_b[o + i] != s.charAt(i))
                    return false;
            _j = _k - l;
            return true;
        }

        /// <summary>
        ///     setto(s) sets (j+1),...k to the characters in the string s, readjusting k.
        /// </summary>
        /// <param name="s"></param>
        private void Setto(string s)
        {
            var l = s.Length;
            var o = _j + 1;
            for (var i = 0; i < l; i++)
                _b[o + i] = s.charAt(i);
            _k = _j + l;
            _dirty = true;
        }

        /// <summary>
        ///     r(s) is used further down.
        /// </summary>
        /// <param name="s"></param>
        private void R(string s)
        {
            if (M() > 0)
                Setto(s);
        }

        /// <summary>
        ///     step1() gets rid of plurals and -ed or -ing. e.g.
        ///     caresses -> caress ponies -> poni ties -> ti caress -> caress cats -> cat
        ///     feed -> feed agreed -> agree disabled -> disable
        ///     matting -> mat mating -> mate meeting -> meet milling -> mill messing ->
        ///     mess
        ///     meetings -> meet
        /// </summary>
        private void Step1()
        {
            if (_b[_k] == 's')
            {
                if (Ends("sses"))
                    _k -= 2;
                else if (Ends("ies"))
                    Setto("i");
                else if (_b[_k - 1] != 's')
                    _k--;
            }
            if (Ends("eed"))
            {
                if (M() > 0)
                    _k--;
            }
            else if ((Ends("ed") || Ends("ing")) && Vowelinstem())
            {
                _k = _j;
                if (Ends("at"))
                    Setto("ate");
                else if (Ends("bl"))
                    Setto("ble");
                else if (Ends("iz"))
                    Setto("ize");
                else if (Doublec(_k))
                {
                    int ch = _b[_k--];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        _k++;
                }
                else if (M() == 1 && Cvc(_k))
                    Setto("e");
            }
        }

        /// <summary>
        ///     step2() turns terminal y to i when there is another vowel in the stem.
        /// </summary>
        private void Step2()
        {
            if (Ends("y") && Vowelinstem())
            {
                _b[_k] = 'i';
                _dirty = true;
            }
        }

        /// <summary>
        ///     step3() maps double suffices to single ones. so -ization ( = -ize plus
        ///     -ation) maps to -ize etc. note that the string before the suffix must
        ///     give m() > 0.
        /// </summary>
        private void Step3()
        {
            if (_k == _k0)
                return; /* For Bug 1 */
            switch (_b[_k - 1])
            {
                case 'a':
                    if (Ends("ational"))
                    {
                        R("ate");
                        break;
                    }
                    if (Ends("tional"))
                    {
                        R("tion");
                    }
                    break;
                case 'c':
                    if (Ends("enci"))
                    {
                        R("ence");
                        break;
                    }
                    if (Ends("anci"))
                    {
                        R("ance");
                    }
                    break;
                case 'e':
                    if (Ends("izer"))
                    {
                        R("ize");
                    }
                    break;
                case 'l':
                    if (Ends("bli"))
                    {
                        R("ble");
                        break;
                    }
                    if (Ends("alli"))
                    {
                        R("al");
                        break;
                    }
                    if (Ends("entli"))
                    {
                        R("ent");
                        break;
                    }
                    if (Ends("eli"))
                    {
                        R("e");
                        break;
                    }
                    if (Ends("ousli"))
                    {
                        R("ous");
                    }
                    break;
                case 'o':
                    if (Ends("ization"))
                    {
                        R("ize");
                        break;
                    }
                    if (Ends("ation"))
                    {
                        R("ate");
                        break;
                    }
                    if (Ends("ator"))
                    {
                        R("ate");
                    }
                    break;
                case 's':
                    if (Ends("alism"))
                    {
                        R("al");
                        break;
                    }
                    if (Ends("iveness"))
                    {
                        R("ive");
                        break;
                    }
                    if (Ends("fulness"))
                    {
                        R("ful");
                        break;
                    }
                    if (Ends("ousness"))
                    {
                        R("ous");
                    }
                    break;
                case 't':
                    if (Ends("aliti"))
                    {
                        R("al");
                        break;
                    }
                    if (Ends("iviti"))
                    {
                        R("ive");
                        break;
                    }
                    if (Ends("biliti"))
                    {
                        R("ble");
                    }
                    break;
                case 'g':
                    if (Ends("logi"))
                    {
                        R("log");
                    }
                    break;
            }
        }

        /// <summary>
        ///     step4() deals with -ic-, -full, -ness etc. similar strategy to step3
        /// </summary>
        private void Step4()
        {
            switch (_b[_k])
            {
                case 'e':
                    if (Ends("icate"))
                    {
                        R("ic");
                        break;
                    }
                    if (Ends("ative"))
                    {
                        R("");
                        break;
                    }
                    if (Ends("alize"))
                    {
                        R("al");
                    }
                    break;
                case 'i':
                    if (Ends("iciti"))
                    {
                        R("ic");
                    }
                    break;
                case 'l':
                    if (Ends("ical"))
                    {
                        R("ic");
                        break;
                    }
                    if (Ends("ful"))
                    {
                        R("");
                    }
                    break;
                case 's':
                    if (Ends("ness"))
                    {
                        R("");
                    }
                    break;
            }
        }

        /// <summary>
        ///     step5() takes off -ant, -ence etc., in context &lt;c&gt;vcvc&lt;v&gt;.
        /// </summary>
        private void Step5()
        {
            if (_k == _k0)
                return; /* for Bug 1 */
            switch (_b[_k - 1])
            {
                case 'a':
                    if (Ends("al"))
                        break;
                    return;
                case 'c':
                    if (Ends("ance"))
                        break;
                    if (Ends("ence"))
                        break;
                    return;
                case 'e':
                    if (Ends("er"))
                        break;
                    return;
                case 'i':
                    if (Ends("ic"))
                        break;
                    return;
                case 'l':
                    if (Ends("able"))
                        break;
                    if (Ends("ible"))
                        break;
                    return;
                case 'n':
                    if (Ends("ant"))
                        break;
                    if (Ends("ement"))
                        break;
                    if (Ends("ment"))
                        break;
                    /* element etc. not stripped before the m */
                    if (Ends("ent"))
                        break;
                    return;
                case 'o':
                    if (Ends("ion") && _j >= 0 && (_b[_j] == 's' || _b[_j] == 't'))
                        break;
                    /* j >= 0 fixes Bug 2 */
                    if (Ends("ou"))
                        break;
                    return;
                /* takes care of -ous */
                case 's':
                    if (Ends("ism"))
                        break;
                    return;
                case 't':
                    if (Ends("ate"))
                        break;
                    if (Ends("iti"))
                        break;
                    return;
                case 'u':
                    if (Ends("ous"))
                        break;
                    return;
                case 'v':
                    if (Ends("ive"))
                        break;
                    return;
                case 'z':
                    if (Ends("ize"))
                        break;
                    return;
                default:
                    return;
            }
            if (M() > 1)
                _k = _j;
        }

        /// <summary>
        ///     step6() removes a final -e if m() > 1.
        /// </summary>
        private void Step6()
        {
            _j = _k;
            if (_b[_k] == 'e')
            {
                var a = M();
                if (a > 1 || a == 1 && !Cvc(_k - 1))
                    _k--;
            }
            if (_b[_k] == 'l' && Doublec(_k) && M() > 1)
                _k--;
        }

        /// <summary>
        ///     Stem a word provided as a String. Returns the result as a String.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string Stem(string s)
        {
            if (Stem(s.ToCharArray(), s.Length))
                return ToString();
            return s;
        }

        /// <summary>
        ///     Stem a word contained in a char[]. Returns true if the stemming process
        ///     resulted in a word different from the input. You can retrieve the result
        ///     with getResultLength()/getResultBuffer() or toString().
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool Stem(char[] word)
        {
            return Stem(word, word.Length);
        }

        /// <summary>
        ///     Stem a word contained in a portion of a char[] array. Returns true if the
        ///     stemming process resulted in a word different from the input. You can
        ///     retrieve the result with getResultLength()/getResultBuffer() or
        ///     toString().
        /// </summary>
        /// <param name="wordBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="wordLen"></param>
        /// <returns></returns>
        public bool Stem(char[] wordBuffer, int offset, int wordLen)
        {
            Reset();
            if (_b.Length < wordLen)
            {
                _b = new char[ArrayUtil.GetShrinkSize(wordLen, NumBytesChar)];
            }
            Array.Copy(wordBuffer, offset, _b, 0, wordLen);
            _i = wordLen;
            return Stem(0);
        }

        /// <summary>
        ///     Stem a word contained in a leading portion of a char[] array. Returns
        ///     true if the stemming process resulted in a word different from the input.
        ///     You can retrieve the result with getResultLength()/getResultBuffer() or
        ///     toString().
        /// </summary>
        /// <param name="word"></param>
        /// <param name="wordLen"></param>
        /// <returns></returns>
        public bool Stem(char[] word, int wordLen)
        {
            return Stem(word, 0, wordLen);
        }

        /// <summary>
        ///     Stem the word placed into the Stemmer buffer through calls to add().
        ///     Returns true if the stemming process resulted in a word different from
        ///     the input. You can retrieve the result with
        ///     getResultLength()/getResultBuffer() or toString().
        /// </summary>
        /// <returns></returns>
        public bool Stem()
        {
            return Stem(0);
        }

        public bool Stem(int i0)
        {
            _k = _i - 1;
            _k0 = i0;
            if (_k > _k0 + 1)
            {
                Step1();
                Step2();
                Step3();
                Step4();
                Step5();
                Step6();
            }
            // Also, a word is considered dirty if we lopped off letters
            // Thanks to Ifigenia Vairelles for pointing this out.
            if (_i != _k + 1)
                _dirty = true;
            _i = _k + 1;
            return _dirty;
        }

        /// <summary>
        ///     Test program for demonstrating the Stemmer. It reads a file and stems
        ///     each word, writing the result to standard out. Usage: Stemmer file-name
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var s = new PorterStemmer();

            for (var i = 0; i < args.Length; i++)
            {
                try
                {
                    var stream = File.Open(args[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                    var buffer = new byte[1024];
                    var offset = 0;

                    var bufferLen = stream.Read(buffer, offset, buffer.Length);
                    offset = 0;
                    s.Reset();

                    while (true)
                    {
                        int ch;
                        if (offset < bufferLen)
                            ch = buffer[offset++];
                        else
                        {
                            bufferLen = stream.Read(buffer, offset, buffer.Length);
                            offset = 0;
                            if (bufferLen < 0)
                                ch = -1;
                            else
                                ch = buffer[offset++];
                        }

                        if (char.IsLetter((char) ch))
                        {
                            s.Add(char.ToLower((char) ch));
                        }
                        else
                        {
                            s.Stem();
                            Trace.WriteLine(s.ToString());
                            s.Reset();
                            if (ch < 0)
                                break;
                            Trace.WriteLine((char) ch);
                        }
                    }

                    stream.Close();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e.ToString(), "error reading " + args[i]);
                }
            }
        }
    }
}