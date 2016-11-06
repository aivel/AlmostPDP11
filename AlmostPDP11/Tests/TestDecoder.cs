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
            HashSet<Int16> set = new HashSet<short>(new short[]{1,2,3});
            Assert.AreEqual(Decoder.GetMnemonic(36864), Mnemonic.MOVB);
            Assert.AreEqual(Decoder.GetMnemonic(57344), Mnemonic.SUB);
            Assert.AreEqual(Decoder.GetMnemonic(28699), Mnemonic.MUL);
            Assert.AreEqual(Decoder.GetMnemonic(30720), Mnemonic.XOR);
            Assert.AreEqual(Decoder.GetMnemonic(2624), Mnemonic.COM);
            Assert.AreEqual(Decoder.GetMnemonic(1280), Mnemonic.BLT);
        }
    }
}
