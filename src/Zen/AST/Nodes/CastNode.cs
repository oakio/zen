namespace Zen.AST.Nodes;

public class CastNode : IAstNode
{
    public readonly string Type;
    public readonly IAstNode Value;

    public CastNode(string type, IAstNode value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString() => $"({Type}){Value}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}