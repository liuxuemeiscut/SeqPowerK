using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CalDistance
{
    class Result
    {
        /// <summary>
        /// key:Fun_k_m  value:matrix
        /// </summary>
        private Dictionary<string, double[,]> _dicResult = new Dictionary<string, double[,]>();

        private Dictionary<string, List<double>> _dicResultVector = new Dictionary<string, List<double>>();

        private readonly int _size = 1024;

        private readonly int _mode = 0;

        private Dictionary<string, double> _dicValue = new Dictionary<string, double>();
        public Dictionary<string, double[,]> DicResult { get => _dicResult; set => _dicResult = value; }
        public Dictionary<string, List<double>> DicResultVector { get => _dicResultVector; set => _dicResultVector = value; }
        public Dictionary<string, double> DicValue { get => _dicValue; set => _dicValue = value; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="seqCount">序列个数</param>
        /// <param name="mode">0：为对称矩阵
        /// 1：为普通矩阵
        /// 2：向量形式
        /// 3：单值形式</param>
        public Result(int seqCount, int mode = 0)
        {
            _size = seqCount;
            _mode = mode;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="mode">0：为对称矩阵
        /// 1：为普通矩阵
        /// 2：向量形式
        /// 3：单值形式</param>
        public Result(int mode)
        {
            _mode = mode;
        }

        public double[,] GetMatrix(string key)
        {
            return DicResult[key];
        }

        public double GetDissimilratyValue(string key)
        {
            return DicValue[key];
        }

        public void AddNewMatrix(string key)
        {
            if (DicResult.ContainsKey(key))
            {
                return;
            }
            DicResult.Add(key, new double[_size, _size]);
        }

        public void AddNewVector(string key)
        {
            if (DicResultVector.ContainsKey(key))
            {
                return;
            }
            DicResultVector.Add(key, new List<double>());
        }

        public void WriteResult(List<string> listSeqName, string savePath, string fun, int k, int m = -1)
        {
            string key = fun + "_k" + k;
            if (fun == "D2S" || fun == "D2Star")
            {
                key += "_M" + m;
            }
            if (DicResult.ContainsKey(key))
            {
                double[,] matrix = DicResult[key];
                string path = savePath + "\\" + fun;
                Directory.CreateDirectory(path);
                path = path + "\\" + key + ".txt";
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.ASCII))
                {
                    for (int i = 0; i < listSeqName.Count; i++)
                    {
                        sw.Write("\t" + listSeqName[i]);
                    }
                    sw.WriteLine();
                    for (int i = 0; i < listSeqName.Count; i++)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(listSeqName[i]);
                        for (int j = 0; j < listSeqName.Count; j++)
                        {
                            sb.Append("\t" + matrix[i, j]);
                        }
                        sw.WriteLine(sb.ToString());
                    }
                }
            }
        }

        public void SaveResult(string key, int i, int j, double value)
        {
            if (_mode == 0 || _mode == 1)
            {
                if (!DicResult.ContainsKey(key))
                {
                    AddNewMatrix(key);
                }
                DicResult[key][i, j] = value;
                if (_mode == 0)
                {
                    DicResult[key][j, i] = value;
                }
            }
            else if (_mode == 2)
            {
                if (!DicResultVector.ContainsKey(key))
                {
                    AddNewVector(key);
                }
                else
                {
                    DicResultVector[key].Add(value);
                }
            }
            else
            {
                DicValue.Add(key, value);
            }
        }

        public Dictionary<string, double> GetMaxValueDic()
        {
            foreach (KeyValuePair<string, double[,]> item in DicResult)
            {
                double max = 0;
                foreach (var value in item.Value)
                {
                    if (max < value)
                    {
                        max = value;
                    }
                }
                DicValue.Add(item.Key, max);
            }
            DicResult = null;
            return DicValue;
        }


        public bool ContiansKeys(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (DicResult.ContainsKey(key) || DicResultVector.ContainsKey(key) || DicValue.ContainsKey(key))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
