namespace Ansj.Net.Util
{
    public class MatrixUtil
    {
        /// <summary>
        ///     向量求和
        /// </summary>
        /// <param name="dbs"></param>
        /// <returns></returns>
        public static double Sum(double[] dbs)
        {
            double value = 0;
            foreach (var d in dbs)
            {
                value += d;
            }
            return value;
        }

        public static int Sum(int[] dbs)
        {
            var value = 0;
            foreach (var d in dbs)
            {
                value += d;
            }
            return value;
        }

        public static double Sum(double[][] w)
        {
            double value = 0;
            foreach (var dbs in w)
            {
                value += Sum(dbs);
            }
            return value;
        }

        public static void Dot(double[] feature, double[] feature1)
        {
            if (feature1 == null)
            {
                return;
            }
            for (var i = 0; i < feature1.Length; i++)
            {
                feature[i] += feature1[i];
            }
        }
    }
}