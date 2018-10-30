using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CalDistance
{
    class Operation
    {
        private List<TreeNode> _listNode = new List<TreeNode>();

        private List<int> _listK = new List<int>();

        private List<int> _listM = new List<int>();

        private List<string> _listFun = new List<string>();

        private List<int> _listWindows = new List<int>();

        private List<int> _listPeriod = new List<int>();

        private Dictionary<char, double> _dicPos = new Dictionary<char, double>();

        private List<int> _listSeqLength = new List<int>();

        private double _insertPos;

        private string _CRMS = null;

        private int _partLength = 0;

        public static int _current = 0;

        private Action<int> _actShowState;

        public static int _cpuCount = 2;


        public List<TreeNode> ListNode { get => _listNode; set => _listNode = value; }
        public List<int> ListK { get => _listK; set => _listK = value; }
        public List<int> ListM { get => _listM; set => _listM = value; }
        public List<string> ListFun { get => _listFun; set => _listFun = value; }
        public List<int> ListWindows { get => _listWindows; set => _listWindows = value; }
        public List<int> ListPeriod { get => _listPeriod; set => _listPeriod = value; }
        public Dictionary<char, double> DicPos { get => _dicPos; set => _dicPos = value; }
        public List<int> ListSeqLength { get => _listSeqLength; set => _listSeqLength = value; }
        public double InsertPos { get => _insertPos; set => _insertPos = value; }
        public string CRMS { get => _CRMS; set => _CRMS = value; }
        public int PartLength { get => _partLength; set => _partLength = value; }
        public Action<int> ActShowState { get => _actShowState; set => _actShowState = value; }





        /// <summary>
        /// 左侧下拉框改变时，右侧的值跟着变动，以确保输入值合法
        /// </summary>
        /// <param name="cbx1">左侧下拉框</param>
        /// <param name="cbx2">右侧下拉框</param>
        public void ComboBoxSetting(ComboBox cbx1, ComboBox cbx2)
        {
            int tmp1 = int.Parse(cbx1.SelectedItem.ToString());
            int tmp2 = int.Parse(cbx1.Items[cbx1.Items.Count - 1].ToString());
            cbx2.Items.Clear();
            for (int i = tmp1; i <= tmp2; i++)
            {
                cbx2.Items.Add(i);
            }
            cbx2.SelectedIndex = 0;
        }

        /// <summary>
        /// 向树添加节点，节点的Tag记录绝对路径，通过递归遍历添加
        /// </summary>
        /// <param name="nodes">节点集合</param>
        /// <param name="path">导入路径</param>
        public void TreeViewAddNode(TreeNodeCollection nodes, params string[] path)
        {
            foreach (var item in path)
            {
                TreeNode node = new TreeNode(Path.GetFileName(item));
                node.Tag = item;
                nodes.Add(node);
                if (!File.Exists(item))
                {
                    string[] files = Directory.GetFiles(item);
                    TreeViewAddNode(node.Nodes, files);
                    string[] dirs = Directory.GetDirectories(item);
                    TreeViewAddNode(node.Nodes, dirs);
                }
            }
            return;
        }

        /// <summary>
        /// 从下拉框中获取数值
        /// </summary>
        /// <param name="cbx1">左侧下拉框</param>
        /// <param name="cbx2">右侧下拉框</param>
        /// <param name="list">用于记录数值</param>
        public void GetValueFromComboBox(ComboBox cbx1, ComboBox cbx2, List<int> list)
        {
            int tmp1 = int.Parse(cbx1.SelectedItem.ToString());
            int tmp2 = int.Parse(cbx2.SelectedItem.ToString());
            for (int i = tmp1; i <= tmp2; i++)
            {
                list.Add(i);
            }
        }

        /// <summary>
        /// 递归获取节点
        /// </summary>
        /// <param name="nodes"></param>
        private void GetListNode(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                string path = (string)node.Tag;
                if (!File.Exists(path))
                {
                    GetListNode(node.Nodes);
                }
                else
                {
                    if (!ListNode.Contains(node.Parent))
                        ListNode.Add(node.Parent);
                }
            }
            return;
        }

        public void InitalState(ProgressBar pgb)
        {
            pgb.Maximum = ListSeqLength.Count;
            pgb.Value = 0;
        }

        public void Star(string saveDir, bool bDivide, int seed, int circle)
        {
            Dictionary<string, double> dicPower = new Dictionary<string, double>();
            Power power = new Power(seed, circle, ListSeqLength, ListK, ListM, ListFun, DicPos, InsertPos, CRMS);
            if (bDivide)
            {
                Directory.CreateDirectory(saveDir);
                string filePath = saveDir + "\\Power.txt";
                dicPower = power.CalPower(ListWindows, ListPeriod, ActShowState, filePath, PartLength);
            }
            else
            {
                Directory.CreateDirectory(saveDir);
                string filePath = saveDir + "\\Power.txt";
                dicPower = power.CalPower(ActShowState, filePath, PartLength);
            }

            //using (StreamWriter sw = new StreamWriter(saveDir + "\\Power.txt", false, Encoding.ASCII))
            //{
            //    foreach (KeyValuePair<string, double> item in dicPower)
            //    {
            //        sw.WriteLine(item.Key + "\t" + item.Value);
            //    }
            //}
        }




        public bool GetStepListInt(TextBox tbxMin, TextBox tbxStep, TextBox tbxMax, List<int> list)
        {
            int min = 0;
            if (!int.TryParse(tbxMin.Text, out min))
            {
                return false;
            }
            int step = 0;
            if (!int.TryParse(tbxStep.Text, out step))
            {
                return false;
            }
            int max = 0;
            if (!int.TryParse(tbxMax.Text, out max))
            {
                return false;
            }
            for (int i = min; i <= max; i += step)
            {
                list.Add(i);
                if (step == 0)
                    break;
            }
            return true;
        }

        public bool GetPossibilityFromComboBox(params ComboBox[] cbxs)
        {
            for (int i = 0; i < cbxs.Length; i++)
            {
                ComboBox cbxD = (ComboBox)cbxs[i].Tag;
                try
                {
                    double a = double.Parse(cbxs[i].SelectedItem.ToString());
                    double b = double.Parse(cbxD.SelectedItem.ToString());
                    string s = (string)cbxD.Tag;
                    _dicPos.Add(s[0], a / b);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }
}
