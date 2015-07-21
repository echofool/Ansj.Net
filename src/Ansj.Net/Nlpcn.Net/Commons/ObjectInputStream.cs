using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace nlpcn.net.commons
{
    [Obsolete("这个东西不能用，只是简单的写一下，先通过编译")]
    public class ObjectInputStream : StreamReader
    {
        public ObjectInputStream(Stream stream)
            : base(stream)
        {
        }

        public object ReadObject()
        {
            IFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(BaseStream);
        }

        public int ReadInt()
        {
            return (int) ReadObject();
        }
    }
}