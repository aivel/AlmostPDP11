using System;
using AlmostPDP11.VM.Decoder;
using NUnit.Framework;

namespace AlmostPDP11.Tests
{
    [TestFixture]
    public class TestEncoder
    {
        [Test]
        public void test_GetCommand()
        {
            Command comm = Encoder.GetCommand("MOV 1%2,3%6");
            Assert.AreEqual(Mnemonic.MOV,comm.Mnemonic);
            Assert.AreEqual(MnemonicType.DoubleOperand, comm.MnemonicType);
            Console.WriteLine(comm.Mnemonic);
            Console.WriteLine(comm.MnemonicType);
            foreach (var i in comm.Operands)
            {
                Console.WriteLine(i);
            }
        }
    }
}