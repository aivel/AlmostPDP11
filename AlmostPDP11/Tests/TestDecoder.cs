using System;
using System.Collections.Generic;
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
            Command command = Decoder.Decode(new ushort[]{ 36888,223});//MOVB
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine("R" + command.Operands[Decoder.DEST]);
            Console.WriteLine();

            command = Decoder.Decode(new ushort[]{57356,54});//SUB
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            command = Decoder.Decode(new ushort[]{2630,45});//COM
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            command = Decoder.Decode(new ushort[]{1300,24});//BLT
            Console.WriteLine(command.Mnemonic);
            Console.WriteLine(command.MnemonicType);
            foreach(var a in command.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();


        }
    }
}
