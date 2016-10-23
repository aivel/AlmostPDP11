using System.Collections.Generic;
using System.Linq;

namespace VM {
    public static class Consts {
        public static int GeneralPurposeRegistersCount = 8;
        public static int GeneralPurposeRegisterBytes = 2;
        public static int GeneralPurposeRegisterBits = GeneralPurposeRegisterBytes * 8;
        public static int StatusWordRegisterBytes = 2;
        public static int KeyboardHandlerBytes = 2;
        //
        public static int SPIncBytes = 2; // by how many bytes do we increment SP
        public static int PCIncBytes = 1; // by how many bytes do we increment PC

        public static Dictionary<string, int> MemorySizes =
        new Dictionary<string, int> {
            {"INT", 12},
            {"RAM", 16 * 1024},
            {"VRAM", 16 * 1024},
            {"ROM", 16 * 1024},
            {"REGISTERS", GeneralPurposeRegistersCount * GeneralPurposeRegisterBytes},
            {"STATUS_WORD", StatusWordRegisterBytes },
            {"KEYBOARD_HANDLER", KeyboardHandlerBytes}
        };

        public static Dictionary<string, int> MemoryOffsets =
        new Dictionary<string, int> {
            {"INT", 0},
            {"RAM", 0},
            {"VRAM", MemorySizes["RAM"]},
            {"ROM", MemorySizes["RAM"] + MemorySizes["VRAM"]},
            {"REGISTERS", MemorySizes["RAM"] + MemorySizes["VRAM"] + MemorySizes["ROM"]},
            {"STATUS_WORD", MemorySizes["RAM"] + MemorySizes["VRAM"] + MemorySizes["ROM"] + MemorySizes["REGISTERS"]},
            {"KEYBOARD_HANDLER", MemorySizes["RAM"] + MemorySizes["VRAM"] + MemorySizes["ROM"] + MemorySizes["REGISTERS"] + MemorySizes["STATUS_WORD"]}
        };

        public static int FullMemorySize = MemorySizes["RAM"] +
                                           MemorySizes["VRAM"] +
                                           MemorySizes["ROM"] +
                                           MemorySizes["REGISTERS"] +
                                           MemorySizes["STATUS_WORD"] +
                                           MemorySizes["KEYBOARD_HANDLER"];

        public static IDictionary<string, int> RegistersOffsets = 
        new Dictionary<string, int>
        {
            {"R0", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 0},
            {"R1", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 1},
            {"R2", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 2},
            {"R3", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 3},
            {"R4", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 4},
            {"R5", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 5},
            {"SP", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 6},
            {"PC", MemoryOffsets["REGISTERS"] + GeneralPurposeRegisterBytes * 7}
        };
        public static IEnumerable<string> RegisterNames = RegistersOffsets.Keys;

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