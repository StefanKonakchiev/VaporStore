﻿using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("Game")]
    public class ExportGamesXMLDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }

        //      <Game title = "Counter-Strike: Global Offensive" >
        //        < Genre > Action </ Genre >
        //        < Price > 12.49 </ Price >
        //      </ Game >
    }
}