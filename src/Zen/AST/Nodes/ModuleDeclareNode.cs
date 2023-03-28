namespace Zen.AST.Nodes;

public class ModuleDeclareNode : IAstNode
{
    public readonly string Id;
    public readonly IAstNode[] Inner;

    public ModuleDeclareNode(string id, IAstNode[] inner)
    {
        Id = id;
        Inner = inner;
    }

    public override string ToString() => $"{Id}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}