namespace BooksRentalSystem.EventSourcing.Common;

public delegate TService ServiceResolver<out TService>(string key);