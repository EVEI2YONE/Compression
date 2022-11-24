# Compression
Compressing string <br>
input: abcabcdefdef <br>
expected: 2[abc]2[def] <br>
<br>
input: abccccabccccdeffeffeff <br>
expected: 2[ab2[cc]]d3[eff] <br>
<br>
Decompressing string <br>
input: abcd4[ef] <br>
expected: abcdefefefef <br>
<br>
input: a3[ab2[cd]] <br>
expected: aabcdcdabcdcdabcdcd
