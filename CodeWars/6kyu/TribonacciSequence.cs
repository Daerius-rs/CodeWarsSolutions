// https://www.codewars.com/kata/556deca17c58da83c00002db



using System;
using System.Linq;

public class Xbonacci
{
  public double[] Tribonacci(
    double[] signature, int n)
  {
    if (n < 3)
      return signature[0..n];
    if (n == 3)
      return signature;

    Array.Resize(
      ref signature,
      signature.Length + 1);
    
    signature[^1] =
      signature[^4..^1]
        .Sum();

    return Tribonacci(
      signature, --n);
  }
}