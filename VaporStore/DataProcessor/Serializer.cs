namespace VaporStore.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var genres = context.Genres
                .Where(e => genreNames.Contains(e.Name))
                .Select(e => new ExportGamesByGenresDto
                {
                    Id = e.Id,
                    Genre = e.Name,
                    Games = e.Games.Where(g => g.Purchases.Any()).Select(n => new ExportGamesDto
                    {
                        Id = n.Id,
                        Title = n.Name,
                        Developer = n.Developer.Name,
                        Tags = string.Join(", ", n.GameTags.Select(g => g.Tag.Name)),//.Aggregate("", (c, n) => c + ", " + n),
                        Players = n.Purchases.Count()
                    })
                    .OrderByDescending(p => p.Players)
                    .ThenBy(g => g.Id)
                    .ToList(),
                    TotalPlayers = e.Games.Where(g => g.Purchases.Any()).Sum(pc => pc.Purchases.Count())
                })
                .OrderByDescending(p => p.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToList();

            var json = JsonConvert.SerializeObject(genres, new JsonSerializerSettings
            {
                //ContractResolver = contractResolver,
                Formatting = Newtonsoft.Json.Formatting.Indented,
            });

            return json;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            //var users = context.Cards
            //    .Where(e => e.Purchases.Any() && e.Purchases.Any(g => g.Game.Price != 0))
            //    .Select(p => new ExportUserPurchasesDto
            //    {
            //        Username = p.User.Username,
            //        Purchases = p.Purchases.Where(g => g.Type.ToString() == storeType && g.Game.Price != 0).Select(g => new ExportPurchasesDto
            //        {
            //            Card = g.Card.Number,
            //            Cvc = g.Card.Cvc,
            //            Date = g.Date.ToString("yyyy-MM-dd HH:mm"),
            //            Game = new ExportGamesXMLDto
            //            {
            //                Title = g.Game.Name,
            //                Genre = g.Game.Genre.Name,
            //                Price = g.Game.Price
            //            }
            //        })
            //        .OrderBy(e => e.Date)
            //        .ToArray(),
            //        TotalSpent = p.Purchases.Where(g => g.Type.ToString() == storeType).Sum(g => g.Game.Price)
            //    })
            //    .OrderByDescending(p => p.TotalSpent)
            //    .ThenBy(e => e.Username)
            //    .ToArray();

            var users = context.Users
                .Select(p => new ExportUserPurchasesDto
                {
                    Username = p.Username,
                    Purchases = p.Cards.SelectMany(e => e.Purchases).Where(g => g.Type.ToString() == storeType).Select(g => new ExportPurchasesDto
                    {
                        Card = g.Card.Number,
                        Cvc = g.Card.Cvc,
                        Date = g.Date.ToString("yyyy-MM-dd HH:mm"),
                        Game = new ExportGamesXMLDto
                        {
                            Title = g.Game.Name,
                            Genre = g.Game.Genre.Name,
                            Price = g.Game.Price
                        }
                    })
                    .OrderBy(e => e.Date)
                    .ToArray(),
                    TotalSpent = p.Cards
                        .SelectMany(e => e.Purchases)
                        .Where(g => g.Type.ToString() == storeType)
                        .Sum(g => g.Game.Price)
                })
                .Where(e => e.Purchases.Any())
                .OrderByDescending(p => p.TotalSpent)
                .ThenBy(e => e.Username)
                .ToArray();

            var sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(ExportUserPurchasesDto[]), new XmlRootAttribute("Users"));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;

            serializer.Serialize(new StringWriter(sb), users, ns);
            return sb.ToString();

            //using (var stream = new StringWriter())
            //using (var writer = XmlWriter.Create(stream, settings))
            //{
            //    serializer.Serialize(writer, users, ns);
            //    return stream.ToString();
            //}

        }
    }
}