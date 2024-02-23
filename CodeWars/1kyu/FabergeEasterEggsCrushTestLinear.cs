https://www.codewars.com/kata/5976c5a5cd933a7bbd000029



using System;
using System.Numerics;

public static class Faberge
{
  private static readonly BigInteger Modulus;
  private static readonly BigInteger[] Cache;

  static Faberge()
  {
    Modulus = new BigInteger(998244353);
    Cache = new BigInteger[80001];

    Cache[0] = BigInteger.Zero;
    Cache[1] = BigInteger.One;

    for (var i = 2; i < Cache.Length; ++i)
    {
      Cache[i] = (Modulus - Modulus / i) * Cache[(int)Modulus % i] % Modulus;
    }
  }

  public static BigInteger Height(
    BigInteger n, BigInteger m)
  {
    var result = BigInteger.Zero;
    var temp = BigInteger.One;

    m %= Modulus;

    for (var i = 1; i <= n; ++i)
    {
      temp = temp * (m - i + 1) * Cache[i] % Modulus;
      result = (result + temp) % Modulus;
    }

    return result % Modulus;
  }
}