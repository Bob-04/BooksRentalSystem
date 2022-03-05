using System.Threading.Tasks;

namespace BooksRentalSystem.Statistics.Services.Messages
{
    public interface IMessageService
    {
        Task<bool> IsDuplicated(object messageData, string propertyFilter, object identifier);
    }
}
