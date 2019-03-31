using System;
using System.Collections.Generic;

namespace RabinEncryption.Lib.Rabin
{
    public class RabinCryptoSystem : IRabinCryptoSystem
    {
        public int P { get; private set; }
        public int Q { get; private set; }
        public int PublicKey { get; private set; }

        // as private key rabin uses two prime number which p = q = 3 mod 
        // public key is a product of P and Q primes
        public RabinCryptoSystem(int p, int q)
        {
            P = p;
            Q = q;
            PublicKey = P * Q;
        }

        public string DecodeMessage(IEnumerable<int> encoded)
        {
            var characters = new List<char>();
            foreach (var e in encoded)
            {
                characters.Add((char)e);
            }
            return string.Concat(characters);
;       }

        int CalculateMod(int firstNum, int secondNum)
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
        public int Encrypt(int plainText, int publicKey)
        {
            // quadratic remainder of the square of the plaintext module the public key
            PublicKey = publicKey;
            var cipherText = (plainText * plainText) % publicKey;
            return cipherText;
        }
        
        // Decryption requires computation of square roots of the cipher modulo the primes p and q
        // so both private keys are needed to to the decryption
        public  int Decrypt(int cipher, int p, int q)
        {
            // Chinese remainder theorem can be applied here because p nad q are primes in the Rabin algorithm
            int mP = Mod((p + 1) / 4, cipher, p),
                mQ = Mod((q + 1) / 4, cipher, q);

            // Extended Euclidean algorithm, we wish to find yp and yq such that yp * p + yq * q = 1
            var arr = ExtendedEuclideanAlgorithm.Calculate(p, q);
            var yP = arr[0] * p * mQ;
            var yQ = arr[1] * q * mP;

            // Calculate square roots +r, -r, +s, -s. One if these square roots mod n is the original
            // plaintext
            var r = CalculateMod(yP + yQ, PublicKey);
            if (r < 128) return r;
            var negativeR = PublicKey - r;
            if (negativeR < 128) return negativeR;

            var s = CalculateMod(yP - yQ, PublicKey);
            if (s < 128) return s;
            var negativeS = PublicKey - s;
            if (negativeS < 128) return negativeS;

            return 0;
        }
    }
}