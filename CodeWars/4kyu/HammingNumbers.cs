// https://www.codewars.com/kata/526d84b98f428f14a60008da



using System;
using System.Collections.Generic;
using System.Linq;

public class Hamming
{
  public static long hamming(
    int n)
  {
    var bases = new[] { 2, 3, 5 };
    var expos = new[] { 0, 0, 0 };
    var hamms = new List<long> { 1 };

    for (var i = 1; i < n; ++i)
    {
      var nextHamms = new long[3];

      for (var j = 0; j < 3; ++j)
      {
        nextHamms[j] = bases[j] * hamms[expos[j]];
      }

      var nextHamm = nextHamms
        .Min();

      hamms.Add(
        nextHamm);

      for (var j = 0; j < 3; ++j)
      {
        expos[j] += Convert.ToInt32(
          nextHamms[j] == nextHamm);
      }
    }
    
    return hamms[^1];
  }
}