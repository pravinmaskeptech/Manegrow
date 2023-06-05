using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
   
    public class VendorInvoiceController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: VendorInvoice
        public ActionResult Index()
        {
            ViewBag.VendorName = db.DispatchTransactions.Where(a => a.VendorName == "ashdfahdfahdgs").ToList();
            return View();
        }
        public ActionResult SetDates(string FROMDATE, string TODATE,string VendorName)
        {

            Session["Fromdate"] = FROMDATE;
            Session["Todate"] = TODATE;
            Session["VendorName"] = VendorName; 

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

                var VendorName = Session["VendorName"]; 

                DateTime? fromd = Convert.ToDateTime(FROMDATE);
                DateTime? tod = Convert.ToDateTime(TODATE);


                var result = (from x in db.DispatchTransactions.Where(w => w.Date >= fromd && w.Date <= tod &&( w.VendorName==VendorName || VendorName==""))                             
                              select new
                              {
                                  DispatchID = x.DispatchID,
                                  Date = x.Date,
                                  LocationFrom = x.LocationFrom,
                                  LocationTo = x.LocationTo,
                                  VendorName = x.VendorName,
                                  LoadINKG = x.LoadINKG,
                                  NoOfBoxes = x.NoOfBoxes,
                                  RateType = x.RateType,
                                  Rate = x.Rate,
                                  Amount = x.Amount,
                                  CreatedBy = x.CreatedBy,
                                  
                              }).OrderBy(x => x.Date).ToList();
               
              var  totalRecord = result.Count();  
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
        public JsonResult GetVendors(string FROMDATE,string TODATE)
        {
            DateTime? fromd = Convert.ToDateTime(FROMDATE);
            DateTime? tod = Convert.ToDateTime(TODATE);


            var tbldata =  db.DispatchTransactions.Where(w => w.Date >= fromd && w.Date <= tod).Select(a => new { a.VendorName }).Distinct().ToList();
            var result = tbldata.OrderBy(a => a.VendorName);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}