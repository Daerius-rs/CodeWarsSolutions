https://www.codewars.com/kata/513e08acc600c94f01000001



using System;

public class Kata
{
  public static string Rgb(
    int r, int g, int b) 
  {
    byte ToByte(int number)
    {
      return (byte)Math.Clamp(
        number, 0, 255);
    }

    return ToByte(r).ToString("X2") +
           ToByte(g).ToString("X2") +
           ToByte(b).ToString("X2");
  }
}