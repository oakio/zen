using System.IO;

namespace Zen.AST;

public interface IAstBuilder
{
    bool TryBuild(string sourceName, StringReader reader, out IAstNode ast);
}