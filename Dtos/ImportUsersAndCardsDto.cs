using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Dtos
{
    public class ImportUsersAndCardsDto
    {
        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [RegularExpression(@"^[A-Z ].[a-z]*(\s[A-Z ].[a-z]*)")]
        public string FullName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        [Range(typeof(int), "3", "103")]
        public int Age { get; set; }

        public ICollection<ImportCardDto> Cards { get; set; }


        //"FullName": "Lorrie Silbert",
        //"Username": "lsilbert",
        //"Email": "lsilbert@yahoo.com",
        //"Age": 33,
        //"Cards": [
        //	{
        //		"Number": "1833 5024 0553 6211",
        //		"CVC": "903",
        //		"Type": "Debit"

        //          },
        //	{
        //		"Number": "5625 0434 5999 6254",
        //		"CVC": "570",
        //		"Type": "Credit"
        //	},
        //	{
        //		"Number": "4902 6975 5076 5316",
        //		"CVC": "091",
        //		"Type": "Debit"
        //	}
        //]
    }
}
