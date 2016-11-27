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

namespace ReestrEditCrypto
{
    public partial class Form1 : Form
    {
        List<string> Hkey = new List<string>();
        string path = "";
        public Form1()
        {
            this.AddListHKey();
            InitializeComponent();
            string[] ar = Environment.GetCommandLineArgs();
            this.path = ar[0];
            this.path=this.path.Substring(0,this.path.LastIndexOf("\\"));
            this.ReestrEditProg();
            this.ReestrEditProgDel();
            
        }

        private void ReestrEditProg()
        {
            try
            {
                for (int i = 0; i < this.Hkey.Count; i++)
                {
                    Registry.SetValue(this.Hkey[i] + "CryptoAndStenograf", "MUIVerb",
                                       "CryptoAndStenograf", RegistryValueKind.String);
                    Registry.SetValue(this.Hkey[i] + "CryptoAndStenograf", "Icon",
                                                   this.path + "\\box.ico", RegistryValueKind.String);
                    Registry.SetValue(this.Hkey[i] + "CryptoAndStenograf\\" + "command","",
                                                   this.path + "\\CryptoAndStenograf.exe", RegistryValueKind.String);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ReestrEditProgDel()
        {
            try
            {
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + "CryptoAndStenograf", "MUIVerb",
                                             this.path + "\\UnistalCrypto.exe", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + "CryptoAndStenograf", "Icon",
                                               this.path + "\\uninstall.ico", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + "CryptoAndStenograf", "",
                                               this.path + "\\UnistalCrypto.exe", RegistryValueKind.String);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddListHKey()
        {
            this.Hkey.Add(@"HKEY_CLASSES_ROOT\*\shell\");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
