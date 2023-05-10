namespace Zen.AST.Nodes;

public class CastNode : IAstNode
{
    public readonly ITypeNode Type;
    public readonly IAstNode Value;

    public CastNode(ITypeNode type, IAstNode value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString() => $"({Type}){Value}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}