// https://www.codewars.com/kata/54bf1c2cd5b56cc47f0007a1



using System;
using System.Linq;

public class Kata
{
  public static int DuplicateCount(
    string source)
  {
    int count = 0;
    char[] sourceChars = source
      .ToLowerInvariant()
      .ToArray();

    for (var i = 0; i < sourceChars.Length; ++i)
    {
      if (sourceChars[i] == default
          || Array.LastIndexOf(sourceChars, sourceChars[i]) == i)
      {
        continue;
      }

      sourceChars = sourceChars
        .Select(sourceChar =>
                  sourceChar == sourceChars[i]
                    ? default
                    : sourceChar)
        .ToArray();
      
      ++count;
    }

    return count;
  }
}