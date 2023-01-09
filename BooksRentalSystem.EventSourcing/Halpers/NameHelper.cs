using System.Text;

namespace BooksRentalSystem.EventSourcing.Halpers;

public static class NameHelper
{
    public static string GetAggregateName(string typeName)
    {
        IList<string> words = typeName
            .Replace("Aggregate", string.Empty)
            .SplitCapitalizedWords()
            .ToList();

        StringBuilder sb = new();

        int wordsCount = words.Count;
        for (int i = 0; i < wordsCount; i++)
            sb.Append(words[i]);

        return sb.ToString();
    }

    public static string GetAggregateName(Type type)
    {
        return GetAggregateName(type.Name);
    }

    public static void ThrowIfAggregateIdIsDefault(Guid id, Type type)
    {
        if (id == default)
            throw new Exception(
                $"Aggregate {GetAggregateName(type.Name)} {nameof(id).ToUpperFirstLetter()} could not be <default>."
            );
    }

    public static string GetStreamName<TAggregate>(TAggregate aggregate, Guid aggregateId)
    {
        return $"{GetAggregateName(aggregate.GetType())}-{aggregateId}";
    }

    public static string GetStreamName(string streamId)
    {
        return streamId.Split("-").First();
    }

    public static string GetPersistentSubscriptionStreamName(string typeName)
    {
        return $"$ce-{GetAggregateName(typeName)}";
    }

    public static string GetPersistentSubscriptionParkedStreamName(string typeName, string groupName)
    {
        return $"$persistentsubscription-{GetPersistentSubscriptionStreamName(typeName)}::{groupName}-parked";
    }

    private static IEnumerable<string> SplitCapitalizedWords(this string value)
    {
        StringBuilder sb = new();
        using (StringReader reader = new(value))
        {
            while (reader.Peek() != -1)
            {
                char c = (char)reader.Read();
                if (char.IsUpper(c) && sb.Length > 0)
                {
                    yield return sb.ToString();
                    sb.Length = 0;
                }

                sb.Append(c);
            }
        }

        if (sb.Length > 0)
            yield return sb.ToString();
    }

    private static string ToUpperFirstLetter(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return char.ToUpperInvariant(value[0]) + value.Substring(1).ToLowerInvariant();
    }
}