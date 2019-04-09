using System;

namespace RabinEncryption.Lib.Generators
{
    public static class RabinKeysGenerator
    {
        public static Tuple<long, long, long> GetKeys()
        {
            long n = 0, p = 0, q = 0;
            while (p % 4 != 3 || q % 4 != 3)
            {
                p = PrimeGenerator.GeneratePrime();
                q = PrimeGenerator.GeneratePrime();
                while (p == q)
                {
                    q = PrimeGenerator.GeneratePrime();
                }
            }
            n = p * q;
            return new Tuple<long, long, long>(p, q, n);
        }
    }
}