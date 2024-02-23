// https://www.codewars.com/kata/6411b91a5e71b915d237332d



using System;
using System.Collections.Generic;

public class Parentheses
{
    public static bool ValidParentheses(
      string source)
    {
        if (source.Length == 0)
            return true;
        if (source.Length == 1)
            return false;

        var stack = new Stack<char>();
      
        foreach (var sourceChar in source)
        {
            if (sourceChar == '(')
            {
                stack.Push(sourceChar);
            }
            else if (sourceChar == ')')
            {
                if (stack.Count == 0
                    || stack.Pop() != '(')
                {
                  return false;
                }
            }
        }
      
        return stack.Count == 0;
    }
}