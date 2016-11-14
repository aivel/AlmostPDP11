using System;
using System.Collections.Generic;

namespace AlmostPDP11.VM.Decoder {
    public class Decoder {

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

        public static readonly String SOURCE_MODE = "SourceMode",
            SOURCE = "Source",
            DEST_MODE = "DestMode",
            DEST = "Dest",
            REG = "Reg",
            MODE = "Mode",
            SRC_DEST = "Src/Dest",
            OFFSET = "Offset",
            ERR = "ERR",
            VALUE = "Value";

        /* return Command object
            operands are different:
                DoubleOperand: SourceMod,Source,DestMod,Dest
                TwoOperand: Register, Mod, Src/Dest
                SingleOperand: Mode, Register,
                ConditionalBranch: Offset
            If ERROR then operands has atribute ERR and MnemonicType has value ERR and\or Mnemonic has value ERR
        */
        public static Command Decode(ushort input) {
            Mnemonic mnemonic = GetMnemonic(input);
            MnemonicType type = GetMnemonicType(mnemonic);
            Dictionary<String,UInt16> operands = new Dictionary<String,UInt16>();

            if(type==MnemonicType.DoubleOperand){
                operands.Add(SOURCE_MODE,Positioner.GetBits(input,9,11));
                operands.Add(SOURCE,Positioner.GetBits(input,6,8));
                operands.Add(DEST_MODE,Positioner.GetBits(input,3,5));
                operands.Add(DEST,Positioner.GetBits(input,0,2));
            }else if(type==MnemonicType.TwoOperand){
                operands.Add(REG,Positioner.GetBits(input,6,8));
                operands.Add(MODE,Positioner.GetBits(input,3,5));
                operands.Add(SRC_DEST,Positioner.GetBits(input,0,2));
            }else if(type==MnemonicType.SingleOperand){
                operands.Add(MODE,Positioner.GetBits(input,3,5));
                operands.Add(REG,Positioner.GetBits(input,0,2));
            }else if (type==MnemonicType.ConditionalBranch){
                operands.Add(OFFSET,Positioner.GetBits(input,0,7));
            }else{
                operands.Add(ERR,1);
            }

            return new Command(mnemonic,type,operands);
        }


        //return mnomonic
        private static Mnemonic GetMnemonic(ushort input){

            //two-operand instructions
            int doubleOperand = Positioner.GetBits(input,12,14);
            if(doubleOperand<7&&doubleOperand>0){//Double-operand instructions
                int result = Positioner.GetBits(input,12,15);
                return (Mnemonic)(result<<12);
            }
            if(doubleOperand==7){//additional two-operand instructions
                int result = Positioner.GetBits(input,9,15);
                return (Mnemonic)(result<<9);
            }

            //single-operand instructions
            int singleOperand = Positioner.GetBits(input,11,14);
            if(singleOperand == 1){//correct single-operand instruction
                int result = Positioner.GetBits(input,6,15);
                return (Mnemonic)(result<<6);
            }
            else{//conditional instructions
                int result = Positioner.GetBits(input,8,15);
                return (Mnemonic)(result<<8);
            }

            return Mnemonic.ERR;
        }

        //return mnemonic type
        public static MnemonicType GetMnemonicType(Mnemonic mnemonic){
            if(DoublOperand.Contains(mnemonic)){
                return MnemonicType.DoubleOperand;
            }
            if(TwoOperand.Contains(mnemonic)){
                return MnemonicType.TwoOperand;
            }
            if(SingleOperand.Contains(mnemonic)){
                return MnemonicType.SingleOperand;
            }
            if(ConditionalBranch.Contains(mnemonic)){
                return MnemonicType.ConditionalBranch;
            }
            return MnemonicType.ERR;
        }
    }

    /*Command information about instruction

    */
    public class Command {
        public Mnemonic Mnemonic { get; }
        public MnemonicType MnemonicType {get;}

        public Dictionary<string,ushort> Operands{get;}

        //Returns in case of ERROR
        public Command()
        {
            Mnemonic = Mnemonic.ERR;
            MnemonicType = MnemonicType.ERR;
            Operands = null;
        }

        public Command(Mnemonic mnemonic, MnemonicType mnemonicType ,Dictionary<String,ushort> operands) {
            Mnemonic = mnemonic;
            MnemonicType = mnemonicType;
            Operands = operands;
        }

        public ushort ToMachineCode()
        {
            var result = (ushort) Mnemonic;
            switch (MnemonicType)
            {
                case MnemonicType.DoubleOperand:
                    result = (ushort) (result + (Operands[Decoder.SOURCE_MODE]<<9) + (Operands[Decoder.SOURCE]<<6)
                                       + (Operands[Decoder.DEST_MODE]<<3) + Operands[Decoder.DEST]);
                    break;
                case MnemonicType.TwoOperand:
                    result = (ushort) (result + (Operands[Decoder.MODE]<<6) + (Operands[Decoder.REG]<<3) + Operands[Decoder.SRC_DEST]);
                    break;
                case MnemonicType.SingleOperand:
                    result = (ushort) (result + (Operands[Decoder.MODE]<<3) + Operands[Decoder.REG]);
                    break;
                case MnemonicType.ConditionalBranch:
                    result += Operands[Decoder.OFFSET];
                    break;
                case MnemonicType.ERR:
                    result = 0;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
    }



    //a utility class for getting values of bits in incoming words
    class Positioner{
        /*
           00000001
           ...
           10000000
           */
        private static readonly ushort[] PossitionMultipliers = {1,2,4,8,16,32,64,128,256,
            512,1024,2048,4096,8192,16384,32768};

        //get value of bit at possition
        public static ushort GetBits(ushort input,int possition){
            return  GetBits(input,possition,possition);
        }

        //get numeric value of bits from beginPossition to endPossition
        public static ushort GetBits(ushort input, int beginPossition,int endPossition){
            if(beginPossition>endPossition){
                throw new Exception("Begin possition more than end possition in GetBits from Positioner");
            }

            if(beginPossition<0||beginPossition>PossitionMultipliers.Length||
               endPossition<0||endPossition>PossitionMultipliers.Length){
                throw new Exception("Illegal possition in GetBits() from Positioner");
            }

            var positions=0;

            for (var i = beginPossition; i <=endPossition ; i+=1) {
                positions = positions | PossitionMultipliers[i];
            }

            return (ushort)((input & positions)>>beginPossition);
        }
    }

    //mapping from oppcode to mnemonic
    public enum Mnemonic{
        ERR, //error value
        MOV=4096,
        MOVB=36864,
        CMP=8192,
        CMPB=40960,
        BIT=12288,
        BITB=45056,
        BIC=16384,
        BICB=49152,
        BIS=20480,
        BISB=53248,
        ADD=24576,
        SUB=57344,
        MUL=28672,
        DIV=29184,
        ASH=29696,
        ASHC=30208,
        XOR=30720,
        SOB=32256,
        SWAB=192,
        CLR=2560,
        CLRB=35328,
        COM=2624,
        COMB=35392,
        INC=2688,
        INCB=35456,
        DEC=2752,
        DECB=35520,
        NEG=2816,
        NEGB=35584,
        ADC=2880,
        ADCB=35648,
        SBC=2944,
        SBCB=35712,
        TST=3008,
        TSTB=35776,
        ROR=3072,
        RORB=35840,
        ROL=3136,
        ROLB=35904,
        ASR=3200,
        ASRB=35968,
        ASL=3264,
        ASLB=36032,
        MARK=3328,
        MTPS=36096,
        MFPI=3392,
        MFPD=36160,
        MTPI=3456,
        MTPD=36224,
        SXT=3520,
        MFPS=36288,
        BR=256,
        BNE=512,
        BEQ=768,
        BGE=1024,
        BLT=1280,
        BGT=1536,
        BLE=1792,
        BPL=32768,
        BMI=33024,
        BHI=33280,
        BLOS=33536,
        BVC=33792,
        BVS=34048,
        BCC=34304,
        BCS=34560

    }

    //types of instructions
    public enum MnemonicType{
        ERR,//error value
        DoubleOperand,TwoOperand,SingleOperand,ConditionalBranch
    }
}
