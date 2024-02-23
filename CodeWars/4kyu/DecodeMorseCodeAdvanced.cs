// https://www.codewars.com/kata/54b72c16cd7f5154e9000457



using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class MorseCodeDecoder
{
    public static string DecodeBits(
      string bits)
    {
      bits = bits.Trim('0');

      var step = (from length in
          from match in Regex.Matches(bits, @"1+|0+")
            select match.Value.Length
        orderby length select length).First();

      var bitsBuilder = new StringBuilder();

      for (var i = 0; i < bits.Length; i += step)
      {
        bitsBuilder.Append(
          bits[i]);
      }

      bits = bitsBuilder
        .ToString();

      return bits
        .Replace("111", "-")
        .Replace("1", ".")
        .Replace("0000000", "   ")
        .Replace("000", " ")
        .Replace("0", "");
    }

    public static string DecodeMorse(
      string morseCode)
    {
      return string.Join(" ", from word in morseCode.Trim().Split("   ")
        select string.Join("", from letter in word.Split(" ")
          select MorseCode.Get(letter)));
    }
}