using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class BinaryOpTests
{
    [Test]
    [TestCase(3, 5, 8)]
    [TestCase(3, -5, -2)]
    [TestCase(-3, 5, 2)]
    [TestCase(-3, -5, -8)]
    public void Add_i32_i32(int a, int b, int expected)
    {
        const string code = "i32 main(i32 a, i32 b) { return a + b; }";
        Runner.Run<int>(code, a, b).Should().Be(expected);
    }

    [Test]
    [TestCase(3, 5, -2)]
    [TestCase(3, -5, 8)]
    [TestCase(-3, 5, -8)]
    [TestCase(-3, -5, 2)]
    public void Sub_i32_i32(int a, int b, int expected)
    {
        const string code = "i32 main(i32 a, i32 b) { return a - b; }";
        Runner.Run<int>(code, a, b).Should().Be(expected);
    }

    [Test]
    [TestCase(3, 5, 15)]
    [TestCase(3, -5, -15)]
    public void Mul_i32_i32(int a, int b, int expected)
    {
        const string code = "i32 main(i32 a, i32 b) { return a * b; }";
        Runner.Run<int>(code, a, b).Should().Be(expected);
    }

    [Test]
    [TestCase(7, 2, 3)]
    [TestCase(7, -2, -3)]
    [TestCase(-7, 2, -3)]
    [TestCase(-7, -2, 3)]
    public void Div_i32_i32(int a, int b, int expected)
    {
        const string code = "i32 main(i32 a, i32 b) { return a / b; }";
        Runner.Run<int>(code, a, b).Should().Be(expected);
    }

    [Test]
    [TestCase(7, 2, 1)]
    [TestCase(7, -2, 1)]
    [TestCase(-7, 2, -1)]
    [TestCase(-7, -2, -1)]
    public void Mod_i32_i32(int a, int b, int expected)
    {
        const string code = "i32 main(i32 a, i32 b) { return a % b; }";
        Runner.Run<int>(code, a, b).Should().Be(expected);
    }

    [Test]
    public void Math_operation_priority_test()
    {
        const string code = "i32 main(i32 x, i32 y) { return x + 2 * 3 + x * 3 + (x + y) * 2 - y / 4 + (x + y) % 3; }";
        Runner.Run<int>(code, 2, 8).Should().Be(33);
    }

    [Test]
    [TestCase("==", 7, 7, true)]
    [TestCase("==", -7, -7, true)]
    [TestCase("==", 7, 5, false)]

    [TestCase("!=", 7, 7, false)]
    [TestCase("!=", -7, -7, false)]
    [TestCase("!=", 7, 5, true)]

    [TestCase("<=", 7, 5, false)]
    [TestCase("<=", 5, 7, true)]
    [TestCase("<=", 5, 5, true)]
    [TestCase("<=", -7, 5, true)]
    [TestCase("<=", 7, -5, false)]
    [TestCase("<=", -7, -5, true)]
    [TestCase("<=", -5, -7, false)]
    [TestCase("<=", -5, -5, true)]

    [TestCase(">=", 7, 5, true)]
    [TestCase(">=", 5, 7, false)]
    [TestCase(">=", 5, 5, true)]
    [TestCase(">=", -7, 5, false)]
    [TestCase(">=", 7, -5, true)]
    [TestCase(">=", -7, -5, false)]
    [TestCase(">=", -5, -7, true)]
    [TestCase(">=", -5, -5, true)]

    [TestCase("<", 7, 5, false)]
    [TestCase("<", 5, 7, true)]
    [TestCase("<", 5, 5, false)]
    [TestCase("<", -7, 5, true)]
    [TestCase("<", 7, -5, false)]
    [TestCase("<", -7, -5, true)]
    [TestCase("<", -5, -7, false)]
    [TestCase("<", -5, -5, false)]

    [TestCase(">", 7, 5, true)]
    [TestCase(">", 5, 7, false)]
    [TestCase(">", 5, 5, false)]
    [TestCase(">", -7, 5, false)]
    [TestCase(">", 7, -5, true)]
    [TestCase(">", -7, -5, false)]
    [TestCase(">", -5, -7, true)]
    [TestCase(">", -5, -5, false)]
    public void Relation_i32_i32_test(string op, int a, int b, bool expected)
    {
        string code = $"i32 main(i32 a, i32 b) {{ return a {op} b ? 5 : 3; }}";
        Runner.Run<int>(code, a, b).Should().Be(expected ? 5 : 3); //TODO: casting is not implemented yet
    }

    [Test]
    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    [TestCase(true, false, false)]
    [TestCase(true, true, true)]
    public void Logical_And_test(bool left, bool right, bool expected)
    {
        const string code = @"i32 main(bool a, bool b) { return a && b ? 5 : 3; }";
        Runner.Run<int>(code, left, right).Should().Be(expected ? 5 : 3); //TODO: casting is not implemented yet
    }

    [Test]
    [TestCase(false, false, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, true)]
    [TestCase(true, true, true)]
    public void Logical_Or_test(bool left, bool right, bool expected)
    {
        const string code = @"i32 main(bool a, bool b) { return a || b ? 5 : 3; }";
        Runner.Run<int>(code, left, right).Should().Be(expected ? 5 : 3); //TODO: casting is not implemented yet
    }
}