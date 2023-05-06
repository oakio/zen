using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class AssignValueTests
{
    [Test]
    public void Assign_i32_const_test()
    {
        const string code = "i32 main(i32 v) { v = 123; return v; }";
        Runner.Run<int>(code).Should().Be(123);
    }

    [Test]
    public void Assign_i32_expression_test()
    {
        const string code = "i32 main(i32 v) { v = v * v; return v; }";
        Runner.Run<int>(code, 3).Should().Be(9);
    }

    [Test]
    public void Assign_f64_const_test()
    {
        const string code = "f64 main(f64 v) { v = 1.23; return v; }";
        Runner.Run<double>(code).Should().Be(1.23);
    }

    [Test]
    public void Assign_f64_expression_test()
    {
        const string code = "f64 main(f64 v) { v = v * v; return v; }";
        Runner.Run<double>(code, 0.5).Should().Be(0.25);
    }
}