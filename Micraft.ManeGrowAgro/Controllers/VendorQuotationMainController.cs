using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class VendorQuotationMainController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            ViewBag.Root = db.RouteMains.OrderBy(a => a.MainRouteName).ToList();
            ViewBag.CityName = db.Cities.Where(a=>a.CityName=="dsfjh").ToList();
            ViewBag.VendorName = db.VendorMasters.Where(a=>a.UserName=="shfdashga").ToList();
            return View();
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();


                var result = db.vendorQuotationMains
                    .AsEnumerable()
                    .Select(x => new { ID = x.ID, MainRouteName = x.MainRouteName, GSTApplicable = x.GSTApplicable, FSCharges = x.FSCharges, QuotationDate = x.QuotationDate }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.vendorQuotationMains.AsEnumerable()
                        .Where(x => x.MainRouteName.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                    .Select(x => new { ID = x.ID, MainRouteName = x.MainRouteName, GSTApplicable = x.GSTApplicable, FSCharges = x.FSCharges, QuotationDate = x.QuotationDate }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.vendorQuotationMains.AsEnumerable()
                    .Select(x => new { ID = x.ID, MainRouteName = x.MainRouteName, GSTApplicable = x.GSTApplicable, FSCharges = x.FSCharges, QuotationDate = x.QuotationDate }).OrderByDescending(x => x.ID).ToList();
                }

                StringBuilder sb = new StringBuilder();
                sb.Clear();
                sb.Append("{");
                sb.Append("\"sEcho\": ");
                sb.Append(sEcho);
                sb.Append(",");
                sb.Append("\"iTotalRecords\": ");
                sb.Append(result.Count());
                sb.Append(",");
                sb.Append("\"iTotalDisplayRecords\": ");
                sb.Append(result.Count());
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


        public JsonResult CheckDuplicateVendorVendorName(int? ID, string MainRouteName) 
        {
            if (ID == null)
            {
                var tbldata = db.vendorQuotationMains.Where(x => x.MainRouteName == MainRouteName).Count();
                return Json(tbldata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var tbldata = db.vendorQuotationMains.Where(x => x.MainRouteName == MainRouteName && x.ID != ID).Count();
                return Json(tbldata, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult Save(List<VendorQuotationDetails> recs,  string MainRouteName,  bool? GSTApplicable, decimal? FSCharges,  string QuotationDate, int? ID,int MainRouteID)
        {
            try
            {
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    var QuotationDates = DateTime.ParseExact(QuotationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    int maxid = 0;
                    if (ID == null)
                    {
                        var mains = new VendorQuotationMain();
                        mains.MainRouteName = MainRouteName;
                     //   mains.VendorID = VendorID;
                        mains.GSTApplicable = GSTApplicable;
                        mains.FSCharges = FSCharges;
                        mains.QuotationDate = QuotationDates;
                        mains.CreatedDate = DateTime.Now;
                        mains.CreatedBy = User.Identity.Name;
                        mains.MainRouteID = MainRouteID;
                        db.vendorQuotationMains.Add(mains);
                        db.SaveChanges();
                        maxid = db.vendorQuotationMains.Select(x => x.ID).Max();

                    }
                    else
                    {
                        maxid = Convert.ToInt32(ID);
                        var mains = db.vendorQuotationMains.Where(a => a.ID == maxid).FirstOrDefault();
                        //mains.VendorName = VendorName;
                        //mains.VendorID = VendorID;
                        mains.GSTApplicable = GSTApplicable;
                        mains.FSCharges = FSCharges;
                        mains.MainRouteID = MainRouteID;
                        mains.MainRouteName = MainRouteName; 
                        mains.FSCharges = FSCharges;
                        mains.QuotationDate = QuotationDates;
                        mains.UpdatedBy = User.Identity.Name;
                        mains.UpdatedDate =  DateTime.Now;




                    }
                    foreach (var x in recs)
                    {
                        if (x.DetailsID == 0)
                        {
                            var details = new VendorQuotationDetails();
                            details.VendorID = x.VendorID;
                            details.VendorName = x.VendorName;
                            details.GSTApplicable = x.GSTApplicable;
                            details.FSCharges = x.FSCharges;
                            details.FromCity=x.FromCity;
                            details.ToCity=x.ToCity;    
                            details.UOM = x.UOM;
                            details.WTFrom3 = x.WTFrom3;
                            details.WTTo3 = x.WTTo3;
                            details.Rate3 = x.Rate3;

                            details.WTFrom1 = x.WTFrom1;
                            details.WTTo1 = x.WTTo1;
                            details.Rate1 = x.Rate1;

                            details.WTFrom2 = x.WTFrom2;
                            details.WTTo2 = x.WTTo2;
                            details.Rate2 = x.Rate2;

                            details.AddWt = x.AddWt;
                            details.AddRate = x.AddRate;

                            details.FromCity = x.FromCity;
                            details.ToCity = x.ToCity;
                            details.MainID = maxid;
                            details.SubRoute = x.SubRoute;
                            details.SubRouteID=x.SubRouteID;

                            details.VendorName = x.VendorName;
                            details.VendorID = x.VendorID;

                            db.vendorQuotationDetails.Add(details);
                        }
                        else
                        {
                            var details = db.vendorQuotationDetails.Where(a => a.DetailsID == x.DetailsID).FirstOrDefault();
                            details.VendorID = x.VendorID;
                            details.VendorName = x.VendorName;
                            details.GSTApplicable = x.GSTApplicable;
                            details.FSCharges = x.FSCharges;
                            details.FromCity = x.FromCity;
                            details.ToCity = x.ToCity;

                            details.UOM = x.UOM;
                            details.WTFrom3 = x.WTFrom3;
                            details.WTTo3 = x.WTTo3;
                            details.Rate3 = x.Rate3;

                            details.WTFrom1 = x.WTFrom1;
                            details.WTTo1 = x.WTTo1;
                            details.Rate1 = x.Rate1;

                            details.WTFrom2 = x.WTFrom2;
                            details.WTTo2 = x.WTTo2;
                            details.Rate2 = x.Rate2;

                            details.AddWt = x.AddWt;
                            details.AddRate = x.AddRate;

                            details.FromCity = x.FromCity;
                            details.ToCity = x.ToCity;

                            details.SubRoute = x.SubRoute;
                            details.SubRouteID = x.SubRouteID;

                            details.VendorName = x.VendorName;
                            details.VendorID = x.VendorID;

                        }
                    }

                    db.SaveChanges();
                    dbTransaction.Commit();

                }
                var result = new { Message = "success", };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message, Max = 0 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Cities/Delete/5
        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var cities = db.vendorQuotationMains.Where(a => a.ID == id).ToList();
                db.vendorQuotationMains.RemoveRange(cities);



                var QuotationDetai = db.vendorQuotationDetails.Where(a => a.MainID == id).ToList();
                db.vendorQuotationDetails.RemoveRange(QuotationDetai);


                db.SaveChanges();
                TempData["success"] = "Record Deleted Success!";

                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetEditData(int ID)
        {
            var data = (from qd in db.vendorQuotationDetails.Where(a => a.MainID == ID)
                        join qm in db.vendorQuotationMains
                        on qd.MainID equals qm.ID
                        select new
                        {

                            //QuotationMain
                            MainID = qm.ID,
                            VendorID = qd.VendorID,
                            VendorName = qd.VendorName,
                            GSTApplicable = qm.GSTApplicable,
                            FSCharges = qm.FSCharges,
                            CreatedDate = qm.CreatedDate,


                            //QuotationDetails
                            DetailsID = qd.DetailsID,
                            FromCity = qd.FromCity,
                            ToCity = qd.ToCity,
                            
                            WTFrom1 = qd.WTFrom1,
                            UOM = qd.UOM,
                            WTTo1 = qd.WTTo1,
                            Rate1 = qd.Rate1,
                            WTFrom2 = qd.WTFrom2,
                            WTTo2 = qd.WTTo2,
                            Rate2 = qd.Rate2,
                            SubRoute = qd.SubRoute,
                            SubRouteID = qd.SubRouteID, 


                            WTFrom3 = qd.WTFrom3,
                            WTTo3 = qd.WTTo3,
                            Rate3 = qd.Rate3,
                            AddWt = qd.AddWt,
                            AddRate = qd.AddRate,
                            MainRouteID = qm.MainRouteID

                        }).OrderByDescending(x => x.CreatedDate).ToList();
            var result = new { Message = "success", data };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetVendorFromCities(int VendorID)
        {
            try {
                var CityList  = db.RouteDetails.Where(a => a.VendorID == VendorID).ToList();

            var result = new { Message = "success" , CityList };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
    }

}
        public JsonResult GetSubRoutes(int RouteMainID)
        {

            var SubRoute = db.SubRouteDetails.Where(a => a.RouteMainID == RouteMainID).OrderBy(a => a.SubRouteName).ToList();
            var RouteDetails = db.RouteDetails.Where(a => a.RouteMainID == RouteMainID).OrderBy(a => a.VendorName).ToList();
            var result = new { Message = "success", SubRoute , RouteDetails };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}