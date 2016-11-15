using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using AlmostPDP11.VM.Decoder;


namespace VM.Assembler
{
    public class Assembler
    {

        public static IEnumerable<ushort> Assembly(IEnumerable<string> program, int baseAddress)
        {
            int useWordsCount = 2;
            //delete comments,
            program = program.Where(s => !s.StartsWith(";;"))
                .Select(s => s.Split(new[]{";;"},StringSplitOptions.RemoveEmptyEntries)[0].Trim());

            var programArray = program.ToArray();
            var result = new List<ushort>();
            for (var i = 0; i < programArray.Length;)
            {
                var forEncoding = new List<string>();
                for (var j = 0; j < useWordsCount; j++)
                {
                    var index = i + j;
                    if (index<programArray.Length)
                    {
                        var str = programArray[index];
                        forEncoding.Add(str);
                    }
                }
                var command = Encoder.GetCommand(forEncoding,baseAddress);
                if (command.Mnemonic == Mnemonic.ERR)
                {
                    throw new InvalidConstraintException(
                        "Invalid code for Assambling by Assembly():\n"
                        +forEncoding[0]+"\n"+forEncoding[1]);
                }

                result.AddRange(command.ToMachineCode()); //add commands in machine code representation

                var commandLength = command.Operands.ContainsKey(DecoderConsts.COMMANDWORDSLENGTH)?
                    command.Operands[DecoderConsts.COMMANDWORDSLENGTH]:1;

                i += commandLength;
            }
            return result;
        }
    }
}