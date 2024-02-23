// https://www.codewars.com/kata/5235c913397cbf2508000048



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class Evaluator
{
  private sealed class OperatorInfo
  {
    private Func<decimal[], decimal> Operation { get; }


    public string Operator { get; }
    public byte Priority { get; }
    public byte NumbersCount { get; }



    public OperatorInfo(string @operator, byte priority,
      Func<decimal[], decimal> operation = null, byte numbersCount = 0)
    {
      Operator = @operator;
      Priority = priority;
      Operation = operation;
      NumbersCount = operation != null
        ? numbersCount
        : (byte)0;
    }



    public decimal Apply(decimal[] numbers)
    {
      if (Operation == null)
        return decimal.Zero;
      if (NumbersCount == 0)
        return Operation(Array.Empty<decimal>());

      return Operation(numbers);
    }
  }



  private static readonly Dictionary<string, OperatorInfo> Operators;



  static Evaluator()
  {
    Operators = new Dictionary<string, OperatorInfo>(10)
    {
      {
        "(", new OperatorInfo("(", 1)
      },
      {
        ")", new OperatorInfo(")", 1)
      },
      { 
        "+", new OperatorInfo("+", 2,
          numbers => numbers[0] + numbers[1], 2)
      },
      {
        "-", new OperatorInfo("-", 2,
          numbers => numbers[0] - numbers[1], 2)
      },
      {
        "*", new OperatorInfo("*", 3,
          numbers => numbers[0] * numbers[1], 2)
      },
      {
        "/", new OperatorInfo("/", 3,
          numbers => numbers[0] / numbers[1], 2)
      },
      {
        "%", new OperatorInfo("%", 4,
          numbers => numbers[0] % numbers[1], 2)
      },
      {
        "^", new OperatorInfo("^", 5,
          numbers => (decimal)Math.Pow((double)numbers[0], (double)numbers[1]), 2)
      },
      {
        "!", new OperatorInfo("!", 6,
          numbers => Enumerable.Range(1, (int)numbers[0]).Aggregate((n1, n2) => n1 * n2), 1)
      }
    };
  }



  private static byte GetPriority(
    string part)
  {
    if (Operators.TryGetValue(part, out var operatorInfo))
      return operatorInfo.Priority;

    return 255;
  }

  private static List<string> GetParts(
    string expression)
  {
    var result = new List<string>(20);
    var index = 0;

    expression = expression.Trim();

    while (index < expression.Length)
    {
      while (char.IsWhiteSpace(expression[index]))
      {
        ++index;
      }

      var part = expression[index].ToString();

      if (!Operators.ContainsKey(part)
        || (part == "-"
          && (result.Count < 1 || Operators.ContainsKey(result[^1]))))
      {
        if (char.IsDigit(expression[index]) || expression[index] == '-')
        {
          for (int i = index + 1;
            i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == ',' || expression[i] == '.');
            ++i)
          {
            part += expression[i];
          }
        }
        else if (char.IsLetter(expression[index]))
        {
          for (int i = index + 1;
            i < expression.Length && (char.IsLetter(expression[i]) || char.IsDigit(expression[i]));
            ++i)
          {
            part += expression[i];
          }
        }
      }
      else if (part == "(" && result.Count > 0 && result[^1] == "-"
           && (result.Count < 2 || Operators.ContainsKey(result[^2]))
           && (result.Count < 2 || result[^2] != ")"))
      {
        index -= 1;

        result.RemoveAt(result.Count - 1);

        var braceIndex = index + 1;
        var parenthesesLevel = 1;

        do
        {
          braceIndex = expression
            .IndexOfAny(new[] { '(', ')' }, braceIndex + 1);

          if (braceIndex == -1)
            break;

          if (expression[braceIndex] == '(')
            ++parenthesesLevel;
          else if (expression[braceIndex] == ')')
            --parenthesesLevel;
        } while (parenthesesLevel != 0);

        expression = expression
          .Remove(index, 1)
          .Insert(index, "(-1*")
          .Insert(braceIndex + 4, ")");

        continue;
      }

      index += part.Length;

      result.Add(part);
    }

    return result;
  }

  private static List<string> ToRPN(
    string expression)
  {
    var stack = new Stack<string>(20);
    var result = new List<string>(20);

    foreach (var part in GetParts(expression))
    {
      if (Operators.ContainsKey(part))
      {
        if (stack.Count > 0 && !part.Equals("("))
        {
          if (part.Equals(")"))
          {
            var element = stack.Pop();

            while (element != "(")
            {
              result.Add(element);

              element = stack.Pop();
            }
          }
          else if (GetPriority(part) > GetPriority(stack.Peek()))
          {
            stack.Push(part);
          }
          else
          {
            while (stack.Count > 0 && GetPriority(part) <= GetPriority(stack.Peek()))
            {
              result.Add(stack.Pop());
            }

            stack.Push(part);
          }
        }
        else
        {
          stack.Push(part);
        }
      }
      else
      {
        result.Add(part);
      }
    }

    result.AddRange(stack);

    return result;
  }



  public double Evaluate(
    string expression, int digitsAfterRounding = 6)
  {
    var stack = new Stack<string>(20);
    var queue = new Queue<string>(
      ToRPN(expression));

    while (queue.Count > 0)
    {
      var element = queue.Dequeue();

      if (!Operators.ContainsKey(element))
      {
        stack.Push(element);
      }
      else
      {
        var operatorInfo = Operators[element];
        var numbers = new decimal[operatorInfo.NumbersCount];

        for (int i = operatorInfo.NumbersCount - 1; i >= 0; --i)
        {
          numbers[i] = Convert.ToDecimal(
            stack.Pop(), CultureInfo.InvariantCulture);
        }

        stack.Push(operatorInfo.Apply(numbers)
          .ToString(CultureInfo.InvariantCulture));
      }
    }

    var result = Convert.ToDecimal(
      stack.Pop(), CultureInfo.InvariantCulture);
    
    return Math.Round(
      (double)result, digitsAfterRounding);
  }
}