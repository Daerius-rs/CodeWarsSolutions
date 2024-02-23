// https://www.codewars.com/kata/5783ef69202c0ee4cb000265



using System;

public class Kata
{
  public static int SearchArray(
    object[][] array, object[] query)
  {
    if (query == null
      || query.Length != 2)
    {
      throw new Exception();
    }

    var index = -1;

    foreach (var subArray in array)
    {
      ++index;

      if (subArray == null
        || subArray.Length != 2)
      {
        throw new Exception();
      }

      if (subArray[0].Equals(query[0])
        && subArray[1].Equals(query[1]))
      {
        return index;
      }
    }

    return -1;
  }
}