using System;
using System.Diagnostics;
using System.IO;
using Ansj.Net.Util;
using Lucene.Net.Support;

namespace Ansj.Net.Library.Company
{
/**
 * 机构名识别词典加载类
 * 
 * @author ansj
 * 
 */

    public class CompanyAttrLibrary
    {
        private static HashMap<string, int[]> _cnMap;

        private CompanyAttrLibrary()
        {
        }

        public static HashMap<string, int[]> GetCompanyMap()
        {
            if (_cnMap != null)
            {
                return _cnMap;
            }
            try
            {
                Init();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                _cnMap = new HashMap<string, int[]>();
            }
            return _cnMap;
        }

        // company_freq

        private static void Init()
        {
            // TODO Auto-generated method stub
            TextReader br = null;
            try
            {
                _cnMap = new HashMap<string, int[]>();
                br = MyStaticValue.GetCompanReader();
                string temp;
                while ((temp = br.ReadLine()) != null)
                {
                    var strs = temp.Split('\t');
                    var cna = new int[2];
                    cna[0] = int.Parse(strs[1]);
                    cna[1] = int.Parse(strs[2]);
                    _cnMap.Add(strs[0], cna);
                }
            }
            finally
            {
                if (br != null)
                    br.Close();
            }
        }
    }
}