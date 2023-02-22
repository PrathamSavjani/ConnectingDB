using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConnectingDB.Models
{
    public class LOC_CityModel
    {
        [Required]
        [DisplayName("Country Name")]
        public int CountryID { get; set; }
        public int? CityID { get; set; }
        [Required]
        [DisplayName("City Name")]
        public string CityName { get; set; }
        [Required]
        [DisplayName("State Name")]
        public int StateID { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime ModificationDate { get; set; }
    }
    public class LOC_CityDropDownModel
    {
        public int CityID { get; set; }
        public string CityName { get; set; }
    }
}
