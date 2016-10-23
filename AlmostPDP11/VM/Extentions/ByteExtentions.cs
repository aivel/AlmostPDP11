using System;

namespace AlmostPDP11.VM.Extentions
{
    public static class ByteExtentions
    {
        public static byte SetBit(this byte target, int index, bool value)
        {
            if (value)
            {
                //left-shift 1, then bitwise OR
                return (byte)(target | (1 << index));
            }

            //left-shift 1, then take complement, then bitwise AND
            return (byte)(target & ~(1 << index));
        }

        public static bool GetBit(this byte target, int index)
        // Assume 0 is the MSB andd 7 is the LSB.
        {
            return (target & (1 << index)) != 0;
        }
    }
}
