using System.Collections.Generic;

namespace AlmostPDP11.VM.Extentions
{
    static class ByteArrayExtention
    {
        public static List<bool> ToBitStream(this byte[] bytes)
        {
            var bitstream = new List<bool>();

            foreach (var b in bytes)
            {
                for (var i = 7; i >= 0; i--)
                {
                    bitstream.Add(b.GetBit(i));
                }
            }

            return bitstream;
        }
    }
}
