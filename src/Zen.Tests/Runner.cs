using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LLVMSharp.Interop;
using Zen.Antlr;
using Zen.AST;
using Zen.CodeGen;
using Zen.Reporting;

namespace Zen.Tests;

public class Runner
{
    private static readonly Func<Type[], Type> MakeNewCustomDelegate = (Func<Type[], Type>)
        Delegate.CreateDelegate(
            typeof(Func<Type[], Type>),
            typeof(Expression)
                .Assembly
                .GetType("System.Linq.Expressions.Compiler.DelegateHelpers")
                .GetMethod("MakeNewCustomDelegate", BindingFlags.NonPublic | BindingFlags.Static));

    public static TReturn Run<TReturn>(string source, params object[] args) =>
        RunInternal<TReturn>(source, args, false);

    public static TReturn RunDebug<TReturn>(string source, params object[] args) =>
        RunInternal<TReturn>(source, args, true);

    private static TReturn RunInternal<TReturn>(string source, object[] args, bool debug)
    {
        Type[] argsTypes = args.Select(a => a.GetType()).ToArray();
        Type delegateType = CreateDelegateType(typeof(TReturn), argsTypes);
        Delegate main = Compile(source, "main", delegateType, debug);
        return (TReturn)main.DynamicInvoke(args);
    }

    private static Type CreateDelegateType(Type returnType, params Type[] parameters)
    {
        var args = new Type[parameters.Length + 1];
        parameters.CopyTo(args, 0);
        args[^1] = returnType;
        return MakeNewCustomDelegate(args);
    }

    private static Delegate Compile(string source, string funcName, Type delegateType, bool debug)
    {
        var reporter = new ConsoleReporter();
        var astBuilder = new ZenAstBuilder(reporter);

        if (!astBuilder.TryBuild(null, new StringReader(source), out IAstNode ast))
        {
            throw new InvalidOperationException("Compilation failed");
        }

        if (debug)
        {
            var astPrinter = new AstPrinter();
            ast.Accept(astPrinter);
        }

        var generator = new LLVMCodeGenerator();
        ast.Accept(generator);
        LLVMModuleRef module = generator.Module;

        if (debug)
        {
            Console.WriteLine(module.PrintToString());
        }

        return Jit.JitX86.Compile(module, funcName, delegateType);
    }
}