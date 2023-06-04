using System;
using System.Collections.Generic;
using System.Linq;
using BooksRentalSystem.Common;
using BooksRentalSystem.Common.Services.Data;
using BooksRentalSystem.Common.Settings;
using BooksRentalSystem.Publishers.Data.Models;
using Microsoft.Extensions.Options;

namespace BooksRentalSystem.Publishers.Data.Seeding
{
    public class PublishersDataSeeder : IDataSeeder
    {
        private readonly ApplicationDbContext _db;
        private readonly ApplicationSettings _applicationSettings;

        public PublishersDataSeeder(ApplicationDbContext db, IOptions<ApplicationSettings> applicationSettings)
        {
            _db = db;
            _applicationSettings = applicationSettings.Value;
        }

        public void SeedData()
        {
            if (!_db.Categories.Any())
            {
                _db.Categories.AddRange(GetCategories());

                _db.SaveChanges();
            }

            if (_applicationSettings.SeedInitialData)
            {
                if (_db.Publishers.Any(p => p.UserId == DataSeederConstants.DefaultUserId))
                {
                    return;
                }

                var categories = _db.Categories.ToList();

                _db.Publishers.AddRange(GetPublishers(categories));

                _db.SaveChanges();
            }
        }

        private static IEnumerable<Category> GetCategories()
            => new List<Category>
            {
                new()
                {
                    Name = "Fantasy",
                    Description =
                        "Fantasy encompasses a huge part of the book world. It’s one of the most popular book genres out there—a personal favorite of mine to read and write."
                },
                new()
                {
                    Name = "Adventure",
                    Description =
                        "Writing a novel in the adventure category will require a trip, journey, or quest of some kind as the overall plot."
                },
                new()
                {
                    Name = "Romance",
                    Description =
                        "Romance authors have one specific goal when it comes to their books: to make you fall in love with the characters just as much as the characters fall in love with each other."
                },
                new()
                {
                    Name = "Contemporary",
                    Description =
                        "This book genre is among the most popular, though most writers aren’t sure of what this category even is."
                },
                new()
                {
                    Name = "Mystery",
                    Description =
                        "We’ve all heard of the mystery book genres. It’s an extremely popular genre, and for a good reason."
                },
                new()
                {
                    Name = "Horror",
                    Description =
                        "Horror novels are characterized by the fact that the main plot revolves around something scary and terrifying."
                }
            };

        private static IEnumerable<Publisher> GetPublishers(IList<Category> categories)
            => new List<Publisher>();
    }
}
