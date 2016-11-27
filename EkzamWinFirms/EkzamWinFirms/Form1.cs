using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


using System.IO;

namespace EkzamWinFirms
{
    public partial class Form1 : Form
    {
        private Action Act = null;
        Dictionary<string, Coder.ICoder> dCoder = new Dictionary<string, Coder.ICoder>();
        Crypto.ICrypto cf = null;
        MemoryStream msSave = null;
        MemoryStream ms = null;

        public Form1()
        {
            InitializeComponent();
            this.dCoderAdd();
            this.radioButton3_CheckedChanged(this, null);
            string[] ar = Environment.GetCommandLineArgs();
            if(ar.Length==2)
            {
                this.textBox1.Text = ar[1];
            }
        }

       

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Coder.ICoder ic = null;
                if (this.checkBox2.Checked)
                {
                    this.dCoder.TryGetValue(this.comboBox1.SelectedItem.ToString(), out ic);
                }
               
                if (this.radioButton3.Checked)
                {
                   this.cf = new Crypto.CryptoFile(this.textBox1.Text, this.textBox4.Text, 
                                                   ic,this.textBox2.Text, new Action<int, string>(ProgBar), 
                                                   this.checkBox1.Checked);
                }
                else if (this.radioButton4.Checked)
                {
                    this.msSave = new MemoryStream();
                    this.ms = new MemoryStream(Encoding.Unicode.GetBytes(this.textBox3.Text));
                    this.cf = new Crypto.CryptoSt(ms, this.textBox4.Text, ic, new Action<byte[]>(this.SaveStream),
                                                  new Action<int, string>(ProgBar));
                }
                else if (this.radioButton1.Checked)
                {
                    if (this.radioButton7.Checked)
                    {
                        this.msSave = new MemoryStream();
                        this.cf = new Crypto.CryptoStenografImage(this.textBox1.Text, this.textBox2.Text, this.checkBox1.Checked,this.checkBox3.Checked,
                                                                  Encoding.Unicode.GetBytes(this.textBox3.Text), new Action<int, 
                                                                  string>(ProgBar), this.checkBox2.Checked, this.textBox4.Text,  
                                                                  ic, new Action<byte[]>(this.SaveStream));
                    }
                    else if(this.radioButton6.Checked)
                    {
                        this.cf = new Crypto.CryptoStenografImage(this.textBox1.Text, this.textBox2.Text, this.checkBox1.Checked,this.checkBox3.Checked,
                                                                  this.textBox5.Text, new Action<int, string>(ProgBar), 
                                                                  this.checkBox2.Checked, this.textBox4.Text, ic);
                    }
                }

                this.toolStripProgressBar1.Visible = true;

                if (this.radioButton5.Checked)
                {
                    this.Act = this.cf.Crypt;
                }
                else if (this.radioButton2.Checked)
                {
                    this.Act = this.cf.Decrypt;
                }
                this.Act.BeginInvoke(Finish, this.Act);
            }
            catch (Exception mex)
            {
                MessageBox.Show(string.Format("Erorr :{0}", mex.Message));
            }

        }
       
        private void dCoderAdd()
        {
           
            this.dCoder.Clear();
            this.comboBox1.Items.Clear();
            this.comboBox1.Text = "Выберите алгоритм шифрования";
            this.dCoder.Add("Xor", new Coder.CoderXor());
            this.dCoder.Add("RC4", new Coder.CoderRC4());

            if (this.radioButton4.Checked)
            {
                this.dCoder.Add("Cezer", new Coder.CoderCezer());
            }
            this.comboBox1.Items.AddRange(this.dCoder.Keys.ToArray());
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            this.dCoderAdd();
            this.groupBox2.Enabled = false;
            this.groupBox1.Enabled = true;
            this.checkBox2.Checked = true;
            this.checkBox2.Enabled = false;
            this.checkBox1.Enabled = true;
            this.checkBox3.Enabled = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            this.dCoderAdd();
            this.groupBox1.Enabled = false;
            this.groupBox2.Enabled = true;
            this.radioButton6.Enabled = false;
            this.radioButton7.Checked = true;
            this.textBox5.Enabled = false;
            this.checkBox2.Checked = true;
            this.checkBox2.Enabled = false;
            this.checkBox1.Enabled = false;
            this.checkBox3.Enabled = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            this.dCoderAdd();
            this.groupBox1.Enabled = true;
            this.groupBox2.Enabled = true;
            this.radioButton6.Enabled = true;
            this.radioButton7.Checked = true;
            this.textBox5.Enabled = true;
            this.label4.Enabled = true;
            this.label4.Enabled = false;
            this.textBox5.Enabled = false;
            this.checkBox2.Checked = false;
            this.checkBox2.Enabled = true;
            this.checkBox1.Enabled = true;
            this.checkBox3.Enabled = true;
            this.groupBox4.Enabled = false;
            if(this.radioButton5.Checked)
                this.checkBox3.Enabled = false;
        }

        private bool ProvDataCoder()
        {
            if (this.comboBox1.SelectedIndex != -1)
            {
             return true;
            }
            else
                throw new Exception("Error not enter Coder ");
        }

        private bool ProvDataStenograf()
        {
            if (this.comboBox1.SelectedIndex != -1)
            {

                if (this.radioButton7.Checked == true || !File.Exists(this.textBox2.Text))
                {
                    return true;
                }
                else
                    throw new Exception("Error file to save already");
            }
            else
                throw new Exception("Error not enter Coder ");

        }
        private void Finish(IAsyncResult iar)
        {
            try
            {
                Action act = (Action)iar.AsyncState;
                act.EndInvoke(iar);
                Action fin = new Action(this.Messeg);
                this.Invoke(fin);
            }
            catch (Exception mex)
            {
                MessageBox.Show(string.Format("Erorr :{0}", mex.Message));
            }
           finally
            {
                Action fact = new Action(this.DisposeData);
                this.Invoke(fact);
                Action gText = new Action(this.GetText);
                this.Invoke(gText);
            }
        }

        private void ProgBar(int i, string str)
        {
            Action<int, string> act = (x, y) =>
            {
                this.toolStripProgressBar1.Value = x;
                this.toolStripStatusLabel1.Text = y;
            };
            this.Invoke(act, i, str);
            i++;
            if (i == 101)
            {
                i = 0;
            }
        }

        private void Messeg()
        {
            MessageBox.Show("Its OK !!!");           
        }

        private void SaveStream(byte[] buf)
        {
            this.msSave.Write(buf, 0, buf.Length);
        }

        private void GetText()
        {
            if (this.msSave != null)
            {
                byte[] b = this.msSave.GetBuffer();
                Array.Resize<byte>(ref b, (int)this.msSave.Length);
                this.textBox3.Text = Encoding.Unicode.GetString(this.msSave.GetBuffer());
                this.msSave.Close();
            }
        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = this.openFileDialog1.FileName;
            }
        }

        private void textBox5_MouseClick(object sender, MouseEventArgs e)
        {
            this.textBox4.Text = "";
        }

        private void textBox5_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBox5.Text = this.openFileDialog1.FileName;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            this.label4.Enabled = false;
            this.textBox5.Enabled = false;
            this.textBox3.Enabled = true;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            this.label4.Enabled = true;

            this.textBox5.Enabled = this.radioButton5.Checked;
            this.textBox3.Enabled = false;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
          this.groupBox4.Enabled = this.checkBox2.Checked;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if(this.radioButton1.Checked && this.radioButton6.Checked)
            {
                this.textBox5.Enabled = false;
            }
            this.checkBox3.Enabled = true;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton1.Checked && this.radioButton6.Checked)
            {
                this.textBox5.Enabled = true;
            }
            this.checkBox3.Enabled = false;
        }

        private void openFileCryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.textBox1_MouseDoubleClick(sender,null);
        }

        private void openFileOrSttenografToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(this.radioButton1.Checked&& this.radioButton5.Checked&& this.radioButton6.Checked)
               this.textBox5_MouseDoubleClick(sender, null);
        }

        private void fileCryptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.radioButton3_CheckedChanged(sender, e);
        }

        private void textCryptoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.radioButton4_CheckedChanged(sender, e);
        }

        private void stenografToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.radioButton1_CheckedChanged(sender, e);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void textToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.stenografToolStripMenuItem_Click(sender, e);
            this.radioButton7_CheckedChanged(sender, e);
        }

        private void fileToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.stenografToolStripMenuItem_Click(sender, e);
            this.radioButton6_CheckedChanged(sender, e);
        }

        private void hideTheProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = true;
            this.Hide();
            this.notifyIcon1.ShowBalloonTip(3000);
        }

        private void openProgramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;
            this.Show();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.notifyIcon1.Visible = false;
            this.Show();
        }

        private void DisposeData()
        {
            this.toolStripStatusLabel1.Text = "";
            this.toolStripProgressBar1.Visible = false;
            this.textBox1.Text = "";
            this.textBox5.Text = "";
            this.textBox2.Text = "";
            this.checkBox1.Checked = this.checkBox3.Checked = false;
        }
    }
}
