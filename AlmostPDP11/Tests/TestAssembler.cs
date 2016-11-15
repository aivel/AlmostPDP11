using System;
using System.Runtime.InteropServices.ComTypes;
using AlmostPDP11.VM.Decoder;
using NUnit.Framework;
using VM.Assembler;

namespace AlmostPDP11.Tests
{
    [TestFixture]
    public class TestAssembler
    {
        [Test]
        public void Test_Assembler_and_Decoder()
        {
            var program = new[]
            {
                "MOV 2%7,0%1 ;; some comments",
                ";;just a comment",
                "1234;;value for loading"
            };
            var encoded = Assembler.Assembly(program, 0);
            var decoded = Decoder.Decode(encoded);
            Assert.AreEqual(encoded,decoded.ToMachineCode());

        }

        [Test]
        public void Test_Complex_Program()
        {
            var program = new[]
            {
                "MOV 0%5,0%1",
                ";; 1234",
                "mov 0%7,0%0"
                    
            };
            var encoded = Assembler.Assembly(program, 0);
        }
    }
}