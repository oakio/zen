namespace Zen.AST.Nodes;

public class ReturnVoidNode : IAstNode
{
    public override string ToString() => "return";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}