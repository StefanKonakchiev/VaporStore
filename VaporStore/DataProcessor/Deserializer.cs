namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Import;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gamesDto = JsonConvert.DeserializeObject<List<ImportGamesDto>>(jsonString);
            var games = new List<Game>();
            var developers = new List<Developer>();
            var tags = new List<Tag>();
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

                var gameTags = new List<Tag>();
                foreach (var tag in game.Tags)
                {
                    if (!IsModelValid(tag))
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    var currentTag = tags.FirstOrDefault(e => e.Name == tag);
                    if (currentTag == null)
                    {
                        currentTag = new Tag() { Name = tag };
                        tags.Add(currentTag);
                    }
                    gameTags.Add(currentTag);
                }

                var currentGame = new Game()
                {
                    Name = game.Name,
                    Price = game.Price,
                    ReleaseDate = game.ReleaseDate,
                    Developer = context.Developers.FirstOrDefault(e => e.Name == game.Developer),
                    Genre = context.Genres.FirstOrDefault(e => e.Name == game.Genre),
                    GameTags = gameTags.Select(e => new GameTag {Tag = e}).ToArray()

                };

                
                games.Add(currentGame);
                sb.AppendLine($"Added {currentGame.Name} ({currentGame.Genre.Name}) with {gameTags.Count} tags");
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
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPurchasesDto[]),
                new XmlRootAttribute("Purchases"));

            var purchasesDto = (ImportPurchasesDto[])xmlSerializer.Deserialize(new StringReader(xmlString));
            var purchases = new List<Purchase>();
            StringBuilder sb = new StringBuilder();

            foreach (var purchase in purchasesDto)
            {
                if (IsModelValid(purchase))
                {
                    if (!context.Cards.Any(e => e.Number == purchase.CardNumber))
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    if (!context.Games.Any(e => e.Name == purchase.GameName))
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }
                    purchase.Game = context.Games.FirstOrDefault(e => e.Name == purchase.GameName);
                    purchase.Card = context.Cards.FirstOrDefault(e => e.Number == purchase.CardNumber);
                    var currentPurchase = Mapper.Map<Purchase>(purchase);
                    purchases.Add(currentPurchase);
                    //currentPurchase.Card.User.Username
                    //var currentUser = context.Users.FirstOrDefault(e => e.Cards.FirstOrDefault() == purchase.Card);
                    sb.AppendLine($"Imported {currentPurchase.Game.Name} for {currentPurchase.Card.User.Username}");
                }
                else
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }
            }

            context.Purchases.AddRange(purchases);
            context.SaveChanges();

            return sb.ToString();
        }

        private static bool IsModelValid(object model)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);
            return Validator.TryValidateObject(model, validationContext, null, true);
        }
    }
}