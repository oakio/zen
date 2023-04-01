using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class CommentTests
{
    [Test]
    public void Ignore_comment_test()
    {
        const string code = @"
i32 main(i32 v) {
    // this is comment
    // v = -v;
    v = v + // 5
    10;
    // return 100;
    return v;
}";

        Runner.Run<int>(code, 3).Should().Be(13);
    }
}