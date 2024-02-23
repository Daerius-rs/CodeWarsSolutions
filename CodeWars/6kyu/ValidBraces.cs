// https://www.codewars.com/kata/5277c8a221e209d3f6000b56



using System;
using System.Collections.Generic;

public class Brace
{
    private static readonly Dictionary<char, char> ParenthesesMap;

    static Brace()
    {
        ParenthesesMap = new Dictionary<char, char>
        {
          {')', '('},
          {']', '['},
          {'}', '{'}
        };
    }

    public static bool validBraces(
      string source)
    {
        if (source.Length == 0)
            return true;
        if (source.Length == 1)
            return false;

        var stack = new Stack<char>();

        foreach (var sourceChar in source)
        {
            if (ParenthesesMap.ContainsValue(
              sourceChar))
            {
                stack.Push(sourceChar);
            }
            else if (ParenthesesMap.TryGetValue(
              sourceChar, out var expectedChar))
            {
                if (stack.Count == 0
                    || stack.Pop() != expectedChar)
                {
                  return false;
                }
            }
        }

        return stack.Count == 0;
    }
}