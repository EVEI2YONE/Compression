using System.Reflection.Metadata.Ecma335;
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
        private Regex repeats;
        private void DecompressRecursive()
        {
            repeats = repeats ?? new Regex($"{Delimiter}(\\d+\\[[\\w+]*\\])");
            string input = stringBuilder.ToString();
            //input = (Delimiter.Length > 0) ? input.Split(Delimiter)[1] : input;
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
                num = int.Parse(value.Substring(Delimiter.Length, numDigits-Delimiter.Length)); //"3"
                start = numDigits + 1; //1+1 = 2 -> 3['a'b]
                end = value.IndexOf("]"); //4 -> 3[ab']'
                string repeatBlock = value.Substring(start, end-start); //3["ab"]
                string repeat = repeatBlock.Repeat(num); //"ab" x 3 = "ababab"
                stringBuilder.Remove(match.Index, value.Length); //db3[ab]cd => abcd
                stringBuilder.Insert(match.Index, repeat); //dbcd => insert "ababab" => dbabababcd
            }
            DecompressRecursive();
        }

        public string Delimiter { get; set; } = "~";
        public string Compress(string input)
        {
            List<object> result = CompressRecursive(input.Cast<object>().ToList());
            return result.Evaluate();
        }

        public string Compress(List<object> list)
        {
            List<object>result = CompressRecursive(list);
            return result.Evaluate();
        }

        public static RepeatPatternDictionary PatternDictionary = new RepeatPatternDictionary();
        public static Dictionary<string, List<object>> PreviousComputations = new Dictionary<string, List<object>>();
        public static int baseCaseCount = 0;
        public static int dictionaryHitCount = 0;
        public static int solutionHitCount = 0;
        public static int recursiveHitCount = 0;
        public static bool Test;
        public static bool Pattern;
        private List<object> CompressRecursive(List<object> input, int size = 1)
        {
            string evaluation = input.Evaluate();
            List<object> solution;
            if (Test && PreviousComputations.ContainsKey(evaluation))
            {
                dictionaryHitCount++;
                return PreviousComputations[evaluation];
            }
            else if (Pattern && PatternDictionary.ContainsSolution(evaluation, out solution))
            {
                solutionHitCount++;
                return solution;
            }
            if (size > input.Count / 2)
            {
                if(Test) PreviousComputations.Add(evaluation, input);
                if(Pattern) PatternDictionary.AddSolutionToStruct(evaluation, input);
                baseCaseCount++;
                return input;
            }
            else
                recursiveHitCount++;
            string optimalPattern;
            List<object> clone = new List<object>(input); //a2[cd]a2[cd]
            Dictionary<string, Dictionary<int, RepeatItem>> repeatOccurrences = new Dictionary<string, Dictionary<int, RepeatItem>>();
            object[] slider;
            for (int i = size; i <= clone.Count - size; i++) //slide across list starting at index 2: 'a', index 3: '2[cd]'
            {
                slider = clone.GetSlider(size, i - size); //a2[cd]
                if (slider.Length == 0) continue;
                for (int j = i; clone.IsRepeat(slider, j) && j <= clone.Count - size; j += size) //compare next size chuck: 'a2[cd]'
                {
                    repeatOccurrences.IncrementSequence(slider, i - size); //(a2[cd], occurrences)
                }
            }
            if (!repeatOccurrences.Any())
                return CompressRecursive(clone, size: size + 1);
            //repeatOccurrences.FilterRecurrences(); //remove recurrent patterns
            repeatOccurrences.ScrubBaseCase(); //remove invalid compression at base case
            repeatOccurrences.ResolveCombinations(clone);
            //repeatOccurrences.CompressNonrecurrences(clone); //compress nonoverlapping, nonrecurrence patterns
            if (repeatOccurrences.Any())
                return CompressRecursive(clone, 1);
            else
                return CompressRecursive(clone, size + 1);
        }
    }

    
}