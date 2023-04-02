using System.Collections.Generic;
using LLVMSharp.Interop;

namespace Zen.CodeGen;

public class ScopeManager
{
    private class Scope
    {
        public readonly List<string> Names;
        public LLVMValueRef ReturnValuePtr;
        public LLVMBasicBlockRef ReturnBlock;
        public LLVMBasicBlockRef BreakBlock;
        public LLVMBasicBlockRef ContinueBlock;

        public Scope()
        {
            Names = new List<string>(); //TODO: lazy
        }

        public Scope(Scope prev) : this()
        {
            ReturnValuePtr = prev.ReturnValuePtr;
            ReturnBlock = prev.ReturnBlock;
            BreakBlock = prev.BreakBlock;
            ContinueBlock = prev.ContinueBlock;
        }
    }

    private readonly Stack<Scope> _scopes;
    private readonly Dictionary<string, Entity> _index;

    public ScopeManager()
    {
        _scopes = new Stack<Scope>();
        _index = new Dictionary<string, Entity>();
    }

    public void Begin()
    {
        Scope scope = _scopes.TryPeek(out Scope current)
            ? new Scope(current)
            : new Scope();
        _scopes.Push(scope);
    }

    public void End()
    {
        Scope scope = _scopes.Pop();
        foreach (string name in scope.Names)
        {
            _index.Remove(name);
        }
    }

    public void Add(string name, LLVMValueRef value, LLVMTypeRef type)
    {
        var entity = new Entity(value, type);
        _index.Add(name, entity);
        Current.Names.Add(name);
    }

    public Entity this[string name] => _index[name];

    public LLVMValueRef ReturnValuePtr
    {
        get => Current.ReturnValuePtr;
        set => Current.ReturnValuePtr = value;
    }

    public LLVMBasicBlockRef ReturnBlock
    {
        get => Current.ReturnBlock;
        set => Current.ReturnBlock = value;
    }

    public LLVMBasicBlockRef BreakBlock
    {
        get => Current.BreakBlock;
        set => Current.BreakBlock = value;
    }

    public LLVMBasicBlockRef ContinueBlock
    {
        get => Current.ContinueBlock;
        set => Current.ContinueBlock = value;
    }

    private Scope Current => _scopes.Peek();
}