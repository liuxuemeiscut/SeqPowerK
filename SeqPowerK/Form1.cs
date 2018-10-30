using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CalDistance
{
    public partial class FrmDistance : Form
    {
        public FrmDistance()
        {
            InitializeComponent();
        }

        Operation op;

        private void Form1_Load(object sender, EventArgs e)
        {
            cbxk1.Tag = cbxk2;
            cbxm1.Tag = cbxm2;
            cbxUpA.Tag = cbxDownA;
            cbxUpG.Tag = cbxDownG;
            cbxUpC.Tag = cbxDownC;
            cbxUpT.Tag = cbxDownT;
            op = new Operation();
            btnMode1.PerformClick();
            int N = Environment.ProcessorCount;
            for (int i = 1; i <= N; i++)
            {
                cbxCPU.Items.Add(i);
            }
            cbxCPU.SelectedIndex = N / 2 - 1;


            //test
            //Test();
            //txtCount.Text = "100";
            //txtSeed.Text = "50";
            //txtSave.Text = @"E:\Windows\DeskTop\3";
            //txtStepSeq.Text = "500";
            //txtPartLength.Text = "5";
            //txtMinSeq.Text = "1000";
            //txtMaxSeq.Text = "2000";
            //cbxk1.Text = "1";
            //cbxk2.Text = "6";
            //cbxm1.Text = "0";
            //cbxm2.Text = "1";
            //clbFun.SelectedIndex = 5;
            //test
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxk1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cbx1 = (ComboBox)sender;
            ComboBox cbx2 = (ComboBox)cbx1.Tag;
            op.ComboBoxSetting(cbx1, cbx2);
        }

        /// <summary>
        /// 拖拽相应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myDragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link; //重要代码：表明是链接类型的数据，比如文件路径
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// 拖拽响应事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myDragDrop(object sender, DragEventArgs e)
        {
            Array arr = ((Array)e.Data.GetData(DataFormats.FileDrop));
            string[] arrPath = new string[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arrPath[i] = arr.GetValue(i).ToString();
            }

            TextBox tbx = (TextBox)sender;

            if (arrPath.Length != 1)
            {
                MessageBox.Show("Only one path,", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tbx.Name == "txtSave")
            {
                if (!File.Exists(arrPath[0]))
                    tbx.Text = arrPath[0];
                else
                {
                    MessageBox.Show("Please input the directory", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (tbx.Name == "txtCRMS")
            {
                if (File.Exists(arrPath[0]))
                    tbx.Text = arrPath[0];
                else
                {
                    MessageBox.Show("Please input the CRMS file", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                if (File.Exists(arrPath[0]))
                    tbx.Text = arrPath[0];
                else
                {
                    MessageBox.Show("Please input the sequence file", "warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }//else
            }//else

        }

        /// <summary>
        /// 检查参数设置
        /// </summary>
        /// <returns></returns>
        private bool CheckSetting()
        {
            if (cbxk1.SelectedItem == null)
            {
                MessageBox.Show("k can not be none", "warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (clbFun.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select the way to calculate", "warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (lblM.Visible == true)
            {
                if (cbxm1.SelectedItem == null)
                {
                    MessageBox.Show("M can not be none", "warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            if (txtSave.Text == "")
            {
                MessageBox.Show("Saving path can not be none", "warning", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            //if(txtCount)
            GetParameter();
            return true;
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpern_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Name == "btnOpen")
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                fbd.ShowDialog();
                if (fbd.SelectedPath != null)
                {
                    txtSave.Text = fbd.SelectedPath;
                }
            }
            else if (btn.Name == "btnCRMS")
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.ShowDialog();
                if (ofd.FileName != null)
                {
                    txtSave.Text = ofd.FileName;
                }
            }
            else if (btn.Name == "btnMode1" || btn.Name == "btnMode2")
            {
                Action<ComboBox, ComboBox, int, int> act = (cbxU, cbxD, a, b) =>
                {
                    //cbxU.SelectedText = "";
                    //cbxD.SelectedText = "";
                    //cbxU.SelectedText = a.ToString();
                    //cbxD.SelectedText = b.ToString();
                    cbxU.SelectedIndex = a - 1;
                    cbxD.SelectedIndex = b - 1;

                };
                if (btn.Name == "btnMode1")
                {
                    act(cbxUpA, cbxDownA, 1, 4);
                    act(cbxUpG, cbxDownG, 1, 4);
                    act(cbxUpC, cbxDownC, 1, 4);
                    act(cbxUpT, cbxDownT, 1, 4);
                }
                else
                {
                    act(cbxUpA, cbxDownA, 1, 6);
                    act(cbxUpG, cbxDownG, 1, 3);
                    act(cbxUpC, cbxDownC, 1, 3);
                    act(cbxUpT, cbxDownT, 1, 6);
                }
            }
            else
            {
                if (!CheckSetting())
                {
                    op = new Operation();
                    return;
                }
                //op.Star(txtSave.Text, treLoad.Nodes, pgb, lblOneSeq.Visible, txtOneSeq.Text);
                Stopwatch sp = new Stopwatch();
                sp.Start();

                CalPower();

                op = new Operation();
                sp.Stop();
                MessageBox.Show("Finished!\r\nTime : " + sp.Elapsed);
            }
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        private void GetParameter()
        {
            //获取函数名
            foreach (var item in clbFun.CheckedItems)
            {
                op.ListFun.Add(item.ToString());
            }

            //获取k参数
            op.GetValueFromComboBox(cbxk1, cbxk2, op.ListK);

            //获取马尔科夫阶数
            if (lblM.Visible == true)
                op.GetValueFromComboBox(cbxm1, cbxm2, op.ListM);

            //获取序列长度
            op.GetStepListInt(txtMinSeq, txtStepSeq, txtMaxSeq, op.ListSeqLength);

            //获取窗口参数以及周期参数
            if (chbDivide.Checked)
            {
                op.GetStepListInt(txtMinWinows, txtWindowStep, txtMaxWindow, op.ListWindows);
                op.GetStepListInt(txtMinPeriod, txtPeriodStep, txtMaxPeriod, op.ListPeriod);
            }

            //获取A,G,C,T的概率参数
            op.GetPossibilityFromComboBox(cbxUpA, cbxUpG, cbxUpC, cbxUpT);

            //获取CRMS
            if (File.Exists(txtCRMS.Text))
            {
                op.CRMS = File.ReadAllLines(txtCRMS.Text)[0];
            }
            else
            {
                if (txtCRMS.Text != "")
                {
                    op.CRMS = txtCRMS.Text;
                }
            }

            //获取插入CRMS的概率
            op.InsertPos = double.Parse(txtPos.Text);

            //替换片段长度
            if (chbReplace.Checked)
            {
                op.PartLength = int.Parse(txtPartLength.Text);
            }
        }


        /// <summary>
        /// 判断是否存在D2S或者D2Star函数，存在则先是马尔科夫参数选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clbFun_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var item in clbFun.CheckedItems)
            {
                string str = item.ToString();
                if (str == "D2S" || str == "D2Star")
                {
                    lblM.Visible = true;
                    cbxm1.Visible = cbxm2.Visible = lblToM.Visible = true;
                }
                else
                {
                    lblM.Visible = false;
                    cbxm1.Visible = cbxm2.Visible = lblToM.Visible = false;
                }
            }
        }

        private void CalPower()
        {
            Operation._cpuCount = cbxCPU.SelectedIndex + 1;
            int seed = 0;
            if (txtSeed.Text != "")
            {
                seed = int.Parse(txtSeed.Text);
            }
            else
            {
                seed = new Random().Next();
            }
            int circle = int.Parse(txtCount.Text);

            string path = txtSave.Text;
            op.InitalState(pgb);
            if (chbReplace.Checked && chbInsert.Checked)
            {
                pgb.Maximum = pgb.Maximum << 1;
            }
            txtSate.Clear();
            op.ActShowState = ShowState;
            if (chbDivide.Checked)
            {
                path += "\\Divide";
            }
            if (chbReplace.Checked)
            {
                op.Star(path + "\\Replace", chbDivide.Checked, seed, circle);
            }
            if (chbInsert.Checked)
            {
                op.PartLength = 0;
                op.Star(path + "\\CRMS", chbDivide.Checked, seed, circle);
            }
        }

        private void chbDivide_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chb = (CheckBox)sender;
            if (chb.Name == "chbDivide")
            {
                panelWindows.Visible = chb.Checked;
            }
            else if (chb.Name == "chbInsert")
            {
                txtCRMS.Visible = lblCRMS.Visible = btnCRMS.Visible = chb.Checked;
            }
            else if (chb.Name == "chbReplace")
            {
                txtPartLength.Visible = lblPartLength.Visible = chb.Checked;
            }
        }

        private void ShowState(int length)
        {
            if (pgb.Value < pgb.Maximum)
                pgb.Value++;
            txtSate.Text += "Length : " + length + " Finished!+\r\n\r\n";
        }
    }
}
