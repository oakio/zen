namespace Zen.AST.Nodes;

public class BinaryOpNode : IAstNode
{
    public readonly BinaryOpType Type;
    public readonly IAstNode Left;
    public readonly IAstNode Right;

    public BinaryOpNode(BinaryOpType type, IAstNode left, IAstNode right)
    {
        Type = type;
        Left = left;
        Right = right;
    }

    public override string ToString() => Type.ToString();

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}