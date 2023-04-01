using System;
using System.IO;
using LLVMSharp.Interop;
using Zen.Antlr;
using Zen.AST;
using Zen.CodeGen;
using Zen.Jit;
using Zen.Reporting;

[assembly: CLSCompliant(false)]

namespace Zen;

public class Program
{
    private delegate int MainFunc(int x);

    public static void Main()
    {
        string source = "i32 main(i32 x) { return x; }";

        var reporter = new ConsoleReporter();
        IAstBuilder astBuilder = new ZenAstBuilder(reporter);
        if (!astBuilder.TryBuild("example.zen", new StringReader(source), out IAstNode ast))
        {
            return;
        }

        Console.WriteLine("============== Source ==============");
        Console.WriteLine(source);
        Console.WriteLine("============== AST ==============");
        ast.Accept(AstPrinter.Instance);

        var llvmGenerator = new LLVMCodeGenerator();
        ast.Accept(llvmGenerator);

        Console.WriteLine("============== LLVM IR ==============");
        LLVMModuleRef module = llvmGenerator.Module;
        Console.WriteLine(module.PrintToString());

        Console.WriteLine("=====================================");
        var main = JitX86.Compile<MainFunc>(module, "main");
        int result = main(123);
        Console.WriteLine($"result = {result}");
    }
}
