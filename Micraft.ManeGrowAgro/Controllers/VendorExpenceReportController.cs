using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class VendorExpenceReportController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: RejectionReport
        public ActionResult Index()
        {
            //  ViewBag.VendorName = db.DispatchTransactions.Where(a => a.VendorName == "ashdfahdfahdgs").ToList();
            return View();
        }
        public ActionResult SetDates(string FROMDATE, string TODATE)
        {

            Session["Fromdate"] = FROMDATE;
            Session["Todate"] = TODATE;
            //  Session["VendorName"] = VendorName;

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
                //var VendorName = Session["VendorName"];

                DateTime? fromd = Convert.ToDateTime(FROMDATE);
                DateTime? tod = Convert.ToDateTime(TODATE);

                var result = (from e in db.ExpenceDetails

                              join v in db.VendorExpence
                             on e.ExpMainID equals v.ID

                              join d in db.dispatchDetails
                             on v.DispatchID equals d.DispatchID

                              join r in db.RouteMains 
                             on d.Route equals r.ID

                              select new
                              {
                                  d.DispatchID,
                                  d.DispatchDate,
                                  d.VendorType,
                                  d.VendorName,
                                  r.MainRouteName,
                                  d.DriverName,
                                  d.VehicalNo,                            
                                  d.TotalWeight,
                                  d.NoofBoxes,   
                                  e.ExpType,
                                  e.Amount,
                                  e.CashMemo,
                                  e.ReceiptNo,
                                  e.ReceiptDate,
                                  v.CreatedDate, 
                                  v.CreatedBy
                              }).OrderBy(x => x.DispatchDate).ToList(); 

                var totalRecord = result.Count();
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
      
    }

   
}