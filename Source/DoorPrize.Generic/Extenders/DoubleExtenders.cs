using System;
using System.Diagnostics;

namespace DoorPrize.Generic
{
    public static partial class Extenders
    {
        [DebuggerHidden]
        public static bool Approximates(this double a, double b,
            int decimals)
        {
            if ((decimals < 1) || (decimals > 15))
                throw new ArgumentOutOfRangeException("decimals");

            return Approximates(a, b, Math.Pow(10, decimals - 1));
        }

        [DebuggerHidden]
        private static bool Approximates(this double a, double b,
            double fraction = 1.0E+15)
        {
            if (fraction <= 0)
                throw new ArgumentOutOfRangeException("fraction");

            if (Equals(a, b))
                return true;

            return Math.Abs(a - b) < Math.Abs(a / fraction);
        }
    }
}
