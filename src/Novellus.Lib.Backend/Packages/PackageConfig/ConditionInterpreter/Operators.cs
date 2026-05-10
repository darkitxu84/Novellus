namespace Novellus.Lib.Backend.Packages.PackageConfig.ConditionInterpreter;

public enum LogicalOperator
{
    And,
    Or
}

public enum ComparisonOperator
{
    NotDefined,
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
}

// used to map tokens type to comparison operators
public static class TokenTypeExt
{
    public static ComparisonOperator GetComparisonOperator(this TokenType type)
    {
        return type switch
        {
            TokenType.Equal => ComparisonOperator.Equal,
            TokenType.NotEqual => ComparisonOperator.NotEqual,
            TokenType.GreaterThan => ComparisonOperator.GreaterThan,
            TokenType.GreaterThanOrEqual => ComparisonOperator.GreaterThanOrEqual,
            TokenType.LessThan => ComparisonOperator.LessThan,
            TokenType.LessThanOrEqual => ComparisonOperator.LessThanOrEqual,
            _ => ComparisonOperator.NotDefined
        };
    }

    public static bool IsComparisonOperator(this TokenType type)
    {
        return type.GetComparisonOperator() != ComparisonOperator.NotDefined;
    }
}