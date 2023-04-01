namespace Zen.AST.Nodes;

public class ParamNode : IAstNode
{
    public readonly string Type;
    public readonly string Id;

    public ParamNode(string type, string id)
    {
        Type = type;
        Id = id;
    }

    public override string ToString() => $"{Type} {Id}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}