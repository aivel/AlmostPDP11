using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using VM;

namespace AlmostPDP11.Tests
{
    [TestFixture]
    class TestVirtualMachine
    {
        private VirtualMachine vm;

        [SetUp]
        public void Init()
        {
            vm = new VirtualMachine();
        }

        [Test]
        public void Test_Stack()
        {
            ushort[] valuesToStack = { 123 , 124, 125};

            foreach (var @ushort in valuesToStack)
            {
                vm.PushToStack(@ushort);
            }

            var valuesFromStack = new ushort[valuesToStack.Length];

            for (var i = 0; i < valuesToStack.Length; i++)
            {
                valuesFromStack[valuesToStack.Length - i - 1] = vm.PopFromStack();
            }

            Assert.AreEqual(valuesToStack, valuesFromStack);
        }
    }
}
