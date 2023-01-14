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
            => new List<Publisher>
            {
                new()
                {
                    Name = "Cool Books",
                    PhoneNumber = "+380682851258",
                    UserId = DataSeederConstants.DefaultUserId,
                    BookAds = new List<BookAd>
                    {
                        new()
                        {
                            Title = "In Search of Lost Time",
                            Author = new Author
                            {
                                Name = "Marcel Proust",
                                Description =
                                    "Valentin Louis Georges Eugène Marcel Proust (French; 10 July 1871 – 18 November 1922), known as Marcel Proust, was a French novelist, critic, and essayist best known for his monumental novel À la recherche du temps perdu (In Search of Lost Time; earlier rendered as Remembrance of Things Past), published in seven parts between 1913 and 1927. He is considered by critics and writers to be one of the most influential authors of the 20th century.",
                                Country = "French",
                                BirthDate = new DateTime(1871, 07, 10)
                            },
                            IsAvailable = true,
                            PricePerDay = 4,
                            BookInfo = new BookInfo
                            {
                                PagesNumber = 468,
                                Language = "English",
                                PublicationDate = new DateTime(2004, 04, 30),
                                CoverType = CoverType.HardCover
                            },
                            Category = categories.FirstOrDefault(c => c.Name == "Contemporary")
                        },
                        new()
                        {
                            Title = "Don Quixote",
                            Author = new Author
                            {
                                Name = "Miguel de Cervantes",
                                Description =
                                    "Miguel de Cervantes Saavedra (Spanish; 29 September 1547 (assumed) – 22 April 1616 NS) was a Spanish writer who is widely regarded as the greatest writer in the Spanish language and one of the world's pre-eminent novelists. His novel Don Quixote has been translated into over 140 languages and dialects; it is, after the Bible, the most-translated book in the world.",
                                Country = "Spanish",
                                BirthDate = new DateTime(1547, 09, 29)
                            },
                            Description =
                                "Alonso Quixano, a retired country gentleman in his fifties, lives in an unnamed section of La Mancha with his niece and a housekeeper. He has become obsessed with books of chivalry, and believes their every word to be true, despite the fact that many of the events in them are clearly impossible. Quixano eventually appears to other people to have lost his mind from little sleep and food and because of so much reading.",
                            IsAvailable = true,
                            PricePerDay = 7,
                            BookInfo = new BookInfo
                            {
                                PagesNumber = 1072,
                                Language = "English",
                                PublicationDate = new DateTime(2003, 02, 25),
                                CoverType = CoverType.PaperCover
                            },
                            Category = categories.FirstOrDefault(c => c.Name == "Adventure")
                        }
                    }
                }
            };
    }
}
