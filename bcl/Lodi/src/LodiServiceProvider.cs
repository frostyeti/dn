using System.Collections.Concurrent;

namespace FrostYeti.Lodi;

/// <summary>
/// A lightweight dependency injection service provider.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// var provider = new LodiServiceProvider();
/// provider.RegisterTransient&lt;IMyService&gt;(sp => new MyService());
/// var service = provider.GetService&lt;IMyService&gt;();
/// </code>
/// </example>
/// </remarks>
public class LodiServiceProvider : IServiceProvider, IDisposable
{
    private readonly ConcurrentBag<LodiDependency> dependencies = new();

    private readonly ConcurrentDictionary<Type, List<LodiDependency>> typeMap = new();

    private readonly ConcurrentDictionary<string, List<LodiDependency>> nameMap = new();

    private readonly ScopedLifetime scopedLifetime = new();

    private bool disposedValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="LodiServiceProvider"/> class.
    /// </summary>
    /// <param name="parent">The parent service provider, if any.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var provider = new LodiServiceProvider();
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider(LodiServiceProvider? parent = null)
    {
        if (parent is null)
        {
            this.IsRoot = true;
            this.RegisterSingleton(typeof(IServiceProviderLifetimeFactory), new LodiServiceProviderLifetimeFactory(this));
            return;
        }

        this.dependencies = parent.dependencies;
        this.typeMap = parent.typeMap;
        this.nameMap = parent.nameMap;
        this.IsRoot = false;

        var descriptor = this.dependencies.FirstOrDefault(o => o.ServiceType == typeof(IServiceProviderLifetimeFactory));
        if (descriptor is null)
        {
            this.RegisterSingleton(typeof(IServiceProviderLifetimeFactory), new LodiServiceProviderLifetimeFactory(this));
            return;
        }

        descriptor.Service = new LodiServiceProviderLifetimeFactory(this);
        descriptor.Factory = _ => descriptor.Service!;
    }

    /// <summary>
    /// Gets a value indicating whether this is the root service provider.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var provider = new LodiServiceProvider();
    /// bool isRoot = provider.IsRoot;
    /// </code>
    /// </example>
    /// </remarks>
    protected bool IsRoot { get; }

    /// <summary>
    /// Registers a dependency.
    /// </summary>
    /// <param name="dependency">The dependency to register.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var dependency = new LodiDependency(typeof(IMyService), ServiceLifetime.Transient, sp => new MyService());
    /// provider.Register(dependency);
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider Register(LodiDependency dependency)
    {
        this.dependencies.Add(dependency);

        if (dependency.Name is not null)
        {
            if (!this.nameMap.TryGetValue(dependency.Name, out var list))
                list = new List<LodiDependency>();

            list.Add(dependency);
            this.nameMap[dependency.Name] = list;
        }
        else
        {
            if (!this.typeMap.TryGetValue(dependency.ServiceType, out var list))
                list = new List<LodiDependency>();

            list.Add(dependency);
            this.typeMap[dependency.ServiceType] = list;
        }

        return this;
    }

    /// <summary>
    /// Registers a transient service.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterTransient(typeof(IMyService), sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterTransient(Type type, Func<LodiServiceProvider, object> factory)
    {
        var dependency = new LodiDependency(type, ServiceLifetime.Transient, factory);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a transient service.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterTransient&lt;IMyService&gt;(sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterTransient<T>(Func<LodiServiceProvider, T> factory)
    {
        var dependency = new LodiDependency(typeof(T), ServiceLifetime.Transient, sp =>
        {
            var instance = factory(sp);
            if (instance is null)
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
            return instance!;
        });
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a transient service.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImpl">The implementation type.</typeparam>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var provider = new LodiServiceProvider();
    /// provider.RegisterTransient&lt;IMyService, MyService&gt;(sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterTransient<TService, TImpl>(Func<LodiServiceProvider, TImpl> factory)
        where TImpl : TService
    {
        return this.RegisterTransient<TService>(sp => (TService)factory(sp)!);
    }

    /// <summary>
    /// Registers a transient service with a name.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="name">The name of the service.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterTransient&lt;IMyService&gt;("MyName", sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterTransient<T>(string name, Func<LodiServiceProvider, T> factory)
    {
        var dependency = new LodiDependency(typeof(T), name, ServiceLifetime.Transient, sp =>
        {
            var instance = factory(sp);
            if (instance is null)
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
            return instance!;
        });
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a transient service with a key.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="key">The key for the service.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterTransient(typeof(IMyService), "MyKey", sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterTransient(Type type, string key, Func<LodiServiceProvider, object> factory)
    {
        var dependency = new LodiDependency(type, key, ServiceLifetime.Transient, factory);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a scoped service.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterScoped(typeof(IMyService), sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterScoped(Type type, Func<LodiServiceProvider, object> factory)
    {
        var dependency = new LodiDependency(type, ServiceLifetime.Scoped, factory);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a scoped service.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterScoped&lt;IMyService&gt;(sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterScoped<T>(Func<LodiServiceProvider, T> factory)
    {
        var dependency = new LodiDependency(typeof(T), ServiceLifetime.Scoped, sp =>
        {
            var instance = factory(sp);
            if (instance is null)
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
            return instance!;
        });
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a scoped service.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImpl">The implementation type.</typeparam>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var provider = new LodiServiceProvider();
    /// provider.RegisterScoped&lt;IMyService, MyService&gt;(sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterScoped<TService, TImpl>(Func<LodiServiceProvider, TImpl> factory)
        where TImpl : TService
    {
        return this.RegisterScoped<TService>(sp => (TService)factory(sp)!);
    }

    /// <summary>
    /// Registers a scoped service with a name.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="name">The name of the service.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterScoped&lt;IMyService&gt;("MyName", sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterScoped<T>(string name, Func<LodiServiceProvider, T> factory)
    {
        var dependency = new LodiDependency(typeof(T), name, ServiceLifetime.Scoped, sp =>
        {
            var instance = factory(sp);
            if (instance is null)
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
            return instance!;
        });
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a scoped service with a key.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="key">The key for the service.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterScoped(typeof(IMyService), "MyKey", sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterScoped(Type type, string key, Func<LodiServiceProvider, object> factory)
    {
        var dependency = new LodiDependency(type, key, ServiceLifetime.Scoped, factory);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton service.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingleton(typeof(IMyService), sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingleton(Type type, Func<LodiServiceProvider, object> factory)
    {
        var dependency = new LodiDependency(type, ServiceLifetime.Singleton, factory);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton service with a key.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="key">The key for the service.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingleton(typeof(IMyService), "MyKey", sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingleton(Type type, string key, Func<LodiServiceProvider, object> factory)
    {
        var dependency = new LodiDependency(type, key, ServiceLifetime.Singleton, factory);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton service.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingleton&lt;IMyService&gt;(sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingleton<T>(Func<LodiServiceProvider, T> factory)
    {
        var dependency = new LodiDependency(typeof(T), ServiceLifetime.Singleton, sp =>
        {
            var instance = factory(sp);
            if (instance is null)
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
            return instance!;
        });
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton service.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <typeparam name="TInstance">The instance type.</typeparam>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingleton&lt;IMyService, MyService&gt;(sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingleton<T, TInstance>(Func<LodiServiceProvider, T> factory)
        where TInstance : T
    {
        var dependency = new LodiDependency(typeof(T), ServiceLifetime.Singleton, sp =>
        {
            var instance = factory(sp);
            if (instance is null)
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
            return instance!;
        });
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton service with a key.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="key">The key for the service.</param>
    /// <param name="factory">The factory function.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingleton&lt;IMyService&gt;("MyKey", sp => new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingleton<T>(string key, Func<LodiServiceProvider, T> factory)
    {
        var dependency = new LodiDependency(typeof(T), key, ServiceLifetime.Singleton, sp =>
        {
            var instance = factory(sp);
            if (instance is null)
                throw new InvalidOperationException($"Factory for type {typeof(T).FullName} returned null.");
            return instance!;
        });
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton instance.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="instance">The service instance.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingleton(typeof(IMyService), new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingleton(Type type, object instance)
    {
        var dependency = new LodiDependency(type, ServiceLifetime.Singleton, instance);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton instance with a key.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <param name="key">The key for the service.</param>
    /// <param name="instance">The service instance.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingleton(typeof(IMyService), "MyKey", new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingleton(Type type, string key, object instance)
    {
        var dependency = new LodiDependency(type, key, ServiceLifetime.Singleton, instance);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton instance.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="instance">The service instance.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingletonInstance&lt;IMyService&gt;(new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingletonInstance<T>(T instance)
    {
        var dependency = new LodiDependency(typeof(T), ServiceLifetime.Singleton, instance!);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Registers a singleton instance with a key.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <param name="key">The key for the service.</param>
    /// <param name="instance">The service instance.</param>
    /// <returns>The service provider instance for chaining.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.RegisterSingletonInstance&lt;IMyService&gt;("MyKey", new MyService());
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProvider RegisterSingletonInstance<T>(string key, T instance)
    {
        var dependency = new LodiDependency(typeof(T), key, ServiceLifetime.Singleton, instance!);
        this.Register(dependency);
        return this;
    }

    /// <summary>
    /// Checks if a service of the specified type is registered.
    /// </summary>
    /// <typeparam name="T">The type of service to check.</typeparam>
    /// <returns>True if the service is registered; otherwise, false.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var provider = new LodiServiceProvider();
    /// bool isRegistered = provider.ContainsService<IServiceProvider>();
    /// </code>
    /// </example>
    /// </remarks>
    public bool ContainsService<T>()
    {
        return this.typeMap.ContainsKey(typeof(T));
    }

    /// <summary>
    /// Checks if a service with the specified key is registered.
    /// </summary>
    /// <param name="key">The key of the service to check.</param>
    /// <returns>True if the service is registered; otherwise, false.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var provider = new LodiServiceProvider();
    /// bool isRegistered = provider.ContainsService("MyKey");
    /// </code>
    /// </example>
    /// </remarks>
    public bool ContainsService(string key)
    {
        return this.nameMap.ContainsKey(key);
    }

    /// <summary>
    /// Gets a service of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of service to get.</param>
    /// <returns>A service object of the specified type, or null if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var service = provider.GetService(typeof(IMyService));
    /// </code>
    /// </example>
    /// </remarks>
    public object? GetService(Type serviceType)
    {
        if (this.typeMap.TryGetValue(serviceType, out var list))
        {
            if (list.Count == 0)
                return null;

            var dependency = list[0];

            if (ServiceLifetime.IsSingleton(dependency.Lifetime))
            {
                if (dependency.Service is null)
                {
                    var instance = dependency.Factory(this);
                    dependency.Service = instance;

                    if (instance is null)
                    {
                        throw new InvalidOperationException($"Factory for type {dependency.ServiceType.FullName} returned null.");
                    }
                }
                else
                {
                }

                return dependency.Service;
            }
            else if (ServiceLifetime.IsScoped(dependency.Lifetime))
            {
                var instance = this.scopedLifetime.GetState(serviceType);
                if (instance is null)
                {
                    instance = dependency.Factory(this);
                    this.scopedLifetime.SetState(serviceType, dependency.Name, instance!);
                }

                return instance;
            }
            else
            {
                return dependency.Factory(this);
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a service with the specified key.
    /// </summary>
    /// <param name="key">The key of the service to get.</param>
    /// <returns>A service object with the specified key, or null if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var service = provider.GetService("MyKey");
    /// </code>
    /// </example>
    /// </remarks>
    public object? GetService(string key)
    {
        if (this.nameMap.TryGetValue(key, out var list))
        {
            if (list.Count == 0)
                return null;

            var dependency = list[0];

            if (ServiceLifetime.IsSingleton(dependency.Lifetime))
            {
                if (dependency.Service is null)
                    dependency.Service = dependency.Factory(this);

                return dependency.Service;
            }
            else if (ServiceLifetime.IsScoped(dependency.Lifetime))
            {
                var instance = this.scopedLifetime.GetState(key);
                if (instance is null)
                {
                    instance = dependency.Factory(this);
                    this.scopedLifetime.SetState(dependency.ServiceType, key, instance!);
                }

                return instance;
            }
            else
            {
                return dependency.Factory(this);
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a service of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>A service object of the specified type, or null if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var service = provider.GetService&lt;IMyService&gt;();
    /// </code>
    /// </example>
    /// </remarks>
    public T? GetService<T>()
    {
        var service = this.GetService(typeof(T));
        if (service is null)
            return default;

        return (T)service;
    }

    /// <summary>
    /// Gets a service with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <param name="key">The key of the service to get.</param>
    /// <returns>A service object with the specified key, or null if not found.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var service = provider.GetService&lt;IMyService&gt;("MyKey");
    /// </code>
    /// </example>
    /// </remarks>
    public T? GetService<T>(string key)
    {
        var service = this.GetService(key);
        if (service is null)
            return default;

        return (T)service;
    }

    /// <summary>
    /// Gets a service of the specified type, throwing an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <returns>A service object of the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var service = provider.GetRequiredService&lt;IMyService&gt;();
    /// </code>
    /// </example>
    /// </remarks>
    public T GetRequiredService<T>()
    {
        var service = this.GetService(typeof(T));
        if (service is null)
            throw new InvalidOperationException($"Service of type {typeof(T).FullName} is not registered.");

        return (T)service;
    }

    /// <summary>
    /// Gets a service with the specified key, throwing an exception if not found.
    /// </summary>
    /// <typeparam name="T">The type of service to get.</typeparam>
    /// <param name="key">The key of the service to get.</param>
    /// <returns>A service object with the specified key.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the service is not registered.</exception>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var service = provider.GetRequiredService&lt;IMyService&gt;("MyKey");
    /// </code>
    /// </example>
    /// </remarks>
    public T GetRequiredService<T>(string key)
    {
        var service = this.GetService(key);
        if (service is null)
            throw new InvalidOperationException($"Service with key '{key}' is not registered.");

        return (T)service;
    }

    /// <summary>
    /// Creates a new scope.
    /// </summary>
    /// <returns>A new service provider representing the scope.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// using var scope = provider.CreateScope();
    /// </code>
    /// </example>
    /// </remarks>
    public IServiceProvider CreateScope()
    {
        return new LodiServiceProvider(this);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// provider.Dispose();
    /// </code>
    /// </example>
    /// </remarks>
    public void Dispose()
    {
        if (this.disposedValue)
            return;

        this.disposedValue = true;

        foreach (var disposable in this.scopedLifetime.GetDisposables())
        {
            disposable.Dispose();
        }

        if (this.IsRoot)
        {
            foreach (var dependency in this.dependencies)
            {
                if (dependency.Service is IDisposable disposable)
                {
                    disposable.Dispose();
                    continue;
                }

                if (dependency.Service is IAsyncDisposable asyncDisposable)
                {
                    asyncDisposable.DisposeAsync();
                }
            }
        }

        this.scopedLifetime.Clear();
        this.typeMap.Clear();
        this.nameMap.Clear();
        this.dependencies.Clear();
    }
}

/// <summary>
/// A factory for creating service provider lifetimes.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// // This is internal, used by LodiServiceProvider
/// </code>
/// </example>
/// </remarks>
internal sealed class LodiServiceProviderLifetimeFactory : IServiceProviderLifetimeFactory
{
    private readonly LodiServiceProvider serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LodiServiceProviderLifetimeFactory"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// // Internal constructor
    /// </code>
    /// </example>
    /// </remarks>
    public LodiServiceProviderLifetimeFactory(LodiServiceProvider serviceProvider)
        => this.serviceProvider = serviceProvider;

    /// <summary>
    /// Creates a new service provider lifetime.
    /// </summary>
    /// <returns>A new service provider lifetime.</returns>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// // Called by infrastructure
    /// </code>
    /// </example>
    /// </remarks>
    public IServiceProviderLifetime CreateLifetime()
    {
        return new LodiScopedServiceLifetime(this.serviceProvider);
    }
}

/// <summary>
/// A scoped service lifetime implementation for Lodi.
/// </summary>
/// <remarks>
/// <example>
/// <code lang="csharp">
/// // Internal class managing scoped lifetime
/// </code>
/// </example>
/// </remarks>
internal sealed class LodiScopedServiceLifetime : IServiceProviderLifetime
{
    private readonly LodiServiceProvider provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="LodiScopedServiceLifetime"/> class.
    /// </summary>
    /// <param name="provider">The parent service provider.</param>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// // Internal constructor
    /// </code>
    /// </example>
    /// </remarks>
    public LodiScopedServiceLifetime(LodiServiceProvider provider)
    {
        this.provider = (LodiServiceProvider)provider.CreateScope();
    }

    /// <summary>
    /// Gets the service provider for this lifetime.
    /// </summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// var sp = lifetime.ServiceProvider;
    /// </code>
    /// </example>
    /// </remarks>
    public IServiceProvider ServiceProvider => this.provider;

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    /// <remarks>
    /// <example>
    /// <code lang="csharp">
    /// lifetime.Dispose();
    /// </code>
    /// </example>
    /// </remarks>
    public void Dispose()
    {
        this.provider.Dispose();
    }
}