namespace Zen.AST.Nodes;

public class AssignNode : IAstNode
{
    public readonly string Id;
    public readonly IAstNode Value;

    public AssignNode(string id, IAstNode value)
    {
        Id = id;
        Value = value;
    }

    public override string ToString() => $"{Id}={Value}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}