// https://www.codewars.com/kata/54b724efac3d5402db00065e



using System;
using System.Linq;

public class MorseCodeDecoder
{
  public static string Decode(string morseCode)
  {
    return string.Join(" ", from word in morseCode.Trim().Split("   ")
      select string.Join("", from letter in word.Split(" ")
        select MorseCode.Get(letter)));
  }
}