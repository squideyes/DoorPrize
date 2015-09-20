using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace DoorPrize.Generic
{
    public static partial class Extenders
    {
        public static int GetLittleEndianInt32(this BinaryReader reader)
        {
            Contract.Requires(reader != null);

            var bytes = reader.ReadBytes(4);

            Array.Reverse(bytes);

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
