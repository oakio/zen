using Zen.AST.Nodes;

namespace Zen.AST;

public interface IAstVisitor
{
    void Visit(ModuleDeclareNode node);
}