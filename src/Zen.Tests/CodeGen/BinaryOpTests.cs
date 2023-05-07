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
    [TestCase(3.1, 5.4, 8.5)]
    [TestCase(3.1, -5.4, -2.3)]
    [TestCase(-3.1, 5.4, 2.3)]
    [TestCase(-3.1, -5.1, -8.2)]
    public void Add_f64_f64(double a, double b, double expected)
    {
        const string code = "f64 main(f64 a, f64 b) { return a + b; }";
        Runner.Run<double>(code, a, b).Should().BeApproximately(expected, 0.000_001);
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
    [TestCase(3.1, 5.4, -2.3)]
    [TestCase(3.1, -5.4, 8.5)]
    [TestCase(-3.1, 5.4, -8.5)]
    [TestCase(-3.1, -5.4, 2.3)]
    public void Sub_f64_f64(double a, double b, double expected)
    {
        const string code = "f64 main(f64 a, f64 b) { return a - b; }";
        Runner.Run<double>(code, a, b).Should().BeApproximately(expected, 0.000_001);
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
    [TestCase(3.1, 5.4, 16.74)]
    [TestCase(3.1, -5.4, -16.74)]
    public void Mul_f64_f64(double a, double b, double expected)
    {
        const string code = "f64 main(f64 a, f64 b) { return a * b; }";
        Runner.Run<double>(code, a, b).Should().BeApproximately(expected, 0.000_001);
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
    [TestCase(7.1, 2.5, 2.84)]
    [TestCase(7.1, -2.5, -2.84)]
    [TestCase(-7.1, 2.5, -2.84)]
    [TestCase(-7.1, -2.5, 2.84)]
    public void Div_f64_f64(double a, double b, double expected)
    {
        const string code = "f64 main(f64 a, f64 b) { return a / b; }";
        Runner.Run<double>(code, a, b).Should().BeApproximately(expected, 0.000_001);
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
    [TestCase(7.1, 2.4, 2.3)]
    [TestCase(7.1, -2.4, 2.3)]
    [TestCase(-7.1, 2.4, -2.3)]
    [TestCase(-7.1, -2.4, -2.3)]
    public void Mod_f64_f64(double a, double b, double expected)
    {
        const string code = "f64 main(f64 a, f64 b) { return a % b; }";
        Runner.Run<double>(code, a, b).Should().BeApproximately(expected, 0.000_0001);
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
        string code = $"bool main(i32 a, i32 b) {{ return a {op} b; }}";
        Runner.Run<bool>(code, a, b).Should().Be(expected);
    }

    [Test]
    [TestCase("==", 7, 7u, true)]
    [TestCase("==", -7, 5u, false)]
    [TestCase("==", 7, 5u, false)]

    [TestCase("!=", 7, 7u, false)]
    [TestCase("!=", -7, 5u, true)]
    [TestCase("!=", 7, 5u, true)]

    [TestCase("<=", 7, 5u, false)]
    [TestCase("<=", 5, 7u, true)]
    [TestCase("<=", 5, 5u, true)]
    [TestCase("<=", -7, 5u, true)]
    [TestCase("<=", -1000, uint.MaxValue, true)]

    [TestCase(">=", 7, 5u, true)]
    [TestCase(">=", 5, 7u, false)]
    [TestCase(">=", 5, 5u, true)]
    [TestCase(">=", -7, 5u, false)]
    [TestCase(">=", -1000, uint.MaxValue, false)]

    [TestCase("<", 7, 5u, false)]
    [TestCase("<", 5, 7u, true)]
    [TestCase("<", 5, 5u, false)]
    [TestCase("<", -7, 5u, true)]
    [TestCase("<", -1000, uint.MaxValue, true)]

    [TestCase(">", 7, 5u, true)]
    [TestCase(">", 5, 7u, false)]
    [TestCase(">", 5, 5u, false)]
    [TestCase(">", -7, 5u, false)]
    [TestCase(">", -1000, uint.MaxValue, false)]
    public void Relation_i32_u32_test(string op, int a, uint b, bool expected)
    {
        string code = $"bool main(i32 a, i32 b) {{ return a {op} b; }}";
        Runner.Run<bool>(code, a, b).Should().Be(expected);
    }

    [Test]
    [TestCase("==", 7.1, 7.1, true)]
    [TestCase("==", -7.1, -7.1, true)]
    [TestCase("==", 7.1, 5.2, false)]

    [TestCase("!=", 7.1, 7.1, false)]
    [TestCase("!=", -7.1, -7.1, false)]
    [TestCase("!=", 7.1, 5.2, true)]

    [TestCase("<=", 7.1, 5.2, false)]
    [TestCase("<=", 5.2, 7.1, true)]
    [TestCase("<=", 5.2, 5.2, true)]
    [TestCase("<=", -7.1, 5.2, true)]
    [TestCase("<=", 7.1, -5.2, false)]
    [TestCase("<=", -7.1, -5.2, true)]
    [TestCase("<=", -5.2, -7.1, false)]
    [TestCase("<=", -5.2, -5.2, true)]

    [TestCase(">=", 7.1, 5.2, true)]
    [TestCase(">=", 5.2, 7.1, false)]
    [TestCase(">=", 5.2, 5.2, true)]
    [TestCase(">=", -7.1, 5.2, false)]
    [TestCase(">=", 7.1, -5.2, true)]
    [TestCase(">=", -7.1, -5.2, false)]
    [TestCase(">=", -5.2, -7.1, true)]
    [TestCase(">=", -5.2, -5.2, true)]

    [TestCase("<", 7.1, 5.2, false)]
    [TestCase("<", 5.2, 7.1, true)]
    [TestCase("<", 5.2, 5.2, false)]
    [TestCase("<", -7.1, 5.2, true)]
    [TestCase("<", 7.1, -5.2, false)]
    [TestCase("<", -7.1, -5.2, true)]
    [TestCase("<", -5.2, -7.1, false)]
    [TestCase("<", -5.2, -5.2, false)]

    [TestCase(">", 7.1, 5.2, true)]
    [TestCase(">", 5.2, 7.1, false)]
    [TestCase(">", 5.2, 5.2, false)]
    [TestCase(">", -7.1, 5.2, false)]
    [TestCase(">", 7.1, -5.2, true)]
    [TestCase(">", -7.1, -5.2, false)]
    [TestCase(">", -5.2, -7.1, true)]
    [TestCase(">", -5.2, -5.2, false)]
    public void Relation_f64_f64_test(string op, double a, double b, bool expected)
    {
        string code = $"bool main(f64 a, f64 b) {{ return a {op} b; }}";
        Runner.Run<bool>(code, a, b).Should().Be(expected);
    }

    [Test]
    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    [TestCase(true, false, false)]
    [TestCase(true, true, true)]
    public void Logical_And_test(bool left, bool right, bool expected)
    {
        const string code = @"bool main(bool a, bool b) { return a && b; }";
        Runner.Run<bool>(code, left, right).Should().Be(expected);
    }

    [Test]
    [TestCase(false, false, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, true)]
    [TestCase(true, true, true)]
    public void Logical_Or_test(bool left, bool right, bool expected)
    {
        const string code = @"bool main(bool a, bool b) { return a || b; }";
        Runner.Run<bool>(code, left, right).Should().Be(expected);
    }
}