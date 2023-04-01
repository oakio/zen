namespace Zen.AST.Nodes;

public class ReturnNode : IAstNode
{
    public readonly IAstNode Value;

    public ReturnNode(IAstNode value)
    {
        Value = value;
    }

    public override string ToString() => $"return {Value}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}