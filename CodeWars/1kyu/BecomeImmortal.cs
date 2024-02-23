// https://www.codewars.com/kata/59568be9cc15b57637000054



using System;
using System.Numerics;

public static class Immortal
{
  /// set true to enable debug
  public static bool Debug = false;

  
  
  private static BigInteger Mod(
    BigInteger number, BigInteger divider)
  {
    var remainder =
      number % divider;

    return remainder < 0
      ? remainder + divider
      : remainder;
  }
  
  private static BigInteger NextPowerOfTwo(
    BigInteger number)
  {
    BigInteger powerOfTwo = 1;

    while (powerOfTwo < number)
    {
      powerOfTwo <<= 1;
    }

    return powerOfTwo;
  }

  private static BigInteger RangeSum(
    BigInteger x, BigInteger y)
  {
    return (x + y) * (y - x + 1) / 2;
  }

  public static long ElderAge(
    long m, long n, long l, long t)
  {
    return (long)ElderAgeInternal(m, n, l, t);
  }
  private static BigInteger ElderAgeInternal(
    BigInteger m, BigInteger n, BigInteger l, BigInteger t)
  {
    if (m == 0 || n == 0)
      return 0;

    if (m > n)
      (m, n) = (n, m);

    var mPowerOfTwo =
      NextPowerOfTwo(m);
    var nPowerOfTwo =
      NextPowerOfTwo(n);

    if (l > nPowerOfTwo)
      return 0;

    if (mPowerOfTwo == nPowerOfTwo)
    {
      var temp =
        RangeSum(1, nPowerOfTwo - l - 1) *
        (m + n - nPowerOfTwo) +
        ElderAgeInternal(nPowerOfTwo - n, mPowerOfTwo - m, l, t);

      return Mod(temp, t);
    }

    if (mPowerOfTwo < nPowerOfTwo)
    {
      mPowerOfTwo = nPowerOfTwo >> 1;

      var temp =
        RangeSum(1, nPowerOfTwo - l - 1) *
        m - (nPowerOfTwo - n) *
        RangeSum(BigInteger.Max(0, mPowerOfTwo - l), nPowerOfTwo - l - 1);

      if (l <= mPowerOfTwo)
      {
        temp +=
          (mPowerOfTwo - l) *
          (mPowerOfTwo - m) *
          (nPowerOfTwo - n) +
          ElderAgeInternal(mPowerOfTwo - m, nPowerOfTwo - n, 0, t);
      }
      else
      {
        temp +=
          ElderAgeInternal(mPowerOfTwo - m, nPowerOfTwo - n, l - mPowerOfTwo, t);
      }

      return Mod(temp, t);
    }

    return 0;
  }
}