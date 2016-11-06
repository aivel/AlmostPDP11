using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlmostPDP11.VM.Decoder;
using NUnit.Framework;
using Decoder = AlmostPDP11.VM.Decoder.Decoder;

namespace AlmostPDP11.Tests
{
    [TestFixture]
    class TestDecoder
    {
        [Test]
        public void Test_Something()
        {
            Command command = Decoder.Decode(36888);//MOVB
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            command = Decoder.Decode(57356);//SUB
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            command = Decoder.Decode(2630);//COM
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            command = Decoder.Decode(1300);//BLT
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

        }
    }
}
