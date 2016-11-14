using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmostPDP11.VM.Decoder {
    public class Decoder {
        /* return Command object
            operands are different:
                DoubleOperand: SourceMod,Source,DestMod,Dest
                TwoOperand: Register, Mod, Src/Dest
                SingleOperand: Mode, Register,
                ConditionalBranch: Offset
            If ERROR then operands has atribute ERR and MnemonicType has value ERR and\or Mnemonic has value ERR
        */
        public static Command Decode(IEnumerable<ushort> inputs)
        {
            var inputsArray = inputs.ToArray();
            var input = inputsArray[0];
            Mnemonic mnemonic = GetMnemonic(input);
            MnemonicType type = GetMnemonicType(mnemonic);
            Dictionary<string, short> operands = new Dictionary<string,short>();
            var usedWords = 1;

            if(type==MnemonicType.DoubleOperand){
                operands.Add(DecoderConsts.SOURCE_MODE,Positioner.GetBits(input,9,11));
                operands.Add(DecoderConsts.SOURCE,Positioner.GetBits(input,6,8));
                operands.Add(DecoderConsts.DEST_MODE,Positioner.GetBits(input,3,5));
                operands.Add(DecoderConsts.DEST,Positioner.GetBits(input,0,2));

                if (operands[DecoderConsts.SOURCE_MODE] == 2 && operands[DecoderConsts.SOURCE] == 7)//use the second word for Incremental mode
                {
                    usedWords++;
                    var value = (short)inputsArray[1];
                    operands.Add(DecoderConsts.VALUE,value);
                }

            }else if(type==MnemonicType.TwoOperand){
                operands.Add(DecoderConsts.REG,Positioner.GetBits(input,6,8));
                operands.Add(DecoderConsts.MODE,Positioner.GetBits(input,3,5));
                operands.Add(DecoderConsts.SRC_DEST,Positioner.GetBits(input,0,2));

                if (operands[DecoderConsts.MODE] == 2 && operands[DecoderConsts.SRC_DEST] == 7)//use the second word for Incremental mode
                {
                    usedWords++;
                    var value = (short)inputsArray[1];
                    operands.Add(DecoderConsts.VALUE,value);
                }
            }else if(type==MnemonicType.SingleOperand){
                operands.Add(DecoderConsts.MODE,Positioner.GetBits(input,3,5));
                operands.Add(DecoderConsts.REG,Positioner.GetBits(input,0,2));
            }else if (type==MnemonicType.ConditionalBranch){
                operands.Add(DecoderConsts.OFFSET,Positioner.GetBits(input,0,7));
            }else{
                operands.Add(DecoderConsts.ERR,1);
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
            if(DecoderConsts.DoublOperand.Contains(mnemonic)){
                return MnemonicType.DoubleOperand;
            }
            if(DecoderConsts.TwoOperand.Contains(mnemonic)){
                return MnemonicType.TwoOperand;
            }
            if(DecoderConsts.SingleOperand.Contains(mnemonic)){
                return MnemonicType.SingleOperand;
            }
            if(DecoderConsts.ConditionalBranch.Contains(mnemonic)){
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

        public Dictionary<string,short> Operands{get;}

        //Returns in case of ERROR
        public Command()
        {
            Mnemonic = Mnemonic.ERR;
            MnemonicType = MnemonicType.ERR;
            Operands = null;
        }

        public Command(Mnemonic mnemonic, MnemonicType mnemonicType ,Dictionary<String,short> operands) {
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
                    result = (ushort) (result + (Operands[DecoderConsts.SOURCE_MODE]<<9) + (Operands[DecoderConsts.SOURCE]<<6)
                                       + (Operands[DecoderConsts.DEST_MODE]<<3) + Operands[DecoderConsts.DEST]);
                    break;
                case MnemonicType.TwoOperand:
                    result = (ushort) (result + (Operands[DecoderConsts.MODE]<<6) + (Operands[DecoderConsts.REG]<<3) + Operands[DecoderConsts.SRC_DEST]);
                    break;
                case MnemonicType.SingleOperand:
                    result = (ushort) (result + (Operands[DecoderConsts.MODE]<<3) + Operands[DecoderConsts.REG]);
                    break;
                case MnemonicType.ConditionalBranch:
                    result += (ushort)Operands[DecoderConsts.OFFSET];
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
        public static short GetBits(ushort input,int possition){
            return  GetBits(input,possition,possition);
        }

        //get numeric value of bits from beginPossition to endPossition
        public static short GetBits(ushort input, int beginPossition,int endPossition){
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

            return (short)((input & positions)>>beginPossition);
        }
    }
}
