using System;
using RabinEncryption.Lib.Rabin;

namespace RabinEncryption.Lib.Generators
{
    public static class RabinKeysGenerator
    {
        private const int KeyLenght = 1024;

        public static BigNumber GenerateKey()
        {
            return GetRandomPrime(KeyLenght);
        }

        static BigNumber GetRandomPrime(int bytesLength)
        {
            var random = new Random();
            BigNumber prime;
            do
            {
                prime = BigNumber.GetPseudoPrime(bytesLength, 10, random);
            }
            while (prime % BigNumber.Four != BigNumber.Three);
            return prime;
        }
    }
}