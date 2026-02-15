namespace FrostYeti.Lodi.Tests;

public class LodiServiceProviderTests
{
    [Fact]
    public void RegisterTransient_ReturnsNewInstanceEachTime()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient<ITestService>(sp => new TestService());

        var instance1 = provider.GetService<ITestService>();
        var instance2 = provider.GetService<ITestService>();

        Assert.NotSame(instance1, instance2);
    }

    [Fact]
    public void RegisterSingleton_ReturnsSameInstanceEachTime()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterSingleton<ITestService>(sp => new TestService());

        var instance1 = provider.GetService<ITestService>();
        var instance2 = provider.GetService<ITestService>();

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void RegisterSingletonInstance_ReturnsProvidedInstance()
    {
        var provider = new LodiServiceProvider();
        var instance = new TestService();
        provider.RegisterSingletonInstance<ITestService>(instance);

        var result = provider.GetService<ITestService>();

        Assert.Same(instance, result);
    }

    [Fact]
    public void RegisterScoped_ReturnsSameInstanceWithinScope()
    {
        var rootProvider = new LodiServiceProvider();
        rootProvider.RegisterScoped<ITestService>(sp => new TestService());

        var scopedProvider = (LodiServiceProvider)rootProvider.CreateScope();

        var instance1 = scopedProvider.GetService<ITestService>();
        var instance2 = scopedProvider.GetService<ITestService>();

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void RegisterScoped_ReturnsDifferentInstancesAcrossScopes()
    {
        var rootProvider = new LodiServiceProvider();
        rootProvider.RegisterScoped<ITestService>(sp => new TestService());

        var scope1 = (LodiServiceProvider)rootProvider.CreateScope();
        var scope2 = (LodiServiceProvider)rootProvider.CreateScope();

        var instance1 = scope1.GetService<ITestService>();
        var instance2 = scope2.GetService<ITestService>();

        Assert.NotSame(instance1, instance2);
    }

    [Fact]
    public void RegisterSingleton_IsSharedAcrossScopes()
    {
        var rootProvider = new LodiServiceProvider();
        rootProvider.RegisterSingleton<ITestService>(sp => new TestService());

        var scope1 = (LodiServiceProvider)rootProvider.CreateScope();
        var scope2 = (LodiServiceProvider)rootProvider.CreateScope();

        var instance1 = scope1.GetService<ITestService>();
        var instance2 = scope2.GetService<ITestService>();

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void GetService_ReturnsNullWhenNotRegistered()
    {
        var provider = new LodiServiceProvider();

        var result = provider.GetService<ITestService>();

        Assert.Null(result);
    }

    [Fact]
    public void GetRequiredService_ThrowsWhenNotRegistered()
    {
        var provider = new LodiServiceProvider();

        var ex = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<ITestService>());
        Assert.Contains("ITestService", ex.Message);
    }

    [Fact]
    public void RegisterTransient_WithKey_ReturnsNamedService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient<ITestService>("serviceA", sp => new TestService("A"));
        provider.RegisterTransient<ITestService>("serviceB", sp => new TestService("B"));

        var resultA = provider.GetService<ITestService>("serviceA");
        var resultB = provider.GetService<ITestService>("serviceB");

        Assert.Equal("A", resultA?.Name);
        Assert.Equal("B", resultB?.Name);
    }

    [Fact]
    public void RegisterSingleton_WithKey_ReturnsSameNamedInstance()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterSingleton<ITestService>("serviceA", sp => new TestService("A"));

        var instance1 = provider.GetService<ITestService>("serviceA");
        var instance2 = provider.GetService<ITestService>("serviceA");

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void RegisterScoped_WithKey_ReturnsSameInstanceWithinScope()
    {
        var rootProvider = new LodiServiceProvider();
        rootProvider.RegisterScoped<ITestService>("serviceA", sp => new TestService("A"));

        var scope = (LodiServiceProvider)rootProvider.CreateScope();

        var instance1 = scope.GetService<ITestService>("serviceA");
        var instance2 = scope.GetService<ITestService>("serviceA");

        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void Register_ReturnsProviderForChaining()
    {
        var provider = new LodiServiceProvider();

        var result = provider
            .RegisterTransient<ITestService>(sp => new TestService())
            .RegisterSingleton<ITestService2>(sp => new TestService2());

        Assert.Same(provider, result);
    }

    [Fact]
    public void CreateScope_ReturnsNewServiceProvider()
    {
        var provider = new LodiServiceProvider();

        var scope = provider.CreateScope();

        Assert.NotSame(provider, scope);
        Assert.IsType<LodiServiceProvider>(scope);
    }

    [Fact]
    public void Dispose_DisposesSingletonServices()
    {
        var provider = new LodiServiceProvider();
        var disposable = new DisposableTestService();
        provider.RegisterSingletonInstance<IDisposableTestService>(disposable);

        provider.Dispose();

        Assert.True(disposable.WasDisposed);
    }

    [Fact]
    public void Dispose_DisposesScopedServices()
    {
        var rootProvider = new LodiServiceProvider();
        rootProvider.RegisterScoped<IDisposableTestService>(sp => new DisposableTestService());

        var scope = (LodiServiceProvider)rootProvider.CreateScope();
        var service = scope.GetService<IDisposableTestService>();
        var disposable = (DisposableTestService)service!;

        scope.Dispose();

        Assert.True(disposable.WasDisposed);
    }

    [Fact]
    public void Dispose_RootProviderDisposesSingletonServices()
    {
        var rootProvider = new LodiServiceProvider();
        rootProvider.RegisterSingleton<IDisposableTestService>(sp => new DisposableTestService());

        var singleton = (DisposableTestService?)rootProvider.GetService<IDisposableTestService>();
        Assert.NotNull(singleton);

        rootProvider.Dispose();

        Assert.True(singleton.WasDisposed);
    }

    [Fact]
    public void Factory_ReceivesServiceProvider()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterSingleton<ITestService>(sp => new TestService());
        provider.RegisterTransient<ITestService2>(sp =>
        {
            var service = sp.GetRequiredService<ITestService>();
            return new TestService2(service);
        });

        var result = provider.GetService<ITestService2>();
        var testService2 = Assert.IsType<TestService2>(result);

        Assert.NotNull(testService2.Dependency);
    }

    [Fact]
    public void GetService_ByType_ReturnsRegisteredService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient(typeof(ITestService), sp => new TestService());

        var result = provider.GetService(typeof(ITestService));

        Assert.NotNull(result);
        Assert.IsType<TestService>(result);
    }

    [Fact]
    public void GetService_ByKey_ReturnsNamedService()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient(typeof(ITestService), "keyA", sp => new TestService("A"));

        var result = provider.GetService("keyA");

        Assert.NotNull(result);
        Assert.IsType<TestService>(result);
    }

    [Fact]
    public void RegisterSingleton_TypeAndInstance_SetsService()
    {
        var provider = new LodiServiceProvider();
        var instance = new TestService();
        provider.RegisterSingleton(typeof(ITestService), instance);

        var result = provider.GetService(typeof(ITestService));

        Assert.Same(instance, result);
    }

    [Fact]
    public void GetRequiredService_WithKey_ThrowsWhenKeyNotFound()
    {
        var provider = new LodiServiceProvider();

        var ex = Assert.Throws<InvalidOperationException>(() => provider.GetRequiredService<ITestService>("missing"));
        Assert.Contains("missing", ex.Message);
    }

    [Fact]
    public void RegisterTransient_GenericFactory_ReturnsNonNull()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterTransient<ITestService>(sp => new TestService());

        var result = provider.GetRequiredService<ITestService>();

        Assert.NotNull(result);
    }

    [Fact]
    public void RegisterScoped_GenericFactory_ReturnsInstance()
    {
        var provider = new LodiServiceProvider();
        provider.RegisterScoped<ITestService>(sp => new TestService());

        var scope = (LodiServiceProvider)provider.CreateScope();
        var result = scope.GetRequiredService<ITestService>();

        Assert.NotNull(result);
    }
}

public interface ITestService
{
    string? Name { get; }
}

public class TestService : ITestService
{
    public string? Name { get; }

    public TestService(string? name = null)
    {
        this.Name = name;
    }
}

public interface ITestService2
{
    ITestService? Dependency { get; }
}

public class TestService2 : ITestService2
{
    public ITestService? Dependency { get; }

    public TestService2(ITestService? dependency = null)
    {
        this.Dependency = dependency;
    }
}

public interface IDisposableTestService : IDisposable
{
    bool WasDisposed { get; }
}

public class DisposableTestService : IDisposableTestService
{
    public bool WasDisposed { get; private set; }

    public void Dispose()
    {
        this.WasDisposed = true;
    }
}