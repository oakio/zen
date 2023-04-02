namespace Zen.AST.Nodes;

public class ContinueNode : IAstNode
{
    public override string ToString() => "continue";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}