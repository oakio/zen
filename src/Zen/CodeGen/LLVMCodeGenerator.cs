using System;
using System.Collections.Generic;
using System.Linq;
using LLVMSharp.Interop;
using Zen.AST;
using Zen.AST.Nodes;

namespace Zen.CodeGen;

public class LLVMCodeGenerator : IAstVisitor
{
    private static readonly LLVMValueRef TrueValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 1);
    private static readonly LLVMValueRef FalseValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 0);

    private LLVMContextRef _context;
    private LLVMBuilderRef _builder;
    private readonly Stack<LLVMValueRef> _stack;
    private readonly Stack<LLVMTypeRef> _typeEval;

    private readonly ScopeManager _scope;
    private readonly Dictionary<string, Entity> _functions;

    private LLVMBasicBlockRef _currentBlock;

    public LLVMModuleRef Module;

    public LLVMCodeGenerator()
    {
        _context = LLVMContextRef.Global;
        _builder = LLVMBuilderRef.Create(_context);
        _stack = new Stack<LLVMValueRef>();
        _typeEval = new Stack<LLVMTypeRef>();
        _scope = new ScopeManager();
        _functions = new Dictionary<string, Entity>();
    }

    public void Visit(ModuleDeclareNode node)
    {
        _scope.Begin();
        Module = _context.CreateModuleWithName(node.Id);
        foreach (FuncDeclareNode funcDeclareNode in node.Inner.Cast<FuncDeclareNode>())
        {
            DeclareFunction(funcDeclareNode);
        }
        AcceptAll(node.Inner);
        _scope.End();
    }

    public void Visit(VarDeclareNode node)
    {
        string id = node.Id;
        LLVMTypeRef type = EvalType(node.Type);
        LLVMValueRef variable = _builder.BuildAlloca(type, id);
        LLVMValueRef value = Eval(node.Value);
        _builder.BuildStore(value, variable);
        _scope.Add(id, variable, type);
    }

    public void Visit(FuncDeclareNode node)
    {
        Entity func = _functions[node.Id];

        LLVMValueRef funcValue = func.Value;
        LLVMTypeRef returnType = func.Type.ReturnType;

        _scope.Begin();

        LLVMBasicBlockRef entryBlock = funcValue.AppendBasicBlock("entry");
        SetCurrentBlock(entryBlock);

        foreach (LLVMValueRef funcParam in funcValue.Params)
        {
            LLVMTypeRef type = funcParam.TypeOf;
            LLVMValueRef valuePtr = _builder.BuildAlloca(type);
            _builder.BuildStore(funcParam, valuePtr);
            _scope.Add(funcParam.Name, valuePtr, type);
        }

        bool returnVoid = returnType == LLVMTypeRef.Void;

        LLVMValueRef returnValuePtr = returnVoid
            ? default
            : _builder.BuildAlloca(returnType);

        LLVMBasicBlockRef bodyBlock = AppendBasicBlock(entryBlock, "body");
        _builder.BuildBr(bodyBlock);

        LLVMBasicBlockRef returnBlock = AppendBasicBlock(bodyBlock, "return");
        SetCurrentBlock(returnBlock);

        if (returnVoid)
        {
            _builder.BuildRetVoid();
        }
        else
        {
            LLVMValueRef returnValue = _builder.BuildLoad2(returnType, returnValuePtr);
            _builder.BuildRet(returnValue);
        }

        _scope.ReturnValuePtr = returnValuePtr;
        _scope.ReturnBlock = returnBlock;

        SetCurrentBlock(bodyBlock);
        node.Body?.Accept(this);
        if (!HasTerminator(_currentBlock))
        {
            _builder.BuildBr(returnBlock);
        }

        _scope.End();
    }

    public void Visit(ParamNode node)
    {
    }

    public void Visit(ReturnNode node)
    {
        LLVMValueRef value = Eval(node.Value);
        _builder.BuildStore(value, _scope.ReturnValuePtr);
        _builder.BuildBr(_scope.ReturnBlock);

        EmitUnreachableBasicBlock();
    }

    public void Visit(ReturnVoidNode node)
    {
        _builder.BuildBr(_scope.ReturnBlock);

        EmitUnreachableBasicBlock();
    }

    public void Visit(IdNode node)
    {
        Entity entity = _scope[node.Id];
        LLVMValueRef value = _builder.BuildLoad2(entity.Type, entity.Value);
        _stack.Push(value);
    }

    public void Visit(IntegerLiteralNode node)
    {
        LLVMValueRef value = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, node.Value);
        _stack.Push(value);
    }

    public void Visit(FloatLiteralNode node)
    {
        LLVMValueRef value = LLVMValueRef.CreateConstReal(LLVMTypeRef.Double, node.Value);
        _stack.Push(value);
    }

    public void Visit(BooleanLiteralNode node)
    {
        LLVMValueRef value = node.Value ? TrueValue : FalseValue;
        _stack.Push(value);
    }

    public void Visit(BlockNode node)
    {
        _scope.Begin();
        AcceptAll(node.Nodes);
        _scope.End();
    }

    public void Visit(BinaryOpNode node)
    {
        BinaryOpType type = node.Type;
        if (type == BinaryOpType.And)
        {
            EmitAndOp(node.Left, node.Right);
            return;
        }

        if (type == BinaryOpType.Or)
        {
            EmitOrOp(node.Left, node.Right);
            return;
        }

        LLVMValueRef left = Eval(node.Left);
        LLVMValueRef right = Eval(node.Right);
        LLVMValueRef result = GetLLVMBinOp(type, left, right);
        _stack.Push(result);
    }

    public void Visit(UnaryOpNode node)
    {
        LLVMValueRef value = Eval(node.Value);

        switch (node.Type)
        {
            case UnaryOpType.Neg:
            {
                LLVMValueRef negValue = IsFloat(value)
                    ? _builder.BuildFNeg(value)
                    : _builder.BuildNSWNeg(value);

                _stack.Push(negValue);
                return;
            }
            case UnaryOpType.Not:
            {
                LLVMValueRef notValue = LLVMValueRef.CreateConstNot(value);
                _stack.Push(notValue);
                return;
            }
            default:
                throw new NotSupportedException(node.Type.ToString());
        }
    }

    public void Visit(CallNode node)
    {
        Entity callee = _functions[node.Id];

        int argsCount = node.Args.Length;
        var args = new LLVMValueRef[argsCount];
        for (int i = 0; i < argsCount; i++)
        {
            args[i] = Eval(node.Args[i]);
        }

        LLVMValueRef result = _builder.BuildCall2(callee.Type, callee.Value, args);
        _stack.Push(result);
    }

    public void Visit(AssignNode node)
    {
        Entity variable = _scope[node.Id];
        LLVMValueRef value = Eval(node.Value);
        _builder.BuildStore(value, variable.Value);
    }

    public void Visit(IfNode node)
    {
        bool hasElseBlock = node.ElseBody != null;
        LLVMBasicBlockRef conditionBlock = AppendBasicBlock(_currentBlock, "if");
        LLVMBasicBlockRef thenBlock = AppendBasicBlock(conditionBlock, "if.then");
        LLVMBasicBlockRef elseBlock = AppendBasicBlock(thenBlock, "if.else");
        LLVMBasicBlockRef mergeBlock = hasElseBlock ? AppendBasicBlock(elseBlock, "if.merge") : elseBlock;

        _builder.BuildBr(conditionBlock);

        // emit Condition
        SetCurrentBlock(conditionBlock);
        LLVMValueRef condition = Eval(node.Condition);
        _builder.BuildCondBr(condition, thenBlock, elseBlock);

        // emit Then
        SetCurrentBlock(thenBlock);
        _scope.Begin();
        Accept(node.ThenBody);
        _scope.End();
        if (!HasTerminator(thenBlock))
        {
            _builder.BuildBr(mergeBlock);
        }

        // emit Else
        if (hasElseBlock)
        {
            SetCurrentBlock(elseBlock);
            _scope.Begin();
            Accept(node.ElseBody);
            _scope.End();
            if (!HasTerminator(elseBlock))
            {
                _builder.BuildBr(mergeBlock);
            }
        }

        SetCurrentBlock(mergeBlock);
    }

    public void Visit(TernaryOpNode node)
    {
        LLVMBasicBlockRef conditionBlock = AppendBasicBlock(_currentBlock, "if");
        LLVMBasicBlockRef thenBlock = AppendBasicBlock(conditionBlock, "if.then");
        LLVMBasicBlockRef elseBlock = AppendBasicBlock(thenBlock, "if.else");
        LLVMBasicBlockRef mergeBlock = AppendBasicBlock(elseBlock, "if.merge");

        _builder.BuildBr(conditionBlock);

        // emit Condition
        SetCurrentBlock(conditionBlock);
        LLVMValueRef condition = Eval(node.Condition);
        _builder.BuildCondBr(condition, thenBlock, elseBlock);

        // emit Then
        SetCurrentBlock(thenBlock);
        Accept(node.ThenBody);
        LLVMValueRef thenValue = _stack.Pop();
        _builder.BuildBr(mergeBlock);

        // emit Else
        SetCurrentBlock(elseBlock);
        Accept(node.ElseBody);
        LLVMValueRef elseValue = _stack.Pop();
        _builder.BuildBr(mergeBlock);

        SetCurrentBlock(mergeBlock);
        LLVMValueRef phiValue = _builder.BuildPhi(thenValue.TypeOf);
        phiValue.AddIncoming(new[] { thenValue }, new[] { thenBlock }, 1);
        phiValue.AddIncoming(new[] { elseValue }, new[] { elseBlock }, 1);
        _stack.Push(phiValue);
    }

    public void Visit(WhileLoopNode node)
    {
        LLVMBasicBlockRef conditionBlock = AppendBasicBlock(_currentBlock, "loop");
        LLVMBasicBlockRef bodyBlock = AppendBasicBlock(conditionBlock, "loop.body");
        LLVMBasicBlockRef endBlock = AppendBasicBlock(bodyBlock, "loop.end");

        _builder.BuildBr(conditionBlock);

        // emit Condition
        SetCurrentBlock(conditionBlock);
        LLVMValueRef condition = Eval(node.Condition);
        _builder.BuildCondBr(condition, bodyBlock, endBlock);

        // emit Body
        SetCurrentBlock(bodyBlock);
        _scope.Begin();
        _scope.BreakBlock = endBlock;
        _scope.ContinueBlock = conditionBlock;

        Accept(node.Body);
        _scope.End();
        _builder.BuildBr(conditionBlock);

        SetCurrentBlock(endBlock);
    }

    public void Visit(BreakNode node)
    {
        _builder.BuildBr(_scope.BreakBlock);
        EmitUnreachableBasicBlock();
    }

    public void Visit(ContinueNode node)
    {
        _builder.BuildBr(_scope.ContinueBlock);
        EmitUnreachableBasicBlock();
    }

    public void Visit(CastNode node)
    {
        LLVMTypeRef targetType = EvalType(node.Type);
        LLVMValueRef value = Eval(node.Value);

        if (targetType == value.TypeOf)
        {
            _stack.Push(value);
            return;
        }

        LLVMValueRef casted = GetLLVMCast(value, targetType);
        _stack.Push(casted);
    }

    public void Visit(BuiltinTypeNode node)
    {
        LLVMTypeRef type = GetLLVMType(node.Type);
        _typeEval.Push(type);
    }

    private void DeclareFunction(FuncDeclareNode node)
    {
        LLVMTypeRef returnType = EvalType(node.ReturnType);
        ParamNode[] parameters = node.Parameters;
        LLVMTypeRef[] paramTypes = new LLVMTypeRef[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            paramTypes[i] = EvalType(parameters[i].Type);
        }

        LLVMTypeRef funcType = LLVMTypeRef.CreateFunction(returnType, paramTypes);
        string id = node.Id;
        LLVMValueRef func = Module.AddFunction(id, funcType);

        for (int i = 0; i < parameters.Length; i++)
        {
            func.Params[i].Name = parameters[i].Id;
        }

        _functions.Add(id, new Entity(func, funcType));
    }

    private void EmitAndOp(IAstNode leftNode, IAstNode rightNode)
    {
        LLVMBasicBlockRef leftBlock = AppendBasicBlock(_currentBlock, "and.left");
        LLVMBasicBlockRef rightBlock = AppendBasicBlock(leftBlock, "and.right");
        LLVMBasicBlockRef mergeBlock = AppendBasicBlock(rightBlock, "and.merge");

        _builder.BuildBr(leftBlock);

        SetCurrentBlock(leftBlock);
        LLVMValueRef left = Eval(leftNode);
        _builder.BuildCondBr(left, rightBlock, mergeBlock);

        SetCurrentBlock(rightBlock);
        LLVMValueRef right = Eval(rightNode);
        _builder.BuildBr(mergeBlock);

        SetCurrentBlock(mergeBlock);
        LLVMValueRef phiValue = _builder.BuildPhi(LLVMTypeRef.Int1);
        phiValue.AddIncoming(new[] { FalseValue }, new[] { leftBlock }, 1);
        phiValue.AddIncoming(new[] { right }, new[] { rightBlock }, 1);
        _stack.Push(phiValue);
    }

    private void EmitOrOp(IAstNode leftNode, IAstNode rightNode)
    {
        LLVMBasicBlockRef leftBlock = AppendBasicBlock(_currentBlock, "or.left");
        LLVMBasicBlockRef rightBlock = AppendBasicBlock(leftBlock, "or.right");
        LLVMBasicBlockRef mergeBlock = AppendBasicBlock(rightBlock, "or.merge");

        _builder.BuildBr(leftBlock);

        SetCurrentBlock(leftBlock);
        LLVMValueRef left = Eval(leftNode);
        _builder.BuildCondBr(left, mergeBlock, rightBlock);

        SetCurrentBlock(rightBlock);
        LLVMValueRef right = Eval(rightNode);
        _builder.BuildBr(mergeBlock);

        SetCurrentBlock(mergeBlock);
        LLVMValueRef phiValue = _builder.BuildPhi(LLVMTypeRef.Int1);
        phiValue.AddIncoming(new[] { TrueValue }, new[] { leftBlock }, 1);
        phiValue.AddIncoming(new[] { right }, new[] { rightBlock }, 1);
        _stack.Push(phiValue);
    }

    private void EmitUnreachableBasicBlock()
    {
        LLVMBasicBlockRef unreachable = AppendBasicBlock(_currentBlock);
        SetCurrentBlock(unreachable);
    }

    private void SetCurrentBlock(LLVMBasicBlockRef block)
    {
        _currentBlock = block;
        _builder.PositionAtEnd(block);
    }

    private static LLVMBasicBlockRef AppendBasicBlock(LLVMBasicBlockRef parent, string name = null)
    {
        LLVMBasicBlockRef next = parent.InsertBasicBlock(name ?? string.Empty);
        next.MoveAfter(parent);
        return next;
    }

    private LLVMTypeRef GetLLVMType(string type) => type switch
    {
        "void" => _context.VoidType,
        "i8" => _context.Int8Type,
        "u8" => _context.Int8Type,
        "i16" => _context.Int16Type,
        "u16" => _context.Int16Type,
        "i32" => _context.Int32Type,
        "u32" => _context.Int32Type,
        "i64" => _context.Int64Type,
        "u64" => _context.Int64Type,
        "f32" => _context.FloatType,
        "f64" => _context.DoubleType,
        "bool" => _context.Int1Type,
        _ => throw new NotSupportedException(type)
    };

    private LLVMValueRef GetLLVMBinOp(BinaryOpType op, LLVMValueRef left, LLVMValueRef right) =>
        IsFloat(left)
            ? GetLLVMFloatBinOp(op, left, right)
            : GetLLVMIntBinOp(op, left, right);

    private LLVMValueRef GetLLVMIntBinOp(BinaryOpType op, LLVMValueRef left, LLVMValueRef right) =>
        op switch
        {
            BinaryOpType.Add => _builder.BuildAdd(left, right),
            BinaryOpType.Sub => _builder.BuildSub(left, right),
            BinaryOpType.Mul => _builder.BuildMul(left, right),
            BinaryOpType.Div => _builder.BuildSDiv(left, right),
            BinaryOpType.Mod => _builder.BuildSRem(left, right),
            BinaryOpType.Eq => _builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, left, right),
            BinaryOpType.Ne => _builder.BuildICmp(LLVMIntPredicate.LLVMIntNE, left, right),
            BinaryOpType.Lt => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT, left, right),
            BinaryOpType.Gt => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSGT, left, right),
            BinaryOpType.Lte => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSLE, left, right),
            BinaryOpType.Gte => _builder.BuildICmp(LLVMIntPredicate.LLVMIntSGE, left, right),
            _ => throw new NotSupportedException(op.ToString())
        };

    private LLVMValueRef GetLLVMFloatBinOp(BinaryOpType op, LLVMValueRef left, LLVMValueRef right) =>
        op switch
        {
            BinaryOpType.Add => _builder.BuildFAdd(left, right),
            BinaryOpType.Sub => _builder.BuildFSub(left, right),
            BinaryOpType.Mul => _builder.BuildFMul(left, right),
            BinaryOpType.Div => _builder.BuildFDiv(left, right),
            BinaryOpType.Mod => _builder.BuildFRem(left, right),
            BinaryOpType.Eq => _builder.BuildFCmp(LLVMRealPredicate.LLVMRealUEQ, left, right),
            BinaryOpType.Ne => _builder.BuildFCmp(LLVMRealPredicate.LLVMRealUNE, left, right),
            BinaryOpType.Lt => _builder.BuildFCmp(LLVMRealPredicate.LLVMRealULT, left, right),
            BinaryOpType.Gt => _builder.BuildFCmp(LLVMRealPredicate.LLVMRealUGT, left, right),
            BinaryOpType.Lte => _builder.BuildFCmp(LLVMRealPredicate.LLVMRealULE, left, right),
            BinaryOpType.Gte => _builder.BuildFCmp(LLVMRealPredicate.LLVMRealUGE, left, right),
            _ => throw new NotSupportedException(op.ToString())
        };

    private LLVMValueRef GetLLVMCast(LLVMValueRef value, LLVMTypeRef targetType)
    {
        if (IsFloat(value))
        {
            return _builder.BuildFPToSI(value, targetType);
        }

        if (targetType == LLVMTypeRef.Double)
        {
            return _builder.BuildSIToFP(value, targetType);
        }

        return targetType.IntWidth < value.TypeOf.IntWidth
            ? _builder.BuildTrunc(value, targetType)
            : _builder.BuildSExt(value, targetType);
    }

    private LLVMValueRef Eval(IAstNode node)
    {
        Accept(node);
        LLVMValueRef value = _stack.Pop();
        return value;
    }

    private LLVMTypeRef EvalType(ITypeNode node)
    {
        Accept(node);
        LLVMTypeRef type = _typeEval.Pop();
        return type;
    }

    private void AcceptAll(IEnumerable<IAstNode> nodes)
    {
        foreach (IAstNode node in nodes)
        {
            Accept(node);
        }
    }

    private void Accept(IAstNode node) => node.Accept(this);

    private static bool HasTerminator(LLVMBasicBlockRef self) => self.Terminator.Handle != IntPtr.Zero;

    private static bool IsFloat(LLVMValueRef value) => value.TypeOf == LLVMTypeRef.Double;
}
