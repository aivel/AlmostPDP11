using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlmostPDP11.VM.Decoder;
using AlmostPDP11.VM.Executor;
using NUnit.Framework;
using Decoder = AlmostPDP11.VM.Decoder.Decoder;
using VM;
using Encoder = AlmostPDP11.VM.Decoder.Encoder;

namespace AlmostPDP11.Tests
{
    public class TestComandHandler
    {
        [Test]
        public void Test_Something()
        {
            Command command = Encoder.GetCommand(new string[2]{"MOV 2%7,1%3", "1234"});
            MemoryManager _memoryManager = new MemoryManager();
            ComandHandler _comandHandler = new ComandHandler(_memoryManager);
            _memoryManager.SetRegister("R2", 232);
            _memoryManager.SetRegister("R3", 321);
            byte[] word = new byte[2];
            word[0] = 0;
            word[1] = 123;
            _memoryManager.SetMemory(321, word);
            Console.WriteLine((word[0] << 8) + word[1]);

            _comandHandler.Operation(command);
            //string sourceaddr = "R" + command.Operands[Decoder.SOURCE];
            //ushort src = _memoryManager.GetRegister(sourceaddr);
            //Console.WriteLine(sourceaddr);
            //Console.WriteLine(_memoryManager.GetRegister("R3"));
            word = _memoryManager.GetMemory(321, 2);
            Console.WriteLine((word[0] << 8) + word[1]);
        }
    }
}