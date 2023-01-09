using System.Reflection;
using System.Text.RegularExpressions;

namespace BooksRentalSystem.EventSourcing.Common;

public interface IEventsMappingProvider
{
    Type GetEventType(string eventName);
}

public sealed class EventsMappingProvider : IEventsMappingProvider
{
    private readonly Dictionary<string, Type> _typeMap;

    public EventsMappingProvider()
    {
        IEnumerable<Type> eventTypes =
            Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "BooksRentalSystem.*.Domain.dll")
                .Select(a => Assembly.Load(AssemblyName.GetAssemblyName(a)))
                .SelectMany(a => a.DefinedTypes
                    .Where(t => Regex.IsMatch(t.Name, @"\w*Event\b")))
                .Select(t => t.AsType());
        _typeMap = eventTypes.ToDictionary(t => t.Name.ToLower());
    }

    public Type GetEventType(string eventName)
    {
        string lowercasedEventName = eventName.ToLower();

        return !_typeMap.ContainsKey(lowercasedEventName) ? null : _typeMap[lowercasedEventName];
    }
}