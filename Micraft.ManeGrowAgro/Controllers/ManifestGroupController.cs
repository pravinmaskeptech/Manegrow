using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{

    public class ManifestGroupController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: ManifestGroup
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            @ViewBag.MainRoute = db.RouteMains.OrderBy(a => a.MainRouteName).ToList();
            @ViewBag.CustomerName = db.CustomerMasters.Where(a => a.CustName == "sadgasdghj").ToList();
            return View();
        }
        

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();

                var result = (from e in db.ManifestGroups

                               join sply in db.RouteMains on e.RouteID equals sply.ID into MainOrder
                               from rt in MainOrder.DefaultIfEmpty() 
                               select new
                               {
                                   ID = e.ID,
                                   GroupName = e.GroupName,
                                   MainRouteName = rt.MainRouteName,                                   
                               } into p
                               group p by new { p.GroupName, p.MainRouteName } into gcs 
                               select new
                               {
                                   ID = gcs.Min(a=>a.ID),
                                   GroupName = gcs.Key.GroupName,
                                   RouteName = gcs.Key.MainRouteName, 
                               }).ToList();



                //var result = db.ManifestGroups.GroupBy(x => new { x.GroupName, x.RouteID })
                // .Select(b => new
                // {
                //     ID = b.Select(bn => bn.ID).ToList(),
                //    // Accessing to DateOfIssue and IssuerName from Key.
                //    GroupName = b.Key.GroupName,
                //     RouteID = b.Key.RouteID
                // }).OrderByDescending(a => a.ID).ToList(); 



                //var result = db.ManifestGroups
                //    .AsEnumerable()
                //    .Select(x => new { ID = x.ID, RouteID = x.RouteID, GroupName = x.GroupName, CustomerID = x.CustomerID}).OrderByDescending(x => x.ID).ToList();

                //if (!string.IsNullOrEmpty(sSearch))
                //{
                //    result = result.Where(a => a.GroupName.ToLower().Contains(sSearch)).Select(b => new
                //    {
                //        b.ID,b.GroupName,b.RouteID
                //    }).OrderByDescending(a => a.ID);
                //}
                //else
                //{
                //    result = result.Select(b => new
                //    {
                //        b.ID,
                //        b.GroupName,
                //        b.RouteID
                //    }).OrderByDescending(a => a.ID);
                //}

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

        public JsonResult GetGroupDetails(int ID)
        {
            var result = db.ManifestGroups.Where(a => a.ID == ID).FirstOrDefault();
            return Json(result , JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ManifestGroup ManifestGroup = await db.ManifestGroups.FindAsync(id);
            if (ManifestGroup== null)
            {
                return HttpNotFound();
            }
            return View(ManifestGroup);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ManifestGroup manifestGroup)
        {
            //manifestGroup.UpdatedDate = DateTime.Now;

            db.Entry(manifestGroup).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Record Edited Successfully";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var regionMaster = db.ManifestGroups.Where(a => a.ID == id).FirstOrDefault();
                db.ManifestGroups.Remove(regionMaster);
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

        public JsonResult GetCustomers(string City, string Root)
        {
            //var Custid = db.SubRouteDetails.Where(a => a.SubRoute == Root && a.Area == City).Select(a => a.CustomerID).ToList();
            //var result = db.CustomerMasters.Where(a => a.IsApproved == true && a.City == City && !Custid.Contains(a.ID)).Select(a => new { a.CustName, a.ID }).ToList();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSubCustomer(int ID ,string GroupName)
        {
            var CustomerData = db.ManifestGroups.ToList();
            var CSTdata = CustomerData.Where(a => a.RouteID != ID && a.GroupName != GroupName).Select(a=>a.CustomerID).ToList();
            // var subid = db.SubRouteCustomers.Where(a => a.RouteMainID == ID && !CSTdata.Contains(a.CustomerID)).Select(a => a.CustomerID).Distinct().ToList();
            var subid = db.SubRouteCustomers.Where(a => a.RouteMainID == ID ).Select(a => a.CustomerID).Distinct().ToList();

            var data = (from c in db.CustomerMasters.Where(a=>subid.Contains(a.ID))
                       
                        select new
                        {                                                 
                            CustName = c.CustName, 
                            Location = c.Area,
                            CustID = c.CustID,
                            CustomerID = c.ID,
                        }                        
                        ).OrderByDescending(x => x.CustName).ToList();

            var Customers = CustomerData.Where(a => a.RouteID == ID).ToList();  

            var result = new { Message = "success", CustData = data, Customers };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
       public ActionResult Save(  List< ManifestGroup> manifestGroup) 
        {
            if(manifestGroup==null) 
            {
                var result1 = new { Message ="Please Select Record...", };
                return Json(result1, JsonRequestBehavior.AllowGet); 
            }
            var routeID = manifestGroup[0].RouteID;
            var GroupName = manifestGroup[0].GroupName;
            var ManifestGRP = db.ManifestGroups.Where(a => a.RouteID == routeID &&a.GroupName== GroupName).ToList(); 
                db.ManifestGroups.RemoveRange(ManifestGRP); 
                db.SaveChanges();

                foreach (var x in manifestGroup)
                {
                    x.CreatedBy = User.Identity.Name;
                    x.CreatedDate = DateTime.Now;
                    db.ManifestGroups.Add(x);
                }
                db.SaveChanges();
                TempData["success"] = "Record Added Success!";
            var result = new { Message = "success", };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

    }
}