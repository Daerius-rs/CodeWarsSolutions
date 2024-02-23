https://www.codewars.com/kata/5265b0885fda8eac5900093b



using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace CompilerKata
{
  public enum AssociativityType : byte
  {
    Left = 1,
    Right = 2
  }

  public enum OperatorType : byte
  {
    Arg = 1,
    Imm = 2,
    Add = 3,
    Sub = 4,
    Mul = 5,
    Div = 6
  }

  public enum TokenType
  {
    Unknown,
    LeftSquareBracket,
    RightSquareBracket,
    LeftBracket,
    RightBracket,
    Identifier,
    Number,
    AdditionOperator,
    SubtractOperator,
    MultiplyOperator,
    DivideOperator,
    EndOfLine,
  }



  public struct Token
  {
    public static Token Unknown { get; }



    public TokenType Type { get; }
    public string Identifier { get; }
    public int Value { get; }



    static Token()
    {
      Unknown = new Token(TokenType.Unknown);
    }



    public Token(
      TokenType type)
      : this(type, string.Empty, 0)
    {

    }
    public Token(
      TokenType type,
      string identifier)
      : this(type, identifier, 0)
    {

    }
    public Token(
      TokenType type,
      int value)
      : this(type, string.Empty, value)
    {

    }
    public Token(
      TokenType type,
      string identifier,
      int value)
    {
      Type = type;
      Identifier = identifier;
      Value = value;
    }
  }



  public class Argument
  {
    public string Name { get; }
    public int Index { get; }



    public Argument(
      string name,
      int index)
    {
      Name = name;
      Index = index;
    }
  }



  public abstract class Operator
  {
    public string Name { get; }
    public OperatorType Type { get; }
    public byte Precedence { get; }



    protected Operator(
      string name,
      OperatorType type,
      byte precedence)
    {
      Name = name;
      Type = type;
      Precedence = precedence;
    }
  }

  public sealed class UnaryOperator : Operator
  {
    public static UnaryOperator Arg { get; }
    public static UnaryOperator Imm { get; }



    private Func<int, int> Operation { get; }
    private Action<InstructionBuilder, int> Instructions { get; }



    static UnaryOperator()
    {
      Arg = new UnaryOperator("arg", OperatorType.Arg, 255,
        number => number,
        (builder, value) => builder.AR(value));
      Imm = new UnaryOperator("imm", OperatorType.Imm, 255,
        number => number,
        (builder, value) => builder.IM(value));
    }



    private UnaryOperator(
      string name,
      OperatorType type,
      byte precedence,
      Func<int, int> operation,
      Action<InstructionBuilder, int> instructions)
      : base(name, type, precedence)
    {
      Operation = operation;
      Instructions = instructions;
    }



    public int Apply(
      int number)
    {
      return Operation?.Invoke(
        number) ?? 0;
    }

    public InstructionBuilder GenerateInstructions(
      InstructionBuilder builder, int value)
    {
      Instructions?.Invoke(
        builder, value);

      return builder;
    }
  }

  public sealed class BinaryOperator : Operator
  {
    public static BinaryOperator Add { get; }
    public static BinaryOperator Sub { get; }
    public static BinaryOperator Mul { get; }
    public static BinaryOperator Div { get; }



    private Func<int, int, int> Operation { get; }
    private Action<InstructionBuilder> Instructions { get; }



    static BinaryOperator()
    {
      Add = new BinaryOperator("+", OperatorType.Add, 1,
        (number1, number2) => number1 + number2,
        builder => builder.AD());
      Sub = new BinaryOperator("-", OperatorType.Sub, 1,
        (number1, number2) => number1 - number2,
        builder => builder.SU());
      Mul = new BinaryOperator("*", OperatorType.Mul, 2,
        (number1, number2) => number1 * number2,
        builder => builder.MU());
      Div = new BinaryOperator("/", OperatorType.Div, 2,
        (number1, number2) => number1 / number2,
        builder => builder.DI());
    }



    private BinaryOperator(
      string name,
      OperatorType type,
      byte precedence,
      Func<int, int, int> operation,
      Action<InstructionBuilder> instructions)
      : base(name, type, precedence)
    {
      Operation = operation;
      Instructions = instructions;
    }



    public int Apply(
      int number1, int number2)
    {
      return Operation?.Invoke(
        number1, number2) ?? 0;
    }

    public InstructionBuilder GenerateInstructions(
      InstructionBuilder builder)
    {
      Instructions?.Invoke(
        builder);

      return builder;
    }
  }



  public abstract class Expression
  {
    public abstract Expression Optimize();

    public InstructionBuilder GenerateInstructions(
      AssociativityType associativity)
    {
      return GenerateInstructions(
        new InstructionBuilder(), associativity);
    }
    public abstract InstructionBuilder GenerateInstructions(
      InstructionBuilder builder,
      AssociativityType associativity);

    public abstract AST ToAST();
  }

  public class UnaryExpression : Expression
  {
    public UnaryOperator Operator { get; }
    public int Value { get; private set; }



    public UnaryExpression(UnaryOperator @operator,
      int value)
    {
      Operator = @operator;
      Value = value;
    }



    public override Expression Optimize()
    {
      return this;
    }

    public override InstructionBuilder GenerateInstructions(
      InstructionBuilder builder, AssociativityType associativity)
    {
      Operator.GenerateInstructions(
        builder, Value);

      return builder;
    }

    public override AST ToAST()
    {
      return new AST(Operator,
        Value);
    }
  }

  public class BinaryExpression : Expression
  {
    public BinaryOperator Operator { get; }
    public Expression Left { get; private set; }
    public Expression Right { get; private set; }



    public BinaryExpression(BinaryOperator @operator,
      Expression left, Expression right)
    {
      Operator = @operator;
      Left = left;
      Right = right;
    }



    public override Expression Optimize()
    {
      Left = Left.Optimize();
      Right = Right.Optimize();
      
      if (!(Left is UnaryExpression { Operator: { Type: OperatorType.Imm } } leftImm) ||
        !(Right is UnaryExpression { Operator: { Type: OperatorType.Imm } } rightImm))
      {
        return this;
      }

      return new UnaryExpression(UnaryOperator.Imm,
        Operator.Apply(leftImm.Value, rightImm.Value));
    }

    public override InstructionBuilder GenerateInstructions(
      InstructionBuilder builder, AssociativityType associativity)
    {
      if (associativity == AssociativityType.Right)
      {
        builder.SW();
        builder.PU();
      }

      Left.GenerateInstructions(builder, AssociativityType.Left);
      builder.SW();

      Right.GenerateInstructions(builder, AssociativityType.Right);
      builder.SW();

      Operator.GenerateInstructions(
        builder);

      if (associativity == AssociativityType.Right)
      {
        builder.SW();
        builder.PO();
        builder.SW();
      }

      return builder;
    }

    public override AST ToAST()
    {
      return new AST(Operator,
        Left.ToAST(), Right.ToAST());
    }
  }



  public class Function 
  {
    public ReadOnlyDictionary<string, Argument> Arguments { get; }
    public Expression Body { get; private set; }



    public Function(
      ReadOnlyDictionary<string, Argument> arguments,
      Expression body)
    {
      Arguments = arguments;
      Body = body;
    }



    public AST ToAST()
    {
      return Body.ToAST();
    }
  }



  public class AST
  {
    public Operator Operator { get; }
    public AST Left { get; }
    public AST Right { get; }
    public int? Value { get; }



    public AST(UnaryOperator @operator,
      int value)
      : this(@operator, null,
        null, value)
    {

    }
    public AST(BinaryOperator @operator,
      AST left, AST right)
      : this(@operator, left,
        right, null)
    {

    }
    private AST(Operator @operator,
      AST left, AST right, int? value)
    {
      Operator = @operator;
      Left = left;
      Right = right;
      Value = value;
    }



    public Expression ToExpression()
    {
      if (Operator is UnaryOperator unaryOperator)
      {
        return new UnaryExpression(unaryOperator,
          Value ?? 0);
      }
      else if (Operator is BinaryOperator binaryOperator)
      {
        var leftExpression = Left.ToExpression();
        var rightExpression = Right.ToExpression();

        return new BinaryExpression(binaryOperator,
          leftExpression, rightExpression);
      }

      return null;
    }



    // For Kata

    public static implicit operator Ast(AST ast)
    {
      if (ast.Operator is UnaryOperator unaryOperator)
      {
        return new UnOp(unaryOperator.Name,
          ast.Value ?? 0);
      }
      else if (ast.Operator is BinaryOperator binaryOperator)
      {
        return new BinOp(binaryOperator.Name,
          ast.Left, ast.Right);
      }

      return null;
    }



    public static implicit operator AST(Ast ast)
    {
      if (ast is UnOp unaryOperator)
      {
        var newOperator = ast.op() switch
        {
          "arg" => UnaryOperator.Arg,
          "imm" => UnaryOperator.Imm,
          _ => null
        };

        return new AST(newOperator,
          unaryOperator.n());
      }
      else if (ast is BinOp binaryOperator)
      {
        var newOperator = ast.op() switch
        {
          "+" => BinaryOperator.Add,
          "-" => BinaryOperator.Sub,
          "*" => BinaryOperator.Mul,
          "/" => BinaryOperator.Div,
          _ => null
        };

        return new AST(newOperator,
          binaryOperator.a(), binaryOperator.b());
      }

      return null;
    }
  }



  public class InstructionBuilder
  {
    private readonly List<string> _instructions;
    public ReadOnlyCollection<string> Instructions
    {
      get
      {
        return new ReadOnlyCollection<string>(
          _instructions);
      }
    }



    public InstructionBuilder()
    {
      _instructions = new List<string>();
    }



    private void CreateInstruction(
      string instruction)
    {
      _instructions.Add(
        instruction);
    }

    private void CreateInstruction(
      string instruction, int value)
    {
      _instructions.Add(
        $"{instruction} {value}");
    }



    public void PU()
    {
      CreateInstruction("PU");
    }
    public void PO()
    {
      CreateInstruction("PO");
    }
    public void SW()
    {
      CreateInstruction("SW");
    }
    public void AR(int value)
    {
      CreateInstruction("AR", value);
    }
    public void IM(int value)
    {
      CreateInstruction("IM", value);
    }
    public void AD()
    {
      CreateInstruction("AD");
    }
    public void SU()
    {
      CreateInstruction("SU");
    }
    public void MU()
    {
      CreateInstruction("MU");
    }
    public void DI()
    {
      CreateInstruction("DI");
    }
  }



  public class Parser
  {
    private struct TokenEnumerator : IEnumerator<Token>
    {
      private List<string> _tokens;
      private int _position;



      public Token Current { get; private set; }



      object IEnumerator.Current
      {
        get
        {
          return Current;
        }
      }



      public TokenEnumerator(
        List<string> tokens)
      {
        _tokens = tokens;
        _position = -1;

        Current = Token.Unknown;
      }



      public bool MoveNext()
      {
        if (_position >= _tokens.Count - 1)
        {
          Current = new Token(TokenType.EndOfLine);

          return false;
        }

        ++_position;

        var token = _tokens[_position];

        Current = token[0] switch
        {
          '+' => new Token(TokenType.AdditionOperator),
          '-' => new Token(TokenType.SubtractOperator),
          '*' => new Token(TokenType.MultiplyOperator),
          '/' => new Token(TokenType.DivideOperator),
          '(' => new Token(TokenType.LeftBracket),
          ')' => new Token(TokenType.RightBracket),
          '[' => new Token(TokenType.LeftSquareBracket),
          ']' => new Token(TokenType.RightSquareBracket),
          var value when char.IsDigit(value) =>
            new Token(TokenType.Number, Convert.ToInt32(token)),
          var value when char.IsLetter(value) =>
            new Token(TokenType.Identifier, token),
          _ => new Token(TokenType.EndOfLine)
        };

        return true;
      }

      public void Reset()
      {
        _position = -1;

        Current = Token.Unknown;
      }

      public void Dispose()
      {
        _tokens = null;
        _position = -1;

        Current = Token.Unknown;
      }
    }



    private static readonly Regex AllTokensRegex = new Regex(
      @"(?:[\[\]\(\)+\-*/]|[A-Za-z]+|[0-9]+)",
      RegexOptions.Compiled | RegexOptions.Singleline);



    private TokenEnumerator _enumerator;

    private Token Current
    {
      get
      {
        return _enumerator.Current;
      }
    }



    public Parser(string program)
    {
      _enumerator = new TokenEnumerator(
        Tokenize(program));

      MoveNext();
    }



    private static List<string> Tokenize(
      string program)
    {
      var tokens = new List<string>();
      var tokenMatch = AllTokensRegex.Match(
        program);

      while (tokenMatch.Success)
      {
        var token = tokenMatch
          .Value
          .Trim();

        if (string.IsNullOrEmpty(token))
        {
          tokenMatch = tokenMatch
            .NextMatch();

          continue;
        }

        tokens.Add(
          token);

        tokenMatch = tokenMatch
          .NextMatch();
      }

      return tokens;
    }



    private bool MoveNext()
    {
      return _enumerator.MoveNext();
    }



    private ReadOnlyDictionary<string, Argument> ParseArguments()
    {
      var arguments = new Dictionary<string, Argument>();
      var index = 0;

      MoveNext();

      while (Current.Type != TokenType.EndOfLine &&
           Current.Type != TokenType.RightSquareBracket)
      {
        if (Current.Type != TokenType.Identifier)
          continue;

        arguments.Add(Current.Identifier,
          new Argument(Current.Identifier, index));

        ++index;

        MoveNext();
      }

      MoveNext();

      return new ReadOnlyDictionary<string, Argument>(
        arguments);
    }

    private Expression ParseExpression(
      ReadOnlyDictionary<string, Argument> arguments)
    {
      var left = ParseTerm(arguments);

      while (Current.Type != TokenType.EndOfLine)
      {
        if (Current.Type == TokenType.AdditionOperator)
        {
          MoveNext();

          var right = ParseTerm(arguments);

          left = new BinaryExpression(
            BinaryOperator.Add, left, right);
        }
        else if (Current.Type == TokenType.SubtractOperator)
        {
          MoveNext();

          var right = ParseTerm(arguments);

          left = new BinaryExpression(
            BinaryOperator.Sub, left, right);
        }
        else
        {
          return left;
        }
      }

      return left;
    }

    private Expression ParseTerm(
      ReadOnlyDictionary<string, Argument> arguments)
    {
      var left = ParseFactor(arguments);

      while (Current.Type != TokenType.EndOfLine)
      {
        if (Current.Type == TokenType.MultiplyOperator)
        {
          MoveNext();

          var right = ParseFactor(arguments);

          left = new BinaryExpression(
            BinaryOperator.Mul, left, right);
        }
        else if (Current.Type == TokenType.DivideOperator)
        {
          MoveNext();

          var right = ParseFactor(arguments);

          left = new BinaryExpression(
            BinaryOperator.Div, left, right);
        }
        else
        {
          return left;
        }
      }

      return left;
    }

    private Expression ParseFactor(
      ReadOnlyDictionary<string, Argument> arguments)
    {
      if (Current.Type == TokenType.LeftBracket)
      {
        MoveNext();

        var expression = ParseExpression(
          arguments);

        MoveNext();

        return expression;
      }
      else if (Current.Type == TokenType.Number)
      {
        return ParseNumber();
      }
      else if (Current.Type == TokenType.Identifier)
      {
        return ParseVariable(arguments);
      }

      return null;
    }

    private Expression ParseVariable(
      ReadOnlyDictionary<string, Argument> arguments)
    {
      if (!arguments.TryGetValue(Current.Identifier, out var argument))
        return null;

      MoveNext();

      return new UnaryExpression(
        UnaryOperator.Arg, argument.Index);
    }

    private Expression ParseNumber()
    {
      var value = Current.Value;

      MoveNext();

      return new UnaryExpression(
        UnaryOperator.Imm, value);
    }



    public Function ParseFunction()
    {
      var arguments = new ReadOnlyDictionary<string, Argument>(
        new Dictionary<string, Argument>());
      Expression body = null;

      if (Current.Type == TokenType.LeftSquareBracket)
        arguments = ParseArguments();
      if (Current.Type != TokenType.EndOfLine)
        body = ParseExpression(arguments);

      return new Function(
        arguments, body);
    }
  }

  public class Compiler
  {
    private AST Pass1(
      string program)
    {
      return new Parser(program)
        .ParseFunction()
        .ToAST();
    }

    private AST Pass2(
      AST ast)
    {
      return ast
        .ToExpression()
        .Optimize()
        .ToAST();
    }

    private ReadOnlyCollection<string> Pass3(
      AST ast)
    {
      return ast
        .ToExpression()
        .GenerateInstructions(AssociativityType.Left)
        .Instructions;
    }



    public ReadOnlyCollection<string> Compile(
      string program)
    {
      return Pass3(Pass2(Pass1(program)));
    }
    
    
    
    // For Kata
    
    public Ast pass1(
      string program)
    {
      return Pass1(program);
    }
    
    public Ast pass2(
      Ast ast)
    {
      return Pass2(ast);
    }
    
    public List<string> pass3(
      Ast ast)
    {
      return Pass3(ast)
        .ToList();
    }
  }
}