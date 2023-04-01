using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using Zen.AST;
using Zen.AST.Nodes;

namespace Zen.Antlr;

public class ZenVisitor : ZenBaseVisitor<IAstNode>
{
    public override IAstNode VisitModule(ZenParser.ModuleContext context)
    {
        string moduleName = context.Start.InputStream.SourceName;
        IAstNode[] inner = context.declaration().Select(Visit).ToArray();
        return new ModuleDeclareNode(moduleName, inner);
    }

    public override IAstNode VisitFuncDeclare(ZenParser.FuncDeclareContext context)
    {
        string id = AsId(context.ID());
        string returnType = AsType(context.TYPE());
        ParamNode[] parameters = context.param().Select(Visit).Cast<ParamNode>().ToArray();
        IAstNode body = context.block() != null
            ? Visit(context.block())
            : null;
        return new FuncDeclareNode(id, returnType, parameters, body);
    }

    public override IAstNode VisitParam(ZenParser.ParamContext context)
    {
        string type = AsType(context.TYPE());
        string id = AsId(context.ID());
        return new ParamNode(type, id);
    }

    public override IAstNode VisitReturn(ZenParser.ReturnContext context)
    {
        ZenParser.ExpressionContext expr = context.expression();
        if (expr == null)
        {
            return new ReturnVoidNode();
        }

        IAstNode value = Visit(expr);
        return new ReturnNode(value);
    }

    public override IAstNode VisitId(ZenParser.IdContext context)
    {
        string id = AsId(context.ID());
        return new IdNode(id);
    }

    public override IAstNode VisitIntegerLiteral(ZenParser.IntegerLiteralContext context)
    {
        string literal = context.INTEGER().GetText();
        ulong value = ulong.Parse(literal);
        return new IntegerLiteralNode(value);
    }

    public override IAstNode VisitBlock(ZenParser.BlockContext context)
    {
        IAstNode[] nodes = context.statement().Select(Visit).ToArray();
        return new BlockNode(nodes);
    }

    public override IAstNode VisitAddition(ZenParser.AdditionContext context)
    {
        BinaryOpType type = ParseBinaryOpType(context.op.Text);
        IAstNode left = Visit(context.left);
        IAstNode right = Visit(context.right);
        return new BinaryOpNode(type, left, right);
    }

    public override IAstNode VisitMultiplication(ZenParser.MultiplicationContext context)
    {
        BinaryOpType type = ParseBinaryOpType(context.op.Text);
        IAstNode left = Visit(context.left);
        IAstNode right = Visit(context.right);
        return new BinaryOpNode(type, left, right);
    }

    public override IAstNode VisitParentheses(ZenParser.ParenthesesContext context)
    {
        ZenParser.ExpressionContext inner = context.expression();
        return Visit(inner);
    }

    private static BinaryOpType ParseBinaryOpType(string type) =>
        type switch
        {
            "+" => BinaryOpType.Add,
            "-" => BinaryOpType.Sub,
            "*" => BinaryOpType.Mul,
            "/" => BinaryOpType.Div,
            "%" => BinaryOpType.Mod,
            _ => throw new NotSupportedException(type)
        };

    private static string AsId(ITerminalNode node) => node.GetText();

    private static string AsType(ITerminalNode node) => node.GetText();
}