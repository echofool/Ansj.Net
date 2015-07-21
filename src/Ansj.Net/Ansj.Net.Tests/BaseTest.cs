using System.Diagnostics;
using Ansj.Net.Library;
using Ansj.Net.SplitWord.Analysis;
using Nlpcn.Net.Commons.Lang.Tire;
using Nlpcn.Net.Commons.Lang.Tire.Domain;
using NUnit.Framework;

namespace Ansj.Net
{
    [TestFixture]
    public class BaseTest
    {
        [Test]
        public void Test1()
        {

            // 增加新词,中间按照'\t'隔开
            UserDefineLibrary.InsertWord("ansj中文分词", "userDefine", 1000);
            var terms = ToAnalysis.Parse("我觉得Ansj中文分词是一个不错的系统!我是王婆!");
            foreach (var term in terms)
            {
                Debug.WriteLine(term.RealName, "增加新词");
            }

            // 删除词语,只能删除.用户自定义的词典.
            UserDefineLibrary.RemoveWord("ansj中文分词");
            terms = ToAnalysis.Parse("我觉得ansj中文分词是一个不错的系统!我是王婆!");
            foreach (var term in terms)
            {
                Debug.WriteLine(term.RealName, "删除用户自定义词典例子");
            }

            // 歧义词
            var value = new Value("济南下车", "济南", "n", "下车", "v");
            terms = ToAnalysis.Parse("我经济南下车到广州.中国经济南下势头迅猛!");
            foreach (var term in terms)
            {
                Debug.WriteLine(term.RealName, "歧义词1");
            }
            StaticLibrary.InsertWord(UserDefineLibrary.AmbiguityForest, value);
            terms = ToAnalysis.Parse("我经济南下车到广州.中国经济南下势头迅猛!");
            foreach (var term in terms)
            {
                Debug.WriteLine(term.RealName, "歧义词2");
            }

            // 多用户词典
            var str = "神探夏洛克这部电影作者.是一个dota迷";
            terms = ToAnalysis.Parse(str);
            foreach (var term in terms)
            {
                Debug.WriteLine(term.RealName, "多用户词典1");
            }
            // 两个词汇 神探夏洛克 douta迷
            var dic1 = new Forest();
            StaticLibrary.InsertWord(dic1, new Value("神探夏洛克", "define", "1000"));
            var dic2 = new Forest();
            StaticLibrary.InsertWord(dic2, new Value("dota迷", "define", "1000"));
            terms = ToAnalysis.Parse(str, dic1, dic2);
            foreach (var term in terms)
            {
                Debug.WriteLine(term.RealName, "多用户词典2");
            }
        }
    }
}