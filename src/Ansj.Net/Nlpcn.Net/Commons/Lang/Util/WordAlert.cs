using System;
using System.Text;

namespace Nlpcn.Net.Commons.Lang.Util
{
    public class WordAlert
    {
        /// <summary>
        ///这个就是(int)'ａ'
        /// </summary>
        public static readonly int MinLower = 65345;
        /// <summary>
        ///这个就是(int)'ｚ'
        /// </summary>
        public static readonly int MaxLower = 65370;
        /// <summary>
        ///差距进行转译需要的
        /// </summary>
        public static readonly int LowerGap = 65248;
        /// <summary>
        ///这个就是(int)'Ａ'
        /// </summary>
        public static readonly int MinUpper = 65313;
        /// <summary>
        ///这个就是(int)'Ｚ'
        /// </summary>
        public static readonly int MaxUpper = 65338;
        /// <summary>
        ///差距进行转译需要的
        /// </summary>
        public static readonly int UpperGap = 65216;
        /// <summary>
        ///这个就是(int)'A'
        /// </summary>
        public static readonly int MinUpperE = 65;
        /// <summary>
        ///这个就是(int)'Z'
        /// </summary>
        public static readonly int MaxUpperE = 90;
        /// <summary>
        ///差距进行转译需要的
        /// </summary>
        public static readonly int UpperGapE = -32;
        /// <summary>
        ///这个就是(int)'０'
        /// </summary>
        public static readonly int MinUpperN = 65296;
        /// <summary>
        ///这个就是(int)'９'
        /// </summary>
        public static readonly int MaxUpperN = 65305;
        /// <summary>
        ///差距进行转译需要的
        /// </summary>
        public static readonly int UpperGapN = 65248;
        private static readonly char[] Charcover = new char[65536];

        static WordAlert()
        {
            for (var i = 0; i < Charcover.Length; i++)
            {
                if (i >= MinLower && i <= MaxLower)
                {
                    Charcover[i] = (char)(i - LowerGap);
                }
                else if (i >= MinUpper && i <= MaxUpper)
                {
                    Charcover[i] = (char)(i - UpperGap);
                }
                else if (i >= MinUpperE && i <= MaxUpperE)
                {
                    Charcover[i] = (char)(i - UpperGapE);
                }
                else if (i >= MinUpperN && i <= MaxUpperN)
                {
                    Charcover[i] = (char)(i - UpperGapN);
                }
                else if (i >= '0' && i <= '9')
                {
                    Charcover[i] = (char)i;
                }
                else if (i >= 'a' && i <= 'z')
                {
                    Charcover[i] = (char)i;
                }
            }
            Charcover['-'] = '·';
            Charcover['．'] = '.';
            Charcover['•'] = '·';
            Charcover[','] = '。';
            Charcover['，'] = '。';
            Charcover['！'] = '。';
            Charcover['!'] = '。';
            Charcover['？'] = '。';
            Charcover['?'] = '。';
            Charcover['；'] = '。';
            Charcover['`'] = '。';
            Charcover['﹑'] = '。';
            Charcover['^'] = '。';
            Charcover['…'] = '。';
            Charcover['“'] = '"';
            Charcover['”'] = '"';
            Charcover['〝'] = '"';
            Charcover['〞'] = '"';
            Charcover['~'] = '"';
            Charcover['\\'] = '。';
            Charcover['∕'] = '。';
            Charcover['|'] = '。';
            Charcover['¦'] = '。';
            Charcover['‖'] = '。';
            Charcover['—'] = '。';
            Charcover['('] = '《';
            Charcover[')'] = '》';
            Charcover['〈'] = '《';
            Charcover['〉'] = '》';
            Charcover['﹞'] = '》';
            Charcover['﹝'] = '《';
            Charcover['「'] = '《';
            Charcover['」'] = '》';
            Charcover['‹'] = '《';
            Charcover['›'] = '》';
            Charcover['〖'] = '《';
            Charcover['〗'] = '"';
            Charcover['】'] = '》';
            Charcover['【'] = '《';
            Charcover['»'] = '》';
            Charcover['«'] = '《';
            Charcover['』'] = '》';
            Charcover['『'] = '《';
            Charcover['〕'] = '》';
            Charcover['〔'] = '《';
            Charcover['}'] = '》';
            Charcover['{'] = '《';
            Charcover[']'] = '》';
            Charcover['['] = '《';
            Charcover['﹐'] = '。';
            Charcover['¸'] = '。';
            Charcover['︰'] = '﹕';
            Charcover['﹔'] = '。';
            Charcover[';'] = '。';
            Charcover['！'] = '。';
            Charcover['¡'] = '。';
            Charcover['？'] = '。';
            Charcover['¿'] = '。';
            Charcover['﹖'] = '。';
            Charcover['﹌'] = '。';
            Charcover['﹏'] = '。';
            Charcover['﹋'] = '。';
            Charcover['＇'] = '。';
            Charcover['´'] = '。';
            Charcover['ˊ'] = '。';
            Charcover['ˋ'] = '。';
            Charcover['―'] = '。';
            Charcover['﹫'] = '@';
            Charcover['︳'] = '。';
            Charcover['︴'] = '。';
            Charcover['﹢'] = '+';
            Charcover['﹦'] = '=';
            Charcover['﹤'] = '《';
            Charcover['<'] = '《';
            Charcover['˜'] = '。';
            Charcover['~'] = '。';
            Charcover['﹟'] = '。';
            Charcover['#'] = '。';
            Charcover['﹩'] = '$';
            Charcover['﹠'] = '。';
            Charcover['&'] = '。';
            Charcover['﹪'] = '%';
            Charcover['﹡'] = '。';
            Charcover['*'] = '。';
            Charcover['﹨'] = '。';
            Charcover['\\'] = '。';
            Charcover['﹍'] = '。';
            Charcover['﹉'] = '。';
            Charcover['﹎'] = '。';
            Charcover['﹊'] = '。';
            Charcover['ˇ'] = '。';
            Charcover['︵'] = '《';
            Charcover['︶'] = '》';
            Charcover['︷'] = '《';
            Charcover['︸'] = '》';
            Charcover['︹'] = '《';
            Charcover['︿'] = '《';
            Charcover['﹀'] = '》';
            Charcover['︺'] = '》';
            Charcover['︽'] = '《';
            Charcover['︾'] = '》';
            Charcover['_'] = '。';
            Charcover['ˉ'] = '。';
            Charcover['﹁'] = '《';
            Charcover['﹂'] = '》';
            Charcover['﹃'] = '《';
            Charcover['﹄'] = '》';
            Charcover['︻'] = '《';
            Charcover['︼'] = '》';
            Charcover['/'] = '。';
            Charcover['（'] = '《';
            Charcover['>'] = '》';
            Charcover['）'] = '》';
            Charcover['<'] = '《';
            Charcover[' '] = ' ';
            Charcover['\t'] = '\t';
            Charcover['。'] = '。';
            Charcover['@'] = '@';
        }

        /// <summary>
        /// 对全角的字符串,大写字母进行转译.如ｓｄｆｓｄｆ
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>

        public static string AlertEnglish(char[] chars, int start, int end)
        {
            for (var i = start; i < start + end; i++)
            {
                if (chars[i] >= MinLower && chars[i] <= MaxLower)
                {
                    chars[i] = (char)(chars[i] - LowerGap);
                }
                if (chars[i] >= MinUpper && chars[i] <= MaxUpper)
                {
                    chars[i] = (char)(chars[i] - UpperGap);
                }
                if (chars[i] >= MinUpperE && chars[i] <= MaxUpperE)
                {
                    chars[i] = (char)(chars[i] - UpperGapE);
                }
            }
            return new string(chars, start, end);
        }

        public static string AlertEnglish(string temp, int start, int end)
        {
            char c;
            var sb = new StringBuilder();
            for (var i = start; i < start + end; i++)
            {
                c = temp.charAt(i);
                if (c >= MinLower && c <= MaxLower)
                {
                    sb.Append((char)(c - LowerGap));
                }
                else if (c >= MinUpper && c <= MaxUpper)
                {
                    sb.Append((char)(c - UpperGap));
                }
                else if (c >= MinUpperE && c <= MaxUpperE)
                {
                    sb.Append((char)(c - UpperGapE));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string AlertNumber(char[] chars, int start, int end)
        {
            for (var i = start; i < start + end; i++)
            {
                if (chars[i] >= MinUpperN && chars[i] <= MaxUpperN)
                {
                    chars[i] = (char)(chars[i] - UpperGapN);
                }
            }
            return new string(chars, start, end);
        }

        public static string AlertNumber(string temp, int start, int end)
        {
            char c;
            var sb = new StringBuilder();
            for (var i = start; i < start + end; i++)
            {
                c = temp.charAt(i);
                if (c >= MinUpperN && c <= MaxUpperN)
                {
                    sb.Append((char)(c - UpperGapN));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 将一个字符串标准化
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>

        public static char[] AlertStr(string str)
        {
            var chars = new char[str.Length];
            char c;
            for (var i = 0; i < chars.Length; i++)
            {
                c = Charcover[str.charAt(i)];
                if (c > 0)
                {
                    chars[i] = c;
                }
                else
                {
                    chars[i] = str.charAt(i);
                }
            }
            return chars;
        }

        /// <summary>
        /// 判断一个字符串是否是english
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>

        public static bool IsEnglish(string word)
        {
            var length = word.Length;
            char c;
            for (var i = 0; i < length; i++)
            {
                c = word.charAt(i);
                if ((c >= 'a' && c <= 'z') || (c >= MinLower && c <= MaxLower) || (c >= MinUpper && c <= MaxUpper) ||
                    (c >= MinUpperE && c <= MaxUpperE))
                {
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 判断一个字符串是否是数字
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>

        public static bool IsNumber(string word)
        {
            char c;
            var len = word.Length;
            for (var i = 0; i < len; i++)
            {
                c = word.charAt(i);
                if ((c >= '0' && c <= '9') || c >= MinUpperN && c <= MaxUpperN || c == '.')
                {
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 将一个char标准化
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>

        public static char CharCover(char c)
        {
            return Charcover[c];
        }

    }
}