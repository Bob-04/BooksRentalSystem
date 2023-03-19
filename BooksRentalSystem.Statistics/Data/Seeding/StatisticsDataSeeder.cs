using System.Linq;
using BooksRentalSystem.Common.Services.Data;
using BooksRentalSystem.Common.Settings;
using Microsoft.Extensions.Options;

namespace BooksRentalSystem.Statistics.Data.Seeding
{
    public class StatisticsDataSeeder : IDataSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly ApplicationSettings _applicationSettings;

        public StatisticsDataSeeder(ApplicationDbContext db, IOptions<ApplicationSettings> applicationSettings)
        {
            _db = db;
            _applicationSettings = applicationSettings.Value;
        }

        public void SeedData()
        {
            if (!_db.Statistics.Any())
            {
                _db.Statistics.Add(new Data.Models.Statistics
                {
                    TotalBookAds = 0,
                    TotalRentedBooks = 0
                });

                _db.SaveChanges();
            }

            if (_applicationSettings.SeedInitialData)
            {
                _db.SaveChanges();
            }
        }
    }
}
