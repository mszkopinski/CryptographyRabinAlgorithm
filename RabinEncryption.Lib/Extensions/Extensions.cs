using System.Collections.Generic;
using System.Linq;

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
    }
}