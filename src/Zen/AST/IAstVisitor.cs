using Zen.AST.Nodes;

namespace Zen.AST;

public interface IAstVisitor
{
    void Visit(ModuleDeclareNode node);
    void Visit(FuncDeclareNode node);
    void Visit(ParamNode node);
    void Visit(ReturnNode node);
    void Visit(ReturnVoidNode node);
    void Visit(IdNode node);
    void Visit(IntegerLiteralNode node);
    void Visit(BlockNode node);
}