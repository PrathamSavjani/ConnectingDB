using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConnectingDB.Models
{
    public class LOC_StateModel
    {
        public int? StateID { get; set; }
        [Required]
        [DisplayName("State Name")]
        public string StateName { get; set; }
        [Required]
        [DisplayName("State Code")]
        public string StateCode { get; set; }
        [Required]
        [DisplayName("Country Name")]
        public int CountryID { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime ModificationDate { get; set; }
        public IFormFile? File { get; set; }
        public string? PhotoPath { get; set; }
    }

    public class LOC_StateDropDownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
