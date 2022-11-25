# Compression
Compressing string  
input: abcabcdefdef  
expected: 2[abc]2[def]  
  
input: abccccabccccdeffeffeff  
expected: 2[ab2[cc]]d3[eff]  
  
Decompressing string  
input: abcd4[ef]  
expected: abcdefefefef  
  
input: a3[ab2[cd]]  
expected: aabcdcdabcdcdabcdcd  
  
# Solution for Decompression
Use Regex to match innermost pattern, then replace match with evaluated expression  
Regex: \<repeats\>[\<pattern\>] -> "(\\\\d+\\\\[\\\\w+\\\\])"  
e.g. 2[ab2[cc]]d3[eff]  
    
  innermost matches: 2  
  match 1: 2[cc]  
  match 2: 3[eff]  
  resolve: 2[abcccc]deffeffeff  
    
  innermost matches: 1  
  match 1: 2[abcccc]  
  resolve: abccccabccccdeffeffeff  
    
  innermost matches: 0  
  
# Solution for Compression
Shift and Parse 2 items at a time to check indicating pattern  
input: aabcdcdabcdcdabcdcd  
  
First parse:
pair patterns checked:  
pair 1: aa bc  
pair 2: ab cd  
pair 3: bc dc  
pair 4: cd cd -> cd cd ab  
...  
extracted pair: cd cd -> 2[cd]  
  
Fist parse results:  
aab2[cd]ab2[cd]ab2[cd]  
  
Second parse:  
pair 1: aa b2[cd]  
pair 2: ab 2[cd]a  
pair 3: b2[cd] ab  
pair 4: 2[cd]a b[2cd]  
...  
No extracted pairs  
  
aab2[cd]ab2[cd]ab2[cd]  
Third parse:  
pair 1: aab ab2[cd]  
pair 2: ab2[cd] ab2[cd]  - pair  
pair 3: b2[cd]a b2[cd]a  - pair  
pair 4: 2[cd]ab 2[cd]ab  - pair  
pair 5: ab2[cd] ab2[cd] - already paired  
pair 6: b2[cd]a b2[cd] - missing 'a' due to End of Line  
  
Third parse results:  
result 1: a2[ab2[cd]]ab2[cd]  
result 2: aa2[b2[cd]a]b2[cd]  
result 3: aab2[2[cd]ab]2[cd]  
  
Expected result: a3[ab2[cd]]
Program should be robust enough to result in the same output  
  
# Note
  
2[cd] will be extracted on first parse  
  
Reparse 2 items at a time  since new item was extracted  
  
There will be NO matches for 2 items during reparse -> Reparse 3 items at a time  
3[ab2[cd]] will be extracted on third parse  
item 1: a  
item 2: b  
item 3: 2[cd]  
  
Reparse 2 items at a time since new item was extracted  
  
Failing to parse N items results in Reparsing N+1 size where 2 <= N <= input.Length/2  
Each successvie parse (extracting a repeating item) results in Reparsing 2 items at a time  
