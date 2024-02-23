// https://www.codewars.com/kata/52ffcfa4aff455b3c2000750



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InterpreterKata
{
    public class Interpreter
    {
        private enum OperatorType : byte
        {
            Unknown = 0,
            Add = 1,
            Sub = 2,
            Mul = 3,
            Div = 4,
            Mod = 5,
            Assign = 6
        }

        private enum AssociativityType : byte
        {
            Unknown = 0,
            Left = 1,
            Right = 2
        }

        private abstract class OperatorBase
        {
            public string Name { get; }
            public OperatorType Type { get; }
            public byte Precedence { get; }
            public AssociativityType Associativity { get; }
            public byte ParametersCount { get; }



            protected OperatorBase(
                string name,
                OperatorType type,
                byte precedence,
                AssociativityType associativity,
                byte parametersCount)
            {
                Name = name;
                Type = type;
                Precedence = precedence;
                Associativity = associativity;
                ParametersCount = parametersCount;
            }
        }

        private class NumberOperator : OperatorBase
        {
            public Func<double[], double> Operation { get; }



            public NumberOperator(
                string name,
                OperatorType type,
                byte precedence,
                AssociativityType associativity,
                byte parametersCount,
                Func<double[], double> operation)
                : base(name, type, precedence,
                    associativity, parametersCount)
            {
                Operation = operation;
            }



            public double? Apply(double[] numbers)
            {
                if (Operation == null)
                    return null;
                if (ParametersCount == 0)
                    return Operation(Array.Empty<double>());

                return Operation(numbers);
            }
        }

        private class VariableOperator : OperatorBase
        {
            public Func<Interpreter, string, double[], double> Operation { get; }



            public VariableOperator(
                string name,
                OperatorType type,
                byte precedence,
                AssociativityType associativity,
                byte parametersCount,
                Func<Interpreter, string, double[], double> operation)
                : base(name, type, precedence,
                    associativity, parametersCount)
            {
                Operation = operation;
            }



            public double? Apply(Interpreter interpreter,
                string name, double[] numbers)
            {
                if (Operation == null)
                    return null;
                if (ParametersCount == 0)
                    return Operation(interpreter, name, Array.Empty<double>());

                return Operation(interpreter, name, numbers);
            }
        }

        private enum KeywordType : byte
        {
            Unknown = 0,
            Clear = 1,
            DefineFunction = 2,
            ShowOperators = 3,
            ShowKeywords = 4,
            ShowVariables = 5,
            ShowFunctions = 6
        }

        private sealed class Keyword
        {
            public string Name { get; }
            public KeywordType Type { get; }



            public Keyword(
                string name,
                KeywordType type)
            {
                Name = name;
                Type = type;
            }
        }

        private sealed class Variable
        {
            public string Name { get; }
            public double? Value { get; set; }



            public Variable(
                string name,
                double? value = null)
            {
                Name = name;
                Value = value;
            }
        }

        private sealed class Function
        {
            private readonly Interpreter _interpreter;
            private readonly Environment _environment;
            private readonly List<string> _tokens;



            public string Name { get; }
            public List<Variable> Parameters { get; }
            public string ParametersString { get; }
            public string BodyString { get; }



            public Function(
                Interpreter interpreter,
                string name,
                List<string> tokens,
                Variable[] parameters)
            {
                _interpreter = interpreter;
                _environment = interpreter
                    ._environment.Copy();
                _tokens = tokens;

                Name = name;
                Parameters = new List<Variable>();
                ParametersString = string.Join(' ', parameters
                    .Select(variable => variable.Name));
                BodyString = string.Join(' ', tokens);

                foreach (var parameter in parameters)
                {
                    _environment.Variables[parameter.Name] = parameter;

                    Parameters.Add(parameter);
                }
            }



            public double? Invoke(params double[] numbers)
            {
                if (numbers.Length != Parameters.Count)
                    throw new Exception("ERROR: Incorrect number of arguments passed to function");

                for (var i = 0; i < Parameters.Count; ++i)
                {
                    Parameters[i].Value = numbers[i];
                }

                return _interpreter.EvaluateInternal(
                    _tokens, _environment);
            }
        }

        private sealed class Environment
        {
            public static Dictionary<string, OperatorBase> Operators { get; }
            public static Dictionary<string, Keyword> Keywords { get; }



            public Dictionary<string, Variable> Variables { get; }
            public Dictionary<string, Function> Functions { get; }



            static Environment()
            {
                Operators = new Dictionary<string, OperatorBase>
                {
                    {"=", new VariableOperator("=", OperatorType.Assign, 1, AssociativityType.Right, 1,
                        (interpreter, name, numbers) => (interpreter._environment.Variables[name].Value = numbers[0]).Value)},
                    {"+", new NumberOperator("+", OperatorType.Add, 2, AssociativityType.Left, 2,
                        numbers => numbers[0] + numbers[1])},
                    {"-", new NumberOperator("-", OperatorType.Sub, 2, AssociativityType.Left, 2,
                        numbers => numbers[0] - numbers[1])},
                    {"*", new NumberOperator("*", OperatorType.Mul, 3, AssociativityType.Left, 2,
                        numbers => numbers[0] * numbers[1])},
                    {"/", new NumberOperator("/", OperatorType.Div, 3, AssociativityType.Left, 2,
                        numbers => numbers[0] / numbers[1])},
                    {"%", new NumberOperator("%", OperatorType.Mod, 3, AssociativityType.Left, 2,
                        numbers => numbers[0] % numbers[1])}
                };
                Keywords = new Dictionary<string, Keyword>
                {
                    {"clear", new Keyword("clear", KeywordType.Clear)},
                    {"fn", new Keyword("fn", KeywordType.DefineFunction)},
                    {"func", new Keyword("func", KeywordType.DefineFunction)},
                    {"ops", new Keyword("ops", KeywordType.ShowOperators)},
                    {"words", new Keyword("words", KeywordType.ShowKeywords)},
                    {"vars", new Keyword("vars", KeywordType.ShowVariables)},
                    {"funcs", new Keyword("funcs", KeywordType.ShowFunctions)}
                };
            }

            public Environment()
                : this(null, null)
            {

            }
            private Environment(
                Dictionary<string, Variable> variables,
                Dictionary<string, Function> functions)
            {
                Variables = new Dictionary<string, Variable>();

                if (variables != null)
                {
                    foreach (var variablePair in variables)
                    {
                        Variables[variablePair.Key] = variablePair.Value;
                    }
                }

                Functions = new Dictionary<string, Function>();

                if (functions != null)
                {
                    foreach (var functionPair in functions)
                    {
                        Functions[functionPair.Key] = functionPair.Value;
                    }
                }
            }



            public void RemoveUndeclaredIdentifiers()
            {
                foreach (var variablePair in Variables
                    .Where(variablePair => !variablePair.Value.Value.HasValue))
                {
                    Variables.Remove(variablePair.Key);
                }
            }

            public Environment Copy()
            {
                return new Environment(
                    Variables, Functions);
            }

            public void Clear()
            {
                Variables.Clear();
                Functions.Clear();
            }
        }



        private static readonly Regex AllTokensRegex = new Regex(
            @"(?:=>|[\-+*/%=\(\)]|[A-Za-z_][A-Za-z0-9_]*|‒?[0-9]+(?:\.[0-9]+)?)",
            RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex NumberTokenRegex = new Regex(
            @"^(?:‒?[0-9]+(?:\.[0-9]+)?)$",
            RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex IdentifierRegex = new Regex(
            @"^(?:[A-Za-z_][A-Za-z0-9_]*)$",
            RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex NumberRegex = new Regex(
            @"^(?:-?[0-9]+(?:\.[0-9]+)?)$",
            RegexOptions.Compiled | RegexOptions.Singleline);
        private static readonly Regex OperatorRegex = new Regex(
            @"^(?:[\-+*/%=])$",
            RegexOptions.Compiled | RegexOptions.Singleline);



        private readonly Environment _environment;



        public Interpreter()
        {
            _environment = new Environment();
        }



        private static List<string> Tokenize(
            string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return new List<string>();

            expression = Regex.Replace(expression,
                @"(?<part1>[\-+*/%=\(\)\s])-(?<part2>[0-9])", "${part1}‒${part2}");
            expression = Regex.Replace(expression,
                @"^-(?<part2>[0-9])", "‒${part2}");

            var tokens = new List<string>();
            var tokenMatch = AllTokensRegex.Match(
                expression);

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

                if (NumberTokenRegex.IsMatch(token)
                    && token[0] == '‒')
                {
                    token = $"-{token[1..]}";
                }

                tokens.Add(
                    token);

                tokenMatch = tokenMatch
                    .NextMatch();
            }

            return tokens;
        }



        private static bool IsIdentifier(
            string token)
        {
            return IdentifierRegex
                .IsMatch(token);
        }

        private static bool IsNumber(
            string token)
        {
            return NumberRegex
                .IsMatch(token);
        }

        private static bool IsOperator(
            string token)
        {
            return OperatorRegex
                .IsMatch(token);
        }



        private List<object> Parse(
            List<string> tokens,
            Environment environment = null)
        {
            environment ??= _environment;

            var resultTokens = new List<object>();
            var operationTokens = new List<string>();

            foreach (var token in tokens)
            {
                if (IsNumber(token))
                {
                    resultTokens.Add(
                        double.Parse(token,
                            CultureInfo.InvariantCulture));
                }
                else if (environment.Functions.TryGetValue(token, out var nextFunction))
                {
                    operationTokens.Add(
                        nextFunction.Name);
                }
                else if (environment.Variables.TryGetValue(token, out var nextVariable))
                {
                    resultTokens.Add(
                        nextVariable);
                }
                else if (Environment.Operators.TryGetValue(token, out var nextOperator))
                {
                    while (operationTokens.Count != 0
                           && Environment.Operators.TryGetValue(operationTokens[^1], out var previousOperator)
                           && (nextOperator.Associativity == AssociativityType.Left
                               && nextOperator.Precedence <= previousOperator.Precedence
                               || nextOperator.Associativity == AssociativityType.Right
                               && nextOperator.Precedence < previousOperator.Precedence))
                    {
                        resultTokens.Add(
                            previousOperator);
                        operationTokens.RemoveAt(
                            operationTokens.Count - 1);
                    }

                    operationTokens.Add(
                        nextOperator.Name);
                }
                else if (token == "(")
                {
                    operationTokens.Add(
                        token);
                }
                else if (token == ")")
                {
                    while (operationTokens.Count != 0
                           && operationTokens[^1] != "("
                           && Environment.Operators.TryGetValue(operationTokens[^1], out var previousOperator))
                    {
                        resultTokens.Add(
                            previousOperator);
                        operationTokens.RemoveAt(
                            operationTokens.Count - 1);
                    }

                    if (operationTokens.Count != 0
                        && operationTokens[^1] == "(")
                    {
                        operationTokens.RemoveAt(
                            operationTokens.Count - 1);
                    }
                    else
                    {
                        _environment.RemoveUndeclaredIdentifiers();
                      
                        throw new Exception("ERROR: Mismatched parentheses");
                    }

                    if (operationTokens.Count != 0
                        && environment.Functions.TryGetValue(operationTokens[^1], out var previousFunction))
                    {
                        resultTokens.Add(
                            previousFunction);
                        operationTokens.RemoveAt(
                            operationTokens.Count - 1);
                    }
                }
                else if (IsIdentifier(token))
                {
                    var variable = new Variable(
                        token);

                    environment.Variables.Add(
                        token, variable);

                    resultTokens.Add(
                        variable);
                }
                else
                {
                    _environment.RemoveUndeclaredIdentifiers();
                  
                    throw new Exception($"ERROR: Invalid token '{token}'");
                }
            }

            while (operationTokens.Count != 0)
            {
                if (Environment.Operators.TryGetValue(operationTokens[^1], out var previousOperator))
                {
                    resultTokens.Add(
                        previousOperator);
                    operationTokens.RemoveAt(
                        operationTokens.Count - 1);
                }
                else if (environment.Functions.TryGetValue(operationTokens[^1], out var previousFunction))
                {
                    resultTokens.Add(
                        previousFunction);
                    operationTokens.RemoveAt(
                        operationTokens.Count - 1);
                }
                else
                {
                    _environment.RemoveUndeclaredIdentifiers();
                  
                    throw new Exception($"ERROR: Invalid operation token '{operationTokens[^1]}'");
                }
            }

            return resultTokens;
        }

        private double? EvaluateInternal(
            List<string> tokens,
            Environment environment = null)
        {
            environment ??= _environment;

            var typedTokens = Parse(
                tokens, environment);
            var resultStack = new Stack<object>();

            for (int i = 0; i < typedTokens.Count; ++i)
            {
                var typedToken = typedTokens[i];

                if (typedToken is Variable variableToken)
                {
                    resultStack.Push(
                        variableToken);
                }
                else if (typedToken is double numberToken)
                {
                    resultStack.Push(
                        numberToken);
                }
                else if (typedToken is VariableOperator variableOperatorToken)
                {
                    if (resultStack.Count < variableOperatorToken.ParametersCount + 1)
                        throw new Exception("ERROR: Incorrect number of arguments passed to variable operator");

                    if (variableOperatorToken.Type == OperatorType.Assign
                        && _environment.Functions.ContainsKey(variableOperatorToken.Name))
                    {
                        throw new Exception("ERROR: Cannot overwrite function with variable");
                    }

                    var numbers = new double[variableOperatorToken.ParametersCount];

                    for (int j = variableOperatorToken.ParametersCount - 1; j >= 0; --j)
                    {
                        var parameter = resultStack.Pop();

                        if (parameter is double numberParameter)
                        {
                            numbers[j] = numberParameter;
                        }
                        else if (parameter is Variable variableParameter)
                        {
                            if (!variableParameter.Value.HasValue)
                            {
                                environment.RemoveUndeclaredIdentifiers();

                                throw new Exception($"ERROR: Undeclared identifier '{variableParameter.Name}' referenced");
                            }

                            numbers[j] = variableParameter.Value.Value;
                        }
                    }

                    var variable = resultStack.Pop();

                    if (!(variable is Variable associatedVariable))
                        throw new Exception("ERROR: Unable to associate variable operator with variable");

                    var variableName = associatedVariable.Name;

                    var operatorResult = variableOperatorToken
                        .Apply(this, variableName, numbers);

                    if (!operatorResult.HasValue)
                        throw new Exception("ERROR: Variable operator apply error");

                    resultStack.Push(
                        operatorResult.Value);
                }
                else if (typedToken is NumberOperator numberOperatorToken)
                {
                    if (resultStack.Count < numberOperatorToken.ParametersCount)
                        throw new Exception("ERROR: Incorrect number of arguments passed to number operator");

                    var numbers = new double[numberOperatorToken.ParametersCount];

                    for (int j = numberOperatorToken.ParametersCount - 1; j >= 0; --j)
                    {
                        var parameter = resultStack.Pop();

                        if (parameter is double numberParameter)
                        {
                            numbers[j] = numberParameter;
                        }
                        else if (parameter is Variable variableParameter)
                        {
                            if (!variableParameter.Value.HasValue)
                            {
                                environment.RemoveUndeclaredIdentifiers();

                                throw new Exception($"ERROR: Undeclared identifier '{variableParameter.Name}' referenced");
                            }

                            numbers[j] = variableParameter.Value.Value;
                        }
                    }

                    var operatorResult = numberOperatorToken
                        .Apply(numbers);

                    if (!operatorResult.HasValue)
                        throw new Exception("ERROR: Number operator apply error");

                    resultStack.Push(
                        operatorResult.Value);
                }
                else if (typedToken is Function functionToken)
                {
                    if (resultStack.Count < functionToken.Parameters.Count)
                        throw new Exception("ERROR: Incorrect number of arguments passed to function");

                    var numbers = new double[functionToken.Parameters.Count];

                    for (int j = functionToken.Parameters.Count - 1; j >= 0; --j)
                    {
                        var parameter = resultStack.Pop();

                        if (parameter is double numberParameter)
                        {
                            numbers[j] = numberParameter;
                        }
                        else if (parameter is Variable variableParameter)
                        {
                            if (!variableParameter.Value.HasValue)
                            {
                                environment.RemoveUndeclaredIdentifiers();

                                throw new Exception($"ERROR: Undeclared identifier '{variableParameter.Name}' referenced");
                            }

                            numbers[j] = variableParameter.Value.Value;
                        }
                    }

                    var operatorResult = functionToken
                        .Invoke(numbers);

                    if (!operatorResult.HasValue)
                        throw new Exception("ERROR: Function invoke error");

                    resultStack.Push(
                        operatorResult.Value);
                }
            }

            if (resultStack.Count != 1)
                throw new Exception("ERROR: Invalid syntax");


            var result = resultStack.Pop();

            if (result is double numberResult)
            {
                return numberResult;
            }
            else if (result is Variable variableResult)
            {
                if (!variableResult.Value.HasValue)
                {
                    environment.RemoveUndeclaredIdentifiers();

                    throw new Exception($"ERROR: Undeclared identifier '{variableResult.Name}' referenced");
                }

                return variableResult.Value.Value;
            }

            return null;
        }



        public string Evaluate(
            string expression)
        {
            var tokens = Tokenize(
                expression);

            if (tokens.Count == 0)
                return string.Empty;

            if (!Environment.Keywords.TryGetValue(tokens[0], out var keyword))
            {
                return EvaluateInternal(tokens)?
                    .ToString(CultureInfo.InvariantCulture) ?? string.Empty;
            }

            if (keyword.Type == KeywordType.Clear)
            {
                _environment.Clear();

                return string.Empty;
            }
            else if (keyword.Type == KeywordType.DefineFunction)
            {
                var functionName = tokens[1];

                if (_environment.Variables.ContainsKey(functionName))
                    throw new Exception("ERROR: Cannot overwrite variable with function");

                var functionOperatorIndex = tokens.IndexOf("=>");
                var functionParametersUnique = tokens
                    .Skip(2)
                    .Take(functionOperatorIndex - 2)
                    .Distinct()
                    .ToArray();

                if (functionParametersUnique.Length != functionOperatorIndex - 2)
                    throw new Exception("ERROR: Duplicate parameters specified");

                var functionParameters = functionParametersUnique
                    .Select(parameter => new Variable(parameter))
                    .ToArray();
                var functionTokens = tokens
                    .Skip(functionOperatorIndex + 1)
                    .ToList();

                foreach (var token in functionTokens)
                {
                    if (IsIdentifier(token)
                        && !functionParametersUnique.Contains(token)
                        && !_environment.Variables.ContainsKey(token)
                        && !_environment.Functions.ContainsKey(token))
                    {
                        throw new Exception($"ERROR: Function body contains unknown identifier '{token}'");
                    }
                }

                var function = new Function(this,
                    functionName, functionTokens,
                    functionParameters);

                _environment.Functions[functionName] = function;

                return string.Empty;
            }
            else if (keyword.Type == KeywordType.ShowOperators)
            {
                return string.Join('\n', Environment.Operators.Keys);
            }
            else if (keyword.Type == KeywordType.ShowKeywords)
            {
                return string.Join('\n', Environment.Keywords.Keys);
            }
            else if (keyword.Type == KeywordType.ShowVariables)
            {
                var builder = new StringBuilder(
                    _environment.Variables.Count * 4);

                foreach (var variable in _environment.Variables.Values)
                {
                    builder.Append(variable.Name);
                    builder.Append(' ');
                    builder.Append('=');
                    builder.Append(' ');
                    builder.Append(variable.Value?.ToString()
                                   ?? "undefined");
                    builder.Append('\n');
                }

                if (builder.Length > 0)
                    builder.Remove(builder.Length - 1, 1);

                return builder.ToString();
            }
            else if (keyword.Type == KeywordType.ShowFunctions)
            {
                var builder = new StringBuilder(
                    _environment.Functions.Count * 10);

                foreach (var function in _environment.Functions.Values)
                {
                    builder.Append(function.Name);
                    builder.Append(' ');
                    builder.Append(function.ParametersString);
                    builder.Append(' ');
                    builder.Append('=');
                    builder.Append('>');
                    builder.Append(' ');
                    builder.Append(function.BodyString);
                    builder.Append('\n');
                }

                if (builder.Length > 0)
                    builder.Remove(builder.Length - 1, 1);

                return builder.ToString();
            }

            throw new Exception($"ERROR: Invalid keyword '{keyword.Name}'");
        }
      
        public double? input(
            string expression)
        {
            var result = Evaluate(
                expression);
          
            if (!double.TryParse(result, out var number))
                return null;
          
            return number;
        }
    }
}