using ConnectingDB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace ConnectingDB.Controllers
{
    public class LOC_StateController : Controller
    {
        private IConfiguration Configuration;
        public LOC_StateController(IConfiguration _configuration)
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
            cmd.CommandText = "PR_LOC_State_SelectAll";
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            return View("LOC_State", dt);
        }

        #region BTN_AddState
        public IActionResult Add(int? StateID)
        {
            #region Dropdown
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve State List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_Country_SelectForDropDown";

            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            conn.Close();

            List<LOC_CountryDropDownModel> list = new List<LOC_CountryDropDownModel>();
            foreach (DataRow dr in dt.Rows)
            {
                LOC_CountryDropDownModel vlist = new LOC_CountryDropDownModel();
                vlist.CountryID = Convert.ToInt32(dr["CountryID"]);
                vlist.CountryName = dr["CountryName"].ToString();
                list.Add(vlist);
            }
            ViewBag.CountryList = list;
            #endregion

            #region Record Select by PK 
            if (StateID != null)
            {
                string stri = this.Configuration.GetConnectionString("addressBookDB");
                SqlConnection conn1 = new SqlConnection(stri);
                conn1.Open();

                SqlCommand cmd1 = conn1.CreateCommand();
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.CommandText = "PR_LOC_State_SelectByPK";
                cmd1.Parameters.Add("@StateID", SqlDbType.Int).Value = StateID;
                DataTable dt1 = new DataTable();
                SqlDataReader sr = cmd1.ExecuteReader();
                dt1.Load(sr);

                LOC_StateModel modelLOC_State = new LOC_StateModel();

                foreach (DataRow dr in dt1.Rows)
                {
                    modelLOC_State.StateName = dr["StateName"].ToString();
                    modelLOC_State.CountryID = Convert.ToInt32(dr["CountryID"]);
                    modelLOC_State.StateCode = dr["StateCode"].ToString();
                    modelLOC_State.CreationDate = Convert.ToDateTime(dr["CreationDate"]);
                    modelLOC_State.ModificationDate = Convert.ToDateTime(dr["ModificationDate"]);
                    modelLOC_State.PhotoPath = Convert.ToString(dr["PhotoPath"]);
                }
                return View("LOC_StateAddEdit", modelLOC_State);
            }
            #endregion

            return View("LOC_StateAddEdit");
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
        public IActionResult Save(LOC_StateModel modelLOC_State)
        {
            //Upload starts here
            if (modelLOC_State.File != null)
            {
                string FilePath = "wwwroot\\Upload";
                string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileNameWithPath = Path.Combine(path, modelLOC_State.File.FileName);
                modelLOC_State.PhotoPath = "~" + FilePath.Replace("wwwroot\\", "/") + "/" + modelLOC_State.File.FileName;
                using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                {
                    modelLOC_State.File.CopyTo(stream);
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

                if (modelLOC_State.StateID == null)
                {
                    cmd.CommandText = "PR_LOC_State_Insert";
                    cmd.Parameters.Add("@CreationDate", SqlDbType.Date).Value = modelLOC_State.CreationDate;
                }
                else
                {
                    cmd.CommandText = "PR_LOC_State_UpdateByPK";
                    cmd.Parameters.Add("@StateID", SqlDbType.Int).Value = modelLOC_State.StateID;
                }
                cmd.Parameters.Add("@PhotoPath", SqlDbType.NVarChar).Value = modelLOC_State.PhotoPath;
                cmd.Parameters.Add("@CountryID", SqlDbType.Int).Value = modelLOC_State.CountryID;
                cmd.Parameters.Add("@StateName", SqlDbType.NVarChar).Value = modelLOC_State.StateName;
                cmd.Parameters.Add("@StateCode", SqlDbType.NVarChar).Value = modelLOC_State.StateCode;
                cmd.Parameters.Add("@ModificationDate", SqlDbType.Date).Value = modelLOC_State.ModificationDate;

                if (Convert.ToBoolean(cmd.ExecuteNonQuery()))
                {
                    if (modelLOC_State.StateID == null)//doubt
                    {
                        TempData["InsertMessage"] = "You Have Inserted Record Successfully";
                    }
                    else
                    {
                        TempData["UpdateMessage"] = "You Have Updated Record Successfully";
                    }
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        public ActionResult Delete(int StateID)
        {
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve Country List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_State_DeleteByPK";
            cmd.Parameters.AddWithValue("@StateID", StateID);
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Index");
        }

        #region Filter Records
        public IActionResult Filter(LOC_StateModel modelLOC_State, string? CountryName)
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_LOC_State_FilterByStateNameAndCode";
            cmd.Parameters.Add("@StateName", SqlDbType.NVarChar).Value = modelLOC_State.StateName;
            cmd.Parameters.Add("@StateCode", SqlDbType.NVarChar).Value = modelLOC_State.StateCode;
            cmd.Parameters.Add("@CountryName", SqlDbType.NVarChar).Value = CountryName;
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            conn.Close();
            return View("LOC_State", dt);
        }
        #endregion
    }
}
