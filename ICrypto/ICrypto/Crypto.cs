using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.IO;

namespace Crypto
{
    public abstract class ICrypto// Абстрактный класс для классов шифрования файлов 
    {
        protected Stream Str = null;
        protected Coder.ICoder Cdr = null;
        protected string Key { set; get; }//Поле хранения ключа
        protected byte[] buf = null;//Буфер для считивания и записи файла 

        protected Action<int, string> act = null;
        protected long iRed { set; get; }

        public abstract void Crypt();
        public abstract void Decrypt();
        protected abstract string MD5ToString(byte[] mas);
    }
}
