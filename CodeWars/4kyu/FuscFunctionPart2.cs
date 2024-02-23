// https://www.codewars.com/kata/57040e445a726387a1001cf7



using System;
using System.Numerics;

public static class FuscSolution
{
  public static BigInteger Fusc(
    BigInteger n)
  {
    var a = BigInteger.One;
    var b = BigInteger.Zero;

    while (n > 0)
    {
      if (n.IsEven)
        a += b;
      else
        b += a;

      n >>= 1;
    }

    return b;
  }
}