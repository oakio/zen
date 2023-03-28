using LLVMSharp.Interop;
using Zen.AST;
using Zen.AST.Nodes;

namespace Zen.CodeGen;

public class LLVMCodeGenerator : IAstVisitor
{
    private readonly LLVMContextRef _context;
    private readonly LLVMBuilderRef _builder;

    public LLVMModuleRef Module;

    public LLVMCodeGenerator()
    {
        _context = LLVMContextRef.Global;
        _builder = LLVMBuilderRef.Create(_context);
    }

    public void Visit(ModuleDeclareNode node)
    {
        Module = _context.CreateModuleWithName(node.Id);
    }
}