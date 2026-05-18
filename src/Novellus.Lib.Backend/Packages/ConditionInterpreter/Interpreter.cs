namespace Novellus.Lib.Backend.Packages.ConditionInterpreter;

public class InterpreterException : Exception
{
    public InterpreterException(string message) : base(message) { }
}

public static class Interpreter
{
    private static Dictionary<string, Package>? BuildCtx { get; set; }
    private static Dictionary<string, object>? PackageCtx { get; set; }
    private static string? MacroModVersion(string id) => BuildCtx!.GetValueOrDefault(id)?.Metadata.Version;
    private static bool MacroModEnabled(string id) => BuildCtx!.ContainsKey(id);

    public static void SetContext(Dictionary<string, Package> ctx)
    {
        BuildCtx = ctx;
    }

    public static bool ShouldBeAdded(AstNode initNode, Dictionary<string, object> pkgCtx)
    {
        PackageCtx = pkgCtx;
        return Evaluate(initNode);
    }

    private static object? ResolveValue(AstNode node) =>
        node switch
        {
            MacroCallNode mac => node switch
            {
                MacroCallNode m when m.Macro == Macro.ModVersion => MacroModVersion(m.Args[0]),

                MacroCallNode m when m.Macro == Macro.ModEnabled =>
                    throw new InterpreterException("MOD_ENABLED macro cannot be used as a value"),
                _ => throw new InterpreterException($"Unknown macro: {mac.Macro}")
            },
            StringValueNode s => s.Value,
            NumberValueNode n => n.Value,
            OptionValueNode opt => PackageCtx!.GetValueOrDefault(opt.Value),

            LogicalOpNode or NotNode => throw new InterpreterException("A boolean expression cannot be used as a value in a comparison"),
            _ => throw new InterpreterException($"Unknown node: {node.GetType().Name}")
        };

    private static bool Evaluate(AstNode node)
    {
        return node switch
        {

            LogicalOpNode log => EvaluateLogicalOp(log),
            NotNode not => !Evaluate(not.Operand),
            ComparisonNode cmp => EvaluateComparison(cmp),
            MacroCallNode mac => EvaluateMacroAsBool(mac),
            OptionValueNode opt => EvaluateOptionAsBool(opt),

            StringValueNode str => throw new InterpreterException($"String {str.Value} cannot be used as a condition!"),
            NumberValueNode num => throw new InterpreterException($"Number {num.Value} cannot be used as a condition!"),
            FloatValueNode flt => throw new InterpreterException($"Float {flt.Value} cannot be used as a condition!"),


            _ => throw new InterpreterException($"Unknown AST node type {node.GetType().Name}")
        };
    }

    public static bool EvaluateLogicalOp(LogicalOpNode node)
    {
        return node.Operator switch
        {
            LogicalOperator.And => Evaluate(node.Left) && Evaluate(node.Right),
            LogicalOperator.Or => Evaluate(node.Left) || Evaluate(node.Right),

            _ => throw new InterpreterException($"Unknown logical operator {node.Operator}")
        };
    }

    private static bool EvaluateComparison(ComparisonNode node)
    {
        var left = ResolveValue(node.Left);
        var right = ResolveValue(node.Right);

        if (left == null || right == null)
            return node.Operator switch
            {
                ComparisonOperator.Equal => left == right,
                ComparisonOperator.NotEqual => left != right,
                _ => false
            };

        // if we have two numbers
        if (TryToDouble(left, out double l) && TryToDouble(right, out double r))
            return ApplyOperator(node.Operator, l.CompareTo(r));

        // if we have two strings
        string ls = left.ToString()!, rs = right.ToString()!;

        /*
        if (IsSemanticVersion)
        */ // TODO: semantic version comparison

        return ApplyOperator(node.Operator, string.Compare(ls, rs, StringComparison.OrdinalIgnoreCase));
    }

    private static bool EvaluateMacroAsBool(MacroCallNode node) =>
        node.Macro switch
        {
            Macro.ModEnabled => MacroModEnabled(node.Args[0]),

            Macro.ModVersion => throw new InterpreterException("Macro MOD_VERSION cannot be evaluated as a boolean!"),
            _ => throw new InterpreterException($"Unknown macro: {node.Macro}")
        };

    private static bool EvaluateOptionAsBool(OptionValueNode node)
    {
        var val = PackageCtx!.GetValueOrDefault(node.Value)
            ?? throw new InterpreterException($"Setting '{node.Value}' not found in package context!");
        if (val is bool b) return b;

        throw new InterpreterException($"Option '{node.Value}' has value of type {val.GetType().Name} which cannot be evaluated as a boolean");
    }

    // used for numbers and strings, returns true if the comparison holds
    private static bool ApplyOperator(ComparisonOperator op, int cmp) =>
        op switch
        {
            ComparisonOperator.Equal => cmp == 0,
            ComparisonOperator.NotEqual => cmp != 0,
            ComparisonOperator.GreaterThan => cmp > 0,
            ComparisonOperator.GreaterThanOrEqual => cmp >= 0,
            ComparisonOperator.LessThan => cmp < 0,
            ComparisonOperator.LessThanOrEqual => cmp <= 0,

            _ => throw new InterpreterException($"Unknown op: {op}")
        };

    // compare numbers of different types by converting them to double, if possible
    private static bool TryToDouble(object value, out double result)
    {
        if (value is double d) { result = d; return true; }
        if (value is int i) { result = i; return true; }
        if (value is float f) { result = f; return true; }

        result = 0;
        if (value is string s)
            return double.TryParse(s,
                System.Globalization.NumberStyles.Float,
                System.Globalization.CultureInfo.InvariantCulture,
                out result);

        return false;
    }
}