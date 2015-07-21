using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ansj.Net.Domain;
using Ansj.Net.Library;
using Nlpcn.Net.Commons.Lang.Tire.Domain;

namespace Ansj.Net.Util
{
    /// <summary>
    ///     停用词过滤,修正词性到用户词性.
    /// </summary>
    public class FilterModifWord
    {
        private const string Tag = "#";
        private static readonly HashSet<string> Filter = new HashSet<string>();
        private static bool _isTag;

        public static void InsertStopWords(List<string> filterWords)
        {
            Filter.UnionWith(filterWords);
        }

        public static void InsertStopWord(params string[] filterWords)
        {
            Filter.UnionWith(filterWords);
        }

        public static void InsertStopNatures(params string[] filterNatures)
        {
            _isTag = true;
            Filter.UnionWith(filterNatures.Select(t => Tag + t));
        }

        /// <summary>
        ///     停用词过滤并且修正词性
        /// </summary>
        /// <param name="all"></param>
        /// <returns></returns>
        public static List<Term> ModifResult(List<Term> all)
        {
            var result = new List<Term>();
            try
            {
                foreach (var term in all)
                {
                    if (Filter.Count > 0 &&
                        (Filter.Contains(term.Name) || (_isTag && Filter.Contains(Tag + term.Nature.natureStr))))
                    {
                        continue;
                    }
                    var @params = UserDefineLibrary.GetParams(term.Name);
                    if (@params != null)
                    {
                        term.Nature = new Nature(@params[0]);
                    }
                    result.Add(term);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Ansj.Net.Util.FilterModifWord.ModifResult");
                Trace.WriteLine("FilterStopWord.updateDic can not be null , " +
                                "you must use set FilterStopWord.setUpdateDic(map) or use method set map");
            }
            return result;
        }

        /*
         * 停用词过滤并且修正词性
         */

        public static List<Term> ModifResult(List<Term> all, params Forest[] forests)
        {
            var result = new List<Term>();
            try
            {
                foreach (var term in all)
                {
                    if (Filter.Count > 0 &&
                        (Filter.Contains(term.Name) || Filter.Contains(Tag + term.Nature.natureStr)))
                    {
                        continue;
                    }
                    foreach (var forest in forests)
                    {
                        var @params = UserDefineLibrary.GetParams(forest, term.Name);
                        if (@params != null)
                        {
                            term.Nature = new Nature(@params[0]);
                        }
                    }
                    result.Add(term);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, "Ansj.Net.Util.FilterModifWord.ModifResult");
                Trace.WriteLine("FilterStopWord.updateDic can not be null , " +
                                "you must use set FilterStopWord.setUpdateDic(map) or use method set map");
            }
            return result;
        }
    }
}