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
            Decoded decoded = Decoder.Decode(36888);//MOVB
            Console.WriteLine(decoded.Mnemonic);
            Console.WriteLine(decoded.MnemonicType);
            foreach(var a in decoded.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            decoded = Decoder.Decode(57356);//SUB
            Console.WriteLine(decoded.Mnemonic);
            Console.WriteLine(decoded.MnemonicType);
            foreach(var a in decoded.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            decoded = Decoder.Decode(2630);//COM
            Console.WriteLine(decoded.Mnemonic);
            Console.WriteLine(decoded.MnemonicType);
            foreach(var a in decoded.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

            decoded = Decoder.Decode(1300);//BLT
            Console.WriteLine(decoded.Mnemonic);
            Console.WriteLine(decoded.MnemonicType);
            foreach(var a in decoded.Operands){
                Console.WriteLine(a);
            }
            Console.WriteLine();

        }
    }
}
