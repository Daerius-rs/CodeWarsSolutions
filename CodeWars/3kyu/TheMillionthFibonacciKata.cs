https://www.codewars.com/kata/53d40c1e2f13e331fc000c26



using System;
using System.Numerics;

public class Fibonacci
{
  public static BigInteger fib(
    BigInteger n)
  {
    if (n.Sign >= 0)
      return fibInternal(n).a;

    return n.IsEven
      ? -fibInternal(-n).a
      : fibInternal(-n).a;
  }
  public static (BigInteger a, BigInteger b) fibInternal(
    BigInteger n)
  {
    if (n == 0)
      return (0, 1);
    if (n == 1)
      return (1, 1);

    var (a, b) = fibInternal(
      n / 2);

    var p = a * (2 * b - a);
    var q = b * b + a * a;

    return n.IsEven
      ? (p, q)
      : (q, p + q);
  }
}