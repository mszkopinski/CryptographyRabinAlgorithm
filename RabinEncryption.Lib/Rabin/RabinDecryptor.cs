using System.Collections.Generic;
using System.Linq;
using RabinEncryption.Lib.Extensions;
using RabinEncryption.Lib.Generators;

namespace RabinEncryption.Lib.Rabin
{
    public interface IRabinDecryptor
    {
        long Decrypt(List<long> cipherText, long p, long q, long n, long breakSize);
    }

    public class RabinDecryptor : IRabinDecryptor
    {
        public long Decrypt(List<long> cipherText, long p, long q, long n, long breakSize)
        {
            long i = 0;
            var messagePieces = new List<long>();

            var res = ExtendedEuclideanAlgorithm.Calculate(p, q);

            var yp = res.Item1;
            if (yp < 0) { yp = q + yp; }

            var yq = res.Item2;
            if (yq < 0) { yq = p + yq; }

            for (i = 0; i < cipherText.Count; i++)
            {
                var mp = Utils.ModPow(cipherText[(int)i], (p + 1) / 4, p);
                var mq = Utils.ModPow(cipherText[(int)i], (q + 1) / 4, q);
                var r = (yp * p * mq + yq * q * mp) % n;
                var s = (yp * p * mq - yq * q * mp) % n;

                // Calculate four possible values
                var m1 = r;
                if (m1 < 0) { m1 = n + m1; }
                var m2 = -r % n;
                if (m2 < 0) { m2 = n + m2; }
                var m3 = s;
                if (m3 < 0) { m3 = n + m3; }
                var m4 = -s % n;
                if (m4 < 0) { m4 = n + m4; }

                var binaryToCheck = new List<int>[4];
                // all are in reverse order
                binaryToCheck[0] = BinaryGenerator.GenerateBinary(m1);
                binaryToCheck[1] = BinaryGenerator.GenerateBinary(m2);
                binaryToCheck[2] = BinaryGenerator.GenerateBinary(m3);
                binaryToCheck[3] = BinaryGenerator.GenerateBinary(m4);

                int j = 0;
                for (j = 0; j < 4; j++)
                {
                    if (binaryToCheck[j].Count < breakSize)
                    {
                        while (binaryToCheck[j].Count != breakSize)
                        {
                            binaryToCheck[j].Add(0);
                        }
                    }
                    if (binaryToCheck[j].Count <= breakSize + 3)
                    {
                        var paddingScheme = -1;
                        paddingScheme = binaryToCheck[j][0] + binaryToCheck[j][1] + binaryToCheck[j][2];
                        if (paddingScheme == 0)
                        {
                            var msg = new List<int>(binaryToCheck[j].Skip(3));
                            msg = msg.GetReversed();
                            var piece = DecimalGenerator.GetDecimal(msg);
                            messagePieces.Add(piece);
                            break;
                        }
                    }
                }
            }

            return GenerateMessage(messagePieces, breakSize);
        }

        long GenerateMessage(List<long> msgPieces, long breakSize)
        {
            var message = new List<int>();
            foreach (var messagePiece in msgPieces)
            {
                var temp = BinaryGenerator.GenerateBinary(messagePiece);
                if (temp.Count < breakSize)
                {
                    while (temp.Count != breakSize)
                    {
                        temp.Add(0);
                    }
                }
                temp = temp.GetReversed();
                message.AddRange(temp);
            }

            var dec = DecimalGenerator.GetDecimal(message);
            return dec;
        }
    }
}