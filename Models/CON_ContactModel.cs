namespace ConnectingDB.Models
{
    public class CON_ContactModel
    {
        public int? ContactID { get; set; }
        public string ContactName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactAddress  { get; set; }
        public string ContactEmail { get; set; }
        public int CountryID { get; set; }
        public int StateID { get; set; }
        public int CityID { get; set; }
        public int ContactCategoryID { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
