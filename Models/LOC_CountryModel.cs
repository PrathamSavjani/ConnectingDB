using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConnectingDB.Models
{
	public class LOC_CountryModel
	{
		public int? CountryID { get; set; }
		[Required(ErrorMessage = "Please Enter Country Name")]
		[StringLength(20,MinimumLength =3)]
		public string CountryName { get; set; }
        [Required(ErrorMessage = "Please Enter Country Code")]
        [StringLength(5, MinimumLength = 2)]
        public string CountryCode { get; set; }
        [Required]
        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }
        [Required]
        [DisplayName("Modification Date")]
        public DateTime ModificationDate { get; set; }
        public IFormFile? File { get; set; }
        public string? PhotoPath { get; set; }
	}
	public class LOC_CountryDropDownModel
	{
		public int CountryID { get; set; }
		public string CountryName { get; set; }
	}
}
