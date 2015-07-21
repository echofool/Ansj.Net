using System;
using System.IO;
using System.Text;

namespace Ansj.Net.Util
{
    /// <summary>
    ///     我又剽窃了下jdk...职业嫖客 为了效率这个流的操作是不支持多线程的,要么就是长时间不写这种东西了。发现好费劲啊 这个reader的特点。。只会输入
    ///     句子不会输出\r\n .会有一个start来记录当前返回字符串。起始偏移量
    /// </summary>
    [Obsolete("这里没有仔细研究怎么实现")]
    public class AnsjReader : TextReader
    {
        private const int DefaultCharBufferSize = 8192;
        private char[] _cb;
        private bool _firstRead = true;
        private bool _isRead;
        private int _len;
        private int _offe;
        private bool _ok;
        private TextReader _reader;
        private int _tempLen;
        private int _tempOffe;
        private int _tempStart;

        /// <summary>
        ///     Creates a buffering character-input stream that uses an input buffer of
        ///     the specified size.
        /// </summary>
        /// <param name="reader">A Reader</param>
        /// <param name="sz">Input-buffer size</param>
        public AnsjReader(TextReader reader, int sz)
        {
            if (sz <= 0)
                throw new ArgumentException("Buffer size <= 0", "sz");
            _reader = reader;
            _cb = new char[sz];
        }

        /// <summary>
        ///     Creates a buffering character-input stream that uses a default-sized
        ///     input buffer.
        /// </summary>
        /// <param name="reader">A Reader</param>
        public AnsjReader(TextReader reader)
            : this(reader, DefaultCharBufferSize)
        {
        }

        public int Start { get; private set; }

        /// <summary>
        ///     Checks to make sure that the stream has not been closed
        /// </summary>
        private void EnsureOpen()
        {
            if (_reader == null)
                throw new IOException("Stream closed");
        }

        /// <summary>
        ///     为了功能的单一性我还是不实现了
        /// </summary>
        /// <param name="cbuf"></param>
        /// <param name="off"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        private int read(char[] cbuf, int off, int len)
        {
            throw new IOException("AnsjBufferedReader not support this interface! ");
        }

        /// <summary>
        ///     读取一行数据。ps 读取结果会忽略 \n \r
        /// </summary>
        /// <returns></returns>
        public override string ReadLine()
        {
            EnsureOpen();

            StringBuilder sb = null;

            Start = _tempStart;

            _firstRead = true;

            while (true)
            {
                _tempLen = 0;
                _ok = false;

                ReadString();
                // if (tempLen != 0)
                // System.out.println(new String(cb, tempOffe, tempLen));

                if (!_isRead && (_tempLen == 0 || _len == 0))
                {
                    if (sb != null)
                    {
                        return sb.ToString();
                    }
                    return null;
                }

                if (!_isRead)
                {
                    // 如果不是需要读状态，那么返回
                    _tempStart += _tempLen;
                    if (sb == null)
                    {
                        return new string(_cb, _tempOffe, _tempLen);
                    }
                    sb.Append(_cb, _tempOffe, _tempLen);
                    return sb.ToString();
                }

                if (_tempLen == 0)
                {
                    continue;
                }

                // 如果是需要读状态那么读取
                if (sb == null)
                {
                    sb = new StringBuilder();
                }
                sb.Append(_cb, _tempOffe, _tempLen);
                _tempStart += _tempLen;
            }
        }

        private void ReadString()
        {
            if (_offe <= 0)
            {
                if (_offe == -1)
                {
                    _isRead = false;
                    return;
                }
                //echofool TODO:这里没有仔细研究怎么实现
                _len = _reader.Read(_cb, _offe, 0);
                if (_len <= 0)
                {
                    // 说明到结尾了
                    _isRead = false;
                    return;
                }
            }

            _isRead = true;

            char c;
            var i = _offe;
            for (; i < _len; i++)
            {
                c = _cb[i];
                if (c != '\r' && c != '\n')
                {
                    break;
                }
                if (!_firstRead)
                {
                    i++;
                    _tempStart++;
                    _offe = i;
                    _tempOffe = _offe;
                    _isRead = false;
                    return;
                }
                _tempStart++;
                Start++;
            }

            if (i == _len)
            {
                _isRead = true;
                _offe = 0;
                return;
            }

            _firstRead = false;

            _offe = i;

            for (; i < _len; i++)
            {
                c = _cb[i];
                if (c == '\n' || c == '\r')
                {
                    _isRead = false;
                    break;
                }
            }

            _tempOffe = _offe;
            _tempLen = i - _offe;

            if (i == _len)
            {
                if (_len < _cb.Length)
                {
                    // 说明到结尾了
                    _isRead = false;
                    _offe = -1;
                }
                else
                {
                    _offe = 0;
                }
            }
            else
            {
                _offe = i;
            }
        }

        public override void Close()
        {
            lock (this)
            {
                if (_reader == null)
                    return;
                try
                {
                    _reader.Close();
                }
                finally
                {
                    _reader = null;
                    _cb = null;
                }
            }
        }
    }
}