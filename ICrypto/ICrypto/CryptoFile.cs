using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Security.Cryptography;
using System.IO;

namespace Crypto
{
    public class CryptoFile : ICrypto
    {
        private FileStream fsSave = null;

        bool Flags = false;

        private string ReadPath { set; get; }//Путь к файлу источнику

        private string WritePath { set; get; }//Путь к файлу результату

        byte[] LAB = null;//Ключевое слово для сохранения и поиска ключа в файле

        public CryptoFile(string ReadPath, string Key, Coder.ICoder ic, string WritePath="", Action<int, string> act = null, bool Flags = false)
        {
            this.ReadPath = ReadPath;
            this.WritePath = WritePath;
            base.Key = Key;
            base.Cdr = ic;
            base.act = act;            
            this.Flags = Flags;
            string s = ic.ToString().Substring(ic.ToString().LastIndexOf('.') + 1);
            this.LAB = Encoding.Unicode.GetBytes(string.Format("CryptoCoder{0}", s));
        }

        /// <summary>Метод считывания , шифрования , записи файла алгоритмом 
        /// 
        /// </summary>
        public override void Crypt()
        {
            if (this.WritePath == "")
            {
                this.WritePath = string.Format("{0}{1}", this.ReadPath.Substring(0, this.ReadPath.Length - this.ReadPath.LastIndexOf(".")), ".tmp");
            }

            if (this.ProvData())
            {
                try
                {
                    this.Str = new FileStream(this.ReadPath, FileMode.Open, FileAccess.ReadWrite);

                     DriveInfo di = new DriveInfo(this.WritePath.Substring(0, this.WritePath.IndexOf(":")));
                     if (di.TotalFreeSpace > this.Str.Length)
                     {

                         this.fsSave = new FileStream(this.WritePath, FileMode.Append, FileAccess.Write);

                         if (FileProvMD5())
                         {
                             Crypto.CryptoSt ct = new CryptoSt(this.Str, this.Key, this.Cdr, new Action<byte[]>(this.GetStream), this.act);
                             ct.Crypt();
                             this.fsSave.Close();
                         }
                         else
                         {
                             throw new Exception("The file was not encrypted as it is already encrypted.");
                         }

                         this.Str.Close();

                         if (this.Flags)
                         {
                             File.Delete(this.ReadPath);
                             this.ReadPath = this.ReadPath.Insert(this.ReadPath.LastIndexOf("."), "_crypto");
                             if (File.Exists(this.ReadPath))
                             {
                                 File.Delete(this.ReadPath);
                             }

                             File.Move(this.WritePath, this.ReadPath);
                         }
                         else
                         {
                             this.ReadPath = this.ReadPath.Insert(this.ReadPath.LastIndexOf("."), "_crypto");
                             if (File.Exists(this.ReadPath))
                             {
                                 File.Delete(this.ReadPath);
                             }
                             File.Move(this.WritePath, this.ReadPath);
                         }
                     }
                     else
                         throw new Exception("Error the file is too large to save");
                }
                catch (Exception mex)
                {
                    this.fsSave.Close();
                    this.Str.Close();
                    File.Delete(this.WritePath);
                    throw new Exception(mex.Message);
                }
            }
        }

        /// <summary>Метод считывания , дешифрования , записи файла алгоритмом RC4
        /// 
        /// </summary>
        public override void Decrypt()
        {
            if (this.WritePath == "")
            {
                this.WritePath = string.Format("{0}{1}",
                                                this.ReadPath.Substring(0, this.ReadPath.Length - this.ReadPath.LastIndexOf("."))
                                                , ".tmp");
            }

            if (this.ProvData())
            {
                try
                {
                    this.Str = new FileStream(this.ReadPath, FileMode.Open, FileAccess.ReadWrite);

                    DriveInfo di = new DriveInfo(this.WritePath.Substring(0, this.WritePath.IndexOf(":")));
                    if (di.TotalFreeSpace > this.Str.Length)
                    {
                        this.fsSave = new FileStream(this.WritePath, FileMode.Append, FileAccess.Write);

                        if (KeyProvMD5(KeyGenMD5()))
                        {
                            Crypto.CryptoSt ct = new CryptoSt(this.Str, this.Key, this.Cdr, new Action<byte[]>(this.GetStream), this.act);
                            ct.Decrypt();
                            this.fsSave.Close();
                        }

                        this.Str.Close();

                        if (this.Flags)
                        {
                            File.Delete(this.ReadPath);
                            this.ReadPath = this.ReadPath.Replace("_crypto", "");
                            if (File.Exists(this.ReadPath))
                            {
                                File.Delete(this.ReadPath);
                            }
                            File.Move(this.WritePath, this.ReadPath);
                        }
                        else
                        {
                            this.ReadPath = this.ReadPath.Replace("_crypto", "");
                            if (File.Exists(this.ReadPath))
                            {
                                File.Delete(this.ReadPath);
                            }
                            File.Move(this.WritePath, this.ReadPath);
                        }
                    }
                    else
                        throw new Exception("Error the file is too large to save");

                }
                catch (Exception mex)
                {
                    this.fsSave.Close();
                    this.Str.Close();
                    File.Delete(this.WritePath);
                    throw new Exception(mex.Message);
                }
            }
        }

        /// <summary>Метод вычисления контрольной суммы ключа  (алгоритмом MD5)
        /// 
        /// </summary>
        /// <returns></returns>
        private byte[] KeyGenMD5()
        {
            byte[] bHash = null;
            byte[] arKey = Encoding.Unicode.GetBytes(this.Key);
            MD5 md5 = MD5.Create();
            bHash = md5.ComputeHash(arKey);
            return bHash;

        }

        /// <summary>Метод  проверки контрольной суммы ключа при дешифрованеи 
        /// 
        /// </summary>
        /// <param name="keyText"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private bool KeyProvMD5(byte[] key)
        {
            bool bKey = false;
            byte[] prov = new byte[LAB.Length + 16];
            if (this.Str.Length > prov.Length)
            {
                Str.Position = this.Str.Length - prov.Length;
                Str.Read(prov, 0, prov.Length);

                byte[] b = new byte[LAB.Length];
                for (int i = 0; i < prov.Length - 16; i++)
                {
                    b[i] = prov[i];
                }
                if (Encoding.Unicode.GetString(b) == Encoding.Unicode.GetString(LAB))
                {
                    int j = 0;
                    b = new byte[16];
                    for (int i = prov.Length - 16; i < prov.Length; i++)
                    {
                        b[j++] = prov[i];
                    }
                    if (this.MD5ToString(b) == this.MD5ToString(key))
                    {
                        bKey = true;
                    }
                    else
                    {
                        throw new Exception("The file is not encrypted with the wrong key.");

                    }
                }
                else
                {
                    throw new Exception("File not found encrypted cipher key words.");

                }
                Str.Position = 0;
            }
           
            return bKey;
        }

        /// <summary>Метод проверки-поиска ключевого слова в файле, для шифрования
        /// 
        /// </summary>
        /// <param name="keyText"></param>
        /// <returns></returns>
        private bool FileProvMD5()
        {
            bool bKey = true;
            if (this.Str.Length > LAB.Length + 16)
            {
                byte[] prov = new byte[LAB.Length + 16];
                Str.Position = this.Str.Length - prov.Length;
                Str.Read(prov, 0, prov.Length);

           
                byte[] b = new byte[LAB.Length];
                for (int i = 0; i < prov.Length - 16; i++)
                {
                    b[i] = prov[i];
                }
                if (Encoding.Unicode.GetString(b) == Encoding.Unicode.GetString(LAB))
                {
                    bKey = false;
                }
            }
            Str.Position = 0;
            return bKey;
      
        }

        /// <summary>Вспомогательний метод для сравнения контрольних сумм
        /// 
        /// </summary>
        /// <param name="mas"></param>
        /// <returns></returns>
        protected override string MD5ToString(byte[] mas)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mas.Length; i++)
                sb.Append(mas[i].ToString("x2"));
            return sb.ToString();
        }

        private void GetStream(byte[] buf)
        {
            fsSave.Write(buf, 0,buf.Length);
        }

        private bool ProvData()
        {
            bool b = false;
           
                if (File.Exists(this.ReadPath))
                {
                   b = true;
                }
                else
                    throw new Exception("Error Exist original file");

            return b;
        }
    }
}
