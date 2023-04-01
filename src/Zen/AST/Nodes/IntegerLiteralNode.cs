namespace Zen.AST.Nodes;

public class IntegerLiteralNode : IAstNode
{
    public readonly ulong Value;

    public IntegerLiteralNode(ulong value)
    {
        Value = value;
    }

    public override string ToString() => Value.ToString();

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}