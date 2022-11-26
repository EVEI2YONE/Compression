using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompressionLibrary
{
    public class RepeatItem
    {
        public static Dictionary<int, RepeatItem> RepeatItems { get; } = new Dictionary<int, RepeatItem>();
        public static string Delimiter { get; set; } = "~";
        public string Expression { get { return $"{Delimiter}{Repeats}[{Pattern}]"; } }
        public string Pattern { get; }
        public int Index { get; }
        public int Repeats { get; set; }
        public int PrevItemKey { get; set; }
        public int NextItemKey { get; set; }
        public int PatternIndexEnd { get { return Index + PatternSize * Repeats; } }
        public int PatternSize { get; set; }
        public bool IsRecurring { get; set; }

        public RepeatItem(string pattern, int index, int repeats, int prevItemKey = -1, int length = 0)
        {
            Pattern = pattern;
            Index = index;
            Repeats = repeats;
            RepeatItems.TryAdd(index, this);
            PrevItemKey = prevItemKey;
            NextItemKey = -1;
            PatternSize = length;
        }

        public override string ToString()
            => Expression;
    }
}
