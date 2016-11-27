using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Coder
{
    public abstract class ICoder// Абстрактний класс для алгоритмов шифрования 
    {
        public abstract byte[] EnCode(byte[] text, byte[] key);

        public abstract byte[] DeCode(byte[] text, byte[] key);
    }
}
