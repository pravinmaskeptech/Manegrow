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
using System.Text;

namespace Micraft.ManeGrowAgro.Controllers
{
    [Authorize]
    public class ZonesController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: Zones
        public async Task<ActionResult> Index()
        {
            return View(await db.Zones.ToListAsync());
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.Zones.Count();

                var result = db.Zones                   
                    .AsEnumerable()
                    .Select(x => new Zones { ID = x.ID, CityFrom=x.CityFrom,CityTo=x.CityTo,Zone=x.Zone,IsActive=x.IsActive}).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.Zones.AsEnumerable()
                        .Where(x => x.Zone.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new Zones { ID = x.ID, CityFrom = x.CityFrom, CityTo = x.CityTo, Zone = x.Zone, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.Zones.AsEnumerable()
                        .Select(x => new Zones { ID = x.ID, CityFrom = x.CityFrom, CityTo = x.CityTo, Zone = x.Zone, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
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
        [HttpPost]
        public JsonResult AddRecord(Zones records)
        {
            try
            {
                db.Zones.Add(records);
                db.SaveChanges();
                var result = new { Message = "success" };
                TempData["success"] = "Record Updated Success!";

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult EditRecord(Zones zones)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var toupdate = db.Zones.Find(zones.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    db.Entry(toupdate).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["success"] = "Record Added Success!";
                    var result = new { Message = "success" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetZone(int ID)
        {
            try
            {
                var result = db.Zones.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var zones = db.Zones.Where(a => a.ID == id).FirstOrDefault();
                db.Zones.Remove(zones);
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Zones/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zones zones = await db.Zones.FindAsync(id);
            if (zones == null)
            {
                return HttpNotFound();
            }
            return View(zones);
        }

        // GET: Zones/Create
        public ActionResult Create()
        {
            var zone = db.ZonesMaster.ToList();
            if (zone != null) { ViewBag.zone = zone; }

            return View();
        }

        // POST: Zones/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Zones zones)
        {

            if (ModelState.IsValid)
            {
                zones.CreatedBy = User.Identity.Name;
                zones.CreatedDate = DateTime.Now;
                zones.IsActive=true;    
                db.Zones.Add(zones);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Added Success!";
                return RedirectToAction("Index");
            }

            return View(zones);
        }


        public ActionResult CheckDuplicate(string CityFrom,string CityTo,string Zone, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.Zones.Where(x => x.CityFrom == CityFrom && x.CityTo == CityTo && x.Zone == Zone && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.Zones.Where(x => x.Zone == Zone).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception Ex)
            {
                var str = Ex.Message.ToString();
                return Json(str, JsonRequestBehavior.AllowGet);
            }

        }




        // GET: Zones/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var zone = db.ZonesMaster.ToList();
            if (zone != null) { ViewBag.zone = zone; }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zones zones = await db.Zones.FindAsync(id);
            if (zones == null)
            {
                return HttpNotFound();
            }
            return View(zones);
        }

        // POST: Zones/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CityFrom,CityTo,Zone,IsActive")] Zones zones)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zones).State = EntityState.Modified;
                await db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(zones);
        }

        // GET: Zones/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Zones zones = await db.Zones.FindAsync(id);
            if (zones == null)
            {
                return HttpNotFound();
            }
            return View(zones);
        }

      

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
