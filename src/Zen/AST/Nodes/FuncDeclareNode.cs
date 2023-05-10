namespace Zen.AST.Nodes;

public class FuncDeclareNode : IAstNode
{
    public readonly string Id;
    public readonly ITypeNode ReturnType;
    public readonly ParamNode[] Parameters;
    public readonly IAstNode Body;

    public FuncDeclareNode(string id, ITypeNode returnType, ParamNode[] parameters, IAstNode body)
    {
        Id = id;
        ReturnType = returnType;
        Parameters = parameters;
        Body = body;
    }

    public override string ToString() => $"{ReturnType} {Id}(...)";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}