using CineSocial.Core.Shared.Interfaces;
using CineSocial.Domain.Entities;
using CineSocial.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CineSocial.Infrastructure.Data.Seeds;

public static class DataSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CineSocialDbContext>>();

        try
        {
            await SeedGenresAsync(unitOfWork);
            await SeedPersonsAsync(unitOfWork);
            await SeedMoviesAsync(unitOfWork);
            await SeedUsersAsync(unitOfWork, passwordHasher);
            await SeedMovieCastAndCrewAsync(unitOfWork);
            await SeedMovieGenresAsync(unitOfWork);
            await SeedRatingsAsync(unitOfWork);
            await SeedReviewsAsync(unitOfWork);
            await SeedCommentsAsync(unitOfWork);
            await SeedReactionsAsync(unitOfWork);
            await SeedFavoritesAsync(unitOfWork);
            await SeedMovieListsAsync(unitOfWork);
            await SeedListFavoritesAsync(unitOfWork);

            await unitOfWork.SaveChangesAsync();
            logger.LogInformation("Data seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding data.");
            throw;
        }
    }

    private static async Task SeedGenresAsync(IUnitOfWork unitOfWork)
    {
        if (await unitOfWork.Genres.AnyAsync(g => true))
            return;

        var genres = new[]
        {
            new Genre { Name = "Action", Description = "Action movies with thrilling sequences" },
            new Genre { Name = "Drama", Description = "Dramatic storytelling and character development" },
            new Genre { Name = "Comedy", Description = "Humorous and entertaining films" },
            new Genre { Name = "Thriller", Description = "Suspenseful and exciting movies" },
            new Genre { Name = "Romance", Description = "Love stories and romantic relationships" },
            new Genre { Name = "Sci-Fi", Description = "Science fiction and futuristic themes" },
            new Genre { Name = "Horror", Description = "Scary and frightening movies" },
            new Genre { Name = "Adventure", Description = "Exciting journeys and quests" },
            new Genre { Name = "Crime", Description = "Criminal activities and investigations" },
            new Genre { Name = "Fantasy", Description = "Magical and fantastical elements" }
        };

        foreach (var genre in genres)
        {
            await unitOfWork.Genres.AddAsync(genre);
        }
    }

    private static async Task SeedPersonsAsync(IUnitOfWork unitOfWork)
    {
        if (await unitOfWork.Persons.AnyAsync(p => true))
            return;

        var persons = new[]
        {
            new Person 
            { 
                Name = "Leonardo DiCaprio", 
                Biography = "Academy Award-winning actor known for his versatile performances.",
                Birthday = new DateTime(1974, 11, 11),
                PlaceOfBirth = "Los Angeles, California, USA",
                KnownForDepartment = "Acting",
                Gender = "Male",
                Popularity = 85.5m
            },
            new Person 
            { 
                Name = "Margot Robbie", 
                Biography = "Australian actress and producer known for her dynamic roles.",
                Birthday = new DateTime(1990, 7, 2),
                PlaceOfBirth = "Dalby, Queensland, Australia",
                KnownForDepartment = "Acting",
                Gender = "Female",
                Popularity = 78.3m
            },
            new Person 
            { 
                Name = "Christopher Nolan", 
                Biography = "British-American film director known for his complex narratives.",
                Birthday = new DateTime(1970, 7, 30),
                PlaceOfBirth = "London, England, UK",
                KnownForDepartment = "Directing",
                Gender = "Male",
                Popularity = 92.1m
            },
            new Person 
            { 
                Name = "Ryan Gosling", 
                Biography = "Canadian actor known for his diverse film roles.",
                Birthday = new DateTime(1980, 11, 12),
                PlaceOfBirth = "London, Ontario, Canada",
                KnownForDepartment = "Acting",
                Gender = "Male",
                Popularity = 73.8m
            },
            new Person 
            { 
                Name = "Greta Gerwig", 
                Biography = "American actress, writer, and director.",
                Birthday = new DateTime(1983, 8, 4),
                PlaceOfBirth = "Sacramento, California, USA",
                KnownForDepartment = "Directing",
                Gender = "Female",
                Popularity = 68.9m
            },
            new Person 
            { 
                Name = "Cillian Murphy", 
                Biography = "Irish actor known for his intense performances.",
                Birthday = new DateTime(1976, 5, 25),
                PlaceOfBirth = "Douglas, Cork, Ireland",
                KnownForDepartment = "Acting",
                Gender = "Male",
                Popularity = 71.2m
            }
        };

        foreach (var person in persons)
        {
            await unitOfWork.Persons.AddAsync(person);
        }
    }

    private static async Task SeedMoviesAsync(IUnitOfWork unitOfWork)
    {
        if (await unitOfWork.Movies.AnyAsync(m => true))
            return;

        var movies = new[]
        {
            new Movie
            {
                Title = "Inception",
                OriginalTitle = "Inception",
                Overview = "A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                ReleaseDate = new DateTime(2010, 7, 16),
                Runtime = 148,
                VoteAverage = 8.8m,
                VoteCount = 32000,
                Language = "en",
                Popularity = 95.3m,
                Status = "Released",
                Budget = 160000000,
                Revenue = 836848102,
                Tagline = "Your mind is the scene of the crime."
            },
            new Movie
            {
                Title = "Barbie",
                OriginalTitle = "Barbie",
                Overview = "To live in Barbie Land is to be a perfect being in a perfect place. Unless you have a full-on existential crisis.",
                ReleaseDate = new DateTime(2023, 7, 21),
                Runtime = 114,
                VoteAverage = 7.1m,
                VoteCount = 8500,
                Language = "en",
                Popularity = 88.7m,
                Status = "Released",
                Budget = 145000000,
                Revenue = 1446000000,
                Tagline = "She's everything. He's just Ken."
            },
            new Movie
            {
                Title = "Oppenheimer",
                OriginalTitle = "Oppenheimer",
                Overview = "The story of J. Robert Oppenheimer's role in the development of the atomic bomb during World War II.",
                ReleaseDate = new DateTime(2023, 7, 21),
                Runtime = 180,
                VoteAverage = 8.3m,
                VoteCount = 6200,
                Language = "en",
                Popularity = 91.2m,
                Status = "Released",
                Budget = 100000000,
                Revenue = 952000000,
                Tagline = "The world forever changes."
            },
            new Movie
            {
                Title = "The Wolf of Wall Street",
                OriginalTitle = "The Wolf of Wall Street",
                Overview = "Based on the true story of Jordan Belfort, from his rise to a wealthy stock-broker living the high life to his fall involving crime, corruption and the federal government.",
                ReleaseDate = new DateTime(2013, 12, 25),
                Runtime = 180,
                VoteAverage = 8.2m,
                VoteCount = 18500,
                Language = "en",
                Popularity = 82.4m,
                Status = "Released",
                Budget = 100000000,
                Revenue = 392000000,
                Tagline = "More is never enough."
            },
            new Movie
            {
                Title = "La La Land",
                OriginalTitle = "La La Land",
                Overview = "A jazz musician and an aspiring actress meet and fall in love in Los Angeles while pursuing their dreams.",
                ReleaseDate = new DateTime(2016, 12, 9),
                Runtime = 128,
                VoteAverage = 7.9m,
                VoteCount = 15200,
                Language = "en",
                Popularity = 76.8m,
                Status = "Released",
                Budget = 30000000,
                Revenue = 449000000,
                Tagline = "Here's to the ones who dream."
            }
        };

        foreach (var movie in movies)
        {
            await unitOfWork.Movies.AddAsync(movie);
        }
    }

    private static async Task SeedUsersAsync(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        if (await unitOfWork.Users.AnyAsync(u => true))
            return;

        var users = new[]
        {
            new User
            {
                Username = "cinemafan01",
                Email = "cinemafan01@example.com",
                PasswordHash = passwordHasher.HashPassword("Password123!"),
                FirstName = "Ahmet",
                LastName = "Yılmaz",
                Bio = "Film tutkunu ve eleştirmeni. Özellikle bilim kurgu filmleri seviyorum.",
                IsEmailVerified = true,
                IsActive = true
            },
            new User
            {
                Username = "moviecritic",
                Email = "moviecritic@example.com",
                PasswordHash = passwordHasher.HashPassword("Password123!"),
                FirstName = "Elif",
                LastName = "Kaya",
                Bio = "Sinema yazarı ve film eleştirmeni. Her türden filme açığım.",
                IsEmailVerified = true,
                IsActive = true
            },
            new User
            {
                Username = "blockbusterlover",
                Email = "blockbusterlover@example.com",
                PasswordHash = passwordHasher.HashPassword("Password123!"),
                FirstName = "Mehmet",
                LastName = "Demir",
                Bio = "Aksiyon ve macera filmleri favorim. IMAX deneyimi harikadır!",
                IsEmailVerified = true,
                IsActive = true
            },
            new User
            {
                Username = "indiefilmfan",
                Email = "indiefilmfan@example.com",
                PasswordHash = passwordHasher.HashPassword("Password123!"),
                FirstName = "Zeynep",
                LastName = "Öztürk",
                Bio = "Bağımsız sinema ve sanat filmleri meraklısı.",
                IsEmailVerified = true,
                IsActive = true
            },
            new User
            {
                Username = "classiccinema",
                Email = "classiccinema@example.com",
                PasswordHash = passwordHasher.HashPassword("Password123!"),
                FirstName = "Can",
                LastName = "Arslan",
                Bio = "Klasik Hollywood ve dünya sineması uzmanı.",
                IsEmailVerified = true,
                IsActive = true
            }
        };

        foreach (var user in users)
        {
            await unitOfWork.Users.AddAsync(user);
        }
    }

    private static async Task SeedMovieCastAndCrewAsync(IUnitOfWork unitOfWork)
    {
        var movies = (await unitOfWork.Movies.GetAllAsync()).ToList();
        var persons = (await unitOfWork.Persons.GetAllAsync()).ToList();

        if (movies.Count == 0 || persons.Count == 0) return;

        var movieCasts = new List<MovieCast>();
        var movieCrews = new List<MovieCrew>();

        // Inception
        var inception = movies.FirstOrDefault(m => m.Title == "Inception");
        var dicaprio = persons.FirstOrDefault(p => p.Name == "Leonardo DiCaprio");
        var nolan = persons.FirstOrDefault(p => p.Name == "Christopher Nolan");

        if (inception != null && dicaprio != null)
        {
            movieCasts.Add(new MovieCast
            {
                MovieId = inception.Id,
                PersonId = dicaprio.Id,
                Character = "Dom Cobb",
                Order = 1
            });
        }

        if (inception != null && nolan != null)
        {
            movieCrews.Add(new MovieCrew
            {
                MovieId = inception.Id,
                PersonId = nolan.Id,
                Job = "Director",
                Department = "Directing"
            });
        }

        // Barbie
        var barbie = movies.FirstOrDefault(m => m.Title == "Barbie");
        var margot = persons.FirstOrDefault(p => p.Name == "Margot Robbie");
        var gosling = persons.FirstOrDefault(p => p.Name == "Ryan Gosling");
        var gerwig = persons.FirstOrDefault(p => p.Name == "Greta Gerwig");

        if (barbie != null && margot != null)
        {
            movieCasts.Add(new MovieCast
            {
                MovieId = barbie.Id,
                PersonId = margot.Id,
                Character = "Barbie",
                Order = 1
            });
        }

        if (barbie != null && gosling != null)
        {
            movieCasts.Add(new MovieCast
            {
                MovieId = barbie.Id,
                PersonId = gosling.Id,
                Character = "Ken",
                Order = 2
            });
        }

        if (barbie != null && gerwig != null)
        {
            movieCrews.Add(new MovieCrew
            {
                MovieId = barbie.Id,
                PersonId = gerwig.Id,
                Job = "Director",
                Department = "Directing"
            });
        }

        // Oppenheimer
        var oppenheimer = movies.FirstOrDefault(m => m.Title == "Oppenheimer");
        var murphy = persons.FirstOrDefault(p => p.Name == "Cillian Murphy");

        if (oppenheimer != null && murphy != null)
        {
            movieCasts.Add(new MovieCast
            {
                MovieId = oppenheimer.Id,
                PersonId = murphy.Id,
                Character = "J. Robert Oppenheimer",
                Order = 1
            });
        }

        if (oppenheimer != null && nolan != null)
        {
            movieCrews.Add(new MovieCrew
            {
                MovieId = oppenheimer.Id,
                PersonId = nolan.Id,
                Job = "Director",
                Department = "Directing"
            });
        }

        foreach (var cast in movieCasts)
        {
            await unitOfWork.MovieCasts.AddAsync(cast);
        }

        foreach (var crew in movieCrews)
        {
            await unitOfWork.MovieCrews.AddAsync(crew);
        }
    }

    private static async Task SeedMovieGenresAsync(IUnitOfWork unitOfWork)
    {
        var movies = (await unitOfWork.Movies.GetAllAsync()).ToList();
        var genres = (await unitOfWork.Genres.GetAllAsync()).ToList();

        if (movies.Count == 0 || genres.Count == 0) return;

        var movieGenres = new List<MovieGenre>();

        var inception = movies.FirstOrDefault(m => m.Title == "Inception");
        var scifi = genres.FirstOrDefault(g => g.Name == "Sci-Fi");
        var thriller = genres.FirstOrDefault(g => g.Name == "Thriller");
        var action = genres.FirstOrDefault(g => g.Name == "Action");

        if (inception != null && scifi != null)
            movieGenres.Add(new MovieGenre { MovieId = inception.Id, GenreId = scifi.Id });
        if (inception != null && thriller != null)
            movieGenres.Add(new MovieGenre { MovieId = inception.Id, GenreId = thriller.Id });

        var barbie = movies.FirstOrDefault(m => m.Title == "Barbie");
        var comedy = genres.FirstOrDefault(g => g.Name == "Comedy");
        var adventure = genres.FirstOrDefault(g => g.Name == "Adventure");

        if (barbie != null && comedy != null)
            movieGenres.Add(new MovieGenre { MovieId = barbie.Id, GenreId = comedy.Id });
        if (barbie != null && adventure != null)
            movieGenres.Add(new MovieGenre { MovieId = barbie.Id, GenreId = adventure.Id });

        var oppenheimer = movies.FirstOrDefault(m => m.Title == "Oppenheimer");
        var drama = genres.FirstOrDefault(g => g.Name == "Drama");

        if (oppenheimer != null && drama != null)
            movieGenres.Add(new MovieGenre { MovieId = oppenheimer.Id, GenreId = drama.Id });

        foreach (var movieGenre in movieGenres)
        {
            await unitOfWork.MovieGenres.AddAsync(movieGenre);
        }
    }

    private static async Task SeedRatingsAsync(IUnitOfWork unitOfWork)
    {
        var users = (await unitOfWork.Users.GetAllAsync()).ToList();
        var movies = (await unitOfWork.Movies.GetAllAsync()).ToList();

        if (users.Count == 0 || movies.Count == 0) return;

        var random = new Random();
        var ratings = new List<Rating>();

        foreach (var user in users.Take(3))
        {
            foreach (var movie in movies.Take(3))
            {
                ratings.Add(new Rating
                {
                    UserId = user.Id,
                    MovieId = movie.Id,
                    Score = random.Next(6, 11) // 6-10 arası puan
                });
            }
        }

        foreach (var rating in ratings)
        {
            await unitOfWork.Ratings.AddAsync(rating);
        }
    }

    private static async Task SeedReviewsAsync(IUnitOfWork unitOfWork)
    {
        var users = (await unitOfWork.Users.GetAllAsync()).ToList();
        var movies = (await unitOfWork.Movies.GetAllAsync()).ToList();

        if (users.Count == 0 || movies.Count == 0) return;

        var reviews = new[]
        {
            new Review
            {
                UserId = users[0].Id,
                MovieId = movies.FirstOrDefault(m => m.Title == "Inception")?.Id ?? movies[0].Id,
                Title = "Zihinsel Bir Şaheser",
                Content = "Christopher Nolan'ın en karmaşık ve etkileyici filmlerinden biri. Rüya içinde rüya konsepti mükemmel işlenmiş. Leonardo DiCaprio harika bir performans sergilemiş.",
                ContainsSpoilers = false,
                LikesCount = 15
            },
            new Review
            {
                UserId = users[1].Id,
                MovieId = movies.FirstOrDefault(m => m.Title == "Barbie")?.Id ?? movies[1].Id,
                Title = "Beklenmedik Derinlik",
                Content = "İlk bakışta basit görünen film aslında toplumsal mesajlarla dolu. Margot Robbie ve Ryan Gosling'in kimyası mükemmel. Rengarenk ve eğlenceli!",
                ContainsSpoilers = false,
                LikesCount = 23
            },
            new Review
            {
                UserId = users[2].Id,
                MovieId = movies.FirstOrDefault(m => m.Title == "Oppenheimer")?.Id ?? movies[2].Id,
                Title = "Tarihi Dramda Ustalık",
                Content = "Cillian Murphy'nin performansı nefes kesici. Film tarihi olayları büyük bir ustalıkla perdeye aktarıyor. Nolan yine harika bir iş çıkarmış.",
                ContainsSpoilers = false,
                LikesCount = 18
            },
            new Review
            {
                UserId = users[3].Id,
                MovieId = movies.FirstOrDefault(m => m.Title == "La La Land")?.Id ?? movies[4].Id,
                Title = "Müzikal Büyüsü",
                Content = "Ryan Gosling ve Emma Stone'un performansları muhteşem. Müzikler kulağımda çınlıyor hala. Modern bir müzikal şaheseri.",
                ContainsSpoilers = false,
                LikesCount = 12
            }
        };

        foreach (var review in reviews)
        {
            await unitOfWork.Reviews.AddAsync(review);
        }
    }

    private static async Task SeedCommentsAsync(IUnitOfWork unitOfWork)
    {
        var users = (await unitOfWork.Users.GetAllAsync()).ToList();
        var reviews = (await unitOfWork.Reviews.GetAllAsync()).ToList();

        if (users.Count == 0 || reviews.Count == 0) return;

        var comments = new[]
        {
            new Comment
            {
                ReviewId = reviews[0].Id,
                UserId = users[1].Id,
                Content = "Kesinlikle katılıyorum! Inception gerçekten zihinsel bir egzersiz.",
                UpvotesCount = 5,
                DownvotesCount = 0
            },
            new Comment
            {
                ReviewId = reviews[0].Id,
                UserId = users[2].Id,
                Content = "Bence biraz fazla karmaşıktı ama yine de güzeldi.",
                UpvotesCount = 2,
                DownvotesCount = 1
            },
            new Comment
            {
                ReviewId = reviews[1].Id,
                UserId = users[0].Id,
                Content = "Barbie filmini çok beğendim! Renkleri ve müzikleri harikasaydı.",
                UpvotesCount = 8,
                DownvotesCount = 0
            },
            new Comment
            {
                ReviewId = reviews[2].Id,
                UserId = users[4].Id,
                Content = "Christopher Nolan'ın en iyi filmlerinden biri olabilir.",
                UpvotesCount = 6,
                DownvotesCount = 0
            },
            new Comment
            {
                ReviewId = reviews[3].Id,
                UserId = users[2].Id,
                Content = "La La Land'in final sahnesi çok etkileyiciydi!",
                UpvotesCount = 4,
                DownvotesCount = 0
            }
        };

        foreach (var comment in comments)
        {
            await unitOfWork.Comments.AddAsync(comment);
        }
    }

    private static async Task SeedReactionsAsync(IUnitOfWork unitOfWork)
    {
        var users = (await unitOfWork.Users.GetAllAsync()).ToList();
        var comments = (await unitOfWork.Comments.GetAllAsync()).ToList();

        if (users.Count == 0 || comments.Count == 0) return;

        var reactions = new List<Reaction>();
        var random = new Random();

        // Her yoruma rastgele tepkiler ekle
        foreach (var comment in comments.Take(3))
        {
            for (int i = 0; i < 2; i++)
            {
                var randomUser = users[random.Next(users.Count)];
                if (!reactions.Any(r => r.CommentId == comment.Id && r.UserId == randomUser.Id))
                {
                    reactions.Add(new Reaction
                    {
                        CommentId = comment.Id,
                        UserId = randomUser.Id,
                        Type = random.Next(2) == 0 ? ReactionType.Upvote : ReactionType.Downvote
                    });
                }
            }
        }

        foreach (var reaction in reactions)
        {
            await unitOfWork.Reactions.AddAsync(reaction);
        }
    }

    private static async Task SeedFavoritesAsync(IUnitOfWork unitOfWork)
    {
        var users = (await unitOfWork.Users.GetAllAsync()).ToList();
        var movies = (await unitOfWork.Movies.GetAllAsync()).ToList();

        if (users.Count == 0 || movies.Count == 0) return;

        var favorites = new[]
        {
            new Favorite { UserId = users[0].Id, MovieId = movies[0].Id },
            new Favorite { UserId = users[0].Id, MovieId = movies[2].Id },
            new Favorite { UserId = users[1].Id, MovieId = movies[1].Id },
            new Favorite { UserId = users[1].Id, MovieId = movies[4].Id },
            new Favorite { UserId = users[2].Id, MovieId = movies[0].Id },
            new Favorite { UserId = users[2].Id, MovieId = movies[3].Id },
            new Favorite { UserId = users[3].Id, MovieId = movies[2].Id },
            new Favorite { UserId = users[4].Id, MovieId = movies[1].Id }
        };

        foreach (var favorite in favorites)
        {
            await unitOfWork.Favorites.AddAsync(favorite);
        }
    }

    private static async Task SeedMovieListsAsync(IUnitOfWork unitOfWork)
    {
        if (await unitOfWork.MovieLists.AnyAsync(ml => true))
            return;

        var users = (await unitOfWork.Users.GetAllAsync()).ToList();
        var movies = (await unitOfWork.Movies.GetAllAsync()).ToList();

        if (users.Count == 0 || movies.Count == 0) return;

        var movieLists = new[]
        {
            // User 1 - cinemafan01
            new MovieList
            {
                UserId = users[0].Id,
                Name = "İzleme Listesi",
                Description = "İzlemeyi planladığım filmler",
                IsWatchlist = true,
                IsPublic = false
            },
            new MovieList
            {
                UserId = users[0].Id,
                Name = "Favori Sci-Fi Filmlerim",
                Description = "En sevdiğim bilim kurgu filmleri. Christopher Nolan'ın eserleri ağırlıkta.",
                IsWatchlist = false,
                IsPublic = true
            },
            new MovieList
            {
                UserId = users[0].Id,
                Name = "Zihin Açıcı Filmler",
                Description = "Düşündüren, analiz etmeye değer filmler",
                IsWatchlist = false,
                IsPublic = true
            },
            
            // User 2 - moviecritic  
            new MovieList
            {
                UserId = users[1].Id,
                Name = "İzleme Listesi",
                Description = "Eleştiri yazacağım filmler",
                IsWatchlist = true,
                IsPublic = false
            },
            new MovieList
            {
                UserId = users[1].Id,
                Name = "2023'ün En İyileri",
                Description = "2023 yılının en beğendiğim filmleri. Barbenheimer dönemi unutulmaz!",
                IsWatchlist = false,
                IsPublic = true
            },
            new MovieList
            {
                UserId = users[1].Id,
                Name = "Kadın Yönetmenlerin Eserleri",
                Description = "Kadın yönetmenlerin harika filmlerini derledim",
                IsWatchlist = false,
                IsPublic = true
            },
            
            // User 3 - blockbusterlover
            new MovieList
            {
                UserId = users[2].Id,
                Name = "İzleme Listesi", 
                Description = "IMAX'te izleyeceğim filmler",
                IsWatchlist = true,
                IsPublic = false
            },
            new MovieList
            {
                UserId = users[2].Id,
                Name = "Aksiyon Dolu Filmler",
                Description = "Nefes kesen aksiyon filmleri koleksiyonum. Adrenalin garantili!",
                IsWatchlist = false,
                IsPublic = true
            },
            new MovieList
            {
                UserId = users[2].Id,
                Name = "Büyük Bütçeli Yapımlar",
                Description = "Hollywood'un en pahalı ve görsel açıdan etkileyici filmleri",
                IsWatchlist = false,
                IsPublic = true
            },

            // User 4 - indiefilmfan
            new MovieList
            {
                UserId = users[3].Id,
                Name = "İzleme Listesi",
                Description = "Keşfetmek istediğim indie filmler",
                IsWatchlist = true,
                IsPublic = false
            },
            new MovieList
            {
                UserId = users[3].Id,
                Name = "Sanat Filmleri",
                Description = "Bağımsız ve sanat filmleri koleksiyonum",
                IsWatchlist = false,
                IsPublic = true
            },
            
            // User 5 - classiccinema
            new MovieList
            {
                UserId = users[4].Id,
                Name = "İzleme Listesi",
                Description = "Yeniden izleyeceğim klasikler",
                IsWatchlist = true,
                IsPublic = false
            },
            new MovieList
            {
                UserId = users[4].Id,
                Name = "Modern Klasikler",
                Description = "Gelecekte klasik olacağını düşündüğüm filmler",
                IsWatchlist = false,
                IsPublic = true
            },
            new MovieList
            {
                UserId = users[4].Id,
                Name = "Müzikal Filmleri",
                Description = "Kaliteli müzikal filmleri bir arada",
                IsWatchlist = false,
                IsPublic = true
            }
        };

        foreach (var movieList in movieLists)
        {
            await unitOfWork.MovieLists.AddAsync(movieList);
        }

        await unitOfWork.SaveChangesAsync();

        // Liste öğelerini ekle
        var savedLists = (await unitOfWork.MovieLists.GetAllAsync()).ToList();
        var inceptionId = movies.FirstOrDefault(m => m.Title == "Inception")?.Id ?? movies[0].Id;
        var barbieId = movies.FirstOrDefault(m => m.Title == "Barbie")?.Id ?? movies[1].Id;
        var oppenheimerIdId = movies.FirstOrDefault(m => m.Title == "Oppenheimer")?.Id ?? movies[2].Id;
        var wolfId = movies.FirstOrDefault(m => m.Title == "The Wolf of Wall Street")?.Id ?? movies[3].Id;
        var laLaLandId = movies.FirstOrDefault(m => m.Title == "La La Land")?.Id ?? movies[4].Id;

        var movieListItems = new List<MovieListItem>();

        // User 1 lists
        var user1Lists = savedLists.Where(l => l.UserId == users[0].Id).ToList();
        if (user1Lists.Count >= 3)
        {
            // Watchlist
            movieListItems.Add(new MovieListItem { MovieListId = user1Lists[0].Id, MovieId = barbieId, Order = 1, Notes = "Çok konuşulan film, mutlaka izlemeliyim" });
            movieListItems.Add(new MovieListItem { MovieListId = user1Lists[0].Id, MovieId = wolfId, Order = 2, Notes = "Leonardo DiCaprio hayranıyım" });
            
            // Sci-Fi
            movieListItems.Add(new MovieListItem { MovieListId = user1Lists[1].Id, MovieId = inceptionId, Order = 1, Notes = "Nolan'ın en iyi sci-fi eseri" });
            movieListItems.Add(new MovieListItem { MovieListId = user1Lists[1].Id, MovieId = oppenheimerIdId, Order = 2, Notes = "Bilimsel tema harika işlenmiş" });
            
            // Düşündüren filmler
            movieListItems.Add(new MovieListItem { MovieListId = user1Lists[2].Id, MovieId = inceptionId, Order = 1, Notes = "Rüya ve gerçeklik arasındaki sınır" });
        }

        // User 2 lists  
        var user2Lists = savedLists.Where(l => l.UserId == users[1].Id).ToList();
        if (user2Lists.Count >= 3)
        {
            // Watchlist
            movieListItems.Add(new MovieListItem { MovieListId = user2Lists[0].Id, MovieId = laLaLandId, Order = 1, Notes = "Eleştiri yazacağım" });
            
            // 2023'ün en iyileri
            movieListItems.Add(new MovieListItem { MovieListId = user2Lists[1].Id, MovieId = barbieId, Order = 1, Notes = "Yılın en büyük sürprizi" });
            movieListItems.Add(new MovieListItem { MovieListId = user2Lists[1].Id, MovieId = oppenheimerIdId, Order = 2, Notes = "Tarihi dramanın zirvesi" });
            
            // Kadın yönetmenler
            movieListItems.Add(new MovieListItem { MovieListId = user2Lists[2].Id, MovieId = barbieId, Order = 1, Notes = "Greta Gerwig'in muhteşem yönetimi" });
        }

        // User 3 lists
        var user3Lists = savedLists.Where(l => l.UserId == users[2].Id).ToList();
        if (user3Lists.Count >= 3)
        {
            // Watchlist
            movieListItems.Add(new MovieListItem { MovieListId = user3Lists[0].Id, MovieId = laLaLandId, Order = 1, Notes = "IMAX'te müzikal deneyimi" });
            
            // Aksiyon filmleri
            movieListItems.Add(new MovieListItem { MovieListId = user3Lists[1].Id, MovieId = inceptionId, Order = 1, Notes = "Aksiyon ve zihin oyunları" });
            movieListItems.Add(new MovieListItem { MovieListId = user3Lists[1].Id, MovieId = wolfId, Order = 2, Notes = "Hızlı tempo ve enerji" });
            
            // Büyük bütçeli
            movieListItems.Add(new MovieListItem { MovieListId = user3Lists[2].Id, MovieId = inceptionId, Order = 1, Notes = "$160M bütçe harika kullanılmış" });
            movieListItems.Add(new MovieListItem { MovieListId = user3Lists[2].Id, MovieId = barbieId, Order = 2, Notes = "$145M ile rengarenk dünya" });
            movieListItems.Add(new MovieListItem { MovieListId = user3Lists[2].Id, MovieId = oppenheimerIdId, Order = 3, Notes = "$100M ile tarihi yeniden canlandırma" });
        }

        // User 4 lists
        var user4Lists = savedLists.Where(l => l.UserId == users[3].Id).ToList();
        if (user4Lists.Count >= 2)
        {
            // Watchlist  
            movieListItems.Add(new MovieListItem { MovieListId = user4Lists[0].Id, MovieId = inceptionId, Order = 1, Notes = "Popüler ama kaliteli görünüyor" });
            
            // Sanat filmleri
            movieListItems.Add(new MovieListItem { MovieListId = user4Lists[1].Id, MovieId = laLaLandId, Order = 1, Notes = "Modern müzikal sanatı" });
        }

        // User 5 lists
        var user5Lists = savedLists.Where(l => l.UserId == users[4].Id).ToList();
        if (user5Lists.Count >= 3)
        {
            // Watchlist
            movieListItems.Add(new MovieListItem { MovieListId = user5Lists[0].Id, MovieId = wolfId, Order = 1, Notes = "Klasik hale geldi mi?" });
            
            // Modern klasikler
            movieListItems.Add(new MovieListItem { MovieListId = user5Lists[1].Id, MovieId = inceptionId, Order = 1, Notes = "2010'ların klasiği" });
            movieListItems.Add(new MovieListItem { MovieListId = user5Lists[1].Id, MovieId = oppenheimerIdId, Order = 2, Notes = "Gelecekte klasik olacak" });
            
            // Müzikaller
            movieListItems.Add(new MovieListItem { MovieListId = user5Lists[2].Id, MovieId = laLaLandId, Order = 1, Notes = "Modern müzikal şaheseri" });
        }

        foreach (var item in movieListItems)
        {
            await unitOfWork.MovieListItems.AddAsync(item);
        }
    }

    private static async Task SeedListFavoritesAsync(IUnitOfWork unitOfWork)
    {
        if (await unitOfWork.ListFavorites.AnyAsync(lf => true))
            return;

        var users = (await unitOfWork.Users.GetAllAsync()).ToList();
        var movieLists = (await unitOfWork.MovieLists.GetAllAsync()).ToList();

        if (users.Count == 0 || movieLists.Count == 0) return;

        // Sadece public listeleri al
        var publicLists = movieLists.Where(ml => ml.IsPublic).ToList();
        
        var listFavorites = new List<ListFavorite>();

        // User 1 (cinemafan01) favorites
        var user2SciFiList = publicLists.FirstOrDefault(l => l.UserId == users[1].Id && l.Name.Contains("2023"));
        if (user2SciFiList != null)
            listFavorites.Add(new ListFavorite { UserId = users[0].Id, MovieListId = user2SciFiList.Id });

        var user3ActionList = publicLists.FirstOrDefault(l => l.UserId == users[2].Id && l.Name.Contains("Aksiyon"));
        if (user3ActionList != null)
            listFavorites.Add(new ListFavorite { UserId = users[0].Id, MovieListId = user3ActionList.Id });

        // User 2 (moviecritic) favorites  
        var user1SciFiList = publicLists.FirstOrDefault(l => l.UserId == users[0].Id && l.Name.Contains("Sci-Fi"));
        if (user1SciFiList != null)
            listFavorites.Add(new ListFavorite { UserId = users[1].Id, MovieListId = user1SciFiList.Id });

        var user5ModernList = publicLists.FirstOrDefault(l => l.UserId == users[4].Id && l.Name.Contains("Modern"));
        if (user5ModernList != null)
            listFavorites.Add(new ListFavorite { UserId = users[1].Id, MovieListId = user5ModernList.Id });

        // User 3 (blockbusterlover) favorites
        var user1MindList = publicLists.FirstOrDefault(l => l.UserId == users[0].Id && l.Name.Contains("Zihin"));
        if (user1MindList != null)
            listFavorites.Add(new ListFavorite { UserId = users[2].Id, MovieListId = user1MindList.Id });

        var user2WomenList = publicLists.FirstOrDefault(l => l.UserId == users[1].Id && l.Name.Contains("Kadın"));
        if (user2WomenList != null)
            listFavorites.Add(new ListFavorite { UserId = users[2].Id, MovieListId = user2WomenList.Id });

        // User 4 (indiefilmfan) favorites
        var user5MusicalList = publicLists.FirstOrDefault(l => l.UserId == users[4].Id && l.Name.Contains("Müzikal"));
        if (user5MusicalList != null)
            listFavorites.Add(new ListFavorite { UserId = users[3].Id, MovieListId = user5MusicalList.Id });

        // User 5 (classiccinema) favorites
        var user2Best2023List = publicLists.FirstOrDefault(l => l.UserId == users[1].Id && l.Name.Contains("2023"));
        if (user2Best2023List != null)
            listFavorites.Add(new ListFavorite { UserId = users[4].Id, MovieListId = user2Best2023List.Id });

        var user3BigBudgetList = publicLists.FirstOrDefault(l => l.UserId == users[2].Id && l.Name.Contains("Büyük"));
        if (user3BigBudgetList != null)
            listFavorites.Add(new ListFavorite { UserId = users[4].Id, MovieListId = user3BigBudgetList.Id });

        foreach (var favorite in listFavorites)
        {
            await unitOfWork.ListFavorites.AddAsync(favorite);
        }
    }
}