using System;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Ansj.Net.Recognition;

namespace Ansj.Net.Util
{
    public class MathUtil
    {
        /// <summary>
        /// 一个参数
        /// </summary>
        private const int MaxFrequence = 2079997; // 7528283+329805;

        /// <summary>
        /// 平滑参数
        /// </summary>
        private static readonly double dSmoothingPara = 0.1;

        /// <summary>
        ///  ﻿Two linked Words frequency
        /// </summary>
        private static readonly double Temp = (double)1 / MaxFrequence;

        /// <summary>
        /// 从一个词的词性到另一个词的词的分数
        /// </summary>
        /// <param name="from">前面的词</param>
        /// <param name="to">后面的词</param>
        /// <returns>分数</returns>

        public static double CompuScore(Term from, Term to)
        {
            double frequency = from.TermNatures.AllFreq + 1;

            if (frequency < 0)
            {
                from.Score = from.Score + MaxFrequence;
                return from.Score;
            }

            var nTwoWordsFreq = NgramLibrary.GetTwoWordFreq(from, to);
            var value =
                -Math.Log(dSmoothingPara * frequency / (MaxFrequence + 80000) +
                          (1 - dSmoothingPara) * ((1 - Temp) * nTwoWordsFreq / frequency + Temp));

            if (value < 0)
            {
                value += frequency;
            }
            return from.Score + value;
        }

        /// <summary>
        /// 词性词频词长.计算出来一个分数
        /// </summary>
        /// <param name="from"></param>
        /// <param name="term"></param>
        /// <returns></returns>
        public static double CompuScoreFreq(Term from, Term term)
        {
            return from.TermNatures.AllFreq + term.TermNatures.AllFreq;
        }

        /// <summary>
        /// 两个词性之间的分数计算
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>

        public static double CompuNatureFreq(NatureRecognition.NatureTerm from, NatureRecognition.NatureTerm to)
        {
            double twoWordFreq = NatureLibrary.GetTwoNatureFreq(from.TermNature.nature, to.TermNature.nature);
            if (twoWordFreq == 0)
            {
                twoWordFreq = Math.Log(from.SelfScore + to.SelfScore);
            }
            var score = from.Score + Math.Log((from.SelfScore + to.SelfScore) * twoWordFreq) + to.SelfScore;
            return score;
        }

        public static void main(string[] args)
        {
            Console.WriteLine(Math.Log(Temp * 2));
        }
    }
}