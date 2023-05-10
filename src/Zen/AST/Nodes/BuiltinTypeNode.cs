namespace Zen.AST.Nodes;

public class BuiltinTypeNode : ITypeNode
{
    public readonly string Type;

    public BuiltinTypeNode(string type)
    {
        Type = type;
    }

    public override string ToString() => Type;

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}