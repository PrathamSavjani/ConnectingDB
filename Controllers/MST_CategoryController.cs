using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using ConnectingDB.Models;

namespace ConnectingDB.Controllers
{
    public class MST_CategoryController : Controller
    {
        private IConfiguration Configuration;
        public MST_CategoryController(IConfiguration _configuration)
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
            cmd.CommandText = "PR_MST_Category_SelectAll";
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data       EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            return View("MST_Category", dt);
        }

        #region BTN_AddState
        public ActionResult Add(int? ContactCategoryID)
        {
            if (ContactCategoryID != null)
            {
                string str = this.Configuration.GetConnectionString("addressBookDB");
                SqlConnection conn = new SqlConnection(str);
                conn.Open();
                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "PR_MST_Category_SelectByPK";
                cmd.Parameters.Add("@ContactCategoryID", SqlDbType.Int).Value = ContactCategoryID;
                DataTable dt = new DataTable();
                SqlDataReader CountryDataByID = cmd.ExecuteReader();
                dt.Load(CountryDataByID);

                MST_CategoryModel Categorymodel = new MST_CategoryModel();

                foreach (DataRow dr in dt.Rows)
                {
                    Categorymodel.ContactCategoryID = Convert.ToInt32(dr["ContactCategoryID"]);
                    Categorymodel.ContactCategoryName = Convert.ToString(dr["ContactCategoryName"]);
                    Categorymodel.ModificationDate = Convert.ToDateTime(dr["ModificationDate"]);
                    Categorymodel.CreationDate = Convert.ToDateTime(dr["CreationDate"]);

                }
                return View("MST_CategoryAddEdit", Categorymodel);
                conn.Close();
            }

            return View("MST_CategoryAddEdit");
        }
        #endregion


        #region AddEdit
        [HttpPost]
        public IActionResult Save(MST_CategoryModel modelMST_Category)
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve Country List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;

            if (modelMST_Category.ContactCategoryID == null)
            {
                cmd.CommandText = "PR_MST_Category_Insert";
                cmd.Parameters.Add("@CreationDate", SqlDbType.Date).Value = modelMST_Category.CreationDate;
            }
            else
            {
                cmd.CommandText = "PR_MST_Category_UpdateByPK";
                cmd.Parameters.Add("@ContactCategoryID", SqlDbType.Int).Value = modelMST_Category.ContactCategoryID;
            }
            cmd.Parameters.Add("@ContactCategoryName", SqlDbType.NVarChar).Value = modelMST_Category.ContactCategoryName;
            cmd.Parameters.Add("@ModificationDate", SqlDbType.Date).Value = modelMST_Category.ModificationDate;

            if (Convert.ToBoolean(cmd.ExecuteNonQuery()))
            {
                if (modelMST_Category.ContactCategoryID == null)//doubt
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


        public ActionResult Delete(int ContactCategoryID)
        {
            //Establish connection using ConnectionStrings
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();

            //SQL Command to retrieve Country List 
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MST_Category_DeleteByPK";
            cmd.Parameters.AddWithValue("@ContactCategoryID", ContactCategoryID);
            cmd.ExecuteNonQuery();
            conn.Close();

            return RedirectToAction("Index");
        }


        #region Cancel
        public IActionResult Back()
        {
            return Index();
        }
        #endregion


        #region Filter Records
        public IActionResult Filter(string ContactCategoryName)
        {
            string str = this.Configuration.GetConnectionString("addressBookDB");
            SqlConnection conn = new SqlConnection(str);
            conn.Open();
            SqlCommand cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PR_MST_Category_FilterByContactCategory";
            cmd.Parameters.Add("@ContactCategoryName", SqlDbType.NVarChar).Value = ContactCategoryName;
            DataTable dt = new DataTable();
            SqlDataReader sdr = cmd.ExecuteReader();//ExecuteReader()-Read All Data      EecuteScalar-Single Row and Single Column      ExecuteNonQuery()-Insert Update Delete
            dt.Load(sdr);
            conn.Close();
            return View("MST_Category", dt);
        }
        #endregion
    }
}
