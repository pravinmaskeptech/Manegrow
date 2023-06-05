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
    public class ZonesMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: ZonesMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.ZonesMaster.ToListAsync());
        }
        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.ZonesMaster.Count();

                var result = db.ZonesMaster
                    .AsEnumerable()
                    .Select(x => new ZonesMaster { ID = x.ID, Zone = x.Zone, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.ZonesMaster.AsEnumerable()
                        .Where(x => x.Zone.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new ZonesMaster { ID = x.ID, Zone = x.Zone, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.ZonesMaster.AsEnumerable()
                        .Select(x => new ZonesMaster { ID = x.ID, Zone = x.Zone, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
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
        // GET: ZonesMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZonesMaster zonesMaster = await db.ZonesMaster.FindAsync(id);
            if (zonesMaster == null)
            {
                return HttpNotFound();
            }
            return View(zonesMaster);
        }

        // GET: ZonesMasters/Create
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost]
        public JsonResult AddRecord(ZonesMaster zonesMasters)
        {
            try
            {
                zonesMasters.CreatedBy = User.Identity.Name;
                zonesMasters.CreatedDate = DateTime.Now;
                zonesMasters.IsActive = true;
                db.ZonesMaster.Add(zonesMasters);
                db.SaveChanges();
                TempData["success"] = "Record Added Success!";

                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        // POST: ZonesMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Zone,IsActive")] ZonesMaster zonesMaster)
        {
            if (ModelState.IsValid)
            {
                db.ZonesMaster.Add(zonesMaster);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(zonesMaster);
        }

        // GET: ZonesMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZonesMaster zonesMaster = await db.ZonesMaster.FindAsync(id);
            if (zonesMaster == null)
            {
                return HttpNotFound();
            }
            return View(zonesMaster);
        }

        [HttpPost]
        public JsonResult UpdateRecord(ZonesMaster zonesMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var toupdate=db.ZonesMaster.Find(zonesMaster.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    db.Entry(toupdate).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["success"] = "Record Updated Success!";

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

        // POST: ZonesMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Zone,IsActive")] ZonesMaster zonesMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(zonesMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(zonesMaster);
        }


        public ActionResult CheckDuplicate(string Zone, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.ZonesMaster.Where(x => x.Zone == Zone && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.ZonesMaster.Where(x => x.Zone == Zone).ToList();
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

        // GET: ZonesMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ZonesMaster zonesMaster = await db.ZonesMaster.FindAsync(id);
            if (zonesMaster == null)
            {
                return HttpNotFound();
            }
            return View(zonesMaster);
        }

        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var zonesMaster = db.ZonesMaster.Where(a => a.ID == id).FirstOrDefault();
                db.ZonesMaster.Remove(zonesMaster);
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
