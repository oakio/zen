using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class ReturnTests
{
    [Test]
    public void Return_const_test()
    {
        const string code = "i32 main() { return 10; }";
        Runner.Run<int>(code).Should().Be(10);
    }

    [Test]
    public void Return_variable_test()
    {
        const string code = "i32 main(i32 x) { return x; }";
        Runner.Run<int>(code, 10).Should().Be(10);
    }

    [Test]
    public void Return_expression_test()
    {
        const string code = "i32 main(i32 x) { return x + 1; }";
        Runner.Run<int>(code, 10).Should().Be(11);
    }
}