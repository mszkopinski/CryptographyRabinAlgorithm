using System;
using RabinEncryption.Lib.Extensions;

namespace RabinEncryption.Lib.Generators
{
    public static class PrimeGenerator
    {
        static readonly RandomNumberGenerator RNG = new RandomNumberGenerator(10000);
        static long _cachedPrime;

        public static long GeneratePrime()
        {
            do
            {
                _cachedPrime = RNG.GetNextValue();
            }
            while (!CheckIfPrime(_cachedPrime));
            return _cachedPrime;
        }

        static bool CheckIfPrime(long number)
        {
            var res = false;
            if (number % 2 != 0)
            {
                if (FermatPrimariltyTest(number) && MillerRabinTest(number))
                {
                    res = true;
                }

            }
            return res;
        }

        static bool FermatPrimariltyTest(long p)
        {
            var res = false;
            const long a = 2;
            if (Utils.ModPow(a, p - 1, p) == 1)
            {
                res = true;
            }
            return res;
        }


        static bool MillerRabinTest(long number)
        {
            if (number == 0 || number == 1)
                return false;
            if (number == 2)
                return true;
            if (number % 2 == 0)
                return false;

            const long low = 2;
            var rest = false;
            long k = 20, i = 0, s = 0;

            var random = new Random();
            var up = number;
            var temp = number - 1;
            while (temp % 2 == 0)
            {
                temp = temp / 2;
                s++;
            }
            var d = temp;

            for (i = 0; i < k; i++)
            {
                var a = random.Next((int)(up - low + 1)) + low;
                var x = Utils.ModPow(a, d, number);
                if (x == 1 || x == number - 1)
                {
                    rest = true;
                }
                for (var r = 1; r < s; r++)
                {
                    x = (x * x) % number;
                    if (x == number - 1)
                    { rest = true; }
                }
            }

            return rest;
        }
    }
}