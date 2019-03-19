using System;
using System.Numerics;

namespace RabinEncryption.Lib.Rabin
{
    public class Rabin : IEncryptionAlgorithm
    {
        readonly Random random = new Random();

        public BigInteger[] GenerateKey(int bitLenght)
        {
            throw new System.NotImplementedException();
        }

        public BigInteger Encrypt(BigInteger valueToEncrypt, BigInteger publicKey)
        {
            throw new System.NotImplementedException();
        }

        public BigInteger[] Decrypt(BigInteger encryptedValue, BigInteger privateKey, BigInteger privateKey2)
        {
            throw new System.NotImplementedException();
        }
    }
}