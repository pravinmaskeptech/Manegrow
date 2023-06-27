using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DataTable = System.Data.DataTable;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class RevenueReportController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: RevenueReport
        public ActionResult Index()
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

                var FROMDATE = Session["Fromdate"];
                var TODATE = Session["Todate"];

                DateTime? fromd = Convert.ToDateTime(FROMDATE);
                DateTime? tod = Convert.ToDateTime(TODATE);

                string test = string.Empty;
                sSearch = sSearch.ToLower();



                var result = (from x in db.MainOrder.Where(w => w.Date >= fromd && w.Date <= tod)
                              join c in db.CustomerMasters
                              on x.CustomerId equals c.ID
                              join p in db.ProductTypes
                              on x.CategoryId equals p.ID
                              join o in db.Order
                              on x.Id equals o.MasterOrderId

                              select new
                              {
                                  //   ID = x.Id,
                                  Date = x.Date,
                                  OrderNo = o.Id,
                                  CategoryName = p.Type,
                                  ProductName = o.ProductName,
                                  Weight = o.Weight,
                                  Qty = o.Qty,
                                  Rate = o.Rate,
                                  TotalAmount = o.Amount,


                              }).OrderBy(x => x.Date).ToList();

              //  result = result.ToList();
                //if (!string.IsNullOrEmpty(sSearch))
                //{
                //    result = result.ToList();

                //    result = result
                //      .Where(x => x.CategoryName.Contains(sSearch) || x.Date.ToString().Contains(sSearch) || x.ProductName.ToString().Contains(sSearch) || x.Qty.ToString().Contains(sSearch) || x.Weight.ToString().Contains(sSearch) || x.Rate.ToString().Contains(sSearch))
                //        // .Skip(iDisplayStart).Take(iDisplayLength)
                //       .Select(a => new
                //       {
                //           a.Date,
                //           a.OrderNo,
                //           a.CategoryName,
                //           a.ProductName,
                //           a.Weight,
                //           a.Qty,
                //           a.Rate,
                //           a.TotalAmount
                //       }).OrderBy(x => x.Date).ToList();


                //    }
                    //else
                    //{
                    //    int totalRecord = result.Count();
                    //    result = result.AsEnumerable()
                    //        .Skip(iDisplayStart).Take(iDisplayLength)
                    //        .Select(a => new {
                    //            a.Date,
                    //            a.OrderNo,
                    //            a.CategoryName,
                    //            a.ProductName,
                    //            a.Weight,
                    //            a.Qty,
                    //            a.Rate,
                    //            a.TotalAmount
                    //        }).OrderBy(x => x.Date).ToList();

                    //}
                   int totalRecord = 1;
                result = result.ToList();

                result = result.Where(a => a.Rate != null && a.Rate != 0).ToList();

                    if (!string.IsNullOrEmpty(sSearch))
                    {
                    result = result.Where(a => a.Rate != null && a.Rate != 0).ToList();
                    result = result.AsEnumerable()
                            .Where(x => x.Date.ToString().Contains(sSearch) || x.OrderNo.ToString().Contains(sSearch) || x.ProductName.ToLower().Contains(sSearch) || x.CategoryName.ToLower().Contains(sSearch))
                            .Skip(iDisplayStart).Take(iDisplayLength)
                      .Select(x => new { Date = x.Date, OrderNo = x.OrderNo, CategoryName = x.CategoryName, ProductName = x.ProductName, Weight = x.Weight, Qty = x.Qty, Rate = x.Rate, TotalAmount = x.TotalAmount }).OrderBy(x => x.Date).ToList();
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

        public ActionResult TotalSalesReport()
        {
            return View();
        }

        [HttpPost]

        public string GetData1(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                List<TotalSales> totalSales = new List<TotalSales>();
                string test = string.Empty;
                sSearch = sSearch.ToLower();
               
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
                    using (SqlCommand cmd = new SqlCommand("SP_TotalSalesReport"))
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
                                    var ts = new TotalSales();
                                    try { ts.DispatchDate = Convert.ToDateTime(row["DispatchDate"]); } catch { }
                                    try { ts.OrderQty = Convert.ToInt32(row["OrderQty"]); } catch { }
                                    try { ts.RejectedQty = Convert.ToInt32(row["RejectedQty"]); } catch { }
                                    try { ts.SoldQty = Convert.ToInt32(row["SoldQty"]); } catch { }
                                    try { ts.ScrapQty = Convert.ToInt32(row["ScrapQty"]); } catch { }
                                    try { ts.RevenueAmount = Convert.ToInt32(row["RevenueAmount"]); } catch { }
                                    try { ts.SoldAmount = Convert.ToInt32(row["SoldAmount"]); } catch { }
                                    try { ts.ScrapAmount = Convert.ToInt32(row["ScrapAmount"]); } catch { }
                                    try { ts.OrderNo = Convert.ToInt32(row["OrderNo"]); } catch { }
                                    try { ts.ResaleType = Convert.ToString(row["ResaleType"]);  } catch { }
                                    try { ts.Rate = Convert.ToInt32(row["Rate"]); } catch { }
                                    totalSales.Add(ts);
                                }

                            }
                        }

                        
                    }
                }

                int totalRecord = 1;
               //var result = totalSales.ToList();
                var result = totalSales
                  .AsEnumerable()
                  .Select(x => new TotalSales { DispatchDate = x.DispatchDate, OrderQty = x.OrderQty, RejectedQty = x.RejectedQty, SoldQty = x.SoldQty,ScrapQty = x.ScrapQty, RevenueAmount = x.RevenueAmount, SoldAmount = x.SoldAmount, ScrapAmount = x.ScrapAmount, OrderNo = x.OrderNo, ResaleType = x.ResaleType, Rate=x.Rate }).OrderBy(x => x.DispatchDate).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = totalSales.AsEnumerable()
                        .Where(x => x.DispatchDate.ToString().Contains(sSearch) || x.OrderNo.ToString().Contains(sSearch) || x.ResaleType.ToLower().Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                  .Select(x => new TotalSales { DispatchDate = x.DispatchDate, OrderQty = x.OrderQty, RejectedQty = x.RejectedQty, SoldQty = x.SoldQty, ScrapQty = x.ScrapQty, RevenueAmount = x.RevenueAmount, SoldAmount = x.SoldAmount, ScrapAmount = x.ScrapAmount, OrderNo = x.OrderNo, ResaleType = x.ResaleType, Rate = x.Rate }).OrderBy(x => x.DispatchDate).ToList();
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
                using (SqlCommand cmd = new SqlCommand("SP_TotalSalesReport"))
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
                                wb.Worksheets.Add(dt, "SP_TotalSalesReport");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";

                                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
                                Response.ContentType = "application/text";
                                Response.AddHeader("content-disposition", "attachment;filename=TotalSalesReport.csv");
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

                    RedirectToAction("TotalSalesReport");
                }
            }

            return View();
        }





    }

    internal class TotalSales
    {
        public DateTime? DispatchDate { get; set; }
        public int? OrderQty { get; set; }

        public int? RejectedQty { get; set; }
        public int? SoldQty { get; set; }

        public int? ScrapQty { get; set; }

        public decimal? RevenueAmount { get; set; }
        public decimal? SoldAmount { get; set; }
        

        public decimal? ScrapAmount { get; set; }
        public int? OrderNo { get; set; }

        public string ResaleType { get; set; }
        
          public int? Rate { get; set; }

       

        
       



    }

}

