using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ConnectingDB.Models;

namespace ConnectingDB.Controllers
{
    public class LOC_CityController : Controller
    {
        private IConfiguration Configuration;
        public LOC_CityController(IConfiguration _configuration)
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
            cmd.CommandText = "PR_LOC_City_SelectAll";
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            return View("LOC_City", dt);
        }

        #region BTN_AddState
        public ActionResult Add(int? CityID)
        {
            List<LOC_StateDropDownModel> list = new List<LOC_StateDropDownModel>();
            ViewBag.StateList = list;

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

            if (CityID != null)
            {
                string stri = this.Configuration.GetConnectionString("addressBookDB");
                SqlConnection conne = new SqlConnection(stri);
                conne.Open();
                SqlCommand cmdn = conne.CreateCommand();
                cmdn.CommandType = CommandType.StoredProcedure;
                cmdn.CommandText = "PR_LOC_City_SelectByPK";
                cmdn.Parameters.Add("@CityID", SqlDbType.Int).Value = CityID;
                DataTable dt1 = new DataTable();
                SqlDataReader CountryDataByID = cmdn.ExecuteReader();
                dt1.Load(CountryDataByID);

                LOC_CityModel Citymodel = new LOC_CityModel(); 

                foreach (DataRow dr in dt1.Rows)
                {
                    Citymodel.CityID = Convert.ToInt32(dr["CityID"]);
                    Citymodel.CityName = Convert.ToString(dr["CityName"]);
                    Citymodel.CountryID = Convert.ToInt32(dr["CountryID"]);
                    Citymodel.StateID = Convert.ToInt32(dr["StateID"]);
                    Citymodel.ModificationDate = Convert.ToDateTime(dr["ModificationDate"]);
                    Citymodel.CreationDate = Convert.ToDateTime(dr["CreationDate"]);

                }
                DataTable dt2 = new DataTable();
                SqlConnection conn2 = new SqlConnection(stri);
                conn2.Open();
                SqlCommand cmd2 = conn2.CreateCommand();
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.CommandText = "PR_LOC_State_SelectComboBoxByCountryID";
                cmd2.Parameters.AddWithValue("@CountryID", Citymodel.CountryID);
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

                return View("LOC_CityAddEdit", Citymodel);
                conne.Close();
            }

            return View("LOC_CityAddEdit");
        }
        #endregion

        #region back
        public IActionResult Back()
        {
            return Index();
        }
        #endregion

        #region Save
        public IActionResult Save(LOC_CityModel modelLOC_City)
        {
            if (ModelState.IsValid)
            {
                string str = this.Configuration.GetConnectionString("addressBookDB");
                SqlConnection conn = new SqlConnection(str);
                conn.Open();

                //SQL Command to retrieve Country List 
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;

                if (modelLOC_City.CityID == null)
                {
                    cmd.CommandText = "PR_LOC_City_Insert";
                    cmd.Parameters.Add("@CreationDate", SqlDbType.Date).Value = modelLOC_City.CreationDate;
                }
                else
                {
                    cmd.CommandText = "PR_LOC_City_UpdateByPK";
                    cmd.Parameters.Add("@CityID", SqlDbType.Int).Value = modelLOC_City.CityID;
                }
                cmd.Parameters.Add("@StateID", SqlDbType.Int).Value = modelLOC_City.StateID;
                cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = modelLOC_City.CountryID;
                cmd.Parameters.Add("@CityName", SqlDbType.NVarChar).Value = modelLOC_City.CityName.Trim();
                cmd.Parameters.Add("@ModificationDate", SqlDbType.Date).Value = modelLOC_City.ModificationDate;

                if (Convert.ToBoolean(cmd.ExecuteNonQuery()))
                {
                    if (modelLOC_City.CityID == null)//doubt
                    {
                        TempData["InsertMessage"] = "You Have Inserted Record Successfully";
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "You Have Updated Record Successfully";
                    }
                }
                conn.Close();
            }
            return RedirectToAction("Index");
        }
        #endregion

        public ActionResult Delete(int CityID)
        {
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve Country List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_City_DeleteByPK";
            cmd.Parameters.AddWithValue("@CityID", CityID);
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
            return View("LOC_CityAddEdit");
        }
        #region Filter Records
        public IActionResult Filter(string CityName, string StateName, string CountryName)
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_City_FilterByCityNameAndCode";
            cmd.Parameters.Add("@StateName", SqlDbType.NVarChar).Value = StateName;
            cmd.Parameters.Add("@CityName", SqlDbType.NVarChar).Value = CityName;
            cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountryName;
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            conn.Close();
            return View("LOC_City", dt);
        }
        #endregion
    }
}