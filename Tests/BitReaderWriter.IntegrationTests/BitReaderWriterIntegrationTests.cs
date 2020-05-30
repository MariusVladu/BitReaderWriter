using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using BitReaderWriter.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitReaderWriter.IntegrationTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class BitReaderWriterIntegrationTests
    {
        private IBitReader bitReader;
        private IBitWriter bitWriter;

        private string randomStringContent;
        private readonly string inputFilePath = "inputFile.txt";
        private readonly string outputFilePath = "outputFile.txt";

        private Stream inputStream;
        private Stream outputStream;

        [TestInitialize]
        public void Setup()
        {
            randomStringContent = RandomString(123);
            File.WriteAllText(inputFilePath, randomStringContent);

            inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read);
            outputStream = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write);
        }

        [TestCleanup]
        public void Cleanup()
        {
            inputStream.Dispose();
            outputStream.Dispose();
        }

        [TestMethod]
        public void TestThatBitWriterWritesToFile()
        {
            using (bitWriter = new BitWriter(outputStream))
            {
                foreach (byte character in randomStringContent.ToArray())
                {
                    bitWriter.WriteNBits(8, character);
                }
            }

            var writtenContent = File.ReadAllText(outputFilePath);
            Assert.AreEqual(randomStringContent, writtenContent);
        }

        [TestMethod]
        public void TestThatBitReaderReadsFileContent()
        {
            var readContent = "";
            using (bitReader = new BitReader(inputStream))
            {
                for (int i = 0; i < randomStringContent.Length; i++)
                {
                    readContent += (char)bitReader.ReadNBits(8);
                }
            }

            Assert.AreEqual(randomStringContent, readContent);
        }

        [TestMethod]
        public void TestThatBitReaderAndBitWriterCanReadAndWriteRandomNumberOfBits()
        {
            var totalNumberOfBits = randomStringContent.Length * 8;
            var random = new Random();

            using (bitReader = new BitReader(inputStream))
            using (bitWriter = new BitWriter(outputStream))
            {
                do
                {
                    int numberOfBits = random.Next(1, 32);
                    if (numberOfBits > totalNumberOfBits)
                        numberOfBits = totalNumberOfBits;

                    uint value = bitReader.ReadNBits(numberOfBits);
                    bitWriter.WriteNBits(numberOfBits, value);

                    totalNumberOfBits -= numberOfBits;

                } while (totalNumberOfBits > 0);
            }

            var writtenContent = File.ReadAllText(outputFilePath);
            Assert.AreEqual(randomStringContent, writtenContent);
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
