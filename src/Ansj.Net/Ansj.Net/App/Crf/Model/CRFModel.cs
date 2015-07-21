using System;
using System.Text;
using Lucene.Net.Support;
using Nlpcn.Net.Commons.Lang.Util;
using org.ansj.app.crf.pojo;

namespace org.ansj.app.crf.model
{
    [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
    public class CRFModel : Model
    {
        /**
	 * 解析crf++生成的可可视txt文件
	 */

        private void parseFile(string path)
        {
            // TODO Auto-generated method stub
            var reader = IOUtil.GetReader(path, Encoding.UTF8);

            string temp = null;

            reader.ReadLine(); // version
            reader.ReadLine(); // cost-factor

            var maxId = int.Parse(reader.ReadLine().Split(':')[1].Trim()); // read
            // maxId

            reader.ReadLine(); // xsize
            reader.ReadLine(); // line

            var statusMap = new HashMap<string, int>();

            // read status
            var tagNum = 0;
            while ((temp = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(temp))
                {
                    break;
                }
                statusMap[temp] = tagNum;
                tagNum++;
            }

            status = new double[tagNum][];
            for (var index = 0; index < status.Length; index++)
            {
                status[index] = new double[tagNum];
            }

            // read config
            var sb = new StringBuilder();
            while ((temp = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(temp))
                {
                    break;
                }
                sb.Append(temp + "\n");
            }

            template = Template.parse(sb.ToString());

            template.tagNum = tagNum;

            template.statusMap = statusMap;

            var featureNum = template.ft.Length;

            var tempFeatureArr = new TempFeature[maxId/tagNum];

            var split = reader.ReadLine().Split(' '); // read first B

            var bIndex = int.Parse(split[0])/tagNum;

            TempFeature tf = null;

            while ((temp = reader.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(temp))
                {
                    break;
                }
                tf = new TempFeature(temp, tagNum);
                tempFeatureArr[tf.id] = tf;
            }

            myGrad = new HashMap<string, Feature>();

            Feature feature = null;
            // 填充
            for (var i = 0; i < tempFeatureArr.Length; i++)
            {
                // 读取转移模板
                if (i == bIndex)
                {
                    for (var j = 0; j < tagNum; j++)
                    {
                        for (var j2 = 0; j2 < tagNum; j2++)
                        {
                            status[j][j2] = double.Parse(reader.ReadLine());
                        }
                    }
                    i += tagNum - 1;
                    continue;
                }
                tf = tempFeatureArr[i];
                feature = myGrad[tf.name];
                if (feature == null)
                {
                    feature = new Feature(featureNum, tagNum);
                    myGrad[tf.name] = feature;
                }
                for (var j = 0; j < tagNum; j++)
                {
                    var f = double.Parse(reader.ReadLine());
                    feature.update(tf.featureId, j, f);
                }
            }
        }

        /**
	 * 解析crf 生成的模型 txt文件
	 * 
	 * @param modelPath
	 * @param templatePath
	 * @return
	 * @throws Exception
	 */

        public static Model parseFileToModel(string modelPath)
        {
            var model = new CRFModel();
            model.parseFile(modelPath);
            return model;
        }
    }
}