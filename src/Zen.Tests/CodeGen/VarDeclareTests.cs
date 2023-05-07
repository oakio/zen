using FluentAssertions;
using NUnit.Framework;

namespace Zen.Tests.CodeGen;

[TestFixture]
public class VarDeclareTests
{
    [Test]
    [TestCase(0)]
    [TestCase(sbyte.MinValue)]
    [TestCase(sbyte.MaxValue)]
    public void Declare_i8_variable_test(sbyte value) => AssertDeclare("i8", value);

    [Test]
    [TestCase(byte.MinValue)]
    [TestCase(byte.MaxValue)]
    public void Declare_u8_variable_test(byte value) => AssertDeclare("u8", value);

    [Test]
    [TestCase(0)]
    [TestCase(short.MinValue)]
    [TestCase(short.MaxValue)]
    public void Declare_i16_variable_test(short value) => AssertDeclare("i16", value);

    [Test]
    [TestCase(ushort.MinValue)]
    [TestCase(ushort.MaxValue)]
    public void Declare_u16_variable_test(ushort value) => AssertDeclare("u16", value);

    [Test]
    [TestCase(0)]
    [TestCase(int.MinValue)]
    [TestCase(int.MaxValue)]
    public void Declare_i32_variable_test(int value) => AssertDeclare("i32", value);

    [Test]
    [TestCase(uint.MinValue)]
    [TestCase(uint.MaxValue)]
    public void Declare_u32_variable_test(uint value) => AssertDeclare("u32", value);

    [Test]
    [TestCase(0)]
    [TestCase(int.MinValue)]
    [TestCase(int.MaxValue)]
    public void Declare_i64_variable_test(int value) => AssertDeclare("i64", value);

    [Test]
    [TestCase(uint.MinValue)]
    [TestCase(uint.MaxValue)]
    public void Declare_u64_variable_test(uint value) => AssertDeclare("u64", value);

    [Test]
    [TestCase(0)]
    [TestCase(float.MinValue)]
    [TestCase(float.MaxValue)]
    public void Declare_f32_variable_test(float value) => AssertDeclare("f32", value);

    [Test]
    [TestCase(0)]
    [TestCase(double.MinValue)]
    [TestCase(double.MaxValue)]
    public void Declare_f64_variable_test(double value) => AssertDeclare("f64", value);

    private static void AssertDeclare<T>(string type, T value)
    {
        string code = $"{type} main({type} a) {{ {type} v = a; return v; }}";
        Runner.Run<T>(code, value).Should().Be(value);
    }
}