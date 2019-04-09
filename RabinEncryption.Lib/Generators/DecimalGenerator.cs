using System;
using System.Collections.Generic;
using System.Linq;

namespace RabinEncryption.Lib.Generators
{
    public class DecimalGenerator
    {
        public static long GetDecimal(List<int> bytes)
        {
            long dec = 0;
            for (int i = bytes.Count - 1; i >= 0; i--)
            {
                long power = bytes.Count - i - 1;
                dec += (long) (bytes.ElementAt(i) * Math.Pow(2, power));
            }
            return dec;
        }
    }
}