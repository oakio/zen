namespace Zen.AST.Nodes;

public class VarDeclareNode : IAstNode
{
    public readonly ITypeNode Type;
    public readonly string Id;
    public readonly IAstNode Value;

    public VarDeclareNode(ITypeNode type, string id, IAstNode value)
    {
        Type = type;
        Id = id;
        Value = value;
    }

    public override string ToString() => $"{Type} {Id} = {Value}";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}