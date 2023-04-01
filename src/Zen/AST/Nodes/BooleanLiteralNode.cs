namespace Zen.AST.Nodes;

public class BooleanLiteralNode : IAstNode
{
    public readonly bool Value;

    public BooleanLiteralNode(bool value)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString();

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}