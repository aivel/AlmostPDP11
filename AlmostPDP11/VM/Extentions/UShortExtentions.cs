using System;

namespace AlmostPDP11.VM.Extentions
{
    public static class UShortExtentions
    {
        public static ushort SetBit(this ushort target, int index, bool value)
        {
            if (value)
            {
                //left-shift 1, then bitwise OR
                return (ushort) (target | (0x1 << index));
            }

            //left-shift 1, then take complement, then bitwise AND
            return (ushort)(target & ~(1 << index));
        }

        public static bool GetBit(this ushort target, int index)
        {
            return (target & (1 << index)) != 0;
        }
    }
}
