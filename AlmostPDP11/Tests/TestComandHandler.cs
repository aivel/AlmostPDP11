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

namespace AlmostPDP11.Tests
{
    public class TestComandHandler
    {
        [Test]
        public void Test_Something()
        {
            Command command = Decoder.Decode(4739); //MOV
            MemoryManager _memoryManager = new MemoryManager();
            ComandHandler _comandHandler = new ComandHandler(_memoryManager);
            _memoryManager.SetRegister("R2", 123);
            _memoryManager.SetRegister("R3", 321);
            _comandHandler.Operation(command);
            //string sourceaddr = "R" + command.Operands[Decoder.SOURCE];
            //ushort src = _memoryManager.GetRegister(sourceaddr);
            //Console.WriteLine(sourceaddr);
            //Console.WriteLine(_memoryManager.GetRegister("R2"));
            Console.WriteLine(_memoryManager.GetRegister("R3"));
        }
    }
}