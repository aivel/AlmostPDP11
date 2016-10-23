using System;

namespace Decoder {
    public class Decoder {

        public static Decoded Decode(ushort input) {
            return null;
        }

        public static String decodeOppcode(String mnemonic) {
            return "";

        }

        public static Mnemonic getMnemonic(ushort input){

            //two-operand instructions
            int doubleOperand = Positioner.getBits(input,12,14);
            if(doubleOperand<7&&doubleOperand>0){//Double-operand instructions
                // int bOperand = Positioner.getBits(input,15);
                int result = Positioner.getBits(input,12,15);
                return (Mnemonic)(result<<12);
            }
            if(doubleOperand==7){//dditional two-operand instructions
                // int additional = Positioner.getBits(input,9,11);
                int result = Positioner.getBits(input,9,15);
                return (Mnemonic)(result<<9);
            }

            //single-operand instructions
            int singleOperand = Positioner.getBits(input,11,14);
            if(singleOperand == 1){//correct single-operand instruction
                int result = Positioner.getBits(input,6,15);
                return (Mnemonic)(result<<6);
            }
            else{//conditional instructions
                
               
                
                int result = Positioner.getBits(input,8,15);
                return (Mnemonic)(result<<8);
            }



            return (Mnemonic)Positioner.getBits(input,0,15);
        }

    }

    //Decoded information about command
    public class Decoded {
        Mnemonic mnemonic { get; }

        int oppcode { get; set; }

        int sourceMode { get; }

        int destMode { get; }

        int sourceAddr { get; }

        int destAdrr { get; }

        Decoded(Mnemonic mnemonic, int oppcode, int sourceMode, int sourceAddr, int destMode, int destAddr) {
            this.mnemonic = mnemonic;
            this.oppcode = oppcode;
            this.sourceMode = sourceMode;
            this.sourceAddr = sourceAddr;
            this.destMode = destMode;
            this.destAdrr = destAdrr;
        }
    }



    //a utility class for getting values of bits in incoming words
    public class Positioner{
        /*
           00000001
           ...
           10000000
           */
        private static ushort[] possitionMultipliers = new ushort[16]{1,2,4,8,16,32,64,128,256,
            512,1024,2048,4096,8192,16384,32768};

        public static ushort getBits(ushort input,int possition){
            return  getBits(input,possition,possition);
        }

        public static ushort getBits(ushort input, int beginPossition,int endPossition){
            if(beginPossition>endPossition){
                throw new Exception("Begin possition more than end possition in getBits from Positioner");
            }

            if(beginPossition<0||beginPossition>possitionMultipliers.Length||
                    endPossition<0||endPossition>possitionMultipliers.Length){
                throw new Exception("Illegal possition in getBits() from Positioner");
            }

            int positions=0;

            for (int i = beginPossition; i <=endPossition ; i+=1) {
                positions = positions | possitionMultipliers[i];
            }

            return (ushort)((input & positions)>>beginPossition);
        }
    }

    //private int EIGHT_POW_4 = 4096;

    public enum Mnemonic{
        ERR=0,//ERROR
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
    };

    enum DoublOperand{
        MOVB,
        MOV ,
        CMP ,
        CMPB,
        BIT ,
        BITB,
        BIC ,
        BICB,
        BIS ,
        BISB,
        ADD ,
        SUB 
    };

    enum TwoOperand{
        MUL,
        DIV,
        ASH,
        ASHC,
        XOR,
        SOB
    };

    enum SingleOperand{
        SWAB,
        CLR ,
        CLRB,
        COM ,
        COMB,
        INC ,
        INCB,
        DEC ,
        DECB,
        NEG ,
        NEGB,
        ADC ,
        ADCB,
        SBC ,
        SBCB,
        TST ,
        TSTB,
        ROR ,
        RORB,
        ROL ,
        ROLB,
        ASR ,
        ASRB,
        ASL ,
        ASLB,
        MARK,
        MTPS,
        MFPI,
        MFPD,
        MTPI,
        MTPD,
        SXT ,
        MFPS
    };

    enum ConditionalBranch{
        BR  ,
        BNE ,
        BEQ ,
        BGE ,
        BLT ,
        BGT ,
        BLE ,
        BPL ,
        BMI ,
        BHI ,
        BLOS,
        BVC ,
        BVS ,
        BCC ,
        BCS 
    };
}
