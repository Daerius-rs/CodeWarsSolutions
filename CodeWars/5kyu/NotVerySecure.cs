// https://www.codewars.com/kata/526dbd6c8c0eb53254000110



using System.Text.RegularExpressions;

public class Kata
{
  public static bool Alphanumeric(string str)
  {
    return Regex.IsMatch(str, @"\A[a-z0-9]+\z", RegexOptions.IgnoreCase);
  }
}
