using BitReaderWriter.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace BitReaderWriter.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class BitWriterUnitTests
    {
        private IBitWriter bitWriter;
        private Mock<Stream> outputStreamMock;

        [TestInitialize]
        public void Setup()
        {
            outputStreamMock = new Mock<Stream>();

            bitWriter = new BitWriter(outputStreamMock.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestThatWhenNIsGreaterThan32WriteNBitsThrowsArgumentException()
        {
            bitWriter.WriteNBits(33, 1);
        }

        [TestMethod]
        public void TestThatWhenNIsLowerThan8WriteNBitsCallsOutputStreamWriteByteOnce()
        {
            using (bitWriter)
            {
                bitWriter.WriteNBits(5, 123);
            }

            outputStreamMock.Verify(x => x.WriteByte(It.IsAny<byte>()), Times.Once);
        }

        [TestMethod]
        public void TestThatWriteNBitsCallsOutputStreamWriteByteOnceEvery8Bits()
        {
            var n = 32;
            var expectedNumberOfCalls = n / 8;

            using (bitWriter)
            {
                bitWriter.WriteNBits(n, 123);
            }

            outputStreamMock.Verify(x => x.WriteByte(It.IsAny<byte>()), Times.Exactly(expectedNumberOfCalls));
        }

        [TestMethod]
        public void TestThatWhenNIsLowerThan8WriteNBitsCallsOutputStreamWriteByteWithExpectedByte()
        {
            var n = 5;
            var value = Convert.ToUInt32("10011", 2);
            byte expectedValueToBeWritten = Convert.ToByte("10011000", 2);

            using (bitWriter)
            {
                bitWriter.WriteNBits(n, value);
            }

            outputStreamMock.Verify(x => x.WriteByte(expectedValueToBeWritten), Times.Once);
        }

        [TestMethod]
        public void TestThatWhenNIs14WriteNBitsCallsOutputStreamWriteByteWithExpectedBytes()
        {
            var n = 14;
            var value = Convert.ToUInt32("01001110010111", 2);
            byte expectedHighByte = Convert.ToByte("01001110", 2);
            byte expectedLowByte = Convert.ToByte("01011100", 2);

            using (bitWriter)
            {
                bitWriter.WriteNBits(n, value);
            }

            outputStreamMock.Verify(x => x.WriteByte(expectedHighByte), Times.Once);
            outputStreamMock.Verify(x => x.WriteByte(expectedLowByte), Times.Once);
        }

        [TestMethod]
        public void TestThatDisposeCallsOutputStreamCloseOnce()
        {
            bitWriter.Dispose();

            outputStreamMock.Verify(x => x.Close(), Times.Once);
        }

        [TestMethod]
        public void TestThatWhenBufferIsEmptyDisposeDoesNotCallOutputStreamWriteByte()
        {
            bitWriter.Dispose();

            outputStreamMock.Verify(x => x.WriteByte(It.IsAny<byte>()), Times.Never);
        }

        [TestMethod]
        public void TestThatWhenBufferIsNotEmptyDisposeCallsOutputStreamWriteByteWithRemainingBits()
        {
            var value = Convert.ToUInt32("10111", 2);
            var expectedByteWithRemainingBitsInBuffer = Convert.ToByte("10111000", 2);

            bitWriter.WriteNBits(5, value);
            bitWriter.Dispose();

            outputStreamMock.Verify(x => x.WriteByte(expectedByteWithRemainingBitsInBuffer), Times.Once);
        }
    }
}
