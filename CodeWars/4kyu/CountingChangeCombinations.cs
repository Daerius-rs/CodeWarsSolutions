https://www.codewars.com/kata/541af676b589989aed0009e7



public static class Kata
{
  public static int CountCombinations(
    int money, int[] coins)
  {
      var ways = new int[money + 1];

      ways[0] = 1;

      foreach (var coin in coins)
          for (var i = coin; i <= money; ++i)
              ways[i] += ways[i - coin];

      return ways[money];
  }
}