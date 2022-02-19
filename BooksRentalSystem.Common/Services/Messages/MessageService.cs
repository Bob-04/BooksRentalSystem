using System;
using System.Threading.Tasks;
using BooksRentalSystem.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace BooksRentalSystem.Common.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly MessageDbContext _dbContext;

        public MessageService(DbContext dbContext)
        {
            _dbContext = dbContext as MessageDbContext
                         ?? throw new InvalidOperationException(
                             $"Messages can only be used with a {nameof(MessageDbContext)}.");
        }

        public async Task<bool> IsDuplicated(object messageData, string propertyFilter, object identifier)
        {
            var messageType = messageData.GetType();

            return await _dbContext.Messages
                .FromSqlRaw(
                    $"SELECT * FROM Messages WHERE Type = '{messageType.AssemblyQualifiedName}' AND JSON_VALUE(serializedData, '$.{propertyFilter}') = {identifier}")
                .AnyAsync();
        }
    }
}
