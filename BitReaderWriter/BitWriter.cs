using BitReaderWriter.Contracts;
using System;
using System.IO;

namespace BitReaderWriter
{
    public class BitWriter: IBitWriter
    {
        private byte bufferWrite;
        private int numberOfBitsInBuffer;
        private readonly Stream outputStream;

        public BitWriter(Stream outputStream)
        {
            this.outputStream = outputStream;

            numberOfBitsInBuffer = 0;
        }

        public void WriteNBits(int n, uint value)
        {
            if(n > 32)
            {
                throw new ArgumentException("n cannot be greater than 32");
            }

            value <<= (32 - n);
            for (int i = 0; i < n; i++)
            {
                uint bit = (value >> 31) & 1;
                value <<= 1;
                WriteBit((int)bit);
            }
        }

        private void WriteBit(int value)
        {
            bufferWrite = (byte)((bufferWrite << 1) + (value & 1));
            numberOfBitsInBuffer++;

            if(IsBufferFull())
            {
                outputStream.WriteByte(bufferWrite);
                numberOfBitsInBuffer = 0;
            }
        }

        public void Dispose()
        {
            outputStream.Dispose();

            Flush();
        }

        private bool IsBufferFull()
        {
            return numberOfBitsInBuffer == 8;
        }

        private void Flush()
        {
            if (numberOfBitsInBuffer == 0)
                return;

            bufferWrite = (byte)(bufferWrite << (8 - numberOfBitsInBuffer));
            outputStream.WriteByte(bufferWrite);
            numberOfBitsInBuffer = 0;
        }
    }
}
