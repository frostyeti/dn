using Xunit;

namespace FrostYeti.Results.Tests;

public class ResultExtensions_Tests
{
    [Fact]
    public void IsOkAnd_True_WhenOkAndPredicateTrue()
    {
        var result = Result<string, string>.Ok("value");

        var ok = result.IsOkAnd(v => v.Length == 5);

        Assert.True(ok);
    }

    [Fact]
    public void IsOkAnd_False_WhenError()
    {
        var result = Result<string, string>.Fail("err");

        var ok = result.IsOkAnd(v => v.Length == 5);

        Assert.False(ok);
    }

    [Fact]
    public void IsErrorAnd_True_WhenErrorAndPredicateTrue()
    {
        var result = Result<string, string>.Fail("err");

        var isMatch = result.IsErrorAnd(e => e == "err");

        Assert.True(isMatch);
    }

    [Fact]
    public void IsErrorAnd_False_WhenOk()
    {
        var result = Result<string, string>.Ok("value");

        var isMatch = result.IsErrorAnd(e => e == "err");

        Assert.False(isMatch);
    }

    [Fact]
    public void Expect_ReturnsValue_WhenOk()
    {
        var result = Result<string, string>.Ok("value");

        var value = result.Expect("should not throw");

        Assert.Equal("value", value);
    }

    [Fact]
    public void Expect_Throws_WhenError()
    {
        var result = Result<string, string>.Fail("err");

        Assert.Throws<ResultException>(() => result.Expect("fail"));
    }

    [Fact]
    public void ExpectError_ReturnsError_WhenError()
    {
        var result = Result<string, string>.Fail("err");

        var error = result.ExpectError("should not throw");

        Assert.Equal("err", error);
    }

    [Fact]
    public void ExpectError_Throws_WhenOk()
    {
        var result = Result<string, string>.Ok("value");

        Assert.Throws<ResultException>(() => result.ExpectError("fail"));
    }
}