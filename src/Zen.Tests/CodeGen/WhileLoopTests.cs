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

    [Test]
    public void Break_statement_in_while_loop_test()
    {
        const string code = @"
i32 main(i32 x) {
    while (x < 3) { x = x + 1; break; x = x + 10; }
    return x;
}";
        Runner.Run<int>(code, 1).Should().Be(2);
    }

    [Test]
    public void Continue_statement_in_while_loop_test()
    {
        const string code = @"
i32 main() {
    i32 x = 0;
    i32 y = 0;
    while (x < 10) {
        x = x + 1;
        if (x < 3) { y = y - 1; continue; y = y + 10; }
        y = y + 1;
    }
    return y;
}";

        Runner.Run<int>(code).Should().Be(6);
    }

    [Test]
    public void Nested_while_loop_break_test()
    {
        const string code = @"
i32 main() {
    i32 i = 0;
    i32 k = 0;
    while (i < 10) {
        i32 j = 0;
        while (j < 5) {
            if (j > i) { break; }
            j = j + 1;
            k = k + 1;
        }
        if (k == 10) { break; }
        i = i + 1;
    }
    return i + k;
}";
        Runner.Run<int>(code).Should().Be(13);
    }
}