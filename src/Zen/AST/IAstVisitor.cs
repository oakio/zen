using Zen.AST.Nodes;

namespace Zen.AST;

public interface IAstVisitor
{
    void Visit(ModuleDeclareNode node);
    void Visit(VarDeclareNode node);
    void Visit(FuncDeclareNode node);
    void Visit(ParamNode node);
    void Visit(ReturnNode node);
    void Visit(ReturnVoidNode node);
    void Visit(IdNode node);
    void Visit(IntegerLiteralNode node);
    void Visit(BooleanLiteralNode node);
    void Visit(BlockNode node);
    void Visit(BinaryOpNode node);
    void Visit(CallNode node);
    void Visit(AssignNode node);
    void Visit(IfNode node);
    void Visit(TernaryOpNode node);
}