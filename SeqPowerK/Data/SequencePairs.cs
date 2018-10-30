using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalDistance
{
    class SequencePairs
    {
        private SequenceData _sequenceX = new SequenceData();
        private SequenceData _sequenceY = new SequenceData();
        private readonly Random _r;
        public static bool bCRMS = false;

        //test
        //static int no1 = 0;
        //static int no2 = 0;
        //
        /// <summary>
        /// 产生一条指定长度的序列
        /// </summary>
        /// <param name="listSign">AGCT概率表</param>
        /// <param name="length">序列长度</param>
        /// <param name="seed">随机种子</param>
        /// <returns></returns>
        /// 
        public SequencePairs(int seed = -1)
        {
            if (seed != -1)
            {
                _r = new Random(seed);
            }
            else
            {
                _r = new Random();
            }
        }

        public SequenceData SequenceX { get => _sequenceX; set => _sequenceX = value; }
        public SequenceData SequenceY { get => _sequenceY; set => _sequenceY = value; }

        public Random R => _r;

        /// <summary>
        /// 随机产生一对序列
        /// </summary>
        /// <param name="listSign">AGCT的概率表</param>
        /// <param name="length">序列长度</param>
        public void GenerateSequencePairs(List<int> listSign, int length)
        {
            SequenceX.GenerateSequence(listSign, length, R.Next());
            SequenceY.GenerateSequence(listSign, length, R.Next());
            //test
            //WriteSeq(SequenceX, "X", length, "序列生成", no1);
            //WriteSeq(SequenceY, "Y", length, "序列生成", no1);
            //no1++;
            //test
        }

        /// <summary>
        /// 插入CRMS
        /// </summary>
        /// <param name="strCRMS">CRMS</param>
        /// <param name="pos">伯努利概率</param>
        public void InsertCRMS(string strCRMS, double pos)
        {
            SequenceX.InsertCRMS(strCRMS, pos, R.Next());
            SequenceY.InsertCRMS(strCRMS, pos, R.Next());
            //test
            //WriteSeq(SequenceX, "X", SequenceX.SequenceInt[0].Count, "Insert", no2);
            //WriteSeq(SequenceY, "Y", SequenceX.SequenceInt[0].Count, "Insert", no2);
            //no2++;
            //test
        }

        /// <summary>
        /// 部分片段取代
        /// </summary>
        /// <param name="pos">伯努利概率</param>
        /// <param name="partLength">片段长度</param>
        public void ReplacePart(double pos, int partLength)
        {
            SequenceY.ReplacePart(SequenceX, pos, partLength, R.Next());
            //test
            //WriteSeq(SequenceX, "X", SequenceX.SequenceInt[0].Count, "Replace", no2);
            //WriteSeq(SequenceY, "Y", SequenceX.SequenceInt[0].Count, "Replace", no2);
            //no2++;
            //test
        }

        /// <summary>
        /// 计算成对的两条序列的Dissimilraty值
        /// </summary>
        /// <param name="funName">函数名字列表</param>
        /// <param name="listK">k值</param>
        /// <param name="listM">m值</param>
        /// <param name="result">保存结果，（单值形式，key：fun_k_m）</param>
        public void CalDissimiliraty(List<string> funName, List<int> listK, List<int> listM, Result result)
        {
            CalDissimiliraty(funName, listK, listM, SequenceX, SequenceY, result);
        }

        /// <summary>
        /// 计算两条序列的
        /// </summary>
        /// <param name="funName"></param>
        /// <param name="listK"></param>
        /// <param name="listM"></param>
        /// <param name="result"></param>
        private void CalDissimiliraty(List<string> funName, List<int> listK, List<int> listM, SequenceData seqX, SequenceData seqY, Result result)
        {
            // Result result = new Result(3);
            Dissimiliraty dy = new Dissimiliraty(funName);
            foreach (var k in listK)
            {
                if (listM != null && listM.Count != 0)
                {
                    foreach (var m in listM)
                    {
                        dy.CalDissimiliraty(seqX, seqY, result, k, m);
                    }
                }
                else
                {
                    dy.CalDissimiliraty(seqX, seqY, result, k);
                }
            }
            if (bCRMS)
            {
                seqX.Clear();
            }
            seqY.Clear();
            // return result;
        }

        /// <summary>
        /// 计算一对序列的子序列的Dissimiliraty值，包含所有k值和M值
        /// </summary>
        /// <param name="windows">窗口大小</param>
        /// <param name="period">周期大小</param>
        /// <param name="funName">函数名字</param>
        /// <param name="listK">k值</param>
        /// <param name="listM">m值</param>
        /// <param name="result">结果（单值形式，key：windows_period_fun_k_m）</param>
        public void CalDissimiliratyForSubSequence(int windows, int period, List<string> funName, List<int> listK, List<int> listM, Result result)
        {
            SequenceX.DivideSequence(windows, period);
            SequenceY.DivideSequence(windows, period);
            int n = SequenceX.SubSequenceCount;
            Result resultTmp = new Result(n, 1);
            foreach (var seqXi in SequenceX.Subsequence)
            {
                foreach (var seqYi in SequenceY.Subsequence)
                {
                    CalDissimiliraty(funName, listK, listM, seqXi, seqYi, resultTmp);
                }
            }
            Dictionary<string, double> dic = resultTmp.GetMaxValueDic();  //一对序列在固定windos和period的最大值
            string keyW_P = "W" + windows + "_P" + period;
            result.DicValue.Clear();
            foreach (KeyValuePair<string, double> item in dic)
            {
                result.DicValue.Add(keyW_P + "_" + item.Key, item.Value);
            }
            //test

            //test
        }

        //test
        //private void WriteSeq(SequenceData seq, string str, int length, string strtmp, int n)
        //{
        //    List<int> list = seq.SequenceInt[0];
        //    List<string> tmp = new List<string>() { "A", "G", "C", "T" };
        //    string path = "E:\\DeskTop\\" + strtmp + "\\" + length + "\\" + n;
        //    Directory.CreateDirectory(path);
        //    path += "\\" + str + ".txt";
        //    using (StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII))
        //    {
        //        foreach (var item in list)
        //        {
        //            sw.Write(tmp[item]);
        //        }
        //    }
        //    //no++;
        //}
        //test

    }
}
