using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enum;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class ImportPurchasesDto
    {
        [Required]
        [XmlElement("Type")]
        public PurchaseType Type { get; set; }

        [Required]
        [RegularExpression(@"[0-9A-Z]{4}-[0-9A-Z]{4}-[0-9A-Z]{4}")]
        [XmlElement("Key")]
        public string ProductKey { get; set; }

        [XmlIgnore]
        public DateTime DateTime { get; set; }

        [Required]
        [XmlElement("Date")]
        public string Date {
            get { return this.DateTime.ToString(); }
            set { this.DateTime = DateTime.ParseExact(value, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            }
        }
        
        [Required]
        [XmlElement("Card")]
        public string CardNumber { get; set; }

        [XmlIgnore]
        public Card Card { get; set; }

        [Required]
        [XmlAttribute("title")]
        public string GameName { get; set; }

        [XmlIgnore]
        public Game Game { get; set; }
    }
}
