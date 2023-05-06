using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class CastTests
{
    [Test]
    [TestCase(0, 0)]
    [TestCase(10.74, 10)]
    [TestCase(-5.29, -5)]
    [TestCase(-5.29, -5)]
    [TestCase(double.Epsilon, 0)]
    [TestCase(double.MaxValue, int.MinValue)]
    [TestCase(double.MinValue, int.MinValue)]
    public void Cast_f64_to_i32_test(double value, int expected)
    {
        const string code = @"i32 main(f64 v) { return (i32)v; }";
        Runner.Run<int>(code, value).Should().Be(expected);
    }

    [Test]
    [TestCase(0)]
    [TestCase(10)]
    [TestCase(-5)]
    public void Cast_i32_to_i32_test(int value)
    {
        const string code = @"i32 main(i32 v) { return (i32)v; }";
        Runner.Run<int>(code, value).Should().Be(value);
    }

    [Test]
    [TestCase(0, 0.0)]
    [TestCase(10, 10.0)]
    [TestCase(-5, -5.0)]
    public void Cast_i32_to_i64_test(int value, double expected)
    {
        const string code = @"f64 main(i32 v) { return (f64)v; }";
        Runner.Run<double>(code, value).Should().Be(expected);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(10, 10)]
    [TestCase(-5, -5)]
    [TestCase(300, 44)]
    [TestCase(700, -68)]
    [TestCase(-300, -44)]
    [TestCase(-700, 68)]
    public void Cast_i32_to_i8_test(int value, sbyte expected)
    {
        const string code = @"i8 main(i32 v) { return (i8)v; }";
        Runner.Run<sbyte>(code, value).Should().Be(expected);
    }

    [Test]
    [TestCase(0, 0)]
    [TestCase(10, 10)]
    [TestCase(-5, -5)]
    public void Cast_i8_to_i32_test(int value, sbyte expected)
    {
        const string code = @"i32 main(i8 v) { return (i32)v; }";
        Runner.Run<int>(code, value).Should().Be(expected);
    }
}