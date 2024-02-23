https://www.codewars.com/kata/55f8a9c06c018a0d6e000132



using System;
using System.Text.RegularExpressions;

public class Kata
{
  public static bool ValidatePin(string pin)
  {
    return Regex.IsMatch(pin, @"\A(?:\d{4}|\d{6})\z");
  }
}