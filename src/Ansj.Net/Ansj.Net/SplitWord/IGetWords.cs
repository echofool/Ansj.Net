namespace Ansj.Net.SplitWord
{
    public interface IGetWords
    {
        /// <summary>
        ///     全文全词全匹配
        ///     传入的需要分词的句子
        /// </summary>
        /// <returns>返还分完词后的句子</returns>
        string AllWords();

        /// <summary>
        ///     同一个对象传入词语
        /// </summary>
        /// <param name="temp">传入的句子</param>
        void SetStr(string temp);

        void SetChars(char[] chars, int start, int end);
        int GetOffe();
    }
}