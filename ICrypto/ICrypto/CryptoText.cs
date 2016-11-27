using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using System.IO;

namespace Crypto
{
    public class CryptoText : ICrypto
    {
       
        byte[] LAB = null;//Ключевое слово для сохранения и поиска ключа в файле

        public CryptoText(byte [] Text, string Key, Coder.ICoder ic, Action<int, string> act = null)
        {
            base.buf = Text;
            base.Key = Key;
            base.Cdr = ic;
            string s = ic.ToString().Substring(ic.ToString().LastIndexOf('.') + 1);
            this.LAB = Encoding.Unicode.GetBytes(string.Format("CryptoCoder{0}", s));
            
        }

        /// <summary>Метод считывания , шифрования , записи файла алгоритмом 
        /// 
        /// </summary>
        public override void Crypt()
        {
            if (DataProvMD5())
            {
               this.buf = this.Cdr.EnCode(this.buf, Encoding.Unicode.GetBytes(this.Key));
               this.buf=SaveCrypt();
            }
            else
                throw new Exception("The file was not encrypted as it is already encrypted.");
        }

        /// <summary>Метод считывания , дешифрования , записи файла алгоритмом 
        /// 
        /// </summary>
        public override void Decrypt()
        {
            if (KeyProvMD5(KeyGenMD5()))
            {
                this.buf = ResizeCrypt();
                this.buf = this.Cdr.DeCode(this.buf, Encoding.Unicode.GetBytes(this.Key));
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
            
            if (!DataProvMD5())
            {
                int j = 0;
                byte [] b = new byte[16];
                for (int i = this.buf.Length - b.Length; j < b.Length ; i++)
                {
                    b[j++] = this.buf[i];
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
            return bKey;
        }

        /// <summary>Метод проверки-поиска ключевого слова в файле, для шифрования
        /// 
        /// </summary>
        /// <param name="keyText"></param>
        /// <returns></returns>
        private bool DataProvMD5()
        {          
            bool bKey = true;
            byte[] b = new byte[LAB.Length];
            int x = 0;
            if (this.buf.Length>this.LAB.Length)
            {
                for (int i = this.buf.Length - this.LAB.Length-16; x < this.LAB.Length; i++)
                {
                    b[x++] = this.buf[i];
                }
                if (Encoding.Unicode.GetString(b) == Encoding.Unicode.GetString(LAB))
                {
                    return false;
                }
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

        public string GetText()
        {
            return Encoding.Unicode.GetString(this.buf);
        }

        public byte [] GetData()
        {
            return this.buf;
        }

        private byte[] SaveCrypt()
        {
            byte[]tmp = new byte[this.buf.Length + this.LAB.Length + 16];
            Array.Copy(this.buf, tmp, this.buf.Length);
            Array.Copy(this.LAB, 0, tmp, this.buf.Length, this.LAB.Length);
            Array.Copy(KeyGenMD5(), 0, tmp, this.buf.Length + this.LAB.Length, 16);

            return tmp;
        }
        private byte [] ResizeCrypt()
        {
            byte[] tmp = new byte[this.buf.Length - this.LAB.Length - 16];
            Array.Copy(this.buf, tmp,tmp.Length);

            return tmp;
        }
    }
}
