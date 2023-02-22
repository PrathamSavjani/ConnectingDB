using ConnectingDB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace ConnectingDB.Controllers
{
	public class LOC_CountryController : Controller
	{
        private IConfiguration Configuration;
        public LOC_CountryController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }//doubt

        #region BTN_AddCountry
        public ActionResult Add(int? CountryID)
        {
            if(CountryID != null)
            { 
                string str = this.Configuration.GetConnectionString("addressBookDB");
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_LOC_Country_SelectByPK";
                cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = CountryID;
                DataTable dt = new DataTable();
                SqlDataReader CountryDataByID = cmd.ExecuteReader();
                dt.Load(CountryDataByID);

                LOC_CountryModel Countrymodel = new LOC_CountryModel();
                
                foreach(DataRow dr in dt.Rows)
                {
                    Countrymodel.CountryID = Convert.ToInt32(dr["CountryID"]);
                    Countrymodel.CountryName = Convert.ToString(dr["CountryName"]);
                    Countrymodel.CountryCode = Convert.ToString(dr["CountryCode"]);
                    Countrymodel.ModificationDate = Convert.ToDateTime(dr["ModificationDate"]);
                    Countrymodel.CreationDate = Convert.ToDateTime(dr["CreationDate"]);
                    Countrymodel.PhotoPath = Convert.ToString(dr["PhotoPath"]);

                }
                return View("LOC_CountryAddEdit", Countrymodel);
                conn.Close();
            }

            return View("LOC_CountryAddEdit");
        }
        #endregion

        #region SelectAll
        public ActionResult Index()
		{
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_Country_SelectAll";
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
			return View("LOC_Country",dt);
		}
        #endregion

        #region Delete
        public ActionResult Delete(int CountryID)
        {
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve Country List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_Country_DeleteByPK";
            cmd.Parameters.AddWithValue("@CountryID", CountryID);
            cmd.ExecuteNonQuery();
            conn.Close();
            TempData["DeleteMessage"] = "You Have Deleted Record Successfully";
            return RedirectToAction("Index");
        }
        #endregion

        #region Cancel
        public IActionResult Back()
        {
            return Index();
        }
        #endregion

        #region AddEdit
        [HttpPost]
        public IActionResult Save(LOC_CountryModel modelLOC_Country)
        {
            //Upload starts here
            if(modelLOC_Country.File != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path,modelLOC_Country.File.FileName);
                modelLOC_Country.PhotoPath = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + modelLOC_Country.File.FileName;
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    modelLOC_Country.File.CopyTo(stream);
                }
            }

            if (ModelState.IsValid)
            {

                string str = this.Configuration.GetConnectionString("addressBookDB");
                SqlConnection conn = new SqlConnection(str);
                conn.Open();

                //SQL Command to retrieve Country List 
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;

                if (modelLOC_Country.CountryID == null)
                {
                    cmd.CommandText = "PR_LOC_Country_Insert";
                    cmd.Parameters.Add("@CreationDate", SqlDbType.Date).Value = modelLOC_Country.CreationDate;
                }
                else
                {
                    cmd.CommandText = "PR_LOC_Country_UpdateByPK";
                    cmd.Parameters.Add("@CountryID", SqlDbType.NVarChar).Value = modelLOC_Country.CountryID;
                }
                cmd.Parameters.Add("@PhotoPath", SqlDbType.NVarChar).Value = modelLOC_Country.PhotoPath;
                cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = modelLOC_Country.CountryName;
                cmd.Parameters.Add("@CountryCode", SqlDbType.NVarChar).Value = modelLOC_Country.CountryCode;
                cmd.Parameters.Add("@ModificationDate", SqlDbType.Date).Value = modelLOC_Country.ModificationDate;

                if (Convert.ToBoolean(cmd.ExecuteNonQuery()))
                {
                    if (modelLOC_Country.CountryID == null)//doubt
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

        #region Filter Records
        public IActionResult Filter(string? CountryName=null, string? CountryCode=null)
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_Country_FilterByCountryNameAndCode";
            if(CountryName != null && CountryCode != null)
            {
                cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountryName;
                cmd.Parameters.Add("@CountryCode", SqlDbType.NVarChar).Value = CountryCode;
            }
            if(CountryName == null && CountryCode != null)
            {
                cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = DBNull.Value;
                cmd.Parameters.Add("@CountryCode", SqlDbType.NVarChar).Value = CountryCode;
            }
            if (CountryName != null && CountryCode == null)
            {
                cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountryName;
                cmd.Parameters.Add("@CountryCode", SqlDbType.NVarChar).Value = DBNull.Value;
            }
            if (CountryName == null && CountryCode == null)
            {
                cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = DBNull.Value;
                cmd.Parameters.Add("@CountryCode", SqlDbType.NVarChar).Value = DBNull.Value;
            }
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            conn.Close();
            return View("LOC_Country", dt);
        }
        #endregion
    }
}
