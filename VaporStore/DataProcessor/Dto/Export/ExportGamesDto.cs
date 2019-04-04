using System;
using System.Collections.Generic;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Export
{
    public class ExportGamesDto
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Developer { get; set; }

        public string Tags { get; set; }

        public int Players { get; set; }


        //Id = n.Id,
        //Title = n.Name,
        //Developer = n.Developer.Name,
        //Tags = n.GameTags.Select(t => t.Tag.Name),//.Aggregate("", (c, n) => c + ", " + n),
        //Players = n.Purchases.Count()

    }
}
