using BitReaderWriter.Contracts;
using System;
using System.IO;

namespace BitReaderWriter
{
    public class BitReader : IBitReader
    {
        private byte bufferRead;
        private int numberOfBitsInBuffer;
        private readonly Stream inputStream;

        public BitReader(Stream inputStream)
        {
            this.inputStream = inputStream;

            numberOfBitsInBuffer = 0;
        }

        public uint ReadNBits(int n)
        {
            if(n > 32)
            {
                throw new ArgumentException("n cannot be greater than 32");
            }

            uint result = 0;
            for (int i = 0; i < n; i++)
            {
                result = (uint)((result << 1) + ReadBit());
            }

            return result;
        }

        public int ReadBit()
        {
            if(IsBufferEmpty())
            {
                bufferRead = (byte)inputStream.ReadByte();
                numberOfBitsInBuffer = 8;
            }

            int result = (bufferRead >> 7) & 1;

            bufferRead = (byte)(bufferRead << 1);
            numberOfBitsInBuffer--;

            return result;
        }

        public void Dispose()
        {
            inputStream.Dispose();
        }

        private bool IsBufferEmpty()
        {
            return numberOfBitsInBuffer == 0;
        }
    }
}
