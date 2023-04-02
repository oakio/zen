using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class UnaryOpTests
{
    [Test]
    [TestCase(0, 0)]
    [TestCase(5, -5)]
    [TestCase(-5, 5)]
    [TestCase(int.MaxValue, -int.MaxValue)]
    [TestCase(int.MinValue, int.MinValue)]
    public void Neg_i32_test(int value, int expected)
    {
        const string code = "i32 main(i32 v) { return -v; }";

        Runner.Run<int>(code, value).Should().Be(expected);
    }

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    public void Not_test(bool value, bool expected)
    {
        const string code = "bool main(bool v) { return !v; }";

        Runner.Run<bool>(code, value).Should().Be(expected);
    }
}