using System.Text;

namespace org.ansj.app.crf.pojo
{
    public class Feature
    {
        public int tagNum;
        public double value;
        public double[][] w;

        public Feature(int featureNum, int tagNum)
        {
            w = new double[featureNum][];
            this.tagNum = tagNum;
        }

        public Feature(double[][] w)
        {
            this.w = w;
        }

        public void update(int fIndex, int sta, double step)
        {
            value += step;
            if (w[fIndex].Length == 0)
            {
                w[fIndex] = new double[tagNum];
            }
            w[fIndex][sta] += step;
        }

        public override string ToString()
        {
            // TODO Auto-generated method stub
            var sb = new StringBuilder();
            foreach (var ints in w)
            {
                sb.Append(string.Join(",", ints));
                sb.Append("\n");
            }
            return sb.ToString();
        }
    }
}