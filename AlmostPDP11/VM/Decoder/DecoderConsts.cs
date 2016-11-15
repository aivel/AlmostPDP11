using System;
using System.Collections.Generic;


namespace AlmostPDP11.VM.Decoder
{
    public class DecoderConsts
    {
        public static readonly HashSet<Mnemonic>  DoublOperand = new HashSet<Mnemonic>(
            new[]{Mnemonic.MOVB, Mnemonic.MOV , Mnemonic.CMP , Mnemonic.CMPB,
                Mnemonic.BIT , Mnemonic.BITB, Mnemonic.BIC , Mnemonic.BICB, Mnemonic.BIS ,
                Mnemonic.BISB, Mnemonic.ADD , Mnemonic.SUB });

        public static readonly HashSet<Mnemonic> TwoOperand = new HashSet<Mnemonic>(
            new[]{ Mnemonic.MUL, Mnemonic.DIV, Mnemonic.ASH, Mnemonic.ASHC, Mnemonic.XOR, Mnemonic.SOB});

        public static readonly HashSet<Mnemonic> SingleOperand = new HashSet<Mnemonic>(
            new[]{ Mnemonic.SWAB, Mnemonic.CLR , Mnemonic.CLRB, Mnemonic.COM , Mnemonic.COMB, Mnemonic.INC , Mnemonic.INCB, Mnemonic.DEC , Mnemonic.DECB, Mnemonic.NEG , Mnemonic.NEGB, Mnemonic.ADC , Mnemonic.ADCB, Mnemonic.SBC , Mnemonic.SBCB, Mnemonic.TST , Mnemonic.TSTB, Mnemonic.ROR , Mnemonic.RORB, Mnemonic.ROL , Mnemonic.ROLB, Mnemonic.ASR , Mnemonic.ASRB, Mnemonic.ASL , Mnemonic.ASLB, Mnemonic.MARK, Mnemonic.MTPS, Mnemonic.MFPI, Mnemonic.MFPD, Mnemonic.MTPI, Mnemonic.MTPD, Mnemonic.SXT , Mnemonic.MFPS } );

        public static readonly HashSet<Mnemonic> ConditionalBranch = new HashSet<Mnemonic>(
            new[]{ Mnemonic.BR  , Mnemonic.BNE , Mnemonic.BEQ , Mnemonic.BGE , Mnemonic.BLT , Mnemonic.BGT , Mnemonic.BLE ,
                Mnemonic.BPL , Mnemonic.BMI , Mnemonic.BHI , Mnemonic.BLOS, Mnemonic.BVC , Mnemonic.BVS , Mnemonic.BCC , Mnemonic.BCS,
                Mnemonic.JMP} );

        public static readonly String SOURCE_MODE = "SourceMode";
        public static readonly String SOURCE = "Source";
        public static readonly String DEST_MODE = "DestMode";
        public static readonly String DEST = "Dest";
        public static readonly String REG = "Reg";
        public static readonly String MODE = "Mode";
        public static readonly String SRC_DEST = "Src/Dest";
        public static readonly String OFFSET = "Offset";
        public static readonly String ERR = "ERR";
        public static readonly String VALUE = "Value";
        public static readonly String COMMANDWORDSLENGTH = "Used words for command";
    }

    //mapping from oppcode to mnemonic
    public enum Mnemonic
    {
        ERR, //error value
        MOV = 4096,
        MOVB = 36864,
        CMP = 8192,
        CMPB = 40960,
        BIT = 12288,
        BITB = 45056,
        BIC = 16384,
        BICB = 49152,
        BIS = 20480,
        BISB = 53248,
        ADD = 24576,
        SUB = 57344,
        MUL = 28672,
        DIV = 29184,
        ASH = 29696,
        ASHC = 30208,
        XOR = 30720,
        SOB = 32256,
        SWAB = 192,
        CLR = 2560,
        CLRB = 35328,
        COM = 2624,
        COMB = 35392,
        INC = 2688,
        INCB = 35456,
        DEC = 2752,
        DECB = 35520,
        NEG = 2816,
        NEGB = 35584,
        ADC = 2880,
        ADCB = 35648,
        SBC = 2944,
        SBCB = 35712,
        TST = 3008,
        TSTB = 35776,
        ROR = 3072,
        RORB = 35840,
        ROL = 3136,
        ROLB = 35904,
        ASR = 3200,
        ASRB = 35968,
        ASL = 3264,
        ASLB = 36032,
        MARK = 3328,
        MTPS = 36096,
        MFPI = 3392,
        MFPD = 36160,
        MTPI = 3456,
        MTPD = 36224,
        SXT = 3520,
        MFPS = 36288,
        BR = 256,
        BNE = 512,
        BEQ = 768,
        BGE = 1024,
        BLT = 1280,
        BGT = 1536,
        BLE = 1792,
        BPL = 32768,
        BMI = 33024,
        BHI = 33280,
        BLOS = 33536,
        BVC = 33792,
        BVS = 34048,
        BCC = 34304,
        BCS = 34560,
        JMP = 64
    }

    //types of instructions
    public enum MnemonicType
    {
        ERR,//error value
        DoubleOperand, TwoOperand, SingleOperand, ConditionalBranch
    }
}