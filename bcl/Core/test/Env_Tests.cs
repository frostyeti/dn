namespace FrostYeti.Tests;

public class Env_Tests
{
    [Fact]
    public void JoinPath_AndSplitPath_RoundTrip()
    {
        var joined = Env.JoinPath("alpha", "beta");

        var split = Env.SplitPath(joined);

        Assert.Equal(2, split.Length);
        Assert.Equal("alpha", split[0]);
        Assert.Equal("beta", split[1]);
    }

    [Fact]
    public void HasPath_StringArray_RespectsPlatformComparison()
    {
        var paths = new[] { "Alpha" };

        var has = Env.HasPath("alpha", paths);

        if (OperatingSystem.IsWindows())
            Assert.True(has);
        else
            Assert.False(has);
    }

    [Fact]
    public void TrySetGetUnset_ProcessVariable_Works()
    {
        var key = "FY_TEST_" + Guid.NewGuid().ToString("N");

        try
        {
            var set = Env.TrySetEnv(key, "value", EnvironmentVariableTarget.Process);
            Assert.True(set.IsOk);

            var get = Env.TryGet(key, EnvironmentVariableTarget.Process);
            Assert.True(get.IsOk);
            Assert.Equal("value", get.Value);

            var unset = Env.TryUnset(key, EnvironmentVariableTarget.Process);
            Assert.True(unset.IsOk);

            var after = Env.TryGet(key, EnvironmentVariableTarget.Process);
            Assert.True(after.IsError);
        }
        finally
        {
            Environment.SetEnvironmentVariable(key, null, EnvironmentVariableTarget.Process);
        }
    }

    [Fact]
    public void VarsIndexer_SetAndGet_Works()
    {
        var key = "FY_VARS_" + Guid.NewGuid().ToString("N");

        try
        {
            Env.Vars[key] = "abc";

            Assert.Equal("abc", Env.Vars[key]);
        }
        finally
        {
            Environment.SetEnvironmentVariable(key, null, EnvironmentVariableTarget.Process);
        }
    }

    [Fact]
    public void AppendPrependRemovePath_ProcessPath_Works()
    {
        var target = EnvironmentVariableTarget.Process;
        var pathKey = Env.Keys.Path;
        var original = Environment.GetEnvironmentVariable(pathKey, target);

        var first = "fy_first_" + Guid.NewGuid().ToString("N");
        var second = "fy_second_" + Guid.NewGuid().ToString("N");
        var third = "fy_third_" + Guid.NewGuid().ToString("N");

        try
        {
            Env.Set(pathKey, Env.JoinPath(first, second), target);

            Env.PrependPath(third, target);
            var afterPrepend = Env.SplitPath(target);
            Assert.Equal(third, afterPrepend[0]);

            Env.AppendPath(third, target);
            var afterAppend = Env.SplitPath(target);
            Assert.Equal(third, afterAppend[0]);

            Env.RemovePath(third, target);
            var afterRemove = Env.SplitPath(target);
            Assert.DoesNotContain(third, afterRemove);
            Assert.Equal(first, afterRemove[0]);
            Assert.Equal(second, afterRemove[1]);
        }
        finally
        {
            Environment.SetEnvironmentVariable(pathKey, original, target);
        }
    }
}