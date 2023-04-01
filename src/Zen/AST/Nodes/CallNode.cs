namespace Zen.AST.Nodes;

public class CallNode : IAstNode
{
    public readonly string Id;
    public readonly IAstNode[] Args;

    public CallNode(string id, IAstNode[] args)
    {
        Id = id;
        Args = args;
    }

    public override string ToString() => $"{Id}(...)";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}