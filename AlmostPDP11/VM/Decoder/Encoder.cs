using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AlmostPDP11.VM.Decoder
{
    public class Encoder
    {
        public static readonly Char OPPERAND_DELIMETER = ' ';
        public static readonly Char OPERANDS_DELIMETER = ',';
        public static readonly Char MOD_DELIMETER = '%';

        public static Command GetCommand(IEnumerable<string> textCommands)
        {
            var textCommandArray = textCommands.ToArray();
            var textCommand = textCommandArray[0];
            String[] parts = textCommand.Trim().Split(OPPERAND_DELIMETER);
            Mnemonic mnemonic = (Mnemonic) Enum.Parse(typeof(Mnemonic), parts[0], true);
            MnemonicType type = Decoder.GetMnemonicType(mnemonic);
            short usedWords = 1;
            var opps = new Dictionary<string, short>();

            if (type == MnemonicType.DoubleOperand)
            {
                String[] operands = parts[1].Trim().Split(OPERANDS_DELIMETER);
                if (operands.Length != 2)
                {
                    return new Command();//ERROR
                }
                var op1 = operands[0].Trim().Split(MOD_DELIMETER);
                var op2 = operands[1].Trim().Split(MOD_DELIMETER);
                if (op1.Length != 2 || op2.Length != 2)
                {
                    return new Command();//ERROR
                }
                opps.Add(DecoderConsts.SOURCE_MODE,Int16.Parse(op1[0]));
                opps.Add(DecoderConsts.SOURCE,Int16.Parse(op1[1]));
                opps.Add(DecoderConsts.DEST_MODE,Int16.Parse(op2[0]));
                opps.Add(DecoderConsts.DEST, Int16.Parse(op2[1]));

                if (opps[DecoderConsts.SOURCE_MODE] == 2 && opps[DecoderConsts.SOURCE] == 7)//use the second word for Incremental mode
                {
                    usedWords++;
                    var value = Int16.Parse(textCommandArray[1]);
                    opps.Add(DecoderConsts.VALUE,value);
                }
            }
            if  (type == MnemonicType.TwoOperand)
            {
                var operands = parts[1].Trim().Split(OPERANDS_DELIMETER);
                if (operands.Length != 2)
                {
                    return new Command();//ERROR
                }
                var op1 = operands[0].Trim().Split(MOD_DELIMETER);
                var op2 = operands[1].Trim().Split(MOD_DELIMETER);
                if (op1.Length != 2 )
                {
                    return new Command();//ERROR
                }

                opps.Add(DecoderConsts.REG,Int16.Parse(op1[0]));
                opps.Add(DecoderConsts.MODE,Int16.Parse(op1[1]));
                opps.Add(DecoderConsts.SRC_DEST,Int16.Parse(op2[0]));
                if (opps[DecoderConsts.MODE] == 2 && opps[DecoderConsts.SRC_DEST] == 7)//use the second word for Incremental mode
                {
                    usedWords++;
                    var value = Int16.Parse(textCommandArray[1]);
                    opps.Add(DecoderConsts.VALUE,value);
                }
            }
            if  (type == MnemonicType.SingleOperand)
            {
                String[] operand = parts[1].Trim().Split(MOD_DELIMETER);
                if (operand.Length != 2)
                {
                    return new Command();
                }
                opps.Add(DecoderConsts.MODE,Int16.Parse(operand[0]));
                opps.Add(DecoderConsts.REG,Int16.Parse(operand[1]));
            }
            if  (type == MnemonicType.ConditionalBranch)
            {
                opps.Add(DecoderConsts.OFFSET,Int16.Parse(parts[1]));

            }
            if (type == MnemonicType.ERR)
            {
                return new Command();
            }

            opps.Add(DecoderConsts.COMMANDWORDSLENGTH,usedWords);
            return new Command(mnemonic:mnemonic,mnemonicType:type,operands:opps);
        }
    }
}