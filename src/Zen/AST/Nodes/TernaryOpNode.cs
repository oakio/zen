namespace Zen.AST.Nodes;

public class TernaryOpNode : IAstNode
{
    public readonly IAstNode Condition;
    public readonly IAstNode ThenBody;
    public readonly IAstNode ElseBody;

    public TernaryOpNode(IAstNode condition, IAstNode thenBody, IAstNode elseBody)
    {
        Condition = condition;
        ThenBody = thenBody;
        ElseBody = elseBody;
    }

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}