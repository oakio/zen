using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class VarDeclareTests
{
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
}