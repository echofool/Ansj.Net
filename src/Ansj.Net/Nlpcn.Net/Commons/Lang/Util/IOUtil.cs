using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Nlpcn.Net.Commons.Lang.Util
{
    /// <summary>
    /// 一个简单的io操作
    /// </summary>

    // ReSharper disable InconsistentNaming
    public class IOUtil
    // ReSharper restore InconsistentNaming
    {
        public static readonly string Table = "\t";
        public static readonly string Line = "\n";
        public static readonly byte[] TableBytes = Encoding.UTF8.GetBytes("\t");
        public static readonly byte[] LineBytes = Encoding.UTF8.GetBytes("\n");

        public static FileStream GetInputStream(string path)
        {
            try
            {
                return File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        public static TextReader GetReader(string path, Encoding charEncoding)
        {
            return GetReader(File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite), charEncoding);
        }

        private static TextReader GetReader(FileStream file, Encoding charEncoding)
        {
            return new StreamReader(file, charEncoding);
        }

        public static void Writer(string path, Encoding charEncoding, string content)
        {
            File.WriteAllText(path, content, charEncoding);
        }

        /// <summary>
        /// 将输入流转化为字节流
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="charEncoding"></param>
        /// <returns></returns>

        public static TextReader GetReader(Stream inputStream, Encoding charEncoding)
        {
            return new StreamReader(inputStream, charEncoding);
        }

        /// <summary>
        /// 从流中读取正文内容
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="charEncoding"></param>
        /// <returns></returns>

        public static string GetContent(Stream stream, Encoding charEncoding)
        {
            TextReader reader = null;
            try
            {
                reader = GetReader(stream, charEncoding);
                return GetContent(reader);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
            finally
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                    }
                    catch (IOException e)
                    {
                        Trace.WriteLine(e);
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// 从文件中读取正文内容
        /// </summary>
        /// <param name="file"></param>
        /// <param name="charEncoding"></param>
        /// <returns></returns>
        public static string GetContent(string file, Encoding charEncoding)
        {
            try
            {
                File.ReadAllLines(file, charEncoding);
            }
            catch (FileNotFoundException e)
            {
                Trace.WriteLine(e);
            }
            return "";
        }

        public static string GetContent(TextReader reader)
        {
            var sb = new StringBuilder();
            try
            {
                string temp;
                while ((temp = reader.ReadLine()) != null)
                {
                    sb.Append(temp);
                    sb.Append("\n");
                }
            }
            finally
            {
                Close(reader);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 关闭字符流
        /// </summary>
        /// <param name="reader"></param>
        public static void Close(TextReader reader)
        {
            try
            {
                if (reader != null)
                    reader.Close();
            }
            catch (IOException e)
            {
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// 关闭字节流
        /// </summary>
        /// <param name="stream"></param>

        public static void Close(Stream stream)
        {
            try
            {
                if (stream != null)
                    stream.Close();
            }
            catch (IOException e)
            {
                Trace.WriteLine(e);
            }
        }

        /// <summary>
        /// 关闭字节流
        /// </summary>
        /// <param name="os"></param>

        public static void Close(StreamReader os)
        {
            try
            {
                if (os != null)
                {
                    os.Close();
                }
            }
            catch (IOException e)
            {
                Trace.WriteLine(e);
            }
        }
    }
}