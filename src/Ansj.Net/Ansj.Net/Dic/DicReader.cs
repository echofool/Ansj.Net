using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Ansj.Net.Dic
{
    /// <summary>
    ///     加载词典用的类
    /// </summary>
    public class DicReader
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static TextReader GetReader(string name)
        {
            try
            {
                return new StreamReader(GetInputStream(name), Encoding.UTF8);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e, name);
            }
            return null;
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Stream GetInputStream(string name)
        {
            Stream input = File.Open("Resources/" + name, FileMode.Open, FileAccess.ReadWrite);
            return input;
        }
    }
}