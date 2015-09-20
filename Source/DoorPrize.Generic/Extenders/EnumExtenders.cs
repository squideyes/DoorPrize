using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

namespace DoorPrize.Generic
{
    public static partial class Extenders
    {
        [DebuggerHidden]
        [Pure]
        public static T ToEnum<T>(this string value) where T : struct
        {
            Contract.Requires(typeof(T).IsEnum);

            return (T)Enum.Parse(typeof(T), value, true);
        }

        [DebuggerHidden]
        [Pure]
        public static bool IsDefined<T>(this T value)
        {
            Contract.Requires(typeof(T).IsEnum);

            dynamic dyn = value;

            var max = Enum.GetValues(value.GetType()).
                Cast<dynamic>().Aggregate((e1, e2) => e1 | e2);

            return (max & dyn) == dyn;
        }

        public static bool IsParsableEnum<T>(this string value)
            where T : struct
        {
            T enumeration;

            return Enum.TryParse(value, true, out enumeration);
        }
    }
}
