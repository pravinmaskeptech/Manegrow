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
    public class SubRouteDetailsController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        public ActionResult Index()
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


                var result = (from c in db.SubRouteMain.Where(a => a.SubRouteName == "dskjfhkd")
                              orderby (c.ID)
                              group c by new
                              {
                                  c.SubRouteName
                              } into gcs
                              select new
                              {
                                  ID = gcs.Max(a => a.ID),
                                  Root = gcs.Key.SubRouteName
                              }).ToList();


                if (!string.IsNullOrEmpty(sSearch))
                {
                    //result = result.AsEnumerable()
                    //    .Where(x => x.Root.ToLower().Contains(sSearch))
                    //    .Take(iDisplayLength)
                    //    .Select(x => new { Root = x.Root}).Distinct().ToList();

                    var results = (from c in db.SubRouteMain.Where(x => x.SubRouteName.ToLower().Contains(sSearch))
                                   orderby (c.ID)
                                   group c by new
                                   {
                                       c.SubRouteName
                                   } into gcs
                                   select new
                                   {
                                       ID = gcs.Max(a => a.ID),
                                       Root = gcs.Key.SubRouteName
                                   }).ToList();

                }
                else
                {
                    result = (from c in db.SubRouteMain.Where(x => x.SubRouteName.ToLower().Contains(sSearch))
                              orderby (c.ID)
                              group c by new
                              {
                                  c.SubRouteName
                              } into gcs
                              select new
                              {
                                  ID = gcs.Max(a => a.ID),
                                  Root = gcs.Key.SubRouteName
                              }).ToList();
                }

                result = result.OrderBy(a => a.Root).ToList();

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

        public ActionResult Create()
        {
            ViewBag.ShortNames = db.CustomerMasters.Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            @ViewBag.CustomerName = db.CustomerMasters.Where(a => a.CustName == "sadgasdghj").ToList();
            @ViewBag.MainRoute = db.RouteMains.OrderBy(a => a.MainRouteName).ToList();
            @ViewBag.CityName = db.Cities.Where(a => a.IsActive == true && a.CityName == "sahjdfhgasdf").OrderBy(a => a.CityName).ToList();
            return View();
        }

        public JsonResult SaveSubRouteMain(int RouteMainID, string RouteMainName, string SubRoute)
        {
            //var RouteID = 0;
            //var RouteMAin = 0;
            //var Main = db.SubRouteMain.Where(a => a.MainRouteID == RouteMainID).FirstOrDefault();
            //if (Main == null)
            //{
            //    var routemain = new SubRouteMain();
            //    routemain.SubRouteName = SubRoute;

            //    routemain.MainRouteID = RouteMainID;
            //    routemain.MainRouteName = RouteMainName;

            //    routemain.CreatedDate = DateTime.Now;
            //    routemain.CreatedBy = User.Identity.Name;
            //    db.SubRouteMain.Add(routemain);
            //    db.SaveChanges();
            //    RouteMAin = RouteMainID;
            //    RouteID = db.SubRouteMain.Max(a => a.ID);
            //}
            //else
            //{
            //    RouteMAin = Main.MainRouteID;
            //    RouteID = Main.ID;
            //}
            var routdetails = db.RouteDetails.Where(a => a.RouteMainID == RouteMainID).ToList(); 
            List<RouteCity> LST = new List<RouteCity>();
            foreach (var x in routdetails)
            {
                var city1 = new RouteCity();
                city1.City = x.FromRoute;
                LST.Add(city1);
                var city = new RouteCity();
                city.City = x.ToRoute;
                LST.Add(city);

            }

            var City = LST.Select(a => new { a.City }).Distinct().ToList();

            var result = new { Message = "success", City };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCustomers(string City, string Root)
        {
            //var Custid = db.SubRouteDetails.Where(a => a.SubRoute == Root && a.Area == City).Select(a => a.CustomerID).ToList();
            //var result = db.CustomerMasters.Where(a => a.IsApproved == true && a.City==City && ! Custid.Contains(a.ID)).Select(a=>new { a.CustName,a.ID }).ToList();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRootDetails(int ID)
        {

            //Root = db.SubRouteDetails.Where(a => a.ID == ID).Select(a => a.SubRoute).SingleOrDefault();
            var root = db.SubRouteMain.Where(a => a.ID == ID).FirstOrDefault();
            var result = new { root };
            return Json(result, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{

            //    //var root = db.SubRouteDetails.Where(a => a.SubRoute == Root).ToList();
            //    //var bom = db.VendorCityMaster.Where(a => a.RouteName == Root).OrderBy(a => a.ID).ToList();
            //  //  var result = new { root, bom };
            //    return Json("", JsonRequestBehavior.AllowGet);;

            //}

        }
        public JsonResult GetCustomerVendorRootDetails(int CustomerID, string Root)
        {

            var bom = db.VendorCityMaster.Where(a => a.RouteName == Root && a.CustomerID == CustomerID).OrderBy(a => a.ID).ToList();
            var CustName = db.CustomerMasters.Where(a => a.ID == CustomerID).Select(a => a.CustName).SingleOrDefault();
            var result = new { bom, CustName };
            return Json(result, JsonRequestBehavior.AllowGet);



        }

        //[HttpPost]
        //public JsonResult SaveRoot(List<SubRouteDetails> RootList, string Root,String City) 
        //{
        //   foreach(var x in RootList) 
        //    {
        //        var rootdata = db.SubRouteDetails.Where(a => a.CustomerID == x.CustomerID && a.SubRoute == Root && a.Area == City).FirstOrDefault();
        //        if(rootdata==null)
        //        {
        //            var cst = db.CustomerMasters.Where(a => a.ID == x.CustomerID).FirstOrDefault();
        //            if(cst !=null)
        //            {
        //                cst.RootName = Root;
        //            }

        //            var rt = new SubRouteDetails();
        //            rt.CustomerID = x.CustomerID;
        //            rt.CustomerName = x.CustomerName;
        //            rt.Area = City;
        //            rt.SubRoute = Root;
        //            rt.CreatedBy = User.Identity.Name;
        //            rt.CreatedDate = DateTime.Today;
        //            db.SubRouteDetails.Add(rt);
        //        }              
        //    }
        //    db.SaveChanges();
        //    var result = new {Message="success" };
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public JsonResult RevertRootData(List<SubRouteDetails> RootList, string Root, String City)
        //{
        //    var RootID = RootList.Select(a => a.ID).ToList();
        //    var lst = db.SubRouteDetails.Where(c => RootID.Contains(c.ID)).ToList();
        //    foreach(var xx in lst)
        //    {
        //        var cust = db.CustomerMasters.Where(a => a.ID == xx.CustomerID).FirstOrDefault(); 
        //        cust.RootName = "";
        //    }

        //    db.SubRouteDetails.RemoveRange(lst); 
        //    db.SaveChanges();
        //    var result = new { Message = "success" };
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public JsonResult SaveSubRouteDetails(List<SubRouteDetails> bomdata, string RouteMainName, int RouteMainID, string SubRouteName, int? SubRouteID)
        {

            foreach (var x in bomdata)
            {
                if (SubRouteID == null)
                {
                    var data = db.SubRouteDetails.Where(a => a.RouteMainName == RouteMainName && a.RouteMainID == RouteMainID && a.SubRouteName == x.SubRouteName && a.SR1 == x.SR1 && a.SR2 == x.SR2 && a.SR3 == x.SR3 && a.SR4 == x.SR4 && a.SR5 == x.SR5 && a.SR6 == x.SR6 && a.SR7 == x.SR7 && a.SR8 == x.SR8).FirstOrDefault();
                    if (data == null)
                    {
                        var bom = new SubRouteDetails();
                        bom.RouteMainName = RouteMainName;
                        bom.RouteMainID = RouteMainID;
                        bom.SubRouteName = x.SubRouteName;
                        bom.SR1 = x.SR1;
                        bom.SR2 = x.SR2;
                        bom.SR3 = x.SR3;
                        bom.SR4 = x.SR4;
                        bom.SR5 = x.SR5;
                        bom.SR6 = x.SR6;
                        bom.SR7 = x.SR7;
                        bom.SR8 = x.SR8;

                        bom.CreatedBy = User.Identity.Name;
                        bom.CreatedDate = DateTime.Now;
                        db.SubRouteDetails.Add(bom);
                        db.SaveChanges();
                        var result = new { Message = "success" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var results = new { Message = "Record Already Added" };
                        return Json(results, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var bom = db.SubRouteDetails.Where(a => a.ID == SubRouteID).FirstOrDefault();
                    bom.RouteMainName = RouteMainName;
                    bom.RouteMainID = RouteMainID;
                    bom.SubRouteName = SubRouteName;
                    bom.SR1 = x.SR1;
                    bom.SR2 = x.SR2;
                    bom.SR3 = x.SR3;
                    bom.SR4 = x.SR4;
                    bom.SR5 = x.SR5;
                    bom.SR6 = x.SR6;
                    bom.SR7 = x.SR7;
                    bom.SR8 = x.SR8;

                    bom.UpdatedBy = User.Identity.Name;
                    bom.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                }

            }


            var result1 = new { Message = "" };
            return Json(result1, JsonRequestBehavior.AllowGet);

        }


        public ActionResult GetData(int RouteMainID)
        {
            var lst_subroute = db.SubRouteDetails.Where(a => a.RouteMainID == RouteMainID ).OrderByDescending(a => a.ID).ToList();
            var result = new { lst_subroute, Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public ActionResult RemoveSubRoute(int ID)
        {

            try
            {
                db.SubRouteDetails.RemoveRange(db.SubRouteDetails.Where(a => a.ID == ID));
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


        public JsonResult GetSubCustomer(int ID)
        {


            var data = (from x in db.SubRouteCustomers.Where(x => x.RouteDetailsID == ID)
                        join c in db.CustomerMasters
                        on x.CustomerID equals c.ID

                        select new
                        {
                            ID = x.ID,
                            CustName = c.CustName,
                            Location = c.Area,
                            CustID = c.CustID,
                            CustomerID = c.ID,

                        }).OrderByDescending(x => x.ID).ToList();

            var result = new { Message = "success", CustData = data };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RemoveSubRouteCustomer(int ID)
        {

            try
            {
                db.SubRouteCustomers.RemoveRange(db.SubRouteCustomers.Where(a => a.ID == ID));
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

        public ActionResult SaveSubRouteCustomers(List<SubRouteCustomers> recs)
        {
            foreach (var x in recs)
            {
                if (x.ID == 0)
                {
                    if (x.CustomerID != null)
                    {
                        var cst = new SubRouteCustomers();
                        x.CreatedBy = User.Identity.Name;
                        x.CreatedDate = DateTime.Today;
                        db.SubRouteCustomers.Add(x);
                    }
                }
            }
            db.SaveChanges();
            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
   


    internal class RouteCity
    {
        public string City { get; set; } 
    }
}