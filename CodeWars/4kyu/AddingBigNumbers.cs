// https://www.codewars.com/kata/525f4206b73515bffb000b21



using System;

public static class Kata
{
  public static string Add(
    string string1, string string2)
  {
    // Maximum chars allocated on stack = lengthThreshold
    // Maximum bytes allocated on stack = lengthThreshold * 2
    const int lengthThreshold = 512;
    // Char code for '0'
    const int zeroCharCode = 48;



    static (char Digit, int Carry) DigitsSum(
      char digit1, char digit2, int carry)
    {
      var sum = carry
            + (digit1 - zeroCharCode)
            + (digit2 - zeroCharCode);

      return ((char)(sum % 10 + zeroCharCode),
        sum / 10);
    }



    ref var minString = ref string2;
    ref var maxString = ref string1;

    if (string2.Length > string1.Length)
    {
      minString = ref string1;
      maxString = ref string2;
    }

    var minLength = minString.Length;
    var maxLength = maxString.Length + 1;

    var result = maxLength <= lengthThreshold
      ? stackalloc char[maxLength]
      : new char[maxLength];

    result[0] = '0';

    maxString.AsSpan()
      .CopyTo(result[1..]);

    var carry = 0;

    for (var i = maxLength - 1; i >= maxLength - minLength; --i)
    {
      (result[i], carry) = DigitsSum(
        result[i],
        minString[i - maxLength + minLength],
        carry);
    }
    for (var i = maxLength - minLength - 1; i >= 0; --i)
    {
      (result[i], carry) = DigitsSum(
        result[i],
        '0',
        carry);
    }

    result = result
      .TrimStart('0');

    return new string(result);
  }
}