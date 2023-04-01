using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class IfElseTests
{
    [Test]
    [TestCase(true, 5)]
    [TestCase(false, 2)]
    public void If_then_test(bool condition, int expected)
    {
        const string code = @"
i32 main(bool c) {
    if (c) { return 5; }
    return 2;
}";
        Runner.Run<int>(code, condition).Should().Be(expected);
    }

    [Test]
    [TestCase(true, 5)]
    [TestCase(false, 2)]
    public void If_then_else_test(bool condition, int expected)
    {
        const string code = @"
i32 main(bool c) {
    if (c) { return 5; } else { return 2; }
    return 4;
}";
        Runner.Run<int>(code, condition).Should().Be(expected);
    }
}