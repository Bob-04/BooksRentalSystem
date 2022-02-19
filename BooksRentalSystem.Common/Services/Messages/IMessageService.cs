using System.Threading.Tasks;

namespace BooksRentalSystem.Common.Services.Messages
{
    public interface IMessageService
    {
        Task<bool> IsDuplicated(object messageData, string propertyFilter, object identifier);
    }
}
