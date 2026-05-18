using Novellus.Lib.Backend.Logging;
using System.Globalization;

namespace Novellus.Lib.Backend.Packages.ConditionInterpreter;

public static class Parser
{
    private static List<Token>? Tokens { get; set; }
    private static int Pos { get; set; } = 0;

    // PRECEDENCE:
    // or -> and -> not -> comparison -> primary

    public static AstNode? Parse(List<Token> tokens)
    {
        Tokens = tokens;
        Pos = 0;

        var node = ParseOr();
        if (node is null) return null;

        return !Check(TokenType.Eof) ? null : node;
    }

    private static AstNode? ParseOr()
    {
        var left = ParseAnd();
        if (left is null) return null;

        while (Check(TokenType.Or))
        {
            var opToken = Current(); // before parse and, for logging purposes
            Advance();

            var right = ParseAnd();
            if (right is null)
            {
                Logger.Error($"expected expression after 'or' at position {opToken.Position}");
                return null;
            }
            left = new LogicalOpNode(LogicalOperator.Or, left, right);
        }

        return left;
    }
    private static AstNode? ParseAnd()
    {
        var left = ParseNot();
        if (left is null) return null;

        while (Check(TokenType.And))
        {
            var opToken = Current();
            Advance();

            var right = ParseNot();
            if (right is null)
            {
                Logger.Error($"expected expression after 'and' at position {opToken.Position}");
                return null;
            }
            left = new LogicalOpNode(LogicalOperator.And, left, right);
        }

        return left;
    }
    private static AstNode? ParseNot()
    {
        if (Check(TokenType.Not))
        {
            var opToken = Current();
            Advance();

            var operand = ParseNot();
            if (operand is null)
            {
                Logger.Error($"expected expression after 'not' at position {opToken.Position}");
                return null;
            }
            return new NotNode(operand);
        }

        return ParseComparison();
    }
    private static AstNode? ParseComparison()
    {
        var left = ParsePrimary();
        if (left is null) return null;

        if (Current().Type.IsComparisonOperator())
        {
            var opToken = Current();
            var op = opToken.Type.GetComparisonOperator();
            Advance();

            var right = ParsePrimary();
            if (right is null)
            {
                Logger.Error($"expected value after '{opToken.Value}' at position {opToken.Position}");
                return null;
            }

            return new ComparisonNode(op, left, right);
        }

        return left;
    }
    private static AstNode? ParsePrimary()
    {
        Token t = Current();
        TokenType type = t.Type;

        switch (type)
        {
            case TokenType.ModEnabled:
                return ParseMacro(Macro.ModEnabled);
            case TokenType.ModVersion:
                return ParseMacro(Macro.ModVersion);
            case TokenType.StringLiteral:
                Advance();
                return new StringValueNode(t.Value!);
            case TokenType.NumberLiteral:
                Advance();
                return new NumberValueNode(int.Parse(t.Value!)); // TODO: handle cases where parse is null
            case TokenType.FloatLiteral:
                Advance();
                return new FloatValueNode(float.Parse(t.Value!, CultureInfo.InvariantCulture));
            case TokenType.Identifier:
                Advance();
                return new OptionValueNode(t.Value!);
            case TokenType.LeftParen:
                {
                    int openPos = t.Position; // save pos for logging 
                    Advance();

                    var inner = ParseOr(); // go back to the begin
                    if (inner is null) return null;
                    if (!Check(TokenType.RightParen))
                    {
                        Logger.Error($"expected ')' to close parenthesis opened at {openPos} but found {Current().Value} at position {Current().Position}");
                        return null;
                    }

                    Advance();
                    return inner;
                }
            default:
                Logger.Error($"unexpected token at position {Current().Position}");
                return null;
        }
    }
    private static MacroCallNode? ParseMacro(Macro macro)
    {
        Advance();

        if (Expect(TokenType.LeftParen) == null)
        {
            Logger.Error($"expected '(' after macro call at position {Current().Position}");
            return null;
        }

        var idToken = Expect(TokenType.Identifier);
        if (idToken == null)
        {
            Logger.Error($"expected macro argument at position {Current().Position}");
            return null;
        }

        if (Expect(TokenType.RightParen) == null)
        {
            Logger.Error($"expected ')' to close {macro}(...) at position {Current().Position}");
            return null;
        }

        return new MacroCallNode(macro, [idToken.Value!]);
    }

    private static Token Current() => Tokens[Pos];
    private static bool Check(TokenType type) => Current().Type == type;

    private static Token Advance()
    {
        var token = Current();
        if (token.Type != TokenType.Eof)
            Pos++;
        return token;
    }

    private static Token? Expect(TokenType type)
    {
        if (!Check(type))
        {
            Logger.Error($"expected {type} but found {Current().Type} at position {Current().Position}");
            return null;
        }

        return Advance();
    }
}