using ConnectingDB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace ConnectingDB.Controllers
{
    public class CON_ContactController : Controller
    {
        private IConfiguration Configuration;
        public CON_ContactController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }//doubt
        public ActionResult Index()
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_CON_Contact_SelectAll";
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            return View("CON_Contact", dt);
        }

        #region BTN_AddState
        public ActionResult Add(int? ContactID)
        {
            List<LOC_StateDropDownModel> list = new List<LOC_StateDropDownModel>();
            ViewBag.StateList = list;

            List<LOC_CityDropDownModel> lust = new List<LOC_CityDropDownModel>();
            ViewBag.CityList = lust;

            #region DropdownCountry
            //Establish connection using ConnectionStrings
            string st = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection cton = new SqlConnection(st);
            cton.Open();

            //SQL Command to retrieve Country List 
            SqlCommand comd = cton.CreateCommand();
            comd.CommandType = CommandType.StoredProcedure;
            comd.CommandText = "PR_LOC_Country_SelectForDropDown";

            DataTable dtb = new DataTable();
            SqlDataReader sdar = comd.ExecuteReader();
            dtb.Load(sdar);
            cton.Close();

            List<LOC_CountryDropDownModel> lst = new List<LOC_CountryDropDownModel>();
            foreach (DataRow dr in dtb.Rows)
            {
                LOC_CountryDropDownModel vlist = new LOC_CountryDropDownModel();
                vlist.CountryID = Convert.ToInt32(dr["CountryID"]);
                vlist.CountryName = dr["CountryName"].ToString();
                lst.Add(vlist);
            }
            ViewBag.CountryList = lst;
            #endregion

            #region AddRegion
            if (ContactID != null)
            {
                string str = this.Configuration.GetConnectionString("addressBookDB");
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_CON_Contact_SelectByPK";
                cmd.Parameters.Add("@ContactID", SqlDbType.Int).Value = ContactID;
                DataTable dt = new DataTable();
                SqlDataReader CountryDataByID = cmd.ExecuteReader();
                dt.Load(CountryDataByID);

                CON_ContactModel Contactmodel = new CON_ContactModel();

                foreach (DataRow dr in dt.Rows)
                {
                    Contactmodel.ContactName = Convert.ToString(dr["ContactName"]);
                    Contactmodel.ContactAddress = Convert.ToString(dr["ContactAddress"]);
                    Contactmodel.ContactMobile = Convert.ToString(dr["ContactMobile"]);
                    Contactmodel.ContactEmail = Convert.ToString(dr["ContactEmail"]);
                    Contactmodel.CountryID = Convert.ToInt32(dr["CountryID"]);
                    Contactmodel.StateID = Convert.ToInt32(dr["StateID"]);
                    Contactmodel.CityID = Convert.ToInt32(dr["CityID"]);
                    Contactmodel.ContactCategoryID = Convert.ToInt32(dr["ContactCategoryID"]);
                    Contactmodel.ModificationDate = Convert.ToDateTime(dr["ModificationDate"]);
                    Contactmodel.CreationDate = Convert.ToDateTime(dr["CreationDate"]);
                }
                DataTable dt2 = new DataTable();
                SqlConnection conn2 = new SqlConnection(str);
                conn2.Open();
                SqlCommand cmd2 = conn2.CreateCommand();
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "PR_LOC_State_SelectComboBoxByCountryID";
                cmd2.Parameters.AddWithValue("@CountryID", Contactmodel.CountryID);
                SqlDataReader sr1 = cmd2.ExecuteReader();
                dt2.Load(sr1);

                List<LOC_StateDropDownModel> list2 = new List<LOC_StateDropDownModel>();
                foreach (DataRow dr in dt2.Rows)
                {
                    LOC_StateDropDownModel vlst = new LOC_StateDropDownModel();
                    vlst.StateID = Convert.ToInt32(dr["StateID"]);
                    vlst.StateName = dr["StateName"].ToString();
                    list2.Add(vlst);
                }

                ViewBag.StateList = list2;
                conn2.Close();

                DataTable dt3 = new DataTable();
                SqlConnection conn3 = new SqlConnection(str);
                conn3.Open();
                SqlCommand cmd3 = conn3.CreateCommand();
                cmd3.CommandType = CommandType.StoredProcedure;
                cmd3.CommandText = "PR_LOC_City_SelectComboBoxByStateID";
                cmd3.Parameters.AddWithValue("@StateID", Contactmodel.StateID);
                SqlDataReader sr3 = cmd3.ExecuteReader();
                dt3.Load(sr3);

                List<LOC_CityDropDownModel> list3 = new List<LOC_CityDropDownModel>();

                foreach (DataRow dr in dt3.Rows)
                {
                    LOC_CityDropDownModel vlst = new LOC_CityDropDownModel();
                    vlst.CityID = Convert.ToInt32(dr["CityID"]);
                    vlst.CityName = dr["CityName"].ToString();
                    list3.Add(vlst);
                }

                ViewBag.CityList = list3;
                conn3.Close();

                return View("CON_ContactAddEdit", Contactmodel);
                conn.Close();
            }
            #endregion

            return View("CON_ContactAddEdit");
        }
        #endregion

        #region AddEdit
        [HttpPost]
        public IActionResult Save(CON_ContactModel modelCON_Contact)
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve Country List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;

            if (modelCON_Contact.ContactID == null)
            {
                cmd.CommandText = "PR_CON_Contact_Insert";
                cmd.Parameters.Add("@CreationDate", SqlDbType.Date).Value = modelCON_Contact.CreationDate;
            }
            else
            {
                cmd.CommandText = "PR_CON_Contact_UpdateByPK";
                cmd.Parameters.Add("@ContactID", SqlDbType.Int).Value = modelCON_Contact.ContactID;
            }
            cmd.Parameters.Add("@ContactName", SqlDbType.NVarChar).Value = modelCON_Contact.ContactName;
            cmd.Parameters.Add("@ContactMobile", SqlDbType.NVarChar).Value = modelCON_Contact.ContactMobile;
            cmd.Parameters.Add("@ContactEmail", SqlDbType.NVarChar).Value = modelCON_Contact.ContactEmail;
            cmd.Parameters.Add("@ContactAddress", SqlDbType.NVarChar).Value = modelCON_Contact.ContactAddress;
            cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = modelCON_Contact.CountryID;
            cmd.Parameters.Add("@StateID", SqlDbType.Int).Value = modelCON_Contact.StateID;
            cmd.Parameters.Add("@CityID", SqlDbType.Int).Value = modelCON_Contact.CityID;
            cmd.Parameters.Add("@ContactCategoryID", SqlDbType.Int).Value = modelCON_Contact.ContactCategoryID;
            cmd.Parameters.Add("@ModificationDate", SqlDbType.Date).Value = modelCON_Contact.ModificationDate;

            if (Convert.ToBoolean(cmd.ExecuteNonQuery()))
            {
                if (modelCON_Contact.ContactID == null)//doubt
                {
                    TempData["InsertMessage"] = "You Have Inserted Record Successfully";
                }
                else
                {
                    TempData["UpdateMessage"] = "You Have Updated Record Successfully";
                }
            }
            conn.Close();
            return RedirectToAction("Index");
        }
        #endregion

        public ActionResult Delete(int ContactID)
        {
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve Country List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_CON_Contact_DeleteByPK";
            cmd.Parameters.AddWithValue("@ContactID", ContactID);
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Index");
        }

        public IActionResult DropdownByCountry(int CountryID)
        {
            #region DropdownState
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve State List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_State_SelectComboBoxByCountryID";
            cmd.Parameters.AddWithValue("@CountryID", CountryID);
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sqlDataReader);
            conn.Close();


            List<LOC_StateDropDownModel> list = new List<LOC_StateDropDownModel>();
            foreach (DataRow dr in dt.Rows)
            {
                LOC_StateDropDownModel vlist = new LOC_StateDropDownModel();
                vlist.StateID = Convert.ToInt32(dr["StateID"]);
                vlist.StateName = dr["StateName"].ToString();
                list.Add(vlist);
            }
            var vModel = list;
            return Json(vModel);
            #endregion
        }

        public IActionResult DropdownByState(int StateID)
        {
            #region DropdownCity
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve State List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_City_SelectComboBoxByStateID";
            cmd.Parameters.AddWithValue("@StateID", StateID);
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(sqlDataReader);
            conn.Close();


            List<LOC_CityDropDownModel> list = new List<LOC_CityDropDownModel>();
            foreach (DataRow dr in dt.Rows)
            {
                LOC_CityDropDownModel vlist = new LOC_CityDropDownModel();
                vlist.CityID = Convert.ToInt32(dr["CityID"]);
                vlist.CityName = dr["CityName"].ToString();
                list.Add(vlist);
            }
            var vModel = list;
            return Json(vModel);
            #endregion
            return View("LOC_ContactAddEdit");
        }
        #region Filter Records
        public IActionResult Filter(CON_ContactModel modelCON_Contact)
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_CON_Contact_FilterByContactName";
            cmd.Parameters.Add("@ContactName", SqlDbType.NVarChar).Value = modelCON_Contact.ContactName;
            cmd.Parameters.Add("@ContactAddress", SqlDbType.NVarChar).Value = modelCON_Contact.ContactAddress;
            cmd.Parameters.Add("@ContactMobile", SqlDbType.NVarChar).Value = modelCON_Contact.ContactMobile;
            cmd.Parameters.Add("@ContactEmail", SqlDbType.NVarChar).Value = modelCON_Contact.ContactEmail;
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            conn.Close();
            return View("CON_Contact",dt);
        }
        #endregion
    }
}
