namespace org.ansj.app.crf.pojo
{
    public class TempFeature
    {
        public int featureId;
        public int id;
        public string name;

        public TempFeature(string str, int tagNum)
        {
            var split = str.Split(' ');
            id = int.Parse(split[0])/tagNum;
            var splitIndex = split[1].IndexOf(':');
            featureId = int.Parse(split[1].Substring(1, splitIndex));
            name = split[1].Substring(splitIndex + 1).Replace("/", "");
        }
    }
}