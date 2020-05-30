using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using BitReaderWriter.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BitReaderWriter.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class BitReaderUnitTests
    {
        private IBitReader bitReader;
        private Mock<Stream> inputStreamMock;

        [TestInitialize]
        public void Setup()
        {
            inputStreamMock = new Mock<Stream>();

            bitReader = new BitReader(inputStreamMock.Object);
        }

        [TestMethod]
        public void TestThatReadBitCallsInputStreamReadByteOnce()
        {
            bitReader.ReadBit();

            inputStreamMock.Verify(x => x.ReadByte(), Times.Once);
        }

        [TestMethod]
        public void TestThatReadBitReturnsTheFirstBitInStream()
        {
            inputStreamMock.Setup(x => x.ReadByte()).Returns(Convert.ToByte("10111011", 2));

            var result = bitReader.ReadBit();

            Assert.AreEqual(1, result & 1);
        }

        [TestMethod]
        public void TestThatTwoCallsToReadBitReturnsTheFirstTwoBitsInStream()
        {
            inputStreamMock.Setup(x => x.ReadByte()).Returns(Convert.ToByte("10111011", 2));

            var bit1 = bitReader.ReadBit();
            var bit2 = bitReader.ReadBit();

            Assert.AreEqual(1, bit1 & 1);
            Assert.AreEqual(0, bit2 & 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestThatWhenNIsGreaterThan32ReadNBitsThrowsArgumentException()
        {
            bitReader.ReadNBits(33);
        }

        [TestMethod]
        public void TestThatWhenNIsLowerThan8ReadNBitsCallsInputStreamReadByteOnce()
        {
            bitReader.ReadNBits(7);

            inputStreamMock.Verify(x => x.ReadByte(), Times.Once);
        }

        [TestMethod]
        public void TestThatReadNBitsCallsInputStreamReadByteOnceEvery8Bits()
        {
            var n = 32;
            var expectedNumberOfCalls = n / 8;

            bitReader.ReadNBits(n);

            inputStreamMock.Verify(x => x.ReadByte(), Times.Exactly(expectedNumberOfCalls));
        }

        [TestMethod]
        public void TestThatReadNBitsReadsCorrectlyTheFirst5Bits()
        {
            inputStreamMock.Setup(x => x.ReadByte()).Returns(Convert.ToByte("10111011", 2));

            var result = bitReader.ReadNBits(5);

            Assert.IsTrue(result == 23);
        }

        [TestMethod]
        public void TestThatReadNBitsReadsCorrectlyOneWord()
        {
            var byteHigh = Convert.ToByte("10111011", 2);
            var byteLow = Convert.ToByte("10111011", 2);
            inputStreamMock
                .SetupSequence(x => x.ReadByte())
                .Returns(byteHigh)
                .Returns(byteLow);
            var expectedResult = (byteHigh << 8) + byteLow;

            var result = bitReader.ReadNBits(16);

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void TestThatReadNBitsReadsCorrectlyOneInt()
        {
            var expectedResult = 2147003647;
            inputStreamMock
                .SetupSequence(x => x.ReadByte())
                .Returns(expectedResult >> 24)
                .Returns((expectedResult << 8) >> 24)
                .Returns((expectedResult << 16) >> 24)
                .Returns((expectedResult << 24) >> 24);

            var result = bitReader.ReadNBits(32);

            Assert.IsTrue(result == expectedResult);
        }

        [TestMethod]
        public void TestThatDisposeCallsInputStreamCloseOnce()
        {
            bitReader.Dispose();

            inputStreamMock.Verify(x => x.Close(), Times.Once);
        }
    }
}
