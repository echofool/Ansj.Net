using System;
using Nlpcn.Net.Commons.Lang.Util;

namespace Nlpcn.Net.Commons.Lang.Tire.Domain
{
    public class Branch : IWoodInterface
    {
        private readonly char _char;
        /// <summary>
        /// 单独查找出来的对象
        /// </summary>
        private IWoodInterface _branch;
        /// <summary>
        /// status 此字的状态1，继续 2，是个词语但是还可以继续 ,3确定 nature 词语性质
        /// </summary>
        private IWoodInterface[] _branches;

        public Branch(char @char, int status, string[] param)
        {
            _char = @char;
            Status = (byte)status;
            Param = param;
        }

        /// <summary>
        /// 状态
        /// </summary>
        public byte Status { get; set; }
        /// <summary>
        /// 词典后的参数
        /// </summary>
        public string[] Param { get; set; }

        public IWoodInterface Add(IWoodInterface branch)
        {
            if (_branches == null)
            {
                _branches = new IWoodInterface[0];
            }
            var bs = AnsjArrays.BinarySearch(_branches, branch.GetChar());
            if (bs >= 0)
            {
                this._branch = _branches[bs];
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
                return _branch;
            }
            var newBranches = new IWoodInterface[_branches.Length + 1];
            var insert = -(bs + 1);
            Array.Copy(_branches, 0, newBranches, 0, insert);
            Array.Copy(_branches, insert, newBranches, insert + 1, _branches.Length - insert);
            newBranches[insert] = branch;
            _branches = newBranches;
            return branch;
        }

        public IWoodInterface Get(char c)
        {
            if (_branches == null)
            {
                return null;
            }
            var i = AnsjArrays.BinarySearch(_branches, c);
            if (i < 0)
            {
                return null;
            }
            return _branches[i];
        }

        public bool Contains(char c)
        {
            if (_branches == null)
            {
                return false;
            }
            return Array.BinarySearch(_branches, c) > -1;
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

        public char GetChar()
        {
            return _char;
        }

        /// <summary>
        /// 得到第几个参数
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public string GetParam(int i)
        {
            if (Param != null && Param.Length > i)
            {
                return Param[i];
            }
            return null;
        }

        #region Overrides of Object

        /// <summary>
        ///     确定指定的 <see cref="T:System.Object" /> 是否等于当前的 <see cref="T:System.Object" />。
        /// </summary>
        /// <returns>
        ///     如果指定的 <see cref="T:System.Object" /> 等于当前的 <see cref="T:System.Object" />，则为 true；否则为 false。
        /// </returns>
        /// <param name="obj">与当前的 <see cref="T:System.Object" /> 进行比较的 <see cref="T:System.Object" />。</param>
        public override bool Equals(object obj)
        {
            return obj is char && Equals((char)obj);
        }

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
    }
}