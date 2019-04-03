using System.ComponentModel.DataAnnotations;
using VaporStore.Data.Models;

namespace VaporStore.Dtos
{
    public class ImportCardDto
    {
        [Required]
        [RegularExpression(@"^[0-9].{3}(\s[0-9].{3})(\s[0-9].{3})(\s[0-9].{3})")]
        public string Number { get; set; }

        [Required]
        public CardType Type { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{3}$")]
        public string CVC { get; set; }


        //"Cards": [
        //	{
        //		"Number": "1833 5024 0553 6211",
        //		"CVC": "903",
        //		"Type": "Debit"

        //          },
    }
}