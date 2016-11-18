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

            var comm = Encoder.GetCommand(new string[2]{"MOV 2%7,0%3", "1234"});

            Assert.AreEqual(Mnemonic.MOV,comm.Mnemonic);
            Assert.AreEqual(MnemonicType.DoubleOperand, comm.MnemonicType);
            Console.WriteLine(comm.Mnemonic);
            Console.WriteLine(comm.MnemonicType);
            Console.WriteLine(comm.ToMachineCode());
        }
        /*
        [Test]
        public void ComplexTest()
        {
            var comm = Encoder.GetCommand("MOVB 3%5,3%2");
            var transComm = Decoder.Decode(comm.ToMachineCode());
            Assert.AreEqual(comm.Mnemonic,transComm.Mnemonic);


            comm = Encoder.GetCommand("DIV 2,1%1");
            transComm = Decoder.Decode(comm.ToMachineCode());
            Assert.AreEqual(comm.Mnemonic,transComm.Mnemonic);

            comm = Encoder.GetCommand("CLR 1%1");
            transComm = Decoder.Decode(comm.ToMachineCode());
            Assert.AreEqual(comm.Mnemonic,transComm.Mnemonic);

            comm = Encoder.GetCommand("BR 124");
            transComm = Decoder.Decode(comm.ToMachineCode());
            Assert.AreEqual(comm.Mnemonic,transComm.Mnemonic);
        }
        */
    }
}