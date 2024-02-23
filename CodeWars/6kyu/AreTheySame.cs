// https://www.codewars.com/kata/550498447451fbbd7600041c



using System;
using System.Linq;

class AreTheySame
{
  public static bool comp(
    int[] left, int[] right)
  {
    if (left == null && right == null)
      return true;
    if ((left == null || right == null)
      || (left.Length == 0 ^ right.Length == 0)
      || left.Length != right.Length)
    {
      return false;
    }

    return
      left
        .OrderBy(number => number)
        .SequenceEqual(
          right
            .Select(number => (int)Math.Sqrt(number))
            .OrderBy(number => number));
  }
}