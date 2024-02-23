https://www.codewars.com/kata/54cb771c9b30e8b5250011d4



using System;
using System.Numerics;

public static class Faberge
{
  public static BigInteger Height(
    BigInteger n, BigInteger m)
  {
    var result = BigInteger.Zero;
    var temp = BigInteger.One;

    for (int i = 1; i <= n; ++i)
    {
      temp *= m--;
      temp /= i;
      
      result += temp;
    }

    return result;
  }
}