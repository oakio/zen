using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class WhileLoopTests
{
    [Test]
    public void While_loop_test()
    {
        const string code = @"
i32 main(i32 x) {
    while (x < 3) { x = x + 1; }
    return x;
}";
        Runner.Run<int>(code, 0).Should().Be(3);
    }

    [Test]
    public void Return_statement_in_while_loop_test()
    {
        const string code = @"
i32 main(i32 x) {
    while (x < 3) { x = x + 1; return x; x = x + 2; }
    return x;
}";
        Runner.Run<int>(code, 1).Should().Be(2);
    }
}