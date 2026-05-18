using Novellus.Lib.Backend.Logging;
using System.Text;

namespace Novellus.Lib.Backend.Packages.ConditionInterpreter;

public static class Lexer
{
    private static int Pos { get; set; } = 0;
    private static StringBuilder Sb { get; } = new();
    private static string Source = string.Empty;

    public static List<Token>? Tokenize(string source)
    {
        List<Token> tokens = [];

        Source = source.Trim();
        Pos = 0;
        Sb.Clear();

        while (Pos < Source.Length)
        {
            SkipWhiteSpace();
            if (Pos >= Source.Length) break;

            char c = Current();

            if (c == '(')
            {
                tokens.Add(new Token(TokenType.LeftParen, "(", Pos));
                Pos++;
            }
            else if (c == ')')
            {
                tokens.Add(new Token(TokenType.RightParen, ")", Pos));
                Pos++;
            }
            else if (c == ',')
            {
                tokens.Add(new Token(TokenType.Comma, ",", Pos));
                Pos++;
            }

            else if (c == '=')
            {
                if (!Peek().Equals('='))
                {
                    Logger.Error($"expected equals at position {Pos + 1} in condition '{Source}'");
                    return null;
                }
                tokens.Add(new Token(TokenType.Equal, "==", Pos));
                Pos += 2;
            }
            else if (c == '!')
            {
                if (!Peek().Equals('='))
                {
                    Logger.Error($"expected equals at position {Pos + 1} in condition '{Source}'");
                    return null;
                }
                tokens.Add(new Token(TokenType.NotEqual, "!=", Pos));
                Pos += 2;
            }
            else if (c == '>' && Peek() == '=')
            {
                tokens.Add(new Token(TokenType.GreaterThanOrEqual, ">=", Pos));
                Pos += 2;
            }
            else if (c == '<' && Peek() == '=')
            {
                tokens.Add(new Token(TokenType.LessThanOrEqual, "<=", Pos));
                Pos += 2;
            }
            else if (c == '>')
            {
                tokens.Add(new Token(TokenType.GreaterThan, ">", Pos));
                Pos++;
            }
            else if (c == '<')
            {
                tokens.Add(new Token(TokenType.LessThan, "<", Pos));
                Pos++;
            }

            else if (c == '"')
            {
                Token? strLiteral = ReadStringLiteral();
                if (strLiteral is null) return null;
                tokens.Add(strLiteral);
            }
            else if (char.IsDigit(c))
            {
                Token? numberLiteral = ReadNumberLiteral();
                if (numberLiteral is null) return null;
                tokens.Add(numberLiteral);
            }
            else if (IsIdentStartChar(c))
                tokens.Add(ReadWord());

            else
            {
                Logger.Error($"unexpected character {c} at position {Pos} in condition '{Source}'");
                return null;
            }
        }

        tokens.Add(new Token(TokenType.Eof, null, Pos));
        return tokens;
    }

    private static char Current() => Source[Pos];
    private static char Peek() => (Pos + 1 < Source.Length) ? Source[Pos + 1] : '\0';
    private static void SkipWhiteSpace()
    {
        while (Pos < Source.Length && char.IsWhiteSpace(Current()))
            Pos++;
    }
    private static string GetLiteral()
    {
        string literal = Sb.ToString();
        Sb.Clear();
        return literal;
    }
    private static bool IsIdentStartChar(char c) => char.IsLetter(c) || c == '_';
    private static bool IsIdentChar(char c) => IsIdentStartChar(c) || char.IsDigit(c) || c == '.';

    private static Token? ReadStringLiteral()
    {
        int start = Pos;
        Pos++;

        while (Pos < Source.Length && Current() != '"')
        {

            if (Current() == '\\' && Peek() == '"')
            {
                Sb.Append('"');
                Pos += 2;
                continue;
            }
            Sb.Append(Current());
            Pos++;
        }

        if (Pos >= Source.Length && !Source.EndsWith('"'))
        {
            // TODO: some proper error handling :d
            Logger.Error($"you dumbass left a unterminated string literal in condition '{Source}'");
            return null;
        }

        Pos++;
        return new Token(TokenType.StringLiteral, GetLiteral(), start);
    }

    private static Token? ReadNumberLiteral()
    {
        int start = Pos;

        while (Pos < Source.Length && char.IsDigit(Current()))
        {
            Sb.Append(Current());
            Pos++;
        }

        // early return if we have a int
        if (Pos >= Source.Length || Current() == ')' || Current() == ' ')
            return new Token(TokenType.NumberLiteral, GetLiteral(), start);

        if (Current() != '.')
        {
            Logger.Error($"expected a dot (.), a close parenthesis or a empty space at position {Pos} in condition '{Source}'");
            return null;
        }

        // if we have a dot, but if we peek not a number, throw a error
        if (!char.IsDigit(Peek()))
        {
            Logger.Error($"expected a number at position {Pos + 1} in condition '{Source}'");
            return null;
        }

        Sb.Append(Current());
        Pos++;

        while (Pos < Source.Length && char.IsDigit(Current()))
        {
            Sb.Append(Current());
            Pos++;
        }
        return new Token(TokenType.FloatLiteral, GetLiteral(), start);

    }

    private static Token ReadWord()
    {
        int start = Pos;

        while (Pos < Source.Length && IsIdentChar(Current()))
        {
            Sb.Append(Current());
            Pos++;
        }

        string word = GetLiteral();
        TokenType type = word.ToUpperInvariant() switch
        {
            "AND" => TokenType.And,
            "OR" => TokenType.Or,
            "NOT" => TokenType.Not,
            "MOD_ENABLED" => TokenType.ModEnabled,
            "MOD_VERSION" => TokenType.ModVersion,
            _ => TokenType.Identifier
        };

        return new Token(type, word, start);
    }
}