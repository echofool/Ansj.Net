using System;

namespace Nlpcn.Net.Commons.Lang.Tire.Domain
{
    /// <summary>
    /// 一个小树,和Forest的区别是.这个在首字也是用二分查找,做过一次优化.达到到达一定量级自动扩展为hash定位 在ansj分词中这个应用是在自适应分词
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class SmartForest<T> : IComparable<SmartForest<T>>
    {
        private const int MaxSize = 65536;
        private readonly char _char;
        private readonly double _rate = 0.9;
        /// <summary>
        /// 单独查找出来的对象
        /// </summary>
        private SmartForest<T> _branch;
        /// <summary>
        /// status 此字的状态1，继续 2，是个词语但是还可以继续 ,3确定 nature 词语性质
        /// </summary>
        public SmartForest<T>[] Branches;

        /// <summary>
        /// 词典后的参数
        /// </summary>
        public T Param { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        private byte _status = 1;

        public SmartForest()
        {
        }

        /// <summary>
        /// 首位直接数组定位
        /// </summary>
        /// <param name="rate"></param>
        public SmartForest(double rate)
        {
            Branches = new SmartForest<T>[MaxSize];
            _rate = rate;
        }

        // temp branch
        private SmartForest(char @char)
        {
            _char = @char;
        }

        public SmartForest(char @char, int status, T param)
        {
            _char = @char;
            _status = (byte)status;
            Param = param;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public byte Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public int CompareTo(SmartForest<T> o)
        {
            if (_char > o._char)
                return 1;
            if (_char < o._char)
            {
                return -1;
            }
            return 0;
        }

        /// <summary>
        /// 增加子页节点
        /// </summary>
        /// <param name="branch"></param>
        /// <returns></returns>
        private void Add(SmartForest<T> branch)
        {
            if (Branches == null)
            {
                Branches = new SmartForest<T>[0];
            }
            var bs = Get(branch.GetChar());
            if (bs > -1)
            {
                if (Branches[bs] == null)
                {
                    Branches[bs] = branch;
                }
                _branch = Branches[bs];
                switch (branch.Status)
                {
                    case 0:
                        _branch.Status = 1;
                        break;
                    case 1:
                        if (_branch.Status == 3)
                        {
                            _branch.Status = 2;
                        }
                        break;
                    case 3:
                        if (_branch.Status != 3)
                        {
                            _branch.Status = 2;
                        }
                        _branch.Param = branch.Param;
                        break;
                }
                return;
            }

            if (bs < 0)
            {
                // 如果数组内元素接近于最大值直接数组定位，rate是内存和速度的一个平衡
                if (Branches != null && Branches.Length >= MaxSize * _rate)
                {
                    var tempBranches = new SmartForest<T>[MaxSize];
                    foreach (var b in Branches)
                    {
                        tempBranches[b.GetChar()] = b;
                    }
                    tempBranches[branch.GetChar()] = branch;
                    Branches = null;
                    Branches = tempBranches;
                }
                else
                {
                    var newBranches = new SmartForest<T>[Branches.Length + 1];
                    var insert = -(bs + 1);
                    Array.Copy(Branches, 0, newBranches, 0, insert);
                    Array.Copy(Branches, insert, newBranches, insert + 1, Branches.Length - insert);
                    newBranches[insert] = branch;
                    Branches = newBranches;
                }
            }
        }

        public int Get(char c)
        {
            if (Branches == null)
                return -1;
            if (Branches.Length == MaxSize)
            {
                return c;
            }
            var i = Array.BinarySearch(Branches, new SmartForest<T>(c));
            return i;
        }

        public SmartForest<T> GetBranch(char c)
        {
            var index = Get(c);
            if (index < 0)
            {
                return null;
            }
            return Branches[index];
        }

        /// <summary>
        /// 二分查找是否包含
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool Contains(char c)
        {
            if (Branches == null)
            {
                return false;
            }
            return Array.BinarySearch(Branches, c) > -1;
        }

        public int CompareTo(char c)
        {
            if (_char > c)
                return 1;
            if (_char < c)
            {
                return -1;
            }
            return 0;
        }

        public bool Equals(char c)
        {
            return _char == c;
        }

        #region Overrides of Object

        /// <summary>
        ///     用作特定类型的哈希函数。
        /// </summary>
        /// <returns>
        ///     当前 <see cref="T:System.Object" /> 的哈希代码。
        /// </returns>
        public override int GetHashCode()
        {
            return _char;
        }

        #endregion

        public char GetChar()
        {
            return _char;
        }

        /// <summary>
        /// 增加新词
        /// </summary>
        /// <param name="keyWord"></param>
        /// <param name="t"></param>
        public void Add(string keyWord, T t)
        {
            var tempBranch = this;
            for (var i = 0; i < keyWord.Length; i++)
            {
                if (keyWord.Length == i + 1)
                {
                    tempBranch.Add(new SmartForest<T>(keyWord[i], 3, t));
                }
                else
                {
                    tempBranch.Add(new SmartForest<T>(keyWord[i], 1, default(T)));
                }
                tempBranch = tempBranch.Branches[tempBranch.Get(keyWord[i])];
            }
        }

        /// <summary>
        /// 根据一个词获得所取的参数,没有就返回null
        /// </summary>
        /// <param name="keyWord"></param>
        /// <returns></returns>

        public SmartForest<T> GetBranch(string keyWord)
        {
            var tempBranch = this;
            for (var j = 0; j < keyWord.Length; j++)
            {
                var index = tempBranch.Get(keyWord[j]);
                if (index < 0)
                {
                    return null;
                }
                if ((tempBranch = tempBranch.Branches[index]) == null)
                {
                    return null;
                }
            }
            return tempBranch;
        }

        /// <summary>
        /// 根据一个词获得所取的参数,没有就返回null
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        public SmartForest<T> GetBranch(char[] chars)
        {
            var tempBranch = this;
            for (var j = 0; j < chars.Length; j++)
            {
                var index = tempBranch.Get(chars[j]);
                if (index < 0)
                {
                    return null;
                }
                if ((tempBranch = tempBranch.Branches[index]) == null)
                {
                    return null;
                }
            }
            return tempBranch;
        }

        public SmartGetWord<T> GetWord(char[] chars)
        {
            return new SmartGetWord<T>(this, chars);
        }
    }
}