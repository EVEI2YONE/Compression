using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;

namespace CompressionLibrary
{
    public class Compression
    {
        private StringBuilder stringBuilder;
        public string Decompress(string input)
        {
            stringBuilder = new StringBuilder(input);
            DecompressRecursive();
            return stringBuilder.ToString();
        }
        private Regex repeats = new Regex("(\\d+\\[[\\w+]*\\])");
        private void DecompressRecursive()
        {
            string input = stringBuilder.ToString();
            var matches = repeats.Matches(input);
            if (matches.Count == 0)
                return;
            Match match;
            int num, numDigits, start, end;
            string value;
            for(int i = matches.Count-1; i>= 0; i--)
            {
                match = matches[i]; //db3[ab]cd
                value = match.Value; //3[ab]
                numDigits = value.IndexOf("["); //1
                num = int.Parse(value.Substring(0, numDigits)); //"3"
                start = numDigits + 1; //1+1 = 2 -> 3['a'b]
                end = value.IndexOf("]"); //4 -> 3[ab']'
                string repeatBlock = value.Substring(start, end-start); //3["ab"]
                string repeat = repeatBlock.Repeat(num); //"ab" x 3 = "ababab"
                stringBuilder.Remove(match.Index, value.Length); //db3[ab]cd => abcd
                stringBuilder.Insert(match.Index, repeat); //dbcd => insert "ababab" => dbabababcd
            }
            DecompressRecursive();
        }

        public string Compress(string input)
        {
            stringBuilder = new StringBuilder(input);
            CompressRecursive(input, input.Length/2);
            return stringBuilder.ToString();
        }
        private int CompressRecursive(string input, int size) //greedy algorithm - assume longest repeating sequence is 2[length/2]
        {
            if (size < 4) return 0; //aaa -> 3[a] results in more characters
            List<string> repeatingSequences = new List<string>();
            for(int i = 0; i <= input.Length-size; i++)
            {

            }
            if (repeatingSequences.Count > 0)
            {
                int longestIndex = 0;
                for(int i = 0; i < repeatingSequences.Count; i++)
                {

                }
                return 0;
            }
            else
                return CompressRecursive(input, size - 1);
        }
    }

    public static class StringExtensions
    {
        public static string Repeat(this string s, int n) //takes advantage of prealloc
            => new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
    }
}