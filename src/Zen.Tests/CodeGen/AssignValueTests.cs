using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class AssignValueTests
{
    [Test]
    public void Assign_const_test()
    {
        const string code = "i32 main(i32 v) { v = 123; return v; }";
        Runner.Run<int>(code).Should().Be(123);
    }

    [Test]
    public void Assign_expression_test()
    {
        const string code = "i32 main(i32 v) { v = v * v; return v; }";
        Runner.Run<int>(code, 3).Should().Be(9);
    }
}