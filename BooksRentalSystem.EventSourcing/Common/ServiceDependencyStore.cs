namespace BooksRentalSystem.EventSourcing.Common;

public sealed class ServiceDependencyStore<T>
{
    private readonly Dictionary<string, T> _store;

    public ServiceDependencyStore()
    {
        _store = new Dictionary<string, T>();
    }

    public void AddServiceDependency(string key, T service)
    {
        _store[key] = service;
    }

    public T GetRequiredServiceDependency(string key)
    {
        return _store[key];
    }

    public T? GetOptionalServiceDependency(string key)
    {
        _store.TryGetValue(key, out var value);
        return value;
    }

    public IEnumerable<T> GetServiceDependencies()
    {
        return _store.Values.ToList();
    }
}