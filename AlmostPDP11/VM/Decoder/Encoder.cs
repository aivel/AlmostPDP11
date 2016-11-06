using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace AlmostPDP11.VM.Decoder
{
    public class Encoder
    {
        public static readonly Char OPPERAND_DELIMETER = ' ';
        public static readonly Char OPERANDS_DELIMETER = ',';
        public static readonly Char MOD_DELIMETER = '%';

        public static Command GetCommand(String textCommand)
        {
            String[] parts = textCommand.Trim().Split(OPPERAND_DELIMETER);
            Mnemonic mnemonic = (Mnemonic) Enum.Parse(typeof(Mnemonic), parts[0], true);
            MnemonicType type = Decoder.GetMnemonicType(mnemonic);


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
                Dictionary<string, ushort> opps = new Dictionary<string, ushort>();
                opps.Add("SourceMod",UInt16.Parse(op1[0]));
                opps.Add("Source",UInt16.Parse(op1[1]));
                opps.Add("DestMod",UInt16.Parse(op2[0]));
                opps.Add("Dest", UInt16.Parse(op2[1]));
                return new Command(mnemonic:mnemonic,mnemonicType:type,operands:opps);
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
                Dictionary<string, ushort> opps = new Dictionary<string, ushort>();
                opps.Add("Register",UInt16.Parse(op1[0]));
                opps.Add("Mode",UInt16.Parse(op1[1]));
                opps.Add("Src/Dest",UInt16.Parse(op2[0]));
                return new Command(mnemonic:mnemonic,mnemonicType:type,operands:opps);
            }
            if  (type == MnemonicType.SingleOperand)
            {
                String[] operand = parts[1].Trim().Split(MOD_DELIMETER);
                if (operand.Length != 2)
                {
                    return new Command();
                }
                Dictionary<string, ushort> opps = new Dictionary<string, ushort>();
                opps.Add("Mode",UInt16.Parse(operand[0]));
                opps.Add("Register",UInt16.Parse(operand[1]));
                return new Command(mnemonic:mnemonic,mnemonicType:type,operands:opps);
            }
            if  (type == MnemonicType.TwoOperand)
            {
                Dictionary<string, ushort> opps = new Dictionary<string, ushort>();
                opps.Add("Offset",UInt16.Parse(parts[1]));
                return new Command(mnemonic:mnemonic,mnemonicType:type,operands:opps);
            }

            return new Command();//error
        }
    }
}