namespace FrostYeti.Lodi.Tests;

using FrostYeti;

public class IServiceProviderExtensionsTests
{
    [Fact]
    public void LodiGetService_WithKey_ReturnsNamedService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient<ITestService>("keyA", sp => new TestService("A"));
        IServiceProvider sp = provider;

        var result = sp.LodiGetService("keyA");

        Assert.NotNull(result);
        Assert.IsType<TestService>(result);
    }

    [Fact]
    public void LodiGetService_Generic_ReturnsService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient<ITestService>(sp => new TestService());
        IServiceProvider sp = provider;

        var result = sp.LodiGetService<ITestService>();

        Assert.NotNull(result);
    }

    [Fact]
    public void LodiGetService_GenericWithKey_ReturnsNamedService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient<ITestService>("keyA", sp => new TestService("A"));
        IServiceProvider sp = provider;

        var result = sp.LodiGetService<ITestService>("keyA");

        Assert.NotNull(result);
        Assert.Equal("A", result?.Name);
    }

    [Fact]
    public void LodiGetRequiredService_Generic_ReturnsService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterSingleton<ITestService>(sp => new TestService());
        IServiceProvider sp = provider;

        var result = sp.LodiGetRequiredService<ITestService>();

        Assert.NotNull(result);
    }

    [Fact]
    public void LodiGetRequiredService_GenericWithKey_ReturnsNamedService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterSingleton<ITestService>("keyA", sp => new TestService("A"));
        IServiceProvider sp = provider;

        var result = sp.LodiGetRequiredService<ITestService>("keyA");

        Assert.Equal("A", result.Name);
    }

    [Fact]
    public void LodiGetRequiredService_WithKey_ThrowsForNonLodiProvider()
    {
        IServiceProvider sp = new NonLodiProvider();

        Assert.Throws<InvalidOperationException>(() => sp.LodiGetRequiredService<ITestService>("keyA"));
    }

    [Fact]
    public void LodiGetService_Generic_ReturnsNullForNonLodiProvider()
    {
        IServiceProvider sp = new NonLodiProvider();

        var result = sp.LodiGetService<ITestService>();

        Assert.Null(result);
    }

    private class NonLodiProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) => null;
    }
}