using System;
using System.Collections.Generic;
using RabinEncryption.Lib.Extensions;
using RabinEncryption.Lib.Generators;

namespace RabinEncryption.Lib.Rabin
{
    public interface IRabinEncryptor
    {
        List<long> Encrypt(long cipherText, long publicKey, out long breakSize);
    }

    public class RabinEncryptor : IRabinEncryptor
    {
        List<int> binaryArrM = new List<int>();
        List<int> binaryArrN = new List<int>();

        readonly List<List<int>> brokenMessage = new List<List<int>>();
        readonly List<long> finalMessage = new List<long>();

        public List<long> Encrypt(long cipherText, long publicKey, out long breakSize)
        {
            binaryArrM.Clear();
            binaryArrN.Clear();
            brokenMessage.Clear();
            finalMessage.Clear();

            binaryArrM = BinaryGenerator.GenerateBinary(cipherText);
            binaryArrN = BinaryGenerator.GenerateBinary(cipherText);

            binaryArrM = binaryArrM.GetReversed();
            binaryArrN = binaryArrN.GetReversed();
            breakSize = 0;

            var binMsgWithPadding = new List<int>(binaryArrM) {0, 0, 0};
            long padMsg = DecimalGenerator.GetDecimal(binMsgWithPadding);

            if (padMsg < publicKey)
            {
                var c = (long)(Math.Pow(padMsg, 2) % publicKey);
                finalMessage.Add(c);
                breakSize = binaryArrM.Count; 
            }
            else if (padMsg >= publicKey)
            {
                //Normalized padding
                var nSize = binaryArrN.Count;
                var mSize = binaryArrM.Count;
                breakSize = nSize / 2;
                long toAdd;
                if (mSize % breakSize == 0)
                {
                    toAdd = 0;
                }
                else
                {
                    toAdd = breakSize - mSize % breakSize;
                }

                binaryArrM.Reverse();
                for (long i = 0; i < toAdd; i++)
                {
                    binaryArrM.Add(0);
                }

                var normalizedMsg = binaryArrM.GetReversed();
                // breaking data
                var k = 0;
                long noBreaks = normalizedMsg.Count / breakSize;
                for (long i = 0; i < noBreaks; i++)
                {
                    var piece = new List<int>();
                    for (long j = 0; j < breakSize; j++)
                    {
                        piece.Add(normalizedMsg[k]);
                        ++k;
                    }
                    brokenMessage.Add(piece);

                }
                k = 0;

                for (int i = 0; i < noBreaks; i++)
                {
                    brokenMessage[i].Add(0);
                    brokenMessage[i].Add(0);
                    brokenMessage[i].Add(0);
                    k++;
                    var c = DecimalGenerator.GetDecimal(brokenMessage[i]);
                    c = (long)(Math.Pow(c, 2) % publicKey);
                    finalMessage.Add(c);
                }
            }
            return finalMessage;
        }
    }
}