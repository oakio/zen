using System;
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
}