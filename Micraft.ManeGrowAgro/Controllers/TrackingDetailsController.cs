using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Micraft.ManeGrowAgro.Models;
using System.IO;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class TrackingDetailsController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();              
        public ActionResult Create()
        {
            return View();
        }

        public JsonResult TrackOrder(int OrderNo)
        {

            var order = (from e in db.Order.Where(a => a.Id == OrderNo)

                         join dr in db.MainOrder
                          on e.MasterOrderId equals dr.Id

                         join cus in db.CustomerMasters
                         on dr.CustomerId equals cus.ID

                         join pm in db.ProductMasters
                        on e.ProductId equals pm.ID

                         select new
                         {
                             ID = e.Id,                           
                             Qty = e.Qty,
                             Weight = e.Weight,
                             Date = dr.Date,
                             CustName = cus.CustName,
                             CustomerCode = cus.CustID, 
                             ProductName = string.Concat(e.ProductName, " ", pm.Weight, " ", pm.ProdUom),
                             CreatedBy = dr.CreatedBy,
                             ExpDelDate = dr.ExpectedDeliveryDate,
                             ExpDelTime = dr.ExpectedDeliveryTime,
                             PODName = e.PODName, 

                         }).FirstOrDefault();

            // var track = db.trackingDetails.Where(a => a.OrderNo == OrderNo).Select(a => new { a.ID, a.Date, a.CreatedDate, a.CreatedBy, a.Location, a.Status, a.Action, StatusTime=a.CreatedDate.ToShortTimeString() }).OrderByDescending(a => a.ID).ToList();
            var track = db.trackingDetails.Where(a => a.OrderNo == OrderNo).OrderByDescending(a => a.ID).ToList();
            var result = new { Message = "success", track, order,cnt= track.Count() };
                return Json(result, JsonRequestBehavior.AllowGet);
            
            }

        public virtual ActionResult DownloadNew(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Attachments/POD/"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }

    }
}
