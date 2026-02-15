namespace FrostYeti.Lodi.Tests;

public class ServiceLifetimeTests
{
    [Fact]
    public void Transient_HasCorrectName()
    {
        Assert.Equal("transient", ServiceLifetime.Transient.Name);
    }

    [Fact]
    public void Scoped_HasCorrectName()
    {
        Assert.Equal("scoped", ServiceLifetime.Scoped.Name);
    }

    [Fact]
    public void Singleton_HasCorrectName()
    {
        Assert.Equal("singleton", ServiceLifetime.Singleton.Name);
    }

    [Fact]
    public void DefaultConstructor_SetsTransient()
    {
        var lifetime = default(ServiceLifetime);

        Assert.Equal("transient", lifetime.Name);
    }

    [Fact]
    public void ConstructorWithString_SetsLowercaseName()
    {
        var lifetime = new ServiceLifetime("SCOPED");

        Assert.Equal("scoped", lifetime.Name);
    }

    [Fact]
    public void ImplicitOperatorFromString_CreatesLifetime()
    {
        ServiceLifetime lifetime = "singleton";

        Assert.Equal("singleton", lifetime.Name);
    }

    [Fact]
    public void ImplicitOperatorToString_ReturnsName()
    {
        ServiceLifetime lifetime = ServiceLifetime.Singleton;
        string name = lifetime;

        Assert.Equal("singleton", name);
    }

    [Fact]
    public void IsTransient_ReturnsTrueForTransient()
    {
        Assert.True(ServiceLifetime.IsTransient(ServiceLifetime.Transient));
    }

    [Fact]
    public void IsTransient_ReturnsFalseForScoped()
    {
        Assert.False(ServiceLifetime.IsTransient(ServiceLifetime.Scoped));
    }

    [Fact]
    public void IsScoped_ReturnsTrueForScoped()
    {
        Assert.True(ServiceLifetime.IsScoped(ServiceLifetime.Scoped));
    }

    [Fact]
    public void IsScoped_ReturnsFalseForSingleton()
    {
        Assert.False(ServiceLifetime.IsScoped(ServiceLifetime.Singleton));
    }

    [Fact]
    public void IsSingleton_ReturnsTrueForSingleton()
    {
        Assert.True(ServiceLifetime.IsSingleton(ServiceLifetime.Singleton));
    }

    [Fact]
    public void IsSingleton_ReturnsFalseForTransient()
    {
        Assert.False(ServiceLifetime.IsSingleton(ServiceLifetime.Transient));
    }

    [Fact]
    public void ToString_ReturnsName()
    {
        Assert.Equal("singleton", ServiceLifetime.Singleton.ToString());
    }
}