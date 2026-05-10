using System.Text;
using Novellus.Lib.Backend.Logging;

namespace Novellus.Lib.Backend.Packages.PackageConfig.ConditionInterpreter;

public sealed class Lexer(string source)
{
    private string Source { get; init; } = source.Trim();
    private int Pos { get; set; } = 0;
    private StringBuilder Sb { get; } = new();
    
    public List<Token>? Tokenize()
    {
        List<Token> tokens = [];

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
                    Logger.Error($"expected equals at position {Pos + 1}");
                    return null;
                }
                tokens.Add(new Token(TokenType.Equal, "==", Pos)); 
                Pos += 2;
            }
            else if (c == '!')
            {
                if (!Peek().Equals('='))
                {
                    Logger.Error($"expected equals at position {Pos + 1}");
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
                Logger.Error($"unexpected character {c} at position {Pos}" );
                return null;
            }
        }

        tokens.Add(new Token(TokenType.Eof, null, Pos));
        return tokens;
    }
    
    private char Current() =>  Source[Pos];
    private char Peek() => (Pos + 1 < Source.Length) ? Source[Pos + 1] : '\0';
    private void SkipWhiteSpace()
    {
        while (Pos < Source.Length && char.IsWhiteSpace(Current()))
            Pos++;
    }
    private string GetLiteral()
    {
        string literal = Sb.ToString();
        Sb.Clear();
        return literal;
    }
    private bool IsIdentStartChar(char c) => char.IsLetter(c) || c == '_';
    private bool IsIdentChar(char c) => IsIdentStartChar(c) || char.IsDigit(c) || c == '.';
    
    private Token? ReadStringLiteral()
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
            Logger.Error("you dumbass left a unterminated string literal");
            return null;
        }

        Pos++;
        return new Token(TokenType.StringLiteral, GetLiteral(), start);
    }

    private Token? ReadNumberLiteral()
    {
        int start = Pos;
        
        while (Pos < Source.Length && char.IsDigit(Current()))
        {
            Sb.Append(Current());
            Pos++;
        }
        
        // early return if we have a int
        if (Pos >= Source.Length || Current() == ')' ||  Current() == ' ')
            return new Token(TokenType.NumberLiteral, GetLiteral(), start);
        
        if (Current() != '.')
        {
            Logger.Error($"expected a dot (.), a close parenthesis or a empty space at position {Pos}");
            return null;
        }
        
        // if we have a dot, but if we peek not a number, throw a error
        if (!char.IsDigit(Peek()))
        {
            Logger.Error($"expected a number at position {Pos + 1}");
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

    private Token ReadWord()
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
            "MOD_ENABLED" =>  TokenType.ModEnabled,
            "MOD_VERSION"  => TokenType.ModVersion,
            _ => TokenType.Identifier
        };
        
        return new Token(type, word, start);
    }
}