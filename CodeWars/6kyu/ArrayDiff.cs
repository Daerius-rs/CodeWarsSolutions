// https://www.codewars.com/kata/523f5d21c841566fde000009



using System;
using System.Collections.Generic;

public class Kata
{
  public static int[] ArrayDiff(
    int[] a, int[] b)
  {
    if (a == null || b == null)
      return Array.Empty<int>();

    var result = new List<int>();
    var set = new HashSet<int>(b);

    foreach (var element in a)
    {
      if (set.Contains(element))
        continue;

      result.Add(element);
    }

    return result.ToArray();
  }
}