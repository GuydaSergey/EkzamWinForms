using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Coder
{
    public class CoderCezer : ICoder// Класс алгоритма Цезарь (только тексты)
    {
        private string text;// Текст для шифрования или разшифрования

        private int key;//Ключь

        /// <summary> Переопределений метод шифрования от класса ICode 
        /// </summary>
        /// <returns></returns>
        public override byte[] EnCode(byte[] text, byte[] key)
        {
            this.text = Encoding.Unicode.GetString(text);
            string keyBuf = Encoding.Unicode.GetString(key);
            this.key = int.Parse(keyBuf);
            string buf = "";

            for (int i = 0; i < text.Length/2; i++)
            {
                int j = ((int)this.text[i]) + this.key;
                buf += (char)j;
            }
            return Encoding.Unicode.GetBytes(buf);

        }

        /// <summary> Переопределений метод разшифрования от класса ICode 
        /// </summary>
        /// <returns></returns>
        public override byte[] DeCode(byte[] text, byte[] key)
        {
            this.text = Encoding.Unicode.GetString(text);
            string keyBuf = Encoding.Unicode.GetString(key);
            this.key = int.Parse(keyBuf);
            string buf = "";

            for (int i = 0; i < text.Length/2; i++)
            {
                int j = ((int)this.text[i]) - this.key;
                buf += (char)j;
            }
            return Encoding.Unicode.GetBytes(buf);
        }
    }
}
