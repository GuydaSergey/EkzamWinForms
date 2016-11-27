using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Coder
{
    public class CoderRC4 : ICoder// Класс алгоритма RC4
    {
        private byte[] S = new byte[256];

        private int i = 0;
        private int j = 0;

        /// <summary> Для удобства       
        /// </summary>
        /// <returns></returns> 
        private void swap(byte[] array, int ind1, int ind2)
        {
            byte temp = array[ind1];
            array[ind1] = array[ind2];
            array[ind2] = temp;
        }

        /// <summary> Инициализация, алгоритм ключевого расписания      
        /// </summary>
        /// <returns></returns> 
        private void init(byte[] key)
        {
            for (i = 0; i < 256; i++)
            {
                S[i] = (byte)i;
            }

            j = 0;
            for (i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 256;
                swap(S, i, j);
            }
            i = j = 0;
        }

        /// <summary> Генератор псевдослучайной последовательности     
        /// </summary>
        /// <returns></returns>     
        private byte kword()
        {
            i = (i + 1) % 256;
            j = (j + S[i]) % 256;
            swap(S, i, j);
            byte K = S[(S[i] + S[j]) % 256];
            return K;
        }

        /// <summary> Переопределений метод шифрования от класса ICode    
        /// </summary>
        /// <returns></returns>
        public override byte[] EnCode(byte[] text, byte[] key)
        {
            init(key);// Вызов метода init() для ключа
            byte[] data = text.Take(text.Length).ToArray();

            for (int i = 0; i < data.Length; i++)
            {
                text[i] = (byte)(data[i] ^ kword());
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
