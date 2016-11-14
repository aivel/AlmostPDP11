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
                opps.Add(Consts.SOURCE_MODE,Int16.Parse(op1[0]));
                opps.Add(Consts.SOURCE,Int16.Parse(op1[1]));
                opps.Add(Consts.DEST_MODE,Int16.Parse(op2[0]));
                opps.Add(Consts.DEST, Int16.Parse(op2[1]));

                if (opps[Consts.SOURCE_MODE] == 2 && opps[Consts.SOURCE] == 7)//use the second word for Incremental mode
                {
                    usedWords++;
                    var value = Int16.Parse(textCommandArray[1]);
                    opps.Add(Consts.VALUE,value);
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

                opps.Add(Consts.REG,Int16.Parse(op1[0]));
                opps.Add(Consts.MODE,Int16.Parse(op1[1]));
                opps.Add(Consts.SRC_DEST,Int16.Parse(op2[0]));
                if (opps[Consts.MODE] == 2 && opps[Consts.SRC_DEST] == 7)//use the second word for Incremental mode
                {
                    usedWords++;
                    var value = Int16.Parse(textCommandArray[1]);
                    opps.Add(Consts.VALUE,value);
                }
            }
            if  (type == MnemonicType.SingleOperand)
            {
                String[] operand = parts[1].Trim().Split(MOD_DELIMETER);
                if (operand.Length != 2)
                {
                    return new Command();
                }
                opps.Add(Consts.MODE,Int16.Parse(operand[0]));
                opps.Add(Consts.REG,Int16.Parse(operand[1]));
            }
            if  (type == MnemonicType.ConditionalBranch)
            {
                opps.Add(Consts.OFFSET,Int16.Parse(parts[1]));

            }
            if (type == MnemonicType.ERR)
            {
                return new Command();
            }

            opps.Add(Consts.COMMANDWORDSLENGTH,usedWords);
            return new Command(mnemonic:mnemonic,mnemonicType:type,operands:opps);
        }
    }
}