using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Coder
{
    public class CoderXor : ICoder// Класс алгоритма XOR
    {
        /// <summary> Переопределений метод шифрования от класса ICode 
        /// </summary>
        /// <returns></returns>
        public override byte[] EnCode(byte[] text, byte[] key)
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i] ^= key[i % key.Length];
            }
            return text;
        }

        /// <summary> Переопределений метод разшифрования от класса ICode
        /// вызываюший метод EnCode();
        /// </summary>
        /// <returns></returns>
        public override byte[] DeCode(byte[] text, byte[] key)
        {
            return EnCode(text, key);
        }
    }
}
