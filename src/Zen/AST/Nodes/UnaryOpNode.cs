namespace Zen.AST.Nodes;

public class UnaryOpNode : IAstNode
{
    public readonly UnaryOpType Type;
    public readonly IAstNode Value;

    public UnaryOpNode(UnaryOpType type, IAstNode value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString() => $"{Type} {Value}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}