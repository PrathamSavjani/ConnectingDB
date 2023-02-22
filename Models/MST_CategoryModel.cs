using Newtonsoft.Json;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ConnectingDB.Models
{
    public class MST_CategoryModel
    {
        public int? ContactCategoryID { get; set; } 
        [Required]
        [DisplayName("Category Name")]
        public string ContactCategoryName { get; set; }
        [Required]
        public DateTime CreationDate { get; set; }
        [Required]
        public DateTime ModificationDate { get; set; }
    }
}
