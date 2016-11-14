using System;
using System.Collections.Generic;

namespace AlmostPDP11.VM.Decoder
{
    public class Consts
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
            new[]{ Mnemonic.BR  , Mnemonic.BNE , Mnemonic.BEQ , Mnemonic.BGE , Mnemonic.BLT , Mnemonic.BGT , Mnemonic.BLE , Mnemonic.BPL , Mnemonic.BMI , Mnemonic.BHI , Mnemonic.BLOS, Mnemonic.BVC , Mnemonic.BVS , Mnemonic.BCC , Mnemonic.BCS } );

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
}