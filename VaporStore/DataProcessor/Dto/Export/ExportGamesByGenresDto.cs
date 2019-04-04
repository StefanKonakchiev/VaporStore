using System;
using System.Collections.Generic;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.Dto.Export
{
    public class ExportGamesByGenresDto
    {
        public int Id { get; set; }

        public string Genre { get; set; }

        public ICollection<ExportGamesDto> Games { get; set; }

        public int TotalPlayers { get; set; }

    }
}
