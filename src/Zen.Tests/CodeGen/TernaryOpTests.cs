using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class TernaryOpTests
{
    [Test]
    [TestCase(true, 10)]
    [TestCase(false, 5)]
    public void Ternary_operator_test(bool condition, int expected)
    {
        const string code = "i32 main(bool c) { return c ? 10 : 5; }";
        Runner.Run<int>(code, condition).Should().Be(expected);
    }
}