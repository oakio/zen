namespace Zen.AST.Nodes;

public class IdNode : IAstNode
{
    public readonly string Id;

    public IdNode(string id)
    {
        Id = id;
    }

    public override string ToString() => Id;

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}