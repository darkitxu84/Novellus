namespace Novellus.Lib.Backend.Packages.PackageConfig.ConditionInterpreter;

public abstract class AstNode { };
public sealed class LogicalOpNode(LogicalOperator op, AstNode left, AstNode right) : AstNode
{
    public LogicalOperator Operator { get; init; } = op;
    public AstNode Left { get; init; } = left;
    public AstNode Right { get; init; } = right;
}

public sealed class ComparisonNode(ComparisonOperator op, AstNode left, AstNode right) : AstNode
{
    public ComparisonOperator Operator { get; init; } = op;
    public AstNode Left { get; init; } = left;
    public AstNode Right { get; init; } = right;
}

public sealed class NotNode(AstNode operand) : AstNode
{
    public AstNode Operand { get; init; } = operand;
}

public sealed class MacroCallNode(string macroName, List<string> args) : AstNode
{
    public string MacroName { get; init; } =  macroName;
    public List<string> Args { get; init; } = args;
}

public sealed class StringValueNode(string value) : AstNode
{
    public string Value { get; init; } = value;
}

public sealed class NumberValueNode(int value) : AstNode
{
    public int Value { get; init; } = value;
}

public sealed class FloatValueNode(float value) : AstNode
{
    public float Value { get; init; } = value;
}
public sealed class OptionValueNode(string value) : AstNode
{
    public string Value { get; init; } = value;
}
