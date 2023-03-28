namespace Zen.AST;

public interface IAstNode
{
    void Accept(IAstVisitor visitor);
}