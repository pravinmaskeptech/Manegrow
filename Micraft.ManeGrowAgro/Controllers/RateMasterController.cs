using iTextSharp.text;
using iTextSharp.text.pdf;
using Micraft.ManeGrowAgro.Models;
using Micraft.ManeGrowAgro.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class RateMasterController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: RateMaster
        [CustomAuthorize("You dont have access to View Rate Updation", "Admin,RateUpdationEdit,PODUpdationView")]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetDamageDetails( string DATE)
        {
            var dt = DateTime.ParseExact(DATE, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            
          var  isdisabled = false;
            var dtt = DateTime.Today;
            var dttt = dtt.AddDays(-7);

            if(dt != dtt && dt < dttt)
            {
                isdisabled=  true;
            }

            var prd = (from pm in db.Order.Where(a => a.Date == dt)

                       join Mainorderr in db.MainOrder on pm.MasterOrderId equals Mainorderr.Id   into Mainorderdata
                       from rt in Mainorderdata.DefaultIfEmpty()

                       join customer in db.CustomerMasters on rt.CustomerId equals customer.ID into CustData
                       from cust in CustData.DefaultIfEmpty() 

                       join product in db.ProductMasters on pm.ProductId equals product.ID into prddata
                       from prdd in prddata.DefaultIfEmpty() 

                       select new
                       {
                           ID= pm.Id,
                           OrderNo = pm.Id,
                           Qty = pm.Qty, 
                           Date = pm.Date,                         
                           CustomerName = rt.CustomerName == null ? String.Empty : rt.CustomerName,
                           CustomerCode = rt.CustomerShortCode == null ? String.Empty : rt.CustomerShortCode,
                           Location = rt.Location == null ? String.Empty : rt.Location,                           
                           Weight = prdd.Weight == null ? 0 : prdd.Weight,
                           ProductName = pm.ProductName,
                           UOM = pm.UOM,
                           //Weight = pm.Weight,                          
                           TodayRate = pm.TodaysRate,
                           CreatedBy = pm.TodaysRateUpdatedBy,
                           CreatedDate = pm.TodaysRateDate,
                           QuotationID = cust.QuotationID == null ? 0 : cust.QuotationID
                       }).OrderBy(a => a.CustomerName).ToList();

            prd = prd.Where(a => a.QuotationID == null || a.QuotationID == 0).ToList();
            var result = new { Message = "success", prd, isdisabled };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [CustomAuthorize("You dont have access to View Rate Updation", "Admin,RateUpdationEdit")]
        public ActionResult Create()
        {
          
            return View();
        }




       



        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
               // var dt = DateTime.ParseExact(DATE, "dd-MM-yyyy", CultureInfo.InvariantCulture);
               // var rates = db.RateMasters.Where(a => a.Date == dt);



                var result = (from pm in db.Order.Where(a => a.Date == DateTime.Today && a.TodaysRate !=null )
                              join Mainorderr in db.MainOrder on pm.MasterOrderId equals Mainorderr.Id into Mainorderdata
                              from rt in Mainorderdata.DefaultIfEmpty()


                              join product in db.ProductMasters on pm.ProductId equals product.ID into prddata
                              from prdd in prddata.DefaultIfEmpty()

                              select new
                              {
                                  ID = pm.Id, 
                                  OrderNo = pm.Id,
                                  Qty = pm.Qty,
                                  Date = pm.Date,
                                  CustomerName = rt.CustomerName == null ? String.Empty : rt.CustomerName,
                                  Weight = prdd.Weight == null ? 0 : prdd.Weight,
                                  ProductName = pm.ProductName,
                                  UOM = pm.UOM,
                                  //Weight = pm.Weight,

                                  TodayRate = pm.TodaysRate,
                                  CreatedBy = pm.TodaysRateUpdatedBy,
                                  CreatedDate = pm.TodaysRateDate,

                              }).OrderBy(a => a.CustomerName).ToList();


                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = result.Where(a => a.ProductName.ToLower().Contains(sSearch)).OrderByDescending(a => a.ID).ToList();
                }
                else
                {
                    result = result.OrderByDescending(a => a.ID).ToList();
                }
               
                if (User.IsInRole("admin"))
                {
                    result = result.Where(a => a.CreatedBy == User.Identity.Name).ToList();
                }
                
                int totalRecords = result.Count();
                StringBuilder sb = new StringBuilder();
                sb.Clear();
                sb.Append("{");
                sb.Append("\"sEcho\":");
                sb.Append(sEcho);
                sb.Append(",");
                sb.Append("\"iTotalRecords\":");
                sb.Append(totalRecords);
                sb.Append(",");
                sb.Append("\"iTotalDisplayRecords\":");
                sb.Append(totalRecords);
                sb.Append(",");
                sb.Append("\"aaData\":");
                sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                sb.Append("}");
                return sb.ToString();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PODUpdation productDamage = await db.PODUpdations.FindAsync(id);
            if (productDamage == null)
            {
                return HttpNotFound();
            }
            return View(productDamage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PODUpdation productDamageDetails)
        {

            db.Entry(productDamageDetails).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Record Edited Successfully";
            return RedirectToAction("Index");
        }




        [HttpPost]
        public JsonResult SaveData(List<RateMaster> recs, string DATE)
        {
            try
            {
                var dt = DateTime.ParseExact(DATE, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                foreach (var x in recs)
                {
                    var ratedata = db.Order.Where(a => a.Id == x.OrderNo).FirstOrDefault();
                   if(ratedata.OrderStage>=8)
                    {
                        ratedata.TodaysRate = x.TodayRate;
                        ratedata.Rate = x.TodayRate;
                        ratedata.Amount= ratedata.Qty*x.TodayRate;
                        ratedata.TodaysRateDate = dt;
                        ratedata.TodaysRateUpdatedBy = User.Identity.Name;
                    }
                    else
                    {
                        ratedata.TodaysRate = x.TodayRate;
                      //  ratedata.Rate = x.TodayRate;
                        ratedata.TodaysRateDate = dt;
                        ratedata.TodaysRateUpdatedBy = User.Identity.Name;
                    }                    
                }

                try
                {
                    string consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    using (SqlConnection con = new SqlConnection(consString))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_UpdateAmount"))
                        {
                            con.Open();
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = con;

                            cmd.CommandTimeout = 1800;
                            cmd.ExecuteNonQuery();
                            con.Close();
                            cmd.Dispose();
                        }
                    }
                }

                catch (Exception ex)
                {

                }
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }



    }

    public class RateMaster
    {
        
        public int ID { get; set; }

        public DateTime? Date { get; set; }

      
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public decimal? Weight { get; set; }

        public string UOM { get; set; }

        public decimal? TodayRate { get; set; }


        public int OrderNo { get; set; } 
    }
}
