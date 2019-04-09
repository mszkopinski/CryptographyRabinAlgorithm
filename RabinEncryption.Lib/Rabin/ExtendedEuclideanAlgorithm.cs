using System;

namespace RabinEncryption.Lib.Rabin
{
    public static class ExtendedEuclideanAlgorithm
    {
        public static Tuple<long, long> Calculate(long a, long b)
        {
            long x = 0, y = 1;
            long previousX = 1, previousY = 0;
            while (b != 0)
            {
                long q = a / b;
                long r = a % b;
                a = b;
                b = r;

                var temp = x;
                x = previousX - q * x;
                previousX = temp;

                temp = y;
                y = previousY - q * y;
                previousY = temp;
            }

            return new Tuple<long, long>(previousX, previousY);
        }
    }
}