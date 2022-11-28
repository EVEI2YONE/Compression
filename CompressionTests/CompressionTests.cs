using CompressionLibrary;

namespace CompressionTests
{
    public class Tests
    {
        Compression compression;
        [SetUp] public void SetUp() 
        {
            RepeatItem.Delimiter = "";
            compression = new Compression();
        }
        private void Validate_Compression(string input, string expected, bool Test = false)
        {
            Compression.hitCount = 0; Compression.altHitCount = 0; Compression.recursiveHitCount = 0;
            Compression.Test = Test;
            string evaluation = compression.Compress(input);
            Assert.AreEqual(expected, evaluation);
            var prior = input.Length;
            var current = evaluation.Length;
            var compressionPercent = ((double)current / prior);
            Console.WriteLine($"Compressed {input} to {evaluation}, resulting in {(compressionPercent * 100.0):n2}% compressed size compared to original");
            Console.WriteLine($"hit count: {Compression.hitCount} | alt hit count: {Compression.altHitCount} | recursive hit count: {Compression.recursiveHitCount}");
            Console.WriteLine($"total hit count: {Compression.hitCount + Compression.altHitCount + Compression.recursiveHitCount}");
        }

        [TestCase("aaaaaaaa", "8[a]")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaa", "26[a]")]
        [TestCase("aaaaaaaaaaaaabbbbbbbbbbbbb", "13[a]13[b]")]
        [TestCase("aaaaaaaa", "8[a]", true)]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaa", "26[a]", true)]
        [TestCase("aaaaaaaaaaaaabbbbbbbbbbbbb", "13[a]13[b]", true)]
        public void Compress_SimpleCompress_Test(string uncompress, string compress, bool Test = false)
        {
            Validate_Compression(uncompress, compress, Test);
        }
        
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaa", "~26[a]")]
        [TestCase("aaaaaaaaaaaaabbbbbbbbbbbbb", "~13[a]~13[b]")]
        [TestCase("aaaaaaaaaaaaaaaaaaaaaaaaaa", "~26[a]", true)]
        [TestCase("aaaaaaaaaaaaabbbbbbbbbbbbb", "~13[a]~13[b]", true)]
        public void Compress_Delimiter_Test(string uncompress, string compress, bool Test = false)
        {
            RepeatItem.Delimiter = "~";
            Validate_Compression(uncompress, compress, Test);
        }
        
        [TestCase("abababababababababababababababababababababababababab", "~26[ab]")]
        [TestCase("abcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabc", "~26[abc]")]
        [TestCase("abababababababababababababababababababababababababab", "~26[ab]", true)]
        [TestCase("abcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabcabc", "~26[abc]", true)]
        public void Compress_IncreasingSize_Test(string uncompress, string compress, bool Test = false)
        {
            RepeatItem.Delimiter = "~";
            Validate_Compression(uncompress, compress, Test);
        }

        [TestCase("aabcabcaabcabcaabcabcaabcabc", "4[a2[abc]]")]
        //[TestCase("aabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabc", "8[a2[abc]]")]
        //[TestCase("aabcabcaabcabcaabcabcaabcabc", "aa2[2[bca]a]2[bca]2[abc]")] //alt compression
        //[TestCase("aabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabc", "2[aa2[2[bca]a]2[bca]2[abc]]")] //alt compression
        //[TestCase("abcabcbbbbbbbbbbbbbabcabcbbbbbbbbbbbbbabcabcbbbbbbbbbbbbb", "3[2[abc]13[b]]")]
        //[TestCase("aabcabcaabcabcaabcabcaabcabc", "4[a2[abc]]", true)]
        //[TestCase("aabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabc", "8[a2[abc]]", true)]
        //[TestCase("aabcabcaabcabcaabcabcaabcabc", "aa2[2[bca]a]2[bca]2[abc]", true)] //alt compression
        //[TestCase("aabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabcaabcabc", "2[aa2[2[bca]a]2[bca]2[abc]]", true)] //alt compression
        //[TestCase("abcabcbbbbbbbbbbbbbabcabcbbbbbbbbbbbbbabcabcbbbbbbbbbbbbb", "3[2[abc]13[b]]", true)]
        public void Compress_AdvancedCompress_Test(string uncompress, string compress, bool Test = false)
        {
            Validate_Compression(uncompress, compress, Test);
        }

        [TestCase("dabababcdcdcd", "d3[ab]3[cd]")]
        [TestCase("dabababcdcdcddabababcdcdcd", "2[d3[ab]3[cd]]")]
        [TestCase("dabababcdcdcd", "d3[ab]3[cd]", true)]
        [TestCase("dabababcdcdcddabababcdcdcd", "2[d3[ab]3[cd]]", true)]
        public void Compress_Multiple_Test(string uncompress, string compress, bool Test = false)
        {
            Validate_Compression(uncompress, compress, Test);
        }

        [TestCase("dabababcbcbcbcbcbcdabababcbcbcbcbcbc", "2[d3[ab]5[cb]c]")]
        //[TestCase("dabababcbcbcbcbcbcdabababcbcbcbcbcbc", "2[d2[ab]a6[bc]]")] //alternative which is supposed to fail
        //[TestCase("dabababcbcbcbcbcbcdabababcbcbcbcbcbc", "2[d3[ab]5[cb]c]", true)]
        //[TestCase("dabababcbcbcbcbcbcdabababcbcbcbcbcbc", "2[d2[ab]a6[bc]]", true)] //alternative which is supposed to fail
        public void Compress_EquivalentExpression_Test(string uncompress, string expected, bool Test = false)
        {
            Validate_Compression(uncompress, expected, Test);
        }
    }
}