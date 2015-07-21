using System;
using Nlpcn.Net.Commons.Lang.Tire;

namespace Nlpcn.Net.Commons.Lang.Tire.Domain
{
    public class Forest : IWoodInterface
    {
        private IWoodInterface[] chars = new IWoodInterface[65536];

        public byte Status
        {
            get { return 0; }
            set { }
        }

        public string[] Param
        {
            get { return null; }
            set { }
        }

        public IWoodInterface Add(IWoodInterface branch)
        {
            var temp = chars[branch.GetChar()];
            if (temp == null)
                chars[branch.GetChar()] = branch;
            else
            {
                switch (branch.Status)
                {
                    case 1:
                        if (temp.Status == 3)
                        {
                            temp.Status = 2;
                        }
                        break;
                    case 3:
                        if (temp.Status == 1)
                        {
                            temp.Status = 2;
                        }
                        temp.Param = branch.Param;
                        break;
                }
            }

            return chars[branch.GetChar()];
        }

        public bool Contains(char c)
        {
            return chars[c] != null;
        }

        public IWoodInterface Get(char c)
        {
            if (c > 66535)
            {
                Console.WriteLine(c);
                return null;
            }
            return chars[c];
        }

        public int CompareTo(char c)
        {
            return 0;
        }

        public bool Equals(char c)
        {
            return false;
        }

        public char GetChar()
        {
            //TODO:echofool 原来是'\000'
            return '\0';
        }

        public int getNature()
        {
            return 0;
        }

        public void setNature(int nature)
        {
        }

        public int getSize()
        {
            return chars.Length;
        }

        /**
         * 得到一个分词对象
         *
         * @param content
         * @return
         */

        public GetWord getWord(string content)
        {
            return new GetWord(this, content);
        }

        /**
         * 得到一个分词对象
         *
         * @param content
         * @return
         */

        public GetWord getWord(char[] chars)
        {
            return new GetWord(this, chars);
        }

        /**
         * 清空树释放内存
         */

        public void clear()
        {
            chars = new IWoodInterface[65535];
        }
    }
}