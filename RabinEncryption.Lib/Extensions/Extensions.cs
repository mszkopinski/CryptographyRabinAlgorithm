using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RabinEncryption.Lib.Extensions
{
    public static class Extensions
    {
        public static List<T> GetReversed<T>(this IEnumerable<T> enumerable)
        {
            var newList = new List<T>();
            foreach (var element in enumerable.Reverse())
            {
                newList.Add(element);
            }
            return newList;
        }

        public static IEnumerable<T> GetRow<T>(this T[,] array, int row)
        {
            for (int i = 0; i < array.GetLength(1); ++i)
            {
                yield return array[row, i];
            }
        }

        public static IEnumerable<byte> CutLastThreeBytes(this byte[] byteArr)
        {
            if (byteArr.Length < 64)
            {
                for (int i = 0; i < 64 - byteArr.Length; i++)
                {
                    yield return 0;
                }
            }

            for (int i = 0; i < byteArr.Length - 3; i++)
            {
                yield return byteArr[i];
            }
        }

        public static string AsString(this byte[] byteArray)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < byteArray.Length; i++)
            {
                var b = byteArray[i];
                sb.Append(b);
                if (i < byteArray.Length - 1)
                {
                    sb.Append(" ");
                }
            }
            return sb.ToString();
        }
    }
}