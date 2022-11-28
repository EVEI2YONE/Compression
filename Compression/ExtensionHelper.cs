using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CompressionLibrary
{
    public static class StringExtensions
    {
        public static string Repeat(this string s, int n) //takes advantage of prealloc
            => new StringBuilder(s.Length * n).Insert(0, s, n).ToString();
    }

    public static class ListExtensions
    {
        public static object[] GetSlider(this List<object> list, int size, int start)
        {
            object[] slider = new object[size]; //a2[cd]
            list.CopyTo(start, slider, 0, size);
            return slider;
        }
        public static bool IsRepeat(this List<object> list, object[] slider, int pos) //position in list to compare with slider pattern
        {
            //prevent out of index exception
            if (pos + slider.Length > list.Count) // pos: 0, slider: 'ab', list: 'ab'
                return false;
            bool isRepeat = true;
            for(int i = 0; i < slider.Length; i++) //offset for pos when parsing list
            {
                if (!list[pos + i].ExpressionMatches(slider[i]))
                {
                    isRepeat = false;
                    break;
                }
            }
            return isRepeat;
        }
        public static string Evaluate(this List<object> list)
        {
            StringBuilder stringBuilder = new StringBuilder();
            list.ForEach(x =>
            {
                x = (x is RepeatItem) ? (RepeatItem)x : (char)x;
                stringBuilder.Append(x.ToString());
            });
            return stringBuilder.ToString();
        }
    }

    public static class DictionaryExtensions
    {
        public static void IncrementSequence(this Dictionary<string, Dictionary<int, RepeatItem>> dictionary, object[] arr, int index)
        {
            string pattern = arr.ExtractSliderPattern();
            Dictionary<int, RepeatItem> innerDictionary;
            dictionary.TryGetValue(pattern, out innerDictionary);
            RepeatItem item;
            int prevItemKey;
            if (innerDictionary == null) //create new inner dictionary
            {
                innerDictionary = new Dictionary<int, RepeatItem>();
                dictionary.Add(pattern, innerDictionary);
            }
            prevItemKey = (innerDictionary.Any()) ? innerDictionary.LastOrDefault().Key : -1; //add previous mapping
            if (!innerDictionary.TryGetValue(index, out item)) //create new record of pattern at specified index
            {
                item = new RepeatItem(pattern, index, 1, prevItemKey, arr.Length);
                if (prevItemKey != -1) //add reverse mapping
                    innerDictionary[prevItemKey].NextItemKey = index;
                innerDictionary.Add(index, item);
            }
            item = item ?? innerDictionary[index];
            if (innerDictionary.HasRecurringItem(item))
                item.IsRecurring = true;
            item.Repeats++;
        }
        public static bool HasRecurringItem(this Dictionary<int, RepeatItem> dictionary, RepeatItem item) //should be the most recent entry
        {
            if (item.PrevItemKey == -1) return false;
            RepeatItem prevItem = dictionary[item.PrevItemKey];
            if (item.Index <= prevItem.PatternIndexEnd) return true;
            else return false;
        }
        public static void ScrubBaseCase(this Dictionary<string, Dictionary<int, RepeatItem>> dictionary)
        {
            var recurringItems = dictionary.SelectMany(x => x.Value, (innerDictionary, repeatItem) => new { innerDictionary.Key, repeatItem });
            var filterItems = recurringItems.Where(x =>
            {
                var repeatItem = x.repeatItem.Value;
                int len = repeatItem.Pattern.Length;
                int magnitude = repeatItem.Repeats/10 + 1; //based on extracted value for repeats; e.g. "2" is magnitude 0, "99" is magnitude 2
                if ((RepeatItem.Delimiter.Length + magnitude + len + 2) - (len * repeatItem.Repeats) > 0) //include "[]" as 2 constant
                    return true;
                return false;
            });
            foreach(var item in filterItems)
                dictionary[item.Key].Remove(item.repeatItem.Value.Index);
            foreach (var key in dictionary.Keys)
                if (!dictionary[key].Any())
                    dictionary.Remove(key);
        }
        public static void FilterRecurrences(this Dictionary<string, Dictionary<int, RepeatItem>> dictionary)
        {
            var recurringItems = dictionary.SelectMany(x => x.Value, (innerDictionary, repeatItem) => new { innerDictionary.Key, repeatItem });
            var filterItems = recurringItems.Where(x => x.repeatItem.Value.IsRecurring);
            foreach (var item in filterItems)
                dictionary[item.Key].Remove(item.repeatItem.Value.Index);
        }
        public static void CompressNonrecurrences(this Dictionary<string, Dictionary<int, RepeatItem>> dictionary, List<object> list)
        {
            if (!dictionary.Any()) return;
            var items = dictionary.SelectMany(x => x.Value, (innerDictionary, repeatItem) => new { innerDictionary.Key, repeatItem });
            for (int i = items.Max(x => x.repeatItem.Value.Index); i >= items.Min(x => x.repeatItem.Value.Index); i--)
            {
                var item = items.Where(x => x.repeatItem.Value.Index == i).Select(x => x.repeatItem.Value).FirstOrDefault();
                if(item != null)
                {
                    list.RemoveRange(item.Index, item.PatternIndexEnd-item.Index);
                    list.Insert(item.Index, item);
                }
            }
        }
        public static void ResolveCombinations(this Dictionary<string, Dictionary<int, RepeatItem>> dictionary, List<object> list)
        {
            if(!dictionary.Any()) return;
            var items = dictionary.SelectMany(x => x.Value, (innerDictionary, repeatItem) => new { innerDictionary.Key, repeatItem });
            var firstItem = items.First().repeatItem.Value;
            int minIndex = firstItem.Index;
            string minPattern = firstItem.Pattern;
            string minExpression = firstItem.Expression;
            foreach(var item in items.Select(x => x.repeatItem.Value))
            {
                List<object> copy = new List<object>(list);
                copy.RemoveRange(item.Index, item.PatternIndexEnd - item.Index);
                copy.Insert(item.Index, item);
                string evaluation = copy.Evaluate();
                string result = (new Compression()).Compress(copy);//evaluation);
                if (result.Length < evaluation.Length && result.Length < minExpression.Length)
                {
                    minIndex = item.Index;
                    minPattern = item.Pattern;
                    minExpression = evaluation;
                }
            }
            var repeatItem = dictionary[minPattern][minIndex];
            list.RemoveRange(repeatItem.Index, repeatItem.PatternIndexEnd - repeatItem.Index);
            list.Insert(repeatItem.Index, repeatItem);
            repeatItem.ResolutionPostCleanUp();
        }
    }

    public static class ObjectExtensions
    {
        public static bool ExpressionMatches(this object a, object b)
        {
            if (a == null || b == null) return false;
            b = (b is RepeatItem) ? (RepeatItem)b : (char)b;
            a = (a is RepeatItem) ? (RepeatItem)a : (char)a;
            return a.ToString().Equals(b.ToString());
        }
        public static string ExtractSliderPattern(this object[] arr)
            => string.Join("", arr.Select(x => (x is RepeatItem) ? ((RepeatItem)x).Expression : ((char)x).ToString()));
    }
}
