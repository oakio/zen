using System;
using Zen.AST;
using Zen.AST.Nodes;

namespace Zen.Antlr;

public class ZenVisitor : ZenBaseVisitor<IAstNode>
{
    public override IAstNode VisitModule(ZenParser.ModuleContext context)
    {
        string moduleName = context.Start.InputStream.SourceName;
        IAstNode[] inner = Array.Empty<IAstNode>();
        return new ModuleDeclareNode(moduleName, inner);
    }
}