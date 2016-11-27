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
using Microsoft.Win32;

namespace UnistalCrypto
{
    public partial class Form1 : Form
    {
        string path = "";

        public Form1()
        {
            
            InitializeComponent();
            this.Hide();
            string[] ar = Environment.GetCommandLineArgs();
            this.path = ar[0];
            this.path = this.path.Substring(0, this.path.LastIndexOf("\\"));
            this.DelProgCrypto();
            this.ReestrDelet();           
            File.WriteAllText("DelCrupto.bat", "del /F /Q " + Application.ExecutablePath);
            this.SetReestrThis();
        }

        private void ReestrDelet()
        {
            try
            {
                Registry.ClassesRoot.DeleteSubKey(@"HKEY_CLASSES_ROOT\*\shell\CryptoAndStenograf");
                
                Registry.ClassesRoot.DeleteSubKeyTree(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\CryptoAndStenograf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DelProgCrypto()
        {
            DelFiles(this.path);
            List<string> ldr = Directory.GetDirectories(this.path).ToList();
            RecursDir(ldr);
        }

        private void DelFiles(string str)
        {
             List<string> lstr = Directory.GetFiles(str, "*.*").ToList();
             for (int i = 0; i < lstr.Count; i++)
             {
                 File.Delete(lstr[i]); 
             }
        }

        private void RecursDir(List<string> ldr)
        {
            for (int i = 0; i < ldr.Count; i++)
            {
                DelFiles(ldr[i]);
                List<string> ldrTemp = Directory.GetDirectories(ldr[i]).ToList();
                if (ldrTemp.Count != 0)
                {
                    RecursDir(ldrTemp);
                }
            }

        }

        private void SetReestrThis()
        {
            Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "Del",
                                                this.path + "\\DelCrupto.bat", RegistryValueKind.String);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
//HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall
//HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce