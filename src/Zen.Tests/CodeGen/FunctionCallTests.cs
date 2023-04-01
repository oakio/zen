using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class FunctionCallTests
{
    [Test]
    public void Function_call_without_arguments_test()
    {
        const string code = @"
i32 fun() { return 42; }
i32 main() { return fun(); }";

        Runner.Run<int>(code).Should().Be(42);
    }

    [Test]
    public void Function_call_with_arguments_test()
    {
        const string code = @"
i32 inc(i32 v) { return v + 1; }
i32 main(i32 v) { return inc(v); }";

        Runner.Run<int>(code, 4).Should().Be(5);
    }

    [Test]
    public void Function_order_definition_not_matter()
    {
        const string code = @"
i32 main() { return defined_below(); }
i32 defined_below() { return 1; }
";

        Runner.Run<int>(code).Should().Be(1);
    }

    [Test]
    public void Recursion_test()
    {
        const string code = @"
i32 main(i32 n) {
    if (n == 0) { return 0; }
    if (n == 1) { return 1; }
    return main(n - 1) + main(n - 2);
}";

        Runner.Run<int>(code, 12).Should().Be(144);
    }
}