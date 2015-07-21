using System;
using System.IO;
using System.Text;

namespace Nlpcn.Net.Commons.Lang.Dat
{
    /// <summary>
    /// 双数组使用
    /// </summary>
    public class DoubleArrayTire
    {
        public int ArrayLength;
        private Item[] _dat;

        private DoubleArrayTire()
        {
        }

        public static DoubleArrayTire Load(string filePath)
        {
            return LoadText(filePath);
        }

        /// <summary>
        /// 从文本中加载模型
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DoubleArrayTire LoadText<TItem>(string filePath) where TItem : Item, new()
        {
            return LoadText<TItem>(File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
        }

        /// <summary>
        /// 从文本中加载模型
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static DoubleArrayTire LoadText<TItem>(Stream stream) where TItem : Item, new()
        {
            var obj = new DoubleArrayTire();
            var reader = new StreamReader(stream, Encoding.UTF8);
            var lines = reader.ReadToEnd().Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            var temp = lines[0];
            obj.ArrayLength = int.Parse(temp);
            obj._dat = new Item[obj.ArrayLength];
            for (var i = 1; i < lines.Length; i++)
            {
                temp = lines[i];
                var item = new TItem();
                item.InitValue(temp.Split('\t'));
                obj._dat[item.Index] = item;
            }
            return obj;
        }

        /// <summary>
        /// 从文本中加载模型
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static DoubleArrayTire LoadText(string filePath)
        {
            return LoadText<BasicItem>(filePath);
        }
        /// <summary>
        /// 获得dat数组
        /// </summary>
        /// <returns></returns>
        public Item[] GetDat()
        {
            return _dat;
        }

        /// <summary>
        /// 一个词在词典中的id
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetId(string str)
        {
            var item = GetItem<Item>(str);
            if (item == null)
            {
                return 0;
            }
            return item.Index;
        }

        /// <summary>
        /// 获得一个词语的item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>

        public T GetItem<T>(string str) where T : Item
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return null;
            }
            if (str.Length == 1)
            {
                return (T)_dat[str[0]];
            }

            var item = _dat[str[0]];
            if (item == null)
            {
                return null;
            }
            for (var i = 1; i < str.Length; i++)
            {
                var checkValue = item.Index;
                if (item.Base + str[i] > _dat.Length - 1)
                    return null;

                item = _dat[item.Base + str[i]];
                if (item == null)
                {
                    return null;
                }
                if (item.Check != -1 && item.Check != checkValue)
                {
                    return null;
                }
            }
            return (T)item;
        }

        public T GetItem<T>(int id) where T : Item
        {
            return (T)_dat[id];
        }
    }
}