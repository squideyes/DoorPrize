using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DoorPrize.Generic
{
    public static partial class Extenders
    {
        [DebuggerHidden]
        public static bool HasElements<T>(this List<T> list, 
            Func<T, bool> isValid)
        {
            return list.HasElements(1, int.MaxValue, isValid);
        }

        private static bool HasElements<T>(this IReadOnlyCollection<T> list, 
            int minElements, int maxElements, Func<T, bool> isValid)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (minElements < 0)
                throw new ArgumentOutOfRangeException("minElements");

            if (maxElements < minElements)
                throw new ArgumentOutOfRangeException("maxElements");

            if (list.Count < minElements)
                return false;

            if (list.Count > maxElements)
                return false;

            if (isValid != null)
            {
                foreach (var item in list)
                {
                    if (item.Equals(default(T)))
                        return false;

                    if (!isValid(item))
                        return false;
                }
            }

            return true;
        }
    }
}
