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
