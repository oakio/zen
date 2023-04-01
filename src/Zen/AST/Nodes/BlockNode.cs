namespace Zen.AST.Nodes;

public class BlockNode : IAstNode
{
    public readonly IAstNode[] Nodes;

    public BlockNode(IAstNode[] nodes)
    {
        Nodes = nodes;
    }

    public void Accept(IAstVisitor visitor) => visitor.Visit(this);
}