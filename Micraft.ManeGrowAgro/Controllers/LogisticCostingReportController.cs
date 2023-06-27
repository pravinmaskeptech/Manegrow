using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class LogisticCostingReportController : Controller
    {
        // GET: LogisticCostingReport
        ManeGrowContext db = new ManeGrowContext();
        public ActionResult LogisticCostingReport()
        {
            return View();
        }

        public ActionResult SetDates(string FROMDATE, string TODATE)
        {

            Session["Fromdate"] = FROMDATE;
            Session["Todate"] = TODATE;

            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {

                List<LogisticCostingReport > result = new List<LogisticCostingReport>();
                var frmdate = Session["FromDate"];
                var todate = Session["ToDate"];


                DateTime? Fromdate = null;
                DateTime? Todate = null;


                if (todate != "")
                {
                    Todate = Convert.ToDateTime(todate);
                }
                else
                {
                    Todate = DateTime.Today;
                }
                if (todate != "")
                {
                    Fromdate = Convert.ToDateTime(frmdate);
                }
                else
                {
                    Fromdate = DateTime.Today;
                }
                using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_LogisticCostringReport"))
                    {


                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@FromDt", Fromdate);
                            cmd.Parameters.AddWithValue("@ToDt", Todate);

                            cmd.Connection = con;
                            cmd.CommandTimeout = 1800;
                            sda.SelectCommand = cmd;

                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);
                                con.Close();
                                cmd.Dispose();


                                foreach (DataRow row in dt.Rows)
                                {
                                    var ps = new LogisticCostingReport();
                                    try { ps.VechicleNo = Convert.ToString(row["VechicleNo"]); } catch { }
                                    try { ps.ClientName = Convert.ToString(row["ClientName"]); } catch { }
                                    try { ps.Destination = Convert.ToString(row["Destination"]); } catch { }
                                    try { ps.Ammount = Convert.ToDecimal(row["Ammount"]); } catch { }
                                    try { ps.UOM = Convert.ToString(row["UOM"]); } catch { }
                                    try { ps.CNG = Convert.ToInt32(row["CNG"]); } catch { }
                                    try { ps.Weight = Convert.ToDecimal(row["Weight"]); } catch { }
                                    try { ps.DispatchDate = Convert.ToDateTime(row["DispatchDate"]); } catch { }
                                    try { ps.Box = Convert.ToDecimal(row["Box"]); } catch { }
                                    result.Add(ps);
                                }

                            }
                        }


                    }
                }

                int totalRecord = result.Count();

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

        public ActionResult Export()
        {
            var frmdate = Session["FromDate"];
            var todate = Session["ToDate"];

            DateTime? Fromdate = null;
            DateTime? Todate = null;


            if (todate != "")
            {
                Todate = Convert.ToDateTime(todate);
            }
            else
            {
                Todate = DateTime.Today;
            }
            if (todate != "")
            {
                Fromdate = Convert.ToDateTime(frmdate);
            }
            else
            {
                Fromdate = DateTime.Today;
            }
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("sp_LogisticCostringReport"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDt", Fromdate);
                        cmd.Parameters.AddWithValue("@ToDt", Todate);

                        cmd.Connection = con;
                        cmd.CommandTimeout = 1800;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            con.Close();
                            cmd.Dispose();

                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dt, "LogisticCostringReport");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";

                                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
                                Response.ContentType = "application/text";
                                Response.AddHeader("content-disposition", "attachment;filename=LogisticCostingReport.excl");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                    }

                    RedirectToAction("LogisticCostingReport");
                }
            }

            return View();
        }
    }
}