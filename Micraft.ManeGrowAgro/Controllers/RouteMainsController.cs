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

namespace Micraft.ManeGrowAgro.Controllers
{
    public class RouteMainsController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: RouteMains
        public async Task<ActionResult> Index()
        {
            return View(await db.RouteMains.ToListAsync());
        }

        // GET: RouteMains/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RouteMain routeMain = await db.RouteMains.FindAsync(id);
            if (routeMain == null)
            {
                return HttpNotFound();
            }
            return View(routeMain);
        }

        // GET: RouteMains/Create
        public ActionResult Create()
        {
            ViewBag.VenorNames = db.VendorMasters.OrderBy(a => a.VendorName).ToList();
            ViewBag.Modes = db.ModeMasters.OrderBy(a => a.Mode).ToList();
            ViewBag.CityName = db.RouteDestinations.Where(a => a.Destinations == "shjgdsjf").ToList();
            ViewBag.ShortNames = db.CustomerMasters.Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            @ViewBag.CustomerName = db.CustomerMasters.Where(a => a.CustName == "sadgasdghj").ToList();


            @ViewBag.VehicleType = db.VehicleTypes.OrderBy(a => a.VehicleType).ToList();
            return View();
        }

        // POST: RouteMains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RouteMain routeMain)
        {
            try
            {
                routeMain.CreatedBy = User.Identity.Name;
                routeMain.CreatedDate = DateTime.Today;
                db.RouteMains.Add(routeMain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }catch(Exception Ex)
            {

            }
          
            return View(routeMain);
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

        
        public ActionResult DeleteSubCustData(int Custid,int RouteDetailsID,int MainRouteID)
        {
            try
            {
                var route = db.SubRouteCustomers.Where(a => a.CustomerID == Custid && a.RouteDetailsID== RouteDetailsID && a.RouteMainID== MainRouteID).ToList();
                db.SubRouteCustomers.RemoveRange(route);              
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
        public ActionResult DeleteRouteDetails(int ID)
        {
            try
            {
                var route = db.RouteDetails.Where(a => a.ID == ID).ToList();
                db.RouteDetails.RemoveRange(route);

                var routes = db.SubRouteCustomers.Where(a => a.RouteDetailsID == ID).ToList();
                db.SubRouteCustomers.RemoveRange(routes); 
                db.SaveChanges();

                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }catch(Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: RouteMains/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RouteMain routeMain = await db.RouteMains.FindAsync(id);
            if (routeMain == null)
            {
                return HttpNotFound();
            }
            return View(routeMain);
        }

        // POST: RouteMains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RouteMain routeMain)
        {
            try { 
                db.Entry(routeMain).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
                @TempData["Message"] = "Successfully Saved";
            }catch(Exception Ex)
            {
                @TempData["Message"] = Ex.Message;
                return View(routeMain);
            }
          
        }

    
        public JsonResult SaveRouteMain(string Root)
        {
            var RouteID = 0;
            try
            {
                if(Root=="")
                {
                    var result1 = new { Message = "Please Enter Route" };
                    return Json(result1, JsonRequestBehavior.AllowGet); 
                }
                var Main = db.RouteMains.Where(a => a.MainRouteName == Root).FirstOrDefault();
                if (Main == null)
                {
                    var routemain = new RouteMain();
                    routemain.MainRouteName = Root;
                    routemain.CreatedDate = DateTime.Now;
                    routemain.CreatedBy = User.Identity.Name;
                    db.RouteMains.Add(routemain);
                    db.SaveChanges();
                    RouteID = db.RouteMains.Max(a => a.ID);
                }
                else
                {
                    RouteID = Main.ID;
                }

                var result = new { Message = "success", RouteID };
                return Json(result, JsonRequestBehavior.AllowGet);
            }catch (Exception Ex)
            {
                var result = new { Message = Ex.Message, RouteID };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMainRouteDetails(int RouteID, string Route)  
        {
            var bom = db.RouteDetails.Where(a => a.RouteMainID == RouteID).OrderBy(a=>a.ID).ToList();  
            if(Route==null)
            {
                Route = db.RouteMains.Where(a => a.ID == RouteID).Select(a => a.MainRouteName).SingleOrDefault();
            }
            var result = new {Message="success",Route,bom }; 
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult SaveBOM(List<RouteDetails> bomdata, int?  RouteID, string Root) 
        {
         //   db.RouteDetails.RemoveRange(db.RouteDetails.Where(a => a.RouteMainID == RouteID).ToList());  
          //  db.SaveChanges();

            //  var RouteName = db.rootMasters.Where(a => a.Root == Route).FirstOrDefault();
           
                foreach (var x in bomdata)
                {
                    var details = db.RouteDetails.Where(a => a.ID == x.ID).FirstOrDefault();
                    if (details == null)
                    {
                        var bom = new RouteDetails();
                        bom.FromRoute = x.FromRoute;
                        bom.ToRoute = x.ToRoute;
                        bom.VendorName = x.VendorName;
                        bom.VendorID = x.VendorID;
                        bom.RouteMainID = x.RouteMainID;

                        bom.Mode = x.Mode; 
                        bom.KM = x.KM;
                    bom.Mile=x.Mile; 

                    bom.VehicleType = x.VehicleType;


                    bom.Rate = x.Rate;
                    bom.Unit = x.Unit;
                    bom.RateType = x.RateType;

                    bom.WTFrom1 = x.WTFrom1;
                    bom.WTTo1 = x.WTTo1;
                    bom.Rate1 = x.Rate1;

                    bom.WTFrom2 = x.WTFrom2;
                    bom.WTTo2 = x.WTTo2;
                    bom.Rate2 = x.Rate2; 

                    bom.WTFrom3 = x.WTFrom3; 
                    bom.WTTo3 = x.WTTo3;
                    bom.Rate3 = x.Rate3;
                     
                    bom.CreatedBy = User.Identity.Name;
                        bom.CreatedDate = DateTime.Now;
                        db.RouteDetails.Add(bom); 
                    }else
                    {
                        details.FromRoute = x.FromRoute;
                        details.ToRoute = x.ToRoute;
                        details.VendorName = x.VendorName;
                        details.VendorID = x.VendorID;
                        details.RouteMainID = x.RouteMainID; 
                        details.Mode = x.Mode;
                        details.KM = x.KM;
                    details.VehicleType = x.VehicleType;
                    details.UpdatedBy = User.Identity.Name;
                        details.UpdatedDate = DateTime.Now;
                    details.Rate = x.Rate;
                    details.Unit = x.Unit;
                    details.RateType = x.RateType;

                    details.WTFrom1 = x.WTFrom1;
                    details.WTTo1 = x.WTTo1;
                    details.Rate1 = x.Rate1;

                    details.WTFrom2 = x.WTFrom2;
                    details.WTTo2 = x.WTTo2;
                    details.Rate2 = x.Rate2;

                    details.WTFrom3 = x.WTFrom3; 
                    details.WTTo3 = x.WTTo3;
                    details.Rate3 = x.Rate3;
                    details.Mile = x.Mile;
                }
                    //   }

                }
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            

        }

        public JsonResult SaveDestinations(List<RouteDestinations> recs) 
        {
           foreach(var x in recs)
            {
                var destid = db.RouteDestinations.Where(a => a.DestID == x.DestID).FirstOrDefault();
                if (x.DestID==0)
                {
                    var dest = new RouteDestinations();
                    dest.Destinations = x.Destinations;
                    dest.RouteID = x.RouteID;
                    dest.CreatedBy = User.Identity.Name;
                    dest.CreatedDate = DateTime.Today;
                    db.RouteDestinations.Add(dest);
                }
            }
            db.SaveChanges();
            
            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
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
                        //x.SubRouteID = x.SubRouteID;
                        //x.CustomerID = x.CustomerID;
                        x.CreatedBy = User.Identity.Name;
                        x.CreatedDate = DateTime.Today;
                    }
                    db.SubRouteCustomers.Add(x);
                }
            }
            db.SaveChanges();
            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDestinations(int ID)
        {
            var RouteData = db.RouteDestinations.Where(a => a.RouteID == ID).OrderBy(a=>a.Destinations).OrderBy(a=>a.Destinations).ToList();

            var result = new { Message = "success", RouteData };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
