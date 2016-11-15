using System;
using System.Collections.Generic;
using System.IO;
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
            var path = "";
            var file = File.OpenRead(path + "AlmostPDP11/Resources/Keyboard_Driver.otasm"); // "Resources/Keyboard_Driver.otasm"
            using (var streamReader = new StreamReader(file))
            {
                var program = new List<string>();
                string line;
                while ((line = streamReader.ReadLine())!=null)
                {
                    program.Add(line);
                }
                var encoded = Assembler.Assembly(program, 0);
                using (var streamWriter = new StreamWriter(File.OpenWrite(path+"AlmostPDP11/Resources/Keyboard_Driver.mc")))
                {
                    foreach (var v in encoded)
                    {
                        streamWriter.WriteLine(v);
                        Console.WriteLine(v);
                    }
                }

            }
            file.Close();
        }
    }
}