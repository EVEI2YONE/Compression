using CompressionLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTests
{
    public class DecompressionTests
    {
        Compression compression;
        [SetUp]
        public void SetUp()
        {
            compression = new Compression();
            compression.Delimiter = "";
        }

        private void Validate_Decompression(string input, string expected)
        {
            Assert.AreEqual(expected, compression.Decompress(input));
        }

        [TestCase("4[a2[abc]]", "aabcabcaabcabcaabcabcaabcabc")]
        public void Decompress_Simple_Test(string input, string expected)
        {
            Validate_Decompression(input, expected);
        }

        [TestCase("d3[ba]bcd", "dbabababcd")]
        [TestCase("ab3[ac4[ef]]cd", "abacefefefefacefefefefacefefefefcd")]
        [TestCase("ab3[ac4[ef]2[gh]]cd", "abacefefefefghghacefefefefghghacefefefefghghcd")]
        [TestCase("ab3[ac]3[ef]cd", "abacacacefefefcd")]
        [TestCase("ab3[ac4[ef]]4[a2[bc]]cd", "abacefefefefacefefefefacefefefefabcbcabcbcabcbcabcbccd")]
        public void Decompress_Test1(string input, string expected)
        {
            Validate_Decompression(input, expected);
        }
        
        [TestCase("2[d3[ab]5[cb]c]", "dabababcbcbcbcbcbcdabababcbcbcbcbcbc")]
        [TestCase("2[d2[ab]a6[bc]]", "dabababcbcbcbcbcbcdabababcbcbcbcbcbc")]
        public void Decompress_EquivalentExpression_Test2(string input, string expected)
        {
            Validate_Decompression(input, expected);
        }
    }
}
