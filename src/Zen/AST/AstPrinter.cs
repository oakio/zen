using System;
using System.Collections.Generic;
using Zen.AST.Nodes;

namespace Zen.AST;

public class AstPrinter : IAstVisitor
{
    public static readonly AstPrinter Instance = new();

    private int _padding;

    public void Visit(ModuleDeclareNode node)
    {
        Print(node);
        Begin();
        AcceptAll(node.Inner);
        End();
    }

    public void Visit(VarDeclareNode node)
    {
        Print(node);
        Begin();
        node.Value.Accept(this);
        End();
    }

    public void Visit(FuncDeclareNode node)
    {
        Print(node);
        Begin();
        AcceptAll(node.Parameters);
        node.Body?.Accept(this);
        End();
    }

    public void Visit(ParamNode node) => Print(node);

    public void Visit(ReturnNode node)
    {
        Print(node);
        Begin();
        node.Value.Accept(this);
        End();
    }

    public void Visit(ReturnVoidNode node) => Print(node);

    public void Visit(IdNode node) => Print(node);

    public void Visit(IntegerLiteralNode node) => Print(node);

    public void Visit(FloatLiteralNode node) => Print(node);

    public void Visit(BooleanLiteralNode node) => Print(node);

    public void Visit(BlockNode node)
    {
        Print(node);
        Begin();
        AcceptAll(node.Nodes);
        End();
    }

    public void Visit(BinaryOpNode node)
    {
        Print(node);
        Begin();
        node.Left.Accept(this);
        node.Right.Accept(this);
        End();
    }

    public void Visit(UnaryOpNode node)
    {
        Print(node);
        Begin();
        node.Value.Accept(this);
        End();
    }

    public void Visit(CallNode node)
    {
        Print(node);
        Begin();
        AcceptAll(node.Args);
        End();
    }

    public void Visit(AssignNode node)
    {
        Print(node);
        Begin();
        node.Value.Accept(this);
        End();
    }

    public void Visit(IfNode node)
    {
        Print(node);
        Begin();
        node.Condition.Accept(this);
        node.ThenBody.Accept(this);
        node.ElseBody?.Accept(this);
        End();
    }

    public void Visit(TernaryOpNode node)
    {
        Print(node);
        Begin();
        node.Condition.Accept(this);
        node.ThenBody.Accept(this);
        node.ElseBody.Accept(this);
        End();
    }

    public void Visit(WhileLoopNode node)
    {
        Print(node);
        Begin();
        node.Condition.Accept(this);
        node.Body.Accept(this);
        End();
    }

    public void Visit(BreakNode node) => Print(node);

    public void Visit(ContinueNode node) => Print(node);

    public void Visit(CastNode node)
    {
        Print(node);
        Begin();
        node.Value.Accept(this);
        End();
    }

    private void Print(IAstNode node)
    {
        string pad = new string(' ', _padding * 4); // TODO: cache
        string nodeType = node.GetType().Name;
        Console.WriteLine($"{pad}[{nodeType}] {node}");
    }

    private void Begin() => _padding++;

    private void End() => _padding--;

    private void AcceptAll(IEnumerable<IAstNode> nodes)
    {
        foreach (IAstNode node in nodes)
        {
            node.Accept(this);
        }
    }
}