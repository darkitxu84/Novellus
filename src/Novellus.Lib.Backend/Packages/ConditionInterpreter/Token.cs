namespace Novellus.Lib.Backend.Packages.ConditionInterpreter;

public enum TokenType
{
    NotDefined,
    Identifier,
    StringLiteral,
    NumberLiteral,
    FloatLiteral,

    // logical operators
    Or,
    And,
    Not,

    // comparison operators
    Equal,
    NotEqual,
    GreaterThanOrEqual,
    LessThanOrEqual,
    GreaterThan,
    LessThan,

    // punctuation
    LeftParen,
    RightParen,
    Comma,

    // macros
    ModEnabled,
    ModVersion,
    Eof
}

public record Token(TokenType Type, string? Value, int Position)
{
    public TokenType Type { get; set; } = Type;
    public string? Value { get; set; } = Value;
    public int Position { get; set; } = Position;
}