using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CalDistance
{
    public class SequenceData
    {
        #region 私有字段

        /// <summary>
        /// key:k值   value:ktuple统计值数据
        /// </summary>
        private Dictionary<int, KtupleData> _dicKtuple = new Dictionary<int, KtupleData>();
        /// <summary>
        /// key:k值   value:markove概率值数据
        /// </summary>
        private Dictionary<int, Dictionary<int, MarkovData>> _dicMarkov = new Dictionary<int, Dictionary<int, MarkovData>>();
        /// <summary>
        /// key:k值    value:对应ktuple的统计值总和
        /// </summary>
        private Dictionary<int, int> _dicTotal = new Dictionary<int, int>();
        /// <summary>
        /// 序列的ID
        /// </summary>
        private int _id = 0;
        /// <summary>
        /// 序列整形格式
        /// </summary>
        private List<List<int>> _lsitSequenceInt = new List<List<int>>();
        /// <summary>
        /// 分割后的子序列
        /// </summary>
        private List<SequenceData> _listSubsequence;

        static public Dictionary<char, int> DicIntAGCT = new Dictionary<char, int>() { { 'A', 0 }, { 'G', 1 }, { 'C', 2 }, { 'T', 3 } };

        private Dictionary<int, double[]> _dicMarkovRtemp = new Dictionary<int, double[]>();
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath">序列文件的路径</param>
        public SequenceData(string filePath, int id)
        {
            GetSeqText(filePath);
            _id = id;
        }

        public SequenceData(List<int> list, int id = 0)
        {
            _lsitSequenceInt.Add(list);
        }

        public SequenceData()
        {

        }

        internal Dictionary<int, KtupleData> Ktuple { get => _dicKtuple; set => _dicKtuple = value; }
        internal Dictionary<int, Dictionary<int, MarkovData>> Markov { get => _dicMarkov; set => _dicMarkov = value; }
        public Dictionary<int, int> DicTotal { get => _dicTotal; set => _dicTotal = value; }
        public int ID { get => _id; set => _id = value; }

        internal Dictionary<int, double[]> MarkovRtemp { get => _dicMarkovRtemp; set => _dicMarkovRtemp = value; }
        public List<List<int>> SequenceInt { get => _lsitSequenceInt; set => _lsitSequenceInt = value; }
        public List<SequenceData> Subsequence { get => _listSubsequence; set => _listSubsequence = value; }

        public int SubSequenceCount { get => _listSubsequence.Count; }




        /// <summary>
        /// 获取序列文本
        /// </summary>
        /// <param name="filePath">序列文件的路径</param>
        private void GetSeqText(string filePath)
        {
            StringBuilder sb = new StringBuilder();
            List<string> listSeqText = new List<string>();
            //Dictionary<char, int> dic = new Dictionary<char, int>();
            //dic.Add('A', 0);
            //dic.Add('G', 1);
            //dic.Add('C', 2);
            //dic.Add('T', 3);
            using (StreamReader sr = new StreamReader(filePath, Encoding.ASCII))
            {
                string firstLine = sr.ReadLine();
                if (firstLine[0] == 'A' || firstLine[0] == 'G' || firstLine[0] == 'C' || firstLine[0] == 'T' || firstLine[0] == 'N')
                    sb.Append(firstLine);
                while (true)
                {
                    string tmp = sr.ReadLine();
                    if (tmp == null)
                        break;
                    sb.Append(tmp);
                }
            }
            listSeqText.AddRange(sb.ToString().Split(new char[] { 'N' }, StringSplitOptions.RemoveEmptyEntries));
            for (int i = 0; i < listSeqText.Count; i++)
            {
                SequenceInt.Add(new List<int>());
                foreach (var item in listSeqText[i])
                {
                    SequenceInt[i].Add(DicIntAGCT[item]);
                }
            }
        }

        /// <summary>
        /// 统计ktuple的个数
        /// </summary>
        /// <param name="k">ktuple的字长</param>
        public void CalKtupleCount(int k)
        {
            if (!Ktuple.ContainsKey(k))
            {
                Ktuple.Add(k, new KtupleData());
                int total = 0;
                total = Ktuple[k].CountKtuple(SequenceInt, k);
                DicTotal.Add(k, total);
            }
        }

        /// <summary>
        /// 计算Markov概率
        /// </summary>
        /// <param name="k">ktuple的字长</param>
        /// <param name="m">markov的阶数</param>
        public void CalMarkov(int k, int m)
        {
            if (!_dicMarkov.ContainsKey(k) || !_dicMarkov[k].ContainsKey(m))
            {
                MarkovData md = new MarkovData();
                md.CalMarkov(k, m, this);
                if (Markov.ContainsKey(k))
                {
                    Markov[k].Add(m, md);
                }
                else
                {
                    Markov.Add(k, new Dictionary<int, MarkovData>());
                    Markov[k].Add(m, md);
                }
            }
        }

        /// <summary>
        /// 获取ktuple的数据
        /// </summary>
        /// <param name="k">ktuple的字长</param>
        /// <returns></returns>
        public List<int> GetKtupleData(int k)
        {
            if (!_dicKtuple.ContainsKey(k))
            {
                CalKtupleCount(k);
            }
            return _dicKtuple[k].ListKtuple;
        }

        /// <summary>
        /// 获取ktuple总个数
        /// </summary>
        /// <param name="k">ktuple的字长</param>
        /// <returns></returns>
        public int GetTotal(int k)
        {
            if (!_dicTotal.ContainsKey(k))
            {
                CalKtupleCount(k);
            }
            return DicTotal[k];
        }

        /// <summary>
        /// 获取markov的数据
        /// </summary>
        /// <param name="k">ktuple的字长</param>
        /// <param name="m">markov的阶数</param>
        /// <returns></returns>
        public List<double> GetMarkovData(int k, int m)
        {
            if (!_dicMarkov.ContainsKey(k) || !_dicMarkov[k].ContainsKey(m))
            {
                CalMarkov(k, m);
            }
            return _dicMarkov[k][m].ListMarkov;
        }

        public bool ContainsMarkovKey(int k, int r)
        {
            if (_dicMarkov.ContainsKey(k) && _dicMarkov[k].ContainsKey(r))
            {
                return true;
            }
            return false;
        }
        public void Clear()
        {
            _dicKtuple.Clear();
            _dicMarkov.Clear();
            _dicTotal.Clear();
            _dicMarkovRtemp.Clear();
            // _lsitSequenceInt = null;
        }

        public void GenerateSequence(List<int> listSign, int length, int seed = -1)
        {
            List<int> listSeq = new List<int>();
            Random r;
            if (seed != -1)
            {
                r = new Random(seed);
            }
            else
            {
                r = new Random();
            }
            for (int i = 0; i < length; i++)
            {
                listSeq.Add(listSign[r.Next(listSign.Count)]);
            }
            SequenceInt.Add(listSeq);
        }

        public void DivideSequence(int windows, int period)
        {
            List<int> list = SequenceInt[0];
            int length = list.Count + period;
            int n = length / (windows + period);

            Subsequence = new List<SequenceData>();
            for (int i = 0; i < n; i++)
            {
                List<int> listTmp = new List<int>();
                int shift = i * (windows + period);
                for (int j = 0; j < windows; j++)
                {
                    listTmp.Add(list[j + shift]);
                }
                Subsequence.Add(new SequenceData(listTmp, i));
            }
        }

        public void InsertCRMS(string strCRMS, double pos, int seed = -1)
        {
            //CRMS转化为整形列表
            List<int> CRMS = new List<int>();
            foreach (var item in strCRMS)
            {
                CRMS.Add(DicIntAGCT[item]);
            }

            //初始化随机数生成器
            Random r;
            if (seed != -1)
            {
                r = new Random(seed);
            }
            else
            {
                r = new Random();
            }

            //概率标志
            int flag = (int)(pos * 100);

            //
            for (int i = 0; i < SequenceInt[0].Count - CRMS.Count;)
            {
                if (r.Next(0,100) > flag)
                {
                    i++;
                }

                else
                {
                    for (int j = 0; j < CRMS.Count; j++)
                    {
                        SequenceInt[0][i + j] = CRMS[j];
                    }
                    i += CRMS.Count;
                }
            }
        }

        public void ReplacePart(SequenceData sd, double pos, int partLength, int seed = -1)
        {
            //初始化随机数生成器
            Random r;
            if (seed != -1)
            {
                r = new Random(seed);
            }
            else
            {
                r = new Random();
            }
            List<int> listSeqSource = sd.SequenceInt[0];
            List<int> listSeqNew = SequenceInt[0];
            int flag = (int)(pos * 100);
            for (int i = 0; i < SequenceInt[0].Count - partLength;)
            {
                if (r.Next(0,100) < flag)
                {
                    for (int j = 0; j < partLength; j++)
                    {
                        listSeqNew[i + j] = listSeqSource[i + j];
                    }
                    i += partLength;
                }
                else
                {
                    i++;
                }
            }
        }

    }
}
