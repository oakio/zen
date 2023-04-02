namespace Zen.AST.Nodes;

public class BreakNode : IAstNode
{
    public override string ToString() => "break";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}