// https://www.codewars.com/kata/534e01fbbb17187c7e0000c6



using System;

public class Spiralizor
{
  public static int[,] Spiralize(
    int size)
  {
    var result = new int[size, size];

    for (var i = 0; i < size; ++i)
    {
      for (var j = 0; j < size; ++j)
      {
        result[i, j] = Convert.ToInt32(
          // top
          (i < (size / 2) + (size % 2 == 1 ? 1 : 0) && i % 2 == 0 && j >= i - 2 && j <= size - i - 1) ||
          // right
          ((size - j) % 2 == 1 && i > size - j - 1 && i <= j) ||
          // bottom
          ((size - i) % 2 == 1 && j > size - i - 1 && j < i) ||
          // left
          (j % 2 == 0 && i > j + 1 && i < size - j)
        );
      }
    }

    return result;
  }
}