using System.Collections.Generic;
using System.Linq;

namespace VM {
    public static class Consts {
        public static int GeneralPurposeRegistersCount = 8;
        public static int AdditionalRegistersCount = 2;
        public static int TotalRegistersCount = GeneralPurposeRegistersCount + AdditionalRegistersCount;

        public static int RegisterBytes = 2;
        public static int RegisterBits = RegisterBytes * 8;

        public static int StatusWordRegisterBytes = 2;
        public static int KeyboardHandlerBytes = 2;
        //
        public static int SPIncBytes = 2; // by how many bytes do we increment SP
        public static int PCIncBytes = 2; // by how many bytes do we increment PC
        //
        public static int WordsInCommand = 2; // how many words there are in a single command
        public static int BytesInWord = 2; // how many bytes there are in a single word
        //
        public static int BitsInColorComponent = 1; // how many bits used to encode color
        public static int ColorComponents = 3; // R, G, B
        public static int PictureScaleFactor = 4; // how many pixel per virtual video pixel
        public static int MonitorHeight = 128;
        public static int MonitorWidth = 256;
        // 
        public static int VMSleepTimeout = 2000;

        // Memory segments

        public static Dictionary<string, int> MemorySizes =
        new Dictionary<string, int> {
            {"INT", 12},
            {"RAM", 16 * 1024},
            {"VRAM", 16 * 1024},
            {"EPROM", 1 * 1024 },
            {"ROM", 16 * 1024},
            {"REGISTERS", TotalRegistersCount * RegisterBytes}
        };

        public static Dictionary<string, int> MemoryOffsets =
        new Dictionary<string, int> {
            {"INT", 0},
            {"RAM", 0},
            {"VRAM", MemorySizes["RAM"]},
            {"EPROM", MemorySizes["RAM"] + MemorySizes["VRAM"]},
            {"ROM", MemorySizes["RAM"] + MemorySizes["VRAM"] + MemorySizes["EPROM"]},
            {"REGISTERS", MemorySizes["RAM"] + MemorySizes["VRAM"] + MemorySizes["EPROM"] + MemorySizes["ROM"]}
        };

        public static int TotalMemorySize = MemorySizes["RAM"] +
                                            MemorySizes["VRAM"] +
                                            MemorySizes["EPROM"] +
                                            MemorySizes["ROM"] +
                                            MemorySizes["REGISTERS"];

        // EPROM; different preset values are installed here

        public static Dictionary<string, int> EPROMSizes = new Dictionary<string, int>
        {
            {"ASCII", 1 * 1024 }, // mapping table: Scan Codes <-> ASCII
            {"FONT", 1 * 1024 },  // font resides here
            {"KB_HANDLER", 1 * 1024 }  // default keyboard interrupt handler
        };

        public static Dictionary<string, int> EPROMOffsets = new Dictionary<string, int>
        {
            {"ASCII", MemoryOffsets["EPROM"]},
            {"FONT", MemoryOffsets["EPROM"] + EPROMSizes["ASCII"]},
            {"KB_HANDLER", MemoryOffsets["EPROM"] + EPROMSizes["ASCII"] + EPROMSizes["FONT"]}
        };

        // Registers

        public static IDictionary<string, int> RegistersOffsets = 
        new Dictionary<string, int>
        {
            {"R0", MemoryOffsets["REGISTERS"] + RegisterBytes * 0},
            {"R1", MemoryOffsets["REGISTERS"] + RegisterBytes * 1},
            {"R2", MemoryOffsets["REGISTERS"] + RegisterBytes * 2},
            {"R3", MemoryOffsets["REGISTERS"] + RegisterBytes * 3},
            {"R4", MemoryOffsets["REGISTERS"] + RegisterBytes * 4},
            {"R5", MemoryOffsets["REGISTERS"] + RegisterBytes * 5},
            {"SP", MemoryOffsets["REGISTERS"] + RegisterBytes * 6},
            {"PC", MemoryOffsets["REGISTERS"] + RegisterBytes * 7},

            {"SW", MemoryOffsets["REGISTERS"] + RegisterBytes * 8},
            {"KH", MemoryOffsets["REGISTERS"] + RegisterBytes * 9}
        };
        public static IEnumerable<string> RegisterNames = RegistersOffsets.Keys;

        // Flags

        public static IDictionary<string, int> StatusFlagBitOffsets = new Dictionary<string, int>
        {
            {"C", 0},
            {"V", 1},
            {"Z", 2},
            {"N", 3},
            {"T", 4}
        };
        public static IEnumerable<string> StatusFlagNames = StatusFlagBitOffsets.Keys;
    }
}