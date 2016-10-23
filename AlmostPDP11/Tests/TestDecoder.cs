using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Decoder;
using NUnit.Framework;
using Decoder = Decoder.Decoder;

namespace AlmostPDP11.Tests
{
    [TestFixture]
    class TestDecoder
    {
        [Test]
        public void Test_Something()
        {
            HashSet<Int16> set = new HashSet<short>(new short[]{1,2,3});
            Assert.AreEqual(global::Decoder.Decoder.getMnemonic(36864), Mnemonic.MOVB);
            Assert.AreEqual(global::Decoder.Decoder.getMnemonic(57344), Mnemonic.SUB);
            Assert.AreEqual(global::Decoder.Decoder.getMnemonic(28699), Mnemonic.MUL);
            Assert.AreEqual(global::Decoder.Decoder.getMnemonic(30720), Mnemonic.XOR);
            Assert.AreEqual(global::Decoder.Decoder.getMnemonic(2624), Mnemonic.COM);
            Assert.AreEqual(global::Decoder.Decoder.getMnemonic(1280), Mnemonic.BLT);
        }
    }
}
