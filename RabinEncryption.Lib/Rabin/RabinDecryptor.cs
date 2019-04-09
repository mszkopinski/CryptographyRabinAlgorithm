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

            var invp = res.Item1;
            if (invp < 0) { invp = q + invp; }

            var invq = res.Item2;
            if (invq < 0) { invq = p + invq; }

            for (i = 0; i < cipherText.Count; i++)
            {
                var r = Utils.ModPow(cipherText[(int)i], (p + 1) / 4, p);
                var s = Utils.ModPow(cipherText[(int)i], (q + 1) / 4, q);
                var x = (invp * p * s + invq * q * r) % n;
                var y = (invp * p * s - invq * q * r) % n;

                // Calculate four possible values
                var m1 = x;
                if (m1 < 0) { m1 = n + m1; }
                var m2 = -x % n;
                if (m2 < 0) { m2 = n + m2; }
                var m3 = y;
                if (m3 < 0) { m3 = n + m3; }
                var m4 = -y % n;
                if (m4 < 0) { m4 = n + m4; }

                var check = new List<int>[4];
                // all are in reverse order
                check[0] = BinaryGenerator.GenerateBinary(m1);
                check[1] = BinaryGenerator.GenerateBinary(m2);
                check[2] = BinaryGenerator.GenerateBinary(m3);
                check[3] = BinaryGenerator.GenerateBinary(m4);

                int j = 0;
                for (j = 0; j < 4; j++)
                {
                    if (check[j].Count < breakSize)
                    {

                        while (check[j].Count != breakSize)
                        {
                            check[j].Add(0);
                        }
                    }
                    if (check[j].Count <= breakSize + 3)
                    {
                        var correct = -1;
                        correct = check[j][0] + check[j][1] + check[j][2];
                        if (correct == 0)
                        {
                            var msg = new List<int>(check[j].Skip(3));
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