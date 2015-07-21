namespace Ansj.Net.Domain
{
    /// <summary>
    ///     新词发现,实体名
    /// </summary>
    public class NewWord
    {
        public NewWord(string name, Nature nature, double score)
        {
            Name = name;
            Nature = nature;
            Score = score;
            AllFreq = 1;
        }

        public NewWord(string name, Nature nature)
        {
            Name = name;
            Nature = nature;
            AllFreq = 1;
        }

        /// <summary>
        ///     总词频
        /// </summary>
        public int AllFreq { get; private set; }

        /// <summary>
        ///     名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     词性
        /// </summary>
        public Nature Nature { get; set; }

        /// <summary>
        ///     分数
        /// </summary>
        public double Score { get; set; }

        //此词是否被激活
        public bool IsActive { get; set; }

        /// <summary>
        ///     更新发现权重,并且更新词性
        /// </summary>
        /// <param name="nature"></param>
        /// <param name="freq"></param>
        public void Update(Nature nature, int freq)
        {
            // TODO Auto-generated method stub
            Score += Score*freq;
            AllFreq += freq;
            if (Nature.NW != nature)
            {
                Nature = nature;
            }
        }

        public override string ToString()
        {
            // TODO Auto-generated method stub
            return Name + "\t" + Score + "\t" + Nature.natureStr;
        }
    }
}