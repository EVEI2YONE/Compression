using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompressionLibrary
{
    public class RepeatPatternDictionary
    {
        public char? character = null;
        public List<object> pattern = null;
        public Dictionary<char, RepeatPatternDictionary> SolutionDictionary = new Dictionary<char, RepeatPatternDictionary>();
        public bool ContainsSolution(string evaluation, out List<object> solution)
        {
            solution = null;
            int count = evaluation.Length;
            if(count == 0) return false;
            RepeatPatternDictionary maxPathItem = this;
            RepeatPatternDictionary path;
            foreach(char ch in evaluation)
            {
                if (!maxPathItem.SolutionDictionary.TryGetValue(ch, out path))
                    break;
                else
                    maxPathItem = path;
            }
            solution = maxPathItem.pattern;
            return (solution == null) ? false : true;
        }
        public void AddSolutionToStruct(string evaluation, List<object> list)
        {
            RepeatPatternDictionary path = this;
            RepeatPatternDictionary result = this;
            char? lastKey = null;
            Compression compress = new Compression();
            evaluation = compress.Decompress(evaluation);
            char ch;
            for (int i = 0; i < evaluation.Length; i++)
            {
                ch = evaluation[i];
                lastKey = ch;
                if (!path.SolutionDictionary.TryGetValue(ch, out result))
                {
                    path.SolutionDictionary.Add(ch, new RepeatPatternDictionary() { character = ch });
                    result = path.SolutionDictionary[ch];
                }
                if (i < evaluation.Length - 1)
                    path = result;
            }
            if(lastKey.HasValue)
                path.SolutionDictionary[lastKey.Value].pattern = list;
        }
    }
}
