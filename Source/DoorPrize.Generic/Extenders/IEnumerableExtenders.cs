using System.Collections.Generic;
using System.Linq;

namespace DoorPrize.Generic
{
    public static partial class Extenders
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return (items == null) || (!items.Any());
        }
    }
}
