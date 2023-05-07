using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class VarDeclareTests
{
    [Test]
    public void Declare_i8_variable_test()
    {
        const string code = @"i8 main(i8 a) { i8 v = a + 7; return v; }";
        Runner.Run<sbyte>(code, 3).Should().Be(10);
    }

    [Test]
    public void Declare_i32_variable_test()
    {
        const string code = @"
i32 main(i32 a) {
    i32 v = a + 7;
    return v;
}";
        Runner.Run<int>(code, 3).Should().Be(10);
    }

    [Test]
    public void Declare_u32_variable_test()
    {
        const string code = @"u32 main(u32 a) { u32 v = a + 10; return v; }";
        Runner.Run<uint>(code, int.MaxValue).Should().Be(2147483657);
    }

    [Test]
    public void Declare_f64_variable_test()
    {
        const string code = @"
f64 main(f64 a) {
    f64 v = a + 7.3;
    return v;
}";
        Runner.Run<double>(code, 3.2).Should().BeApproximately(10.5, 0.000_001);
    }
}