// https://www.codewars.com/kata/54b42f9314d9229fd6000d9c



using System.Linq;

public class Kata
{
  public static string DuplicateEncode(
    string word)
  {
    word = word
      .ToLowerInvariant();
    
    return new string(
      word
        .Select(ch =>
          word
            .Count(ch2 => ch2 == ch) > 1
                ? ')'
                : '(')
        .ToArray());
  }
}