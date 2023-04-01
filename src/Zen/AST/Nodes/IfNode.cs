namespace Zen.AST.Nodes;

public class IfNode : IAstNode
{
    public readonly IAstNode Condition;
    public readonly IAstNode ThenBody;
    public readonly IAstNode ElseBody;

    public IfNode(IAstNode condition, IAstNode thenBody, IAstNode elseBody)
    {
        Condition = condition;
        ThenBody = thenBody;
        ElseBody = elseBody;
    }

    public override string ToString() => $"if ({Condition})";

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}