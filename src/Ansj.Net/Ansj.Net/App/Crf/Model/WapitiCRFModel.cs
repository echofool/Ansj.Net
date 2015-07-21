using System;
using System.Collections.Generic;
using System.Text;
using Lucene.Net.Support;
using Nlpcn.Net.Commons.Lang.Util;
using org.ansj.app.crf.pojo;
using Double = Lucene.Net.Support.Double;

namespace org.ansj.app.crf.model
{
    [Obsolete("原作者说crf要改很多，同时由于默认的java和.net的二进制序列化反序列化不能通用，所以这里就不能用了，等下一个版本！", true)]
    public class WapitiCRFModel : Model
    {
        private int maxSize;
        private HashMap<string, int> statusMap;
        private int tagNum;

        private void parseFile(string path, string templatePath)
        {
            // TODO Auto-generated method stub
            var reader = IOUtil.GetReader(path, Encoding.UTF8);

            statusMap = new HashMap<string, int>();

            // read config
            var content = IOUtil.GetContent(IOUtil.GetReader(templatePath, Encoding.UTF8));

            template = Template.parse(content);

            myGrad = new HashMap<string, Feature>();

            string temp = null;

            var statusLines = new List<string>();

            // 填充
            while ((temp = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(temp) && temp[0] == 'b')
                {
                    statusLines.Add(temp);
                }
            }

            foreach (var str in statusLines)
            {
                var split = str.Split('\t');
                addStatus(split[1]);
                addStatus(split[2]);
            }
            template.tagNum = tagNum;
            status = new double[tagNum][];
            for (var i = 0; i < status.Length; i++)
            {
                status[i] = new double[tagNum];
            }
            foreach (var str in statusLines)
            {
                var split = str.Split('\t');
                status[statusMap[split[1]]][statusMap[split[2]]] = Double.Parse(split[3]);
            }

            //fix status range sbme
            status[statusMap["S"]][statusMap["E"]] = double.MinValue;
            status[statusMap["S"]][statusMap["M"]] = double.MinValue;

            status[statusMap["B"]][statusMap["B"]] = double.MinValue;
            status[statusMap["B"]][statusMap["S"]] = double.MinValue;

            status[statusMap["M"]][statusMap["S"]] = double.MinValue;
            status[statusMap["M"]][statusMap["B"]] = double.MinValue;

            status[statusMap["E"]][statusMap["M"]] = double.MinValue;
            status[statusMap["E"]][statusMap["E"]] = double.MinValue;


            IOUtil.Close(reader);

            // read feature
            reader = IOUtil.GetReader(path, Encoding.UTF8);
            while ((temp = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(temp) && temp[0] == 'u')
                {
                    parseGrad(temp, template.ft.Length);
                }
                // 后面的不保留
                if (maxSize > 0 && myGrad.Count > maxSize)
                {
                    break;
                }
            }
            IOUtil.Close(reader);

            template.statusMap = statusMap;
        }

        private void parseGrad(string temp, int featureNum)
        {
            var split = temp.Split('\t');

            var mIndex = split[0].IndexOf(':');

            var name = fixName(split[0].Substring(mIndex + 1));

            var fIndex = int.Parse(split[0].Substring(1, mIndex));
            var sta = statusMap[split[2]];
            var step = double.Parse(split[3]);

            var feature = myGrad[name];
            if (feature == null)
            {
                feature = new Feature(featureNum, tagNum);
                myGrad.Add(name, feature);
            }
            feature.update(fIndex, sta, step);
        }

        private string fixName(string substring)
        {
            // TODO Auto-generated method stub
            var split = substring.Split(' ');
            var sb = new StringBuilder();
            foreach (var item in split)
            {
                if (item.StartsWith("_x"))
                {
                    sb.Append((char) ('B' + int.Parse(item.Substring(2))));
                }
                else
                {
                    sb.Append(item);
                }
            }

            return sb.ToString();
        }

        public void addStatus(string sta)
        {
            if (statusMap.ContainsKey(sta))
            {
                return;
            }
            statusMap.Add(sta, tagNum);
            tagNum++;
        }

        /**
         * 解析wapiti 生成的模型 dump 出的文件
         * 
         * @param modelPath
         * @param templatePath
         * @param maxSize
         *            模型存储内容大小
         * @throws Exception
         */

        public static Model parseFileToModel(string modelPath, string templatePath, int maxSzie)
        {
            var model = new WapitiCRFModel();
            model.maxSize = maxSzie;
            model.parseFile(modelPath, templatePath);
            return model;
        }
    }
}