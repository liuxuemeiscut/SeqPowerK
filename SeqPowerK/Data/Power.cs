using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text;

namespace CalDistance
{
    class Power
    {
        /// <summary>
        /// 用于记录k值
        /// </summary>
        private readonly List<int> _listK = new List<int>();
        /// <summary>
        /// 用于记录马尔科夫阶数
        /// </summary>
        private readonly List<int> _listM = new List<int>();
        /// <summary>
        /// 用于记录对比的方法名
        /// </summary>
        private readonly List<string> _listFun = new List<string>();
        /// <summary>
        /// 用于记录伯努利概率
        /// </summary>
        private readonly double _pos = 4;
        /// <summary>
        /// 用于记录AGCT出现的概率
        /// </summary>
        private readonly List<int> _listSign = new List<int>();
        /// <summary>
        /// 用于记录要生成序列的长度
        /// </summary>
        private readonly List<int> _listSeqLength;
        /// <summary>
        /// 用于记录循环次数或执行次数
        /// </summary>
        private readonly int _circle;
        /// <summary>
        /// 随机变量产生器
        /// </summary>
        private readonly Random _r = new Random();
        /// <summary>
        /// 用于记录CRMS
        /// </summary>
        private readonly string _CRMS;

        public List<int> ListK => _listK;

        public List<int> ListM => _listM;

        public List<string> ListFun => _listFun;

        public double Pos => _pos;

        public List<int> ListSign => _listSign;

        public List<int> ListSeqLength => _listSeqLength;

        public int Circle => _circle;

        public Random R => _r;

        public string CRMS => _CRMS;

        public object Direectory { get; private set; }


        // private Dictionary<int, Dictionary<string, double>> _dicPower = new Dictionary<int, Dictionary<string, double>>();


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seed">随机数种子</param>
        /// <param name="circle">循环次数</param>
        /// <param name="listSeqLength">序列长度链表</param>
        /// <param name="listK">k值链表</param>
        /// <param name="listM">m值链表</param>
        /// <param name="listFun">方法链表</param>
        /// <param name="dicPos">AGCT概率</param>
        /// <param name="pos">伯努利概率</param>
        /// <param name="CRMS">CRMS片段</param>
        public Power(int seed, int circle, List<int> listSeqLength, List<int> listK, List<int> listM, List<string> listFun, Dictionary<char, double> dicPos, double pos, string CRMS = null)
        {
            _listK = listK;
            _listM = listM;
            _listFun = listFun;
            _pos = pos;
            _listSeqLength = listSeqLength;
            _r = new Random(seed);
            _circle = circle;
            _CRMS = CRMS;
            //通过AGCT的数量来衡量概率
            Action<int, int> add = (symbol, count) =>
             {
                 for (int i = 0; i < count; i++)
                 {
                     _listSign.Add(symbol);
                 }
             };
            foreach (KeyValuePair<char, double> item in dicPos)
            {
                int n = (int)(2520 * item.Value);
                int s = SequenceData.DicIntAGCT[item.Key];
                add(s, n);
            }
        }



        /// <summary>
        /// 计算power值
        /// </summary>
        /// <param name="partLength">替换片段的长度（进入替换模式），若为0，则进入CRMS模式</param>
        /// <returns></returns>
        public Dictionary<string, double> CalPower(Action<int> act, string filePath, int partLength = 0)
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            foreach (var length in ListSeqLength)
            {
                CalPowerForOneLength(length, dic, filePath, partLength);
                act(length);
            }
            return dic;
        }

        /// <summary>
        /// 计算某一序列长度的power值
        /// </summary>
        /// <param name="length">序列长度</param>
        /// <param name="dic">用来保存结果
        /// key:fun_k_M_L  value:power</param>
        /// <param name="partLength">替换片段的长度（进入替换模式），若为0，则进入CRMS模式</param>
        private void CalPowerForOneLength(int length, Dictionary<string, double> dic, string filePath, int partLength = 0)
        {
            List<int> listSeed = new List<int>();
            List<Result> listResultBg = new List<Result>();
            List<Result> listResultFg = new List<Result>();
            for (int i = 0; i < Circle; i++)
            {
                listSeed.Add(R.Next());
                listResultBg.Add(new Result(3));
                listResultFg.Add(new Result(3));
            }

            Parallel.For(0, Circle, new ParallelOptions() { MaxDegreeOfParallelism = Operation._cpuCount }, (i) =>
            {
                SequencePairs sp = new SequencePairs(listSeed[i]);
                sp.GenerateSequencePairs(ListSign, length);

                sp.CalDissimiliraty(ListFun, ListK, ListM, listResultBg[i]);
                if (partLength == 0)
                {
                    SequencePairs.bCRMS = true;
                    sp.InsertCRMS(CRMS, Pos);
                }
                else
                {
                    SequencePairs.bCRMS = false;
                    sp.ReplacePart(Pos, partLength);
                }
                sp.CalDissimiliraty(ListFun, ListK, ListM, listResultFg[i]);
                //Interlocked.Increment(ref Operation._current);
            });
            //for (int i = 0; i < Circle; i++)
            //{
            //    SequencePairs sp = new SequencePairs(listSeed[i]);
            //    sp.GenerateSequencePairs(ListSign, length);

            //    sp.CalDissimiliraty(ListFun, ListK, ListM, listResultBg[i]);
            //    if (partLength == 0)
            //    {
            //        sp.InsertCRMS(CRMS, Pos);
            //    }
            //    else
            //    {
            //        sp.ReplacePart(Pos, partLength);
            //    }
            //    sp.CalDissimiliraty(ListFun, ListK, ListM, listResultFg[i]);
            //    Interlocked.Increment(ref Operation._current);
            //}
            CalPowerValue(listResultBg, listResultFg, length, filePath, dic);
        }

        /// <summary>
        /// 计算某一长度的序列的power值
        /// </summary>
        /// <param name="length">序列长度</param>
        /// <param name="listWindows">窗口链表</param>
        /// <param name="listPeriod">周期链表</param>
        /// <param name="dic">用来记录对比结果
        /// key：windows_period_fun_k_m_L  value:power</param>
        /// <param name="partLength">替换片段的长度（进入替换模式），若为0，则进入CRMS模式</param>
        private void CalPowerForOneLength(int length, List<int> listWindows, List<int> listPeriod, Dictionary<string, double> dic, string filePath, int partLength = 0)
        {
            List<int> listSeed = new List<int>();
            List<Result> listResultBg = new List<Result>();
            List<Result> listResultFg = new List<Result>();
            for (int i = 0; i < Circle; i++)
            {
                listSeed.Add(R.Next());
                listResultBg.Add(new Result(3));
                listResultFg.Add(new Result(3));
            }
            foreach (var windows in listWindows)
            {
                foreach (var period in listPeriod)
                {
                    //for (int i = 0; i < Circle; i++)
                    //{
                    Parallel.For(0, Circle, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (i) =>
                    {
                        SequencePairs sp = new SequencePairs(listSeed[i]);
                        sp.GenerateSequencePairs(ListSign, length);
                        sp.CalDissimiliratyForSubSequence(windows, period, ListFun, ListK, ListM, listResultBg[i]);
                        if (partLength == 0)
                        {
                            SequencePairs.bCRMS = true;
                            sp.InsertCRMS(CRMS, Pos);
                    
                        }
                        else
                        {
                            SequencePairs.bCRMS = false;
                            sp.ReplacePart(Pos, partLength);
                        }
                        sp.CalDissimiliratyForSubSequence(windows, period, ListFun, ListK, ListM, listResultFg[i]);
                        //Interlocked.Increment(ref Operation._current);
                    });

                    //}
                }
            }
            CalPowerValue(listResultBg, listResultFg, length, filePath, dic);
        }

        /// <summary>
        /// 计算某一序列长度的power值
        /// </summary>
        /// <param name="listResultBg">背景序列的数据</param>
        /// <param name="listResultFg">前景序列的数据</param>
        /// <param name="length">序列长度</param>
        /// <returns>key:fun_k_M_L  value:power</returns>
        private void CalPowerValue(List<Result> listResultBg, List<Result> listResultFg, int length, string filePath, Dictionary<string, double> dicPower)
        {
            Func<List<Result>, Dictionary<string, List<double>>> func = (listResult) =>
            {
                Dictionary<string, List<double>> dic = new Dictionary<string, List<double>>();
                for (int i = 0; i < listResult.Count; i++)
                {
                    Dictionary<string, double> dicTmp = listResult[i].DicValue;
                    foreach (KeyValuePair<string, double> item in dicTmp)
                    {
                        if (!dic.ContainsKey(item.Key))
                        {
                            dic.Add(item.Key, new List<double>());
                        }
                        dic[item.Key].Add(item.Value);
                    }
                }
                return dic;
            };
            Dictionary<string, List<double>> dicBg = func(listResultBg);
            Dictionary<string, List<double>> dicFg = func(listResultFg);

            //test
            //WriteResult(length, dicBg, "Bg");
            //WriteResult(length, dicFg, "Fg");
            //test

            int postition = Circle * 95 / 100 - 1; //D 和 d

            foreach (KeyValuePair<string, List<double>> item in dicBg)
            {
                item.Value.Sort();
                double flag = item.Value[postition];
                double i = 0;
                foreach (var data in dicFg[item.Key])
                {
                    if (data >= flag)  //D 和 d
                    {
                        i++;
                    }
                }
                //dicPower.Add(item.Key + "_L" + length, i / dicFg[item.Key].Count);
                WriteResult(item.Key + "_L" + length + "\t" + (i / dicFg[item.Key].Count), filePath);
            }
        }

        /// <summary>
        /// 计算power值
        /// </summary>
        /// <param name="listWindows">窗口链表</param>
        /// <param name="listPeriod">周期链表</param>
        /// <param name="partLength">替换片段的长度（进入替换模式），若为0，则进入CRMS模式</param>
        /// <returns>key：windows_period_fun_k_m_L  value:power</returns>
        public Dictionary<string, double> CalPower(List<int> listWindows, List<int> listPeriod, Action<int> act, string filePath, int partLength = 0)
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            foreach (var length in ListSeqLength)
            {
                CalPowerForOneLength(length, listWindows, listPeriod, dic, filePath, partLength);
                act(length);
            }
            return dic;
        }

        private void WriteResult(string str, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath, true, Encoding.ASCII))
            {
                sw.WriteLine(str);
            }
        }
        //test
        //private void WriteResult(int length, Dictionary<string, List<double>> dic, string str)
        //{
        //    string path = "E:\\DeskTop\\power\\" + length;
        //    Directory.CreateDirectory(path);
        //    using (StreamWriter sw = new StreamWriter(path + "\\" + str + ".txt", false, Encoding.ASCII))
        //    {
        //        foreach (KeyValuePair<string, List<double>> item in dic)
        //        {
        //            sw.Write(item.Key);
        //            foreach (var value in item.Value)
        //            {
        //                sw.Write("\t" + value);
        //            }
        //            sw.WriteLine();
        //        }
        //    }

        //}
        //test
    }
}
