using System;
using System.Globalization;
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

    public override IAstNode VisitVarDeclare(ZenParser.VarDeclareContext context)
    {
        ITypeNode type = AsType(context.type());
        string id = AsId(context.ID());
        ZenParser.ExpressionContext valueExpression = context.expression();
        IAstNode value = Visit(valueExpression);
        return new VarDeclareNode(type, id, value);
    }

    public override IAstNode VisitFuncDeclare(ZenParser.FuncDeclareContext context)
    {
        string id = AsId(context.ID());
        ITypeNode returnType = AsType(context.type());
        ParamNode[] parameters = context.param().Select(Visit).Cast<ParamNode>().ToArray();
        IAstNode body = context.block() != null
            ? Visit(context.block())
            : null;
        return new FuncDeclareNode(id, returnType, parameters, body);
    }

    public override IAstNode VisitParam(ZenParser.ParamContext context)
    {
        ITypeNode type = AsType(context.type());
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

    public override IAstNode VisitFloatLiteral(ZenParser.FloatLiteralContext context)
    {
        string literal = context.FLOAT().GetText();
        double value = double.Parse(literal, CultureInfo.InvariantCulture);
        return new FloatLiteralNode(value);
    }

    public override IAstNode VisitBoolLiteral(ZenParser.BoolLiteralContext context)
    {
        string literal = context.GetText();
        bool value = bool.Parse(literal);
        return new BooleanLiteralNode(value);
    }

    public override IAstNode VisitBlock(ZenParser.BlockContext context)
    {
        IAstNode[] nodes = context.statement().Select(Visit).ToArray();
        return new BlockNode(nodes);
    }

    public override IAstNode VisitUnary(ZenParser.UnaryContext context)
    {
        UnaryOpType type = ParseUnaryOpType(context.op.Text);
        IAstNode value = Visit(context.expression());
        return new UnaryOpNode(type, value);
    }

    public override IAstNode VisitAndOperator(ZenParser.AndOperatorContext context)
    {
        IAstNode left = Visit(context.left);
        IAstNode right = Visit(context.right);
        return new BinaryOpNode(BinaryOpType.And, left, right);
    }

    public override IAstNode VisitOrOperator(ZenParser.OrOperatorContext context)
    {
        IAstNode left = Visit(context.left);
        IAstNode right = Visit(context.right);
        return new BinaryOpNode(BinaryOpType.Or, left, right);
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

    public override IAstNode VisitRelational(ZenParser.RelationalContext context)
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

    public override IAstNode VisitCall(ZenParser.CallContext context)
    {
        string id = AsId(context.ID());
        IAstNode[] args = context.expression().Select(Visit).ToArray();
        return new CallNode(id, args);
    }

    public override IAstNode VisitAssign(ZenParser.AssignContext context)
    {
        string id = AsId(context.ID());
        IAstNode value = Visit(context.value);
        return new AssignNode(id, value);
    }

    public override IAstNode VisitIfElse(ZenParser.IfElseContext context)
    {
        IAstNode condition = Visit(context.condition);
        IAstNode thenBody = Visit(context.thenBlock);
        IAstNode elseBody = context.elseBlock != null ? Visit(context.elseBlock) : null;
        return new IfNode(condition, thenBody, elseBody);
    }

    public override IAstNode VisitTernary(ZenParser.TernaryContext context)
    {
        IAstNode condition = Visit(context.condition);
        IAstNode thenValue = Visit(context.thenValue);
        IAstNode elseValue = Visit(context.elseValue);
        return new TernaryOpNode(condition, thenValue, elseValue);
    }

    public override IAstNode VisitWhileLoop(ZenParser.WhileLoopContext context)
    {
        IAstNode condition = Visit(context.condition);
        IAstNode body = Visit(context.body);
        return new WhileLoopNode(condition, body);
    }

    public override IAstNode VisitBreak(ZenParser.BreakContext context) => new BreakNode();

    public override IAstNode VisitContinue(ZenParser.ContinueContext context) => new ContinueNode();

    public override IAstNode VisitCasting(ZenParser.CastingContext context)
    {
        ITypeNode type = AsType(context.type());
        IAstNode value = Visit(context.expression());
        return new CastNode(type, value);
    }

    public override IAstNode VisitBuiltinType(ZenParser.BuiltinTypeContext context)
    {
        string type = context.GetText();
        return new BuiltinTypeNode(type);
    }

    private static BinaryOpType ParseBinaryOpType(string type) =>
        type switch
        {
            "+" => BinaryOpType.Add,
            "-" => BinaryOpType.Sub,
            "*" => BinaryOpType.Mul,
            "/" => BinaryOpType.Div,
            "%" => BinaryOpType.Mod,
            "==" => BinaryOpType.Eq,
            "!=" => BinaryOpType.Ne,
            "<=" => BinaryOpType.Lte,
            ">=" => BinaryOpType.Gte,
            "<" => BinaryOpType.Lt,
            ">" => BinaryOpType.Gt,
            "&&" => BinaryOpType.And,
            "||" => BinaryOpType.Or,
            _ => throw new NotSupportedException(type)
        };

    private static UnaryOpType ParseUnaryOpType(string type) =>
        type switch
        {
            "-" => UnaryOpType.Neg,
            "!" => UnaryOpType.Not,
            _ => throw new NotSupportedException(type)
        };

    private static string AsId(ITerminalNode node) => node.GetText();

    private ITypeNode AsType(ZenParser.TypeContext node) => (ITypeNode)Visit(node);
}