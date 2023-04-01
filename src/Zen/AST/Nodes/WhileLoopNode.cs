namespace Zen.AST.Nodes;

public class WhileLoopNode : IAstNode
{
    public readonly IAstNode Condition;
    public readonly IAstNode Body;

    public WhileLoopNode(IAstNode condition, IAstNode body)
    {
        Condition = condition;
        Body = body;
    }

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}