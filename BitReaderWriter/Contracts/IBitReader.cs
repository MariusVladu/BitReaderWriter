using System;

namespace BitReaderWriter.Contracts
{
    public interface IBitReader : IDisposable
    {
        uint ReadNBits(int n);
        int ReadBit();
    }
}
