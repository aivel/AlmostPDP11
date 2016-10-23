using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VM;

namespace AlmostPDP11.Tests
{
    [TestFixture]
    class TestMemoryManager
    {
        private MemoryManager vm;

        [SetUp]
        public void Init()
        {
            vm = new MemoryManager();
        }

        [Test]
        public void Test_GetRegisters_ReturnsAllRegisters()
        {
            var reg = vm.GetRegisters();
            var registerNames = Consts.RegisterNames.ToList();

            foreach (var regName in reg.Keys)
            {
                Assert.Contains(regName, registerNames);
            }
        }

        [Test]
        public void Test_SetRegisters()
        {
            var registers = new Dictionary<string, ushort>();
            var regNames = Consts.RegisterNames;

            ushort i = 1;

            foreach (var regName in regNames)
            {
                registers[regName] = i;
                i += 1;
            }

            vm.SetRegisters(registers);

            var gotRegisters = vm.GetRegisters();

            Assert.AreEqual(registers, gotRegisters);
        }

        [Test]
        public void Test_SetStatusFlags()
        {
            var statusFlagNames = Consts.StatusFlagNames.ToArray();

            foreach (var flagName in statusFlagNames)
            {
                vm.SetStatusFlag(flagName, false);
            }

            foreach (var flagName in statusFlagNames)
            {
                Assert.AreEqual(false, vm.GetStatusFlag(flagName));
            }

            foreach (var flagName in statusFlagNames)
            {
                vm.SetStatusFlag(flagName, true);
            }

            foreach (var flagName in statusFlagNames)
            {
                Assert.AreEqual(true, vm.GetStatusFlag(flagName));
            }
        }
    }
}
