using System;
using System.Collections.Generic;
using System.Linq;
using LLVMSharp.Interop;
using Zen.AST;
using Zen.AST.Nodes;

namespace Zen.CodeGen;

public class LLVMCodeGenerator : IAstVisitor
{
    private LLVMContextRef _context;
    private LLVMBuilderRef _builder;
    private readonly Stack<LLVMValueRef> _stack;
    private readonly ScopeManager _scope;
    private readonly Dictionary<string, Entity> _functions;

    private LLVMBasicBlockRef _currentBlock;

    public LLVMModuleRef Module;

    public LLVMCodeGenerator()
    {
        _context = LLVMContextRef.Global;
        _builder = LLVMBuilderRef.Create(_context);
        _stack = new Stack<LLVMValueRef>();
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
        LLVMTypeRef type = GetLLVMType(node.Type);
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

        LLVMValueRef returnValuePtr = node.ReturnType != "void"
            ? _builder.BuildAlloca(returnType)
            : default;

        LLVMBasicBlockRef bodyBlock = AppendBasicBlock(entryBlock, "body");
        _builder.BuildBr(bodyBlock);

        LLVMBasicBlockRef returnBlock = AppendBasicBlock(bodyBlock, "return");
        SetCurrentBlock(returnBlock);

        if (node.ReturnType == "void")
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
    }

    public void Visit(ReturnVoidNode node) => _builder.BuildBr(_scope.ReturnBlock);

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

    public void Visit(BlockNode node)
    {
        _scope.Begin();
        AcceptAll(node.Nodes);
        _scope.End();
    }

    public void Visit(BinaryOpNode node)
    {
        BinaryOpType type = node.Type;
        LLVMValueRef left = Eval(node.Left);
        LLVMValueRef right = Eval(node.Right);
        LLVMValueRef result = GetLLVMBinOp(type, left, right);
        _stack.Push(result);
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

    private void DeclareFunction(FuncDeclareNode node)
    {
        LLVMTypeRef returnType = GetLLVMType(node.ReturnType);
        ParamNode[] parameters = node.Parameters;
        LLVMTypeRef[] paramTypes = new LLVMTypeRef[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            paramTypes[i] = GetLLVMType(parameters[i].Type);
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

    private LLVMTypeRef GetLLVMType(string type) =>
        type switch
        {
            "void" => _context.VoidType,
            "i32" => _context.Int32Type,
            _ => throw new NotSupportedException(type)
        };

    private LLVMValueRef GetLLVMBinOp(BinaryOpType op, LLVMValueRef left, LLVMValueRef right) =>
        op switch
        {
            BinaryOpType.Add => _builder.BuildAdd(left, right),
            BinaryOpType.Sub => _builder.BuildSub(left, right),
            BinaryOpType.Mul => _builder.BuildMul(left, right),
            BinaryOpType.Div => _builder.BuildSDiv(left, right),
            BinaryOpType.Mod => _builder.BuildSRem(left, right),
            _ => throw new NotSupportedException(op.ToString())
        };

    private LLVMValueRef Eval(IAstNode node)
    {
        Accept(node);
        LLVMValueRef value = _stack.Pop();
        return value;
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
}
