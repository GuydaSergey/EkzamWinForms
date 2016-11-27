using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 
using System.IO;
using System.Drawing;
using System.Collections;
using System.Security.Cryptography;
using System.Threading;

namespace Crypto
{
    public class CryptoStenografImage : ICrypto
    {
        bool FlagsDelFile;

        bool FlagsDelData;

        bool FlagsCoder;

        Action<byte[]> bact = null;

        private string ReadPath { set; get; }//Путь к файлу источнику

        private string WritePath { set; get; }//Путь к файлу результату

        byte[] LAB = Encoding.Unicode.GetBytes("GSV:");//Ключевое слово для сохранения и поиска ключа в файле

        Bitmap bPic = null;

        int StartGetSet = 0;

        int sizeD = 0;

        MemoryStream msSave = null;

        public CryptoStenografImage(string ReadPath, string WritePath = "", bool FlagsDelFile = false,bool FlagsDelData = false, byte[] data = null,
                                    Action<int, string> act = null, bool FlagsCoder = false, string Key = "", 
                                    Coder.ICoder ic = null, Action<byte[]> bact=null)
        {
            this.ReadPath = ReadPath;
            this.WritePath = WritePath;
            base.buf = data;
            this.FlagsDelFile = FlagsDelFile;
            this.FlagsDelData = FlagsDelData;
            base.act = act;
            this.FlagsCoder = FlagsCoder;
            base.Cdr = ic;
            base.Key = Key;
            this.msSave = new MemoryStream();
            this.bact = bact;
        }

        public CryptoStenografImage(string ReadPath, string WritePath = "", bool FlagsDel = false, bool FlagsDelData = false, string DataFile = "",
                                    Action<int, string> act = null, bool FlagsCoder = false, string Key = "",
                                    Coder.ICoder ic = null)
        {
            this.ReadPath = ReadPath;
            this.WritePath = WritePath;
            this.FlagsDelFile = FlagsDel;
            base.act = act;
            this.FlagsCoder = FlagsCoder;
            this.FlagsDelData = FlagsDelData;
            base.Cdr = ic;
            base.Key = Key;
            this.msSave = new MemoryStream();
            if(!DataFile.Equals(""))
                this.SetDataFile(DataFile);
        }

        public override void Crypt()
        {
            if (File.Exists(this.ReadPath))
            {
                if (this.buf != null)
                {
                    try
                    {
                        this.Str = new FileStream(this.ReadPath, FileMode.Open, FileAccess.ReadWrite);

                        if (this.FlagsCoder)
                        {
                            this.CoderData(true);
                        }

                        this.sizeD = base.buf.Length;
                        this.bPic = new Bitmap(this.Str);

                        if (this.ProvDataSize())
                        {
                            this.SaveCrypto();

                            this.ImageCrDr(true);

                            this.SaveRemoveFile(true);
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Str.Close();
                        if (this.bPic != null)
                            this.bPic.Dispose();
                        throw new Exception(ex.Message);
                    }
                }
                else
                    throw new Exception("Error not data ");
            }
            else
                throw new Exception("Error not path File ");
        }

        public override void Decrypt()
        {
           
                 if (File.Exists(this.ReadPath))
                {
                    if (!this.WritePath.Equals(""))
                    {
                        try
                        {
                            this.Str = new FileStream(this.ReadPath, FileMode.Open, FileAccess.ReadWrite);

                            this.bPic = new Bitmap(this.Str);

                            if (ReadCrypto())
                            {
                                this.buf = new byte[this.sizeD];

                                this.ImageCrDr(false);

                                if (this.FlagsCoder)
                                {
                                    this.CoderData(false);
                                }

                                this.GetData();

                                this.SaveRemoveFile(false);
                            }
                            
                        }
                        catch (Exception ex)
                        {
                            this.Str.Close();
                            if (this.bPic != null)
                                this.bPic.Dispose();
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                        throw new Exception("Is not path File save");
                }
                 else
                     throw new Exception("Error not path File ");
            
            
        }

        private void ImageCrDr(bool Metod)
        {
            int x = 0;
            bool f = false;
            for (int i = 0; i < this.bPic.Width; i++)
            {
                int j = 0;
                if (i == 0)
                    j = this.StartGetSet;

                if (f) break;

                for (; j < this.bPic.Height; j++)
                {
                    if (Metod)
                    {
                        if (x == this.buf.Length) { f = true; break; }
                        
                        BitArray ArrData = this.ByteToBit(this.buf[x++]);
                        this.bPic.SetPixel(i, j, CryptoPixel(this.bPic.GetPixel(i, j), ArrData));
                        if(base.act!=null)
                              base.act((int)x * 100 / this.buf.Length, "CryptStenograf");
                    }
                    else
                    {
                        if (x == this.sizeD) { f = true; break; }

                        this.buf.SetValue(DecryptPixel(this.bPic.GetPixel(i, j)), x++);
                        if(this.FlagsDelData)
                        {
                            this.bPic.SetPixel(i,j,DelDAtaPixel(this.bPic.GetPixel(i, j)));
                        }
                        if (base.act != null)
                            base.act((int)x * 100 / this.buf.Length, "DecryptStenograf");
                    }
                }
            }
        }

        private BitArray ByteToBit(byte src)
        {
            BitArray bitArray = new BitArray(8);
            bool st = false;
            for (int i = 0; i < 8; i++)
            {
                if ((src >> i & 1) == 1)
                {
                    st = true;
                }
                else st = false;
                bitArray[i] = st;
            }
            return bitArray;
        }

        private byte BitToByte(BitArray scr)
        {
            byte num = 0;
            for (int i = 0; i < scr.Count; i++)
                if (scr[i] == true)
                    num += (byte)Math.Pow(2, i);
            return num;
        }

        private byte DecryptPixel(Color pixel)
        {
            BitArray Messeg = new BitArray(8);
            BitArray tempArr = ByteToBit(pixel.R);
            Messeg[0] = tempArr[0];
            Messeg[1] = tempArr[1];

            tempArr = ByteToBit(pixel.G);
            Messeg[2] = tempArr[0];
            Messeg[3] = tempArr[1];
            Messeg[4] = tempArr[2];

            tempArr = ByteToBit(pixel.B);
            Messeg[5] = tempArr[0];
            Messeg[6] = tempArr[1];
            Messeg[7] = tempArr[2];

            return BitToByte(Messeg);
        }

        private Color DelDAtaPixel(Color pixel)
        {
            BitArray tempArr = ByteToBit(pixel.R);
            tempArr[0] = false;
            tempArr[1] = false;
            byte nR = BitToByte(tempArr);

            tempArr = ByteToBit(pixel.G);
            tempArr[0] = false;
            tempArr[1] = false;
            tempArr[2] = false;
            byte nG = BitToByte(tempArr);

            tempArr = ByteToBit(pixel.B);
            tempArr[0] = false;
            tempArr[1] = false;
            tempArr[2] = false;
            byte nB = BitToByte(tempArr);

            return Color.FromArgb(nR, nG, nB);
        }

        private Color CryptoPixel(Color pixel, BitArray barr)
        {
            BitArray tempArr = ByteToBit(pixel.R);
            tempArr[0] = barr[0];
            tempArr[1] = barr[1];
            byte nR = BitToByte(tempArr);

            tempArr = ByteToBit(pixel.G);
            tempArr[0] = barr[2];
            tempArr[1] = barr[3];
            tempArr[2] = barr[4];
            byte nG = BitToByte(tempArr);

            tempArr = ByteToBit(pixel.B);
            tempArr[0] = barr[5];
            tempArr[1] = barr[6];
            tempArr[2] = barr[7];
            byte nB = BitToByte(tempArr);

            return Color.FromArgb(nR, nG, nB);
        }

        private void SaveCrypto()
        {
            byte[] dSize = Encoding.Unicode.GetBytes(string.Format("[{0}]", this.sizeD.ToString()));

            for (int i = 0; i < this.LAB.Length; i++)
            {
                this.bPic.SetPixel(0, i, CryptoPixel(this.bPic.GetPixel(0, i), ByteToBit(this.LAB[i])));
            }
            int x = 0;
            for (int i = this.LAB.Length; i < this.LAB.Length + dSize.Length; i++)
            {
                if (x == dSize.Length) { break; }
                this.bPic.SetPixel(0, i, CryptoPixel(this.bPic.GetPixel(0, i), ByteToBit(dSize[x++])));
            }
            this.StartGetSet = dSize.Length + this.LAB.Length;
        }

        private bool ReadCrypto()
        {
            bool bpr = false;
            byte[] prLab = new byte[this.LAB.Length];
            for (int i = 0; i < prLab.Length; i++)
            {
                prLab[i] = DecryptPixel(this.bPic.GetPixel(0, i));
                if (this.FlagsDelData)
                {
                    this.bPic.SetPixel(0, i, DelDAtaPixel(this.bPic.GetPixel(0, i)));
                }
            }

            if (Encoding.Unicode.GetString(prLab).Equals(Encoding.Unicode.GetString(this.LAB)))
            {
                bpr = true;
                byte[] prmas = new byte[30];
                int x = 0;
                for (int i = this.LAB.Length; i < this.LAB.Length + 30; i++)
                {
                    prmas[x++] = DecryptPixel(this.bPic.GetPixel(0, i));

                    if (this.FlagsDelData)
                    {
                        this.bPic.SetPixel(0, i, DelDAtaPixel(this.bPic.GetPixel(0, i)));
                    }
                }
                this.ReadSizePath(prmas);
            }
            else
                throw new Exception("The file is not encrypted information!!");
            return bpr;
        }

        private void ReadSizePath(byte[] prmas)
        {
            string s = Encoding.Unicode.GetString(prmas);

            int str = s.IndexOf("*");
            int fin = s.IndexOf("^");
            if (!this.WritePath.Equals(""))
            {
                if (this.WritePath.IndexOf(".") == -1)
                {
                    this.WritePath = this.WritePath.Insert(this.WritePath.Length, s.Substring(str + 1, fin - str - 1));
                }
                else
                {
                    this.WritePath = this.WritePath.Replace(
                                     this.WritePath.Substring(this.WritePath.IndexOf(".")),
                                                              s.Substring(str + 1, fin - str - 1));
                }
            }
            str = s.IndexOf("[");
            fin = s.IndexOf("]");

            this.StartGetSet = (fin + 1)*2 + this.LAB.Length;

            s = s.Substring(str + 1,fin-str-1);
            try
            {
                this.sizeD = int.Parse(s);                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        protected override string MD5ToString(byte[] mas)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < mas.Length; i++)
                sb.Append(mas[i].ToString("x2"));
            return sb.ToString();
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

        private void SetDataFile(string DataFile)
        {
            if (File.Exists(DataFile))
            {
                using (FileStream fs = new FileStream(DataFile, FileMode.Open, FileAccess.Read))
                {
                    this.buf = new byte[fs.Length];
                    fs.Read(this.buf, 0, this.buf.Length);
                    this.LAB = Encoding.Unicode.GetBytes(string.Format("GSV:*{0}^", DataFile.Substring(DataFile.IndexOf("."))));
                };

            }
            else
                throw new Exception("File data does not exist");
        }

        private void CoderData(bool Flag)
        {
            MemoryStream ms = new MemoryStream(this.buf);
            Crypto.CryptoSt ct = new CryptoSt(ms, this.Key, this.Cdr, new Action<byte[]>(this.SetStream), this.act);

            if (Flag)
                ct.Crypt();
            else
                ct.Decrypt();

            this.buf = this.msSave.GetBuffer();
            Array.Resize<byte>(ref buf, (int)this.msSave.Length);
            this.msSave.Close();
            ms.Close();
        }

        private void SaveRemoveFile(bool flag)
        {
            if (flag)
            {
                if (this.WritePath == "")
                {
                    this.ReadPath = this.ReadPath.Insert(this.ReadPath.LastIndexOf("."), "_crypto");
                    if (File.Exists(this.ReadPath))
                    {
                        File.Delete(this.ReadPath);
                    }

                    this.Str.Close();
                    this.bPic.Save(this.ReadPath, System.Drawing.Imaging.ImageFormat.Bmp);
                    this.bPic.Dispose();
                }
                else
                {
                    this.Str.Close();
                    this.bPic.Save(this.WritePath, System.Drawing.Imaging.ImageFormat.Bmp);
                    this.bPic.Dispose();
                }
            }
            else
            {
                if (this.ReadPath.IndexOf("_crypto") == -1)
                {
                    this.ReadPath = this.ReadPath.Insert(this.ReadPath.LastIndexOf("."), "_crypto");
                }
                else
                {
                    this.ReadPath = this.ReadPath.Replace("_crypto", "");
                }
                if (File.Exists(this.ReadPath))
                {
                    File.Delete(this.ReadPath);
                }
                this.bPic.Save(this.ReadPath, System.Drawing.Imaging.ImageFormat.Bmp);
                this.Str.Close();
                this.bPic.Dispose();
            }
            this.RemoveReadFile();
        }

        private void RemoveReadFile()
        {
            if (this.FlagsDelFile)
            {
                string srtTemp = this.ReadPath.Replace("_crypto", "");
                File.Delete(srtTemp);
                File.Move(this.ReadPath, srtTemp);
            }
        }

        private void SetStream(byte[] buf)
        {
            this.msSave.Write(buf, 0, buf.Length);
        }

        private void GetData()
        {
            if (this.bact != null)
                this.bact(this.buf);
            else if(!this.WritePath.Equals(""))
            {
                using (FileStream fs = new FileStream(this.WritePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(this.buf,0,this.buf.Length);                   
                };
            }
            else
                throw new Exception("Error not delegat GetdData and not path save file ");
        }

        private bool ProvDataSize()
        {
            if(this.buf.Length<this.bPic.Size.Height*this.bPic.Size.Width)
            {
                return true;
            }
            else
                throw new Exception("Error Data size is larger than the size of the image in pixels");
        }

    }
}
