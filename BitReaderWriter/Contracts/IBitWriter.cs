using System;

namespace BitReaderWriter.Contracts
{
    public interface IBitWriter : IDisposable
    {
        void WriteNBits(int n, uint value);
    }
}
