using System;
using System.Runtime.InteropServices;
using LLVMSharp.Interop;

namespace Zen.Jit;

public static class JitX86
{
    static JitX86()
    {
        LLVM.LinkInMCJIT();
        LLVM.InitializeX86TargetMC();
        LLVM.InitializeX86Target();
        LLVM.InitializeX86TargetInfo();
        LLVM.InitializeX86AsmParser();
        LLVM.InitializeX86AsmPrinter();
    }

    public static TDelegate Compile<TDelegate>(LLVMModuleRef module, string funcName) where TDelegate : Delegate =>
        (TDelegate)Compile(module, funcName, typeof(TDelegate));

    public static Delegate Compile(LLVMModuleRef module, string funcName, Type delegateType)
    {
        if (!module.TryCreateExecutionEngine(out LLVMExecutionEngineRef engine, out string error))
        {
            throw new InvalidOperationException(error);
        }

        LLVMValueRef llvmFunc = module.GetNamedFunction(funcName);
        IntPtr funcPtr = engine.GetPointerToGlobal(llvmFunc);
        Delegate func = Marshal.GetDelegateForFunctionPointer(funcPtr, delegateType);
        return func;
    }
}