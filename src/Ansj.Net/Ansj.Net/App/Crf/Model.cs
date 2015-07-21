using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Support;
using Nlpcn.Net.Commons.Lang.Tire.Domain;
using org.ansj.app.crf.pojo;

namespace org.ansj.app.crf
{
    [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
    public abstract class Model
    {
        public enum MODEL_TYPE
        {
            CRF,
            EMM
        };

        public int allFeatureCount = 0;
        public int end1;
        public int end2;
        private List<Element> leftList;
        protected HashMap<string, Feature> myGrad;
        private List<Element> rightList;
        protected SmartForest<double[][]> smartForest = null;
        protected double[][] status = null;
        public Template template = null;
        /**
         * 根据模板文件解析特征
         * 
         * @param template
         * @throws IOException
         */

        private void makeSide(int left, int right)
        {
            // TODO Auto-generated method stub

            leftList = new List<Element>(Math.Abs(left));
            for (var i = left; i < 0; i++)
            {
                leftList.Add(new Element((char) ('B' + i)));
            }

            rightList = new List<Element>(right);
            for (var i = 1; i < right + 1; i++)
            {
                rightList.Add(new Element((char) ('B' + i)));
            }
        }

        /**
         * 讲模型写入
         * 
         * @param path
         * @throws FileNotFoundException
         * @throws IOException
         */
        // TODO:echofool 这里的代码暂时没有实现，因为java的序列化反序列化和.net有什么差别还没有测试
        [Obsolete("这里的代码暂时没有实现，因为java的序列化反序列化和.net有什么差别还没有测试")]
        public void writeModel(string path)
        {
            Console.WriteLine("compute ok now to save model!");
            // 写模型
            //ObjectOutputStream oos = new ObjectOutputStream(new BufferedOutputStream(new GZIPOutputStream(new FileOutputStream(path))));

            //// 配置模板
            //oos.writeObject(template);
            //// 特征转移率
            //oos.writeObject(status);
            //// 总共的特征数
            //oos.writeInt(myGrad.size());
            //double[] ds = null;
            //foreach (KeyValuePair<String, Feature> entry in myGrad) {
            //    oos.writeUTF(entry.getKey());
            //    for (int i = 0; i < template.ft.length; i++) {
            //        ds = entry.getValue().w[i];
            //        for (int j = 0; j < ds.length; j++) {
            //            oos.writeByte(j);
            //            oos.writeFloat((float) ds[j]);
            //        }
            //        oos.writeByte(-1);
            //    }
            //}

            //oos.flush();
            //oos.close();
        }

        /**
         * 模型读取
         * 
         * @param path
         * @return
         * @return
         * @throws FileNotFoundException
         * @throws IOException
         * @throws ClassNotFoundException
         */

        public static Model loadModel(string modelPath)
        {
            return loadModel(File.Open(modelPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
        }

        // TODO:echofool 这里的代码暂时没有实现，因为java的序列化反序列化和.net有什么差别还没有测试
        [Obsolete("这里的代码暂时没有实现，因为java的序列化反序列化和.net有什么差别还没有测试")]
        public static Model loadModel(Stream modelStream)
        {
            //ObjectInputStream ois = null;
            //try {
            //    ois = new ObjectInputStream(new BufferedInputStream(new GZIPInputStream(modelStream)));

            //    Model model = new Model() {

            //        public void writeModel(String path) throws FileNotFoundException, IOException {
            //            // TODO Auto-generated method stub
            //            throw new RuntimeException("you can not to calculate ,this model only use by cut ");
            //        }

            //    };

            //    model.template = (Template) ois.readObject();

            //    model.makeSide(model.template.left, model.template.right);

            //    int tagNum = model.template.tagNum;

            //    int featureNum = model.template.ft.length;

            //    model.smartForest = new SmartForest<double[][]>(0.8);

            //    model.status = (double[][]) ois.readObject();

            //    // 总共的特征数
            //    double[][] w = null;
            //    String key = null;
            //    int b = 0;
            //    int featureCount = ois.readInt();
            //    for (int i = 0; i < featureCount; i++) {
            //        key = ois.readUTF();
            //        w = new double[featureNum][0];
            //        for (int j = 0; j < featureNum; j++) {
            //            while ((b = ois.readByte()) != -1) {
            //                if (w[j].length == 0) {
            //                    w[j] = new double[tagNum];
            //                }
            //                w[j][b] = ois.readFloat();
            //            }
            //        }
            //        model.smartForest.add(key, w);
            //    }

            //    return model;
            //} finally {
            //    if (ois != null) {
            //        ois.close();
            //    }
            //}
            return null;
        }

        public double[] getFeature(int featureIndex, params char[] chars)
        {
            var sf = smartForest;
            sf = sf.GetBranch(chars);
            if (sf == null || sf.Param == null)
            {
                return null;
            }
            return sf.Param[featureIndex];
        }

        /**
         * tag转移率
         * 
         * @param s1
         * @param s2
         * @return
         */

        public double tagRate(int s1, int s2)
        {
            // TODO Auto-generated method stub
            return status[s1][s2];
        }
    }
}