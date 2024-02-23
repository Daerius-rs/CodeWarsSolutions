// https://www.codewars.com/kata/5a08b22b32b8b96f4700001c



using System;
using System.Numerics;

public static class Kata
{
  private static BigInteger Totient(
    BigInteger number)
  {
    var one = BigInteger.One;
    var two = new BigInteger(2);

    var result = number;
    var temp = two;

    while (temp * temp <= number)
    {
      if (number % temp == 0)
      {
        result = result * (temp - one) / temp;

        while (number % temp == 0)
        {
          number /= temp;
        }
      }

      temp += one + temp % two;
    }

    if (number > 1)
      result = result * (number - one) / number;

    return result;
  }

  public static int Tower(
    BigInteger @base, BigInteger height,
    int modulus)
  {
    return (int)TowerInternal(@base,
      height, modulus);
  }
  private static BigInteger TowerInternal(
    BigInteger @base, BigInteger height,
    BigInteger modulus)
  {
    if (modulus == 1)
    {
      return BigInteger.Zero;
    }
    if (@base == BigInteger.One ||
      height == BigInteger.Zero)
    {
      return BigInteger.One;
    }

    var result = BigInteger.One;
    var index = BigInteger.Zero;
    var baseLog = BigInteger.Log(@base);
    var modulusLog = BigInteger.Log(modulus);

    while (index < height)
    {
      if ((double)result * baseLog > modulusLog)
        goto TotientMethod;

      result = BigInteger.Pow(
        @base,
        (int)result);

      ++index;
    }

    return result % modulus;

    // Label
    TotientMethod:



    var totient = Totient(modulus);
    var tower = TowerInternal(@base,
      height - BigInteger.One, totient);

    return BigInteger.ModPow(
      @base,
      totient + tower,
      modulus);
  }
}