https://www.codewars.com/kata/5426d7a2c2c7784365000783



using System;
using System.Collections.Generic;

public class Balanced
{
    public static List<string> BalancedParens(
        int pairsCount)
    {
        var result = new List<string>[pairsCount + 1];

        result[0] = new List<string>(1)
        {
            string.Empty
        };

        for (var i = 1; i <= pairsCount; ++i)
        {
            result[i] = new List<string>(i + 1);

            for (var j = 0; j < i; ++j)
            {
                var left = result[j];
                var right = result[i - j - 1];

                foreach (var front in left)
                    foreach (var back in right)
                        result[i].Add($"({front}){back}");
            }
        }

        return result[pairsCount];
    }
}