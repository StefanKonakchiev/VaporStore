namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Dtos;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gamesDto = JsonConvert.DeserializeObject<List<ImportGamesDto>>(jsonString);
            var games = new List<Game>();
            var developers = new List<Developer>();
            var genres = new List<Genre>();
            StringBuilder sb = new StringBuilder();

            foreach (var game in gamesDto)
            {
                if (IsModelValid(game))
                {
                    if (!developers.Select(e => e.Name).Contains(game.Developer))
                    {
                        developers.Add(new Developer() { Name = game.Developer });
                    }
                    if (!genres.Select(e => e.Name).Contains(game.Genre))
                    {
                        genres.Add(new Genre() { Name = game.Genre });
                    }
                }
            }
            context.Developers.AddRange(developers);
            context.Genres.AddRange(genres);
            context.SaveChanges();

            foreach (var game in gamesDto)
            {
                if (!IsModelValid(game))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var currentGame = new Game()
                {
                    Name = game.Name,
                    Price = game.Price,
                    ReleaseDate = game.ReleaseDate,
                    Developer = context.Developers.FirstOrDefault(e => e.Name == game.Developer),
                    Genre = context.Genres.FirstOrDefault(e => e.Name == game.Genre)

                };

                var tags = new List<Tag>();
                foreach (var tag in game.Tags)
                {
                    if (!context.Tags.Select(e => e.Name).Contains(tag))
                    {
                        if (!tags.Select(e => e.Name).Contains(tag))
                        {
                            tags.Add(new Tag() { Name = tag });
                        }
                    }
                }
                context.Tags.AddRange(tags);
                context.SaveChanges();

                foreach (var tag in tags)
                {
                    currentGame.GameTags.Add(new GameTag() { TagId = tag.Id });
                }
                games.Add(currentGame);
                sb.AppendLine($"Added {currentGame.Name} with {tags.Count()} tags");
            }
            context.Games.AddRange(games);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var usersDto = JsonConvert.DeserializeObject<List<ImportUsersAndCardsDto>>(jsonString);
            var users = new List<User>();
            StringBuilder sb = new StringBuilder();

            foreach (var user in usersDto)
            {
                var isCardValid = true;
                if (!IsModelValid(user))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                var currentUser = new User()
                {
                    FullName = user.FullName,
                    Username = user.Username,
                    Email = user.Email,
                    Age = user.Age
                };

                //var cards = new List<Card>();
                foreach (var card in user.Cards)
                {
                    if (!IsModelValid(card))
                    {
                        sb.AppendLine("Invalid Data");
                        isCardValid = false;
                        continue;
                    }

                    var currentCard = new Card()
                    {
                        Number = card.Number,
                        Type = card.Type,
                        Cvc = card.CVC
                    };

                    if (!context.Cards.Contains(currentCard))
                    {
                        if (!currentUser.Cards.Contains(currentCard))
                        {
                            currentUser.Cards.Add(currentCard);
                        }
                    }
                }
                
                if (!isCardValid)
                {
                    continue;
                }
                sb.AppendLine($"Imported {currentUser.Username} with {currentUser.Cards.Count()} cards");
                users.Add(currentUser);
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            return sb.ToString();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            throw new NotImplementedException();
        }

        private static bool IsModelValid(object model)
        {
            var validationContext = new ValidationContext(model, null, null);
            return Validator.TryValidateObject(model, validationContext, null, true);
        }
    }
}