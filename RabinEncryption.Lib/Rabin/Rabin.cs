using System;
using System.Collections.Generic;

namespace RabinEncryption.Lib.Rabin
{
    public class Rabin
    {
        static int _publicKey;

        public static string DecodeMessage(IEnumerable<int> encoded)
        {
            var characters = new List<char>();
            foreach (var e in encoded)
            {
                characters.Add((char)e);
            }
            return string.Concat(characters);
;       }

        static int CalculateMod(int firstNum, int secondNum)
        {
            return firstNum >= secondNum 
                   ? firstNum % secondNum
                   : (secondNum - Math.Abs(firstNum % secondNum)) % secondNum;
        }

        static int Mod(int k, int b, int m)
        {
            int a = 1;
            List<int> t = new List<int>();

            int i = 0;
            while (k > 0)
            {
                var value = k % 2;
                t.Add(value); 
                k = (k - value) / 2;
                ++i;
            }

            for (int j = 0; j < i; ++j)
            {
                if (t[j] == 1)
                {
                    a = (a * b) % m;
                    b = (b * b) % m;
                }
                else
                {
                    b = (b * b) % m;
                }
            }
            return a;
        }

        // only public key is being used for encryption
        public static int Encrypt(int plainText, int publicKey)
        {
            var cipherText = (plainText * plainText) % publicKey;
            _publicKey = publicKey;
            return cipherText;
        }
        
        // Decryption requires computation of square roots of the cipher modulo the primes p and q
        public static int Decrypt(int cipher, int p, int q)
        {
            int mP = Mod((p + 1) / 4, cipher, p),
                mQ = Mod((q + 1) / 4, cipher, q);
            var arr = ExtendedEuclideanAlgorithm.Calculate(p, q);
            var yP = arr[0] * p * mQ;
            var yQ = arr[1] * q * mP;

            // Chineese remainder theorem. Can be only applied if n is prime or p and q are.
            var r = CalculateMod(yP + yQ, _publicKey);
            if (r < 128) return r;
            var negativeR = _publicKey - r;
            if (negativeR < 128) return negativeR;

            var s = CalculateMod(yP - yQ, _publicKey);
            if (s < 128) return s;
            var negativeS = _publicKey - s;
            if (negativeS < 128) return negativeS;

            return 0;
        }
    }
}