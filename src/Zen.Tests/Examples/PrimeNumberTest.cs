using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.Examples;

[TestFixture]
public class PrimeNumberTest
{
    [Test]
    [TestCase(2, 1)]
    [TestCase(7, 4)]
    [TestCase(71, 20)]
    [TestCase(7919, 1000)]
    public void Count_prime_numbers_in_range(int maxNumber, int expectedCount)
    {
        const string code = @"
bool is_prime(i32 v) {
    if (v <= 1) {
        return false;
    }
    i32 h = v / 2;
    i32 x = 2;
    while (x <= h) {
        if (v % x == 0) {
            return false;
        }
        x = x + 1;
    }
    return true;
}

i32 main(i32 n) {
    i32 x = 1;
    i32 count = 0;
    while (x <= n) {
        if (is_prime(x)) {
            count = count + 1;
        }
        x = x + 1;
    }
    return count;
}
";
        Runner.Run<int>(code, maxNumber).Should().Be(expectedCount);
    }
}