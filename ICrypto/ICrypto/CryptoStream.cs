using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Security.Cryptography;
using System.IO;

namespace Crypto
{
    public class CryptoSt : ICrypto
    {
        Action<byte[]> bact = null;
        byte[] LAB = null;//Ключевое слово для сохранения и поиска ключа в файле

        public CryptoSt(Stream Str, string Key, Coder.ICoder ic, Action<byte[]> bact, Action<int, string> act = null)
        {
            base.Str = Str;
            base.Key = Key;
            base.Cdr = ic;
            this.bact = bact;
            base.act = act;
            string s = ic.ToString().Substring(ic.ToString().LastIndexOf('.') + 1);
            this.LAB = Encoding.Unicode.GetBytes(string.Format("CryptoCoder{0}", s));
        }

        /// <summary>Метод  шифрования , записи алгоритма 
        /// 
        /// </summary>
        public override void Crypt()
        {
            if (this.ProvData())
            {
                if (StreamProvMD5())
                {
                    
                    while (this.Str.Position < this.Str.Length)
                    {
                        base.buf = new byte[4096];
                        int red = this.Str.Read(base.buf, 0, this.buf.Length);
                        base.iRed += red;
                        Array.Resize<byte>(ref this.buf, red);
                        this.buf = this.Cdr.EnCode(this.buf, Encoding.Unicode.GetBytes(this.Key));
                        this.bact(this.buf);
                        if (act != null)
                            base.act((int)(iRed * 100 / Str.Length), "Crypto");
                    }
                    this.bact(this.LAB);
                    this.bact(KeyGenMD5());
                }
                else
                    throw new Exception("The file was not encrypted as it is already encrypted.");
            }
        }

        /// <summary>Метод считывания , дешифрования , записи файла алгоритмом 
        /// 
        /// </summary>
        public override void Decrypt()
        {
            if (this.ProvData())
            {
                if (KeyProvMD5(KeyGenMD5()))
                {
                    bool b = true;
                    while (Str.Position < this.Str.Length)
                    {
                       
                        this.buf = new byte[4096];
                        int red = this.Str.Read(base.buf, 0, this.buf.Length);
                        base.iRed += red;
                        if(Str.Position+4096>this.Str.Length && b)
                        {
                            long x = this.Str.Length - this.Str.Position;
                            if( x < this.LAB.Length+16)
                            {
                                x = this.LAB.Length + 16 - x;
                                Array.Resize<byte>(ref this.buf, red-(int)x);
                                Str.Position = this.Str.Length;
                                b = false;
                            }
                        }
                        if (Str.Position == this.Str.Length && b)
                        {
                            Array.Resize<byte>(ref this.buf, red - this.LAB.Length + 16);
                        }
                        
                        this.buf = this.Cdr.DeCode(this.buf, Encoding.Unicode.GetBytes(this.Key));
                        this.bact(this.buf);
                        if (act != null)
                            base.act((int)(iRed * 100 / Str.Length), "Decrypt");
                    }
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
            if (this.Str.Length > LAB.Length + 16)
            {
                Str.Position = this.Str.Length - LAB.Length - 16;
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
            }
            Str.Position = 0;
            return bKey;
        }

        /// <summary>Метод проверки-поиска ключевого слова в файле, для шифрования
        /// 
        /// </summary>
        /// <param name="keyText"></param>
        /// <returns></returns>
        private bool StreamProvMD5()
        {
            bool bKey = true;
            if (this.Str.Length > LAB.Length + 16)
            {
                Str.Position = this.Str.Length - LAB.Length - 16;               
                byte[] prov = new byte[LAB.Length + 16];
                if (this.Str.Length > prov.Length)
                {
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
            }
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

        private bool ProvData()
        {
            bool b = false;
           
                if (this.Str!=null)
                {
                    if (!this.Key.Equals(""))
                    {
                        if (this.Cdr != null)
                        {
                            if (this.bact != null)
                            {
                                b = true;
                            }
                            else
                                throw new Exception("Error not delegat GetdData");
                        }
                        else
                            throw new Exception("Error not Coder ");
                    }
                    else
                        throw new Exception("Error not pasword ");
                }
                else
                    throw new Exception("Error not data");
           

            return b;
        }
    }
}
