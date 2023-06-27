using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Micraft.ManeGrowAgro.Models;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class ProductSalesReportController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: ProductSalesReport
        public ActionResult ProductSalesReport()
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
                List<ProductSales> productsales = new List<ProductSales>();
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
                    using (SqlCommand cmd = new SqlCommand("SP_ProductSalesReport"))
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
                                    var ps = new ProductSales();
                                    try { ps.ProductType = Convert.ToString(row["ProductType"]); } catch { }
                                    try { ps.ProductName = Convert.ToString(row["ProductName"]); } catch { }
                                    try { ps.CustomerType = Convert.ToString(row["CustomerType"]); } catch { }
                                    try { ps.Amount = Convert.ToInt32(row["Amount"]); } catch { }
                                    try { ps.Average = Convert.ToInt32(row["Average"]); } catch { }
                                    try { ps.PercentageofAmount = Convert.ToInt32(row["PercentageofAmount"]); } catch { }
                                    
                                    productsales.Add(ps);
                                }

                            }
                        }


                    }
                }

                int totalRecord = 1;
                //var result = productsales.ToList();
                var result = productsales
                   .AsEnumerable()
                   .Select(x => new ProductSales { ProductType = x.ProductType, ProductName = x.ProductName, CustomerType = x.CustomerType, Amount = x.Amount, PercentageofAmount = x.PercentageofAmount }).OrderBy(x => x.ProductType).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = productsales.AsEnumerable()
                        .Where(x => x.ProductType.ToLower().Contains(sSearch) || x.ProductName.ToLower().Contains(sSearch) || x.CustomerType.ToLower().Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                    .Select(x => new ProductSales { ProductType = x.ProductType, ProductName = x.ProductName, CustomerType = x.CustomerType, Amount = x.Amount, PercentageofAmount = x.PercentageofAmount }).OrderBy(x => x.ProductType).ToList();
                }
                //else
                //{
                //    result = productsales.ToList();
                //}



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
                using (SqlCommand cmd = new SqlCommand("SP_ProductSalesReport"))
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
                                wb.Worksheets.Add(dt, "SP_ProductSalesReport");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";

                                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
                                Response.ContentType = "application/text";
                                Response.AddHeader("content-disposition", "attachment;filename=ProductSalesReport.csv");
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

                    RedirectToAction("ProductSalesReport");
                }
            }

            return View();
        }

    }

    internal class ProductSales
    {
        public string ProductType { get; set; }
        public string ProductName { get; set; }
        public string CustomerType { get; set; }
        public int? Amount { get; set; }
        public int? Average { get; set; } 
        public decimal? PercentageofAmount { get; set; }
    }
}