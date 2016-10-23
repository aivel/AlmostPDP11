using AlmostPDP11.VM.Extentions;
using NUnit.Framework;

namespace AlmostPDP11.Tests
{
    [TestFixture]
    class TestUtils
    {
        [Test]
        public void Test_SetBit_on_short()
        {
            const ushort a = 3455;
            var updatedA = a.SetBit(0, false);

            Assert.AreEqual(a - updatedA, 1);
        }

        [Test]
        public void Test_SetBit_on_byte()
        {
            const byte a = 35;
            var updatedA = a.SetBit(0, false);

            Assert.AreEqual(a - updatedA, 1);
        }

        [Test]
        public void Test_GetBit_on_short()
        {
            const ushort a = 5;
            Assert.AreEqual(true, a.GetBit(2));
            Assert.AreEqual(false, a.GetBit(1));
            Assert.AreEqual(true, a.GetBit(0));
        }

        [Test]
        public void Test_GetBit_on_byte()
        {
            const byte a = 4;
            Assert.AreEqual(true, a.GetBit(2));
            Assert.AreEqual(false, a.GetBit(0));
        }
    }
}
