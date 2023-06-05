using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Data.OleDb;
using System.Net;
using Micraft.ManeGrowAgro.Security;

namespace Micraft.ManeGrowAgro.Controllers
{
    [Authorize] 
    public class MonthlyProdPlanController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        [CustomAuthorize("You dont have access to View Monthly Prod Plan", "HarvestingEdit,HarvestingView, Admin")]
        public ActionResult Index() 
        {

            return View();
        }

        [CustomAuthorize("You dont have access to Create Monthly Prod Plan", "HarvestingEdit,Admin")]
        public ActionResult Create() 
        {
            ViewBag.RoomNo = db.RoomMaster.ToList();
            return View();
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.MonthlyProdPlan.Count(); 

                var result = db.MonthlyProdPlan
                    .AsEnumerable()
                    .Select(x => new  { ID = x.ID, Year = x.Year, Month = x.Month, RoomNo=x.RoomNo, Total=x.Total }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.MonthlyProdPlan.AsEnumerable()
                        .Where(x => x.ID.ToString().Contains(sSearch) || x.Year.ToString().Contains(sSearch) || x.RoomNo.ToString().Contains(sSearch) || x.Total.ToString().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new  { ID = x.ID, Year = x.Year, Month = x.Month, RoomNo = x.RoomNo, Total = x.Total }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.MonthlyProdPlan.AsEnumerable() 
                        .Select(x => new { x.ID, x.Year,  x.Month,  x.RoomNo,  x.Total }).OrderByDescending(x => x.ID).ToList();
                }


                StringBuilder sb = new StringBuilder();
                sb.Clear();
                sb.Append("{");
                sb.Append("\"sEcho\": ");
                sb.Append(sEcho);
                sb.Append(",");
                sb.Append("\"iTotalRecords\": ");
                sb.Append(totalRecord);
                sb.Append(",");
                sb.Append("\"iTotalDisplayRecords\": ");
                sb.Append(totalRecord);
                sb.Append(",");
                sb.Append("\"aaData\": ");
                sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                sb.Append("}");
                return sb.ToString();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Save(List<MonthlyProdPlan> recs)
        {
            try
            {
                //save without excel.
                var result = new { Message = "success" };
                if (recs == null)
                {
                    recs = new List<MonthlyProdPlan>();
                    result = new { Message = "failed" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //Loop and insert records.
                foreach (MonthlyProdPlan monthlyprodplan in recs)
                {
                    var MonthlyData = db.MonthlyProdPlan.ToList();
                    var isexist = MonthlyData.Where(x => x.Year == monthlyprodplan.Year && x.Month == monthlyprodplan.Month && x.RoomNo == monthlyprodplan.RoomNo).FirstOrDefault();

                    if (isexist == null)
                    {
                        //int daysinmonth = DateTime.DaysInMonth(monthlyprodplan.Year, monthlyprodplan.Month);

                        var total = 0;

                        for (int i=1;i<=31;i++)
                        {
                            var DayName = "D" + i.ToString();
                             var val=  monthlyprodplan.GetType().GetProperty(DayName).GetValue(monthlyprodplan);

                            var val2 = Convert.ToInt32(val);
                            total = total + val2;
                        }

                        monthlyprodplan.Total = total;
                        monthlyprodplan.CreatedBy = User.Identity.Name;
                        monthlyprodplan.CreatedDate = DateTime.Now;
                        db.MonthlyProdPlan.Add(monthlyprodplan);
                    }
                    else if (isexist != null)
                    {
                        isexist.D1 = monthlyprodplan.D1;
                        isexist.D2 = monthlyprodplan.D2;
                        isexist.D3 = monthlyprodplan.D3;
                        isexist.D4 = monthlyprodplan.D4;
                        isexist.D5 = monthlyprodplan.D5;
                        isexist.D6 = monthlyprodplan.D6;
                        isexist.D7 = monthlyprodplan.D7;
                        isexist.D8 = monthlyprodplan.D8;
                        isexist.D9 = monthlyprodplan.D9;
                        isexist.D10 = monthlyprodplan.D10;
                        isexist.D11 = monthlyprodplan.D11;
                        isexist.D12 = monthlyprodplan.D12;
                        isexist.D13 = monthlyprodplan.D13;
                        isexist.D14 = monthlyprodplan.D14;
                        isexist.D15 = monthlyprodplan.D15;
                        isexist.D16 = monthlyprodplan.D16;
                        isexist.D17 = monthlyprodplan.D17;
                        isexist.D18 = monthlyprodplan.D18;
                        isexist.D19 = monthlyprodplan.D19;
                        isexist.D20 = monthlyprodplan.D20;
                        isexist.D21 = monthlyprodplan.D21;
                        isexist.D22 = monthlyprodplan.D22;
                        isexist.D23 = monthlyprodplan.D23;
                        isexist.D24 = monthlyprodplan.D24;
                        isexist.D25 = monthlyprodplan.D25;
                        isexist.D26 = monthlyprodplan.D26;
                        isexist.D27 = monthlyprodplan.D27;
                        isexist.D28 = monthlyprodplan.D28;
                        isexist.D29 = monthlyprodplan.D29;
                        isexist.D30 = monthlyprodplan.D30;
                        isexist.D31 = monthlyprodplan.D31;

                        var total = 0;

                        for (int i = 1; i <= 31; i++)
                        {
                            var DayName = "D" + i.ToString();
                            var val = monthlyprodplan.GetType().GetProperty(DayName).GetValue(monthlyprodplan);

                            var val2 = Convert.ToInt32(val);
                            total = total + val2;
                        }

                        isexist.Total = total;

                        isexist.UpdatedBy = User.Identity.Name;
                        isexist.UpdatedDate = DateTime.Now;

                        db.Entry(isexist).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }

                //string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
                //SqlConnection con = new SqlConnection(constring);

                //SqlCommand com = new SqlCommand("SP_UpdateMonthlyProdPlanTotal", con);

                //com.CommandType = CommandType.StoredProcedure;
                //com.Parameters.AddWithValue("@Year", recs[0].Year);
                //com.Parameters.AddWithValue("@Month", recs[0].Month);
                
                //con.Open();
                //com.ExecuteNonQuery();
                //con.Close();

                result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadExcel(HttpPostedFileBase postedFile)
        {
            try
            {

                var todelete=db.MonthlyExcel.ToList();
                db.MonthlyExcel.RemoveRange(todelete);
                db.SaveChanges();

                string filePath = string.Empty;
                string extension = string.Empty;
                if (Request.Files.Count > 0)
                {
                    if (Request.Files.Count > 0)
                    {
                        var files = Request.Files;
                        //iterating through multiple file collection   
                        foreach (string str in files)
                        {
                            HttpPostedFileBase file = Request.Files[str] as HttpPostedFileBase;
                            if (file != null)
                            {
                                string path = Server.MapPath("~/Uploads/");
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                filePath = path + Path.GetFileName(file.FileName);
                                extension = Path.GetExtension(file.FileName);
                                file.SaveAs(filePath);
                            }
                        }
                    }
                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                    }
                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();
                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();
                            }
                        }
                    }
                    
                    //remove duplicate recors in excel
                    dt = dt.DefaultView.ToTable(true, "Room No", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31");

                    conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "dbo.MonthlyExcel";
                            sqlBulkCopy.ColumnMappings.Add("Room No", "RoomNo");
                            sqlBulkCopy.ColumnMappings.Add("1", "C1");
                            sqlBulkCopy.ColumnMappings.Add("2", "C2");
                            sqlBulkCopy.ColumnMappings.Add("3", "C3");
                            sqlBulkCopy.ColumnMappings.Add("4", "C4");
                            sqlBulkCopy.ColumnMappings.Add("5", "C5");
                            sqlBulkCopy.ColumnMappings.Add("6", "C6");
                            sqlBulkCopy.ColumnMappings.Add("7", "C7");
                            sqlBulkCopy.ColumnMappings.Add("8", "C8");
                            sqlBulkCopy.ColumnMappings.Add("9", "C9");
                            sqlBulkCopy.ColumnMappings.Add("10", "C10");
                            sqlBulkCopy.ColumnMappings.Add("11", "C11");
                            sqlBulkCopy.ColumnMappings.Add("12", "C12");
                            sqlBulkCopy.ColumnMappings.Add("13", "C13");
                            sqlBulkCopy.ColumnMappings.Add("14", "C14");
                            sqlBulkCopy.ColumnMappings.Add("15", "C15");
                            sqlBulkCopy.ColumnMappings.Add("16", "C16");
                            sqlBulkCopy.ColumnMappings.Add("17", "C17");
                            sqlBulkCopy.ColumnMappings.Add("18", "C18");
                            sqlBulkCopy.ColumnMappings.Add("19", "C19");
                            sqlBulkCopy.ColumnMappings.Add("20", "C20");
                            sqlBulkCopy.ColumnMappings.Add("21", "C21");
                            sqlBulkCopy.ColumnMappings.Add("22", "C22");
                            sqlBulkCopy.ColumnMappings.Add("23", "C23");
                            sqlBulkCopy.ColumnMappings.Add("24", "C24");
                            sqlBulkCopy.ColumnMappings.Add("25", "C25");
                            sqlBulkCopy.ColumnMappings.Add("26", "C26");
                            sqlBulkCopy.ColumnMappings.Add("27", "C27");
                            sqlBulkCopy.ColumnMappings.Add("28", "C28");
                            sqlBulkCopy.ColumnMappings.Add("29", "C29");
                            sqlBulkCopy.ColumnMappings.Add("30", "C30");
                            sqlBulkCopy.ColumnMappings.Add("31", "C31");

                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }
                }

                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var result = new { Message = e.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTblData(/*string SETTAG*/)
        {
            var tbldata = db.MonthlyExcel.ToList();

            var todelete = db.MonthlyExcel.ToList();
            db.MonthlyExcel.RemoveRange(todelete);
            db.SaveChanges();

            return Json(tbldata, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [CustomAuthorize("You dont have access to Delete Monthly Prod Plan", "HarvestingEdit,Admin")] 
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var MonthlyProdPlan = db.MonthlyProdPlan.Where(a => a.ID == id).FirstOrDefault();
                db.MonthlyProdPlan.Remove(MonthlyProdPlan);
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult GetRecord(int ID)
        {
            try
            {
                var result = db.MonthlyProdPlan.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

      

    }
}