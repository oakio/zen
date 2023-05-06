using System.Globalization;

namespace Zen.AST.Nodes;

public class FloatLiteralNode : IAstNode
{
    public readonly double Value;

    public FloatLiteralNode(double value)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString(CultureInfo.InvariantCulture);

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}