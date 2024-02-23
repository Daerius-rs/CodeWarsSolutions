// https://www.codewars.com/kata/61559bc4ead5b1004f1aba83



using System.Numerics;

public class Spiral
{
  public static BigInteger Sum(
    BigInteger size)
  {
    return BigInteger.Pow(++size, 2) / 2 - 1;
  }
}