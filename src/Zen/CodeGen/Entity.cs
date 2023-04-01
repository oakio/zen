using LLVMSharp.Interop;

namespace Zen.CodeGen;

public readonly struct Entity
{
    public readonly LLVMValueRef Value;
    public readonly LLVMTypeRef Type;

    public Entity(LLVMValueRef value, LLVMTypeRef type)
    {
        Value = value;
        Type = type;
    }
}