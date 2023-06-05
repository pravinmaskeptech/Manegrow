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
    public class CutOffMasatersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: CutOffMasaters
        public async Task<ActionResult> Index()
        {
            return View(await db.CutOffMasaters.ToListAsync());
        }

        // GET: CutOffMasaters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CutOffMasater cutOffMasater = await db.CutOffMasaters.FindAsync(id);
            if (cutOffMasater == null)
            {
                return HttpNotFound();
            }
            return View(cutOffMasater);
        }

        // GET: CutOffMasaters/Create
        public ActionResult Create()
        {
            var Vendor = db.VendorMasters.ToList();
            if (Vendor != null)
            {
                ViewBag.Vendor = Vendor;
            }
            ViewBag.TrMode = db.TransModes.ToList();
            return View();
        }
        [HttpPost]
        public JsonResult AutoCity(string prefix)
        {
            var customers = (from c in db.Cities
                             where c.CityName.StartsWith(prefix)
                             select new
                             {
                                 label = c.CityName,
                                 val = c.State
                             }).Take(10).ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }
        // POST: CutOffMasaters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CityCutOff,Mode,Vendor,PackingCutoff,PlantCutoff,DispatchCutoff")] CutOffMasater cutOffMasater)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    cutOffMasater.CreatedBy = User.Identity.Name;
                    cutOffMasater.CreatedDate = DateTime.Now;
                    db.CutOffMasaters.Add(cutOffMasater);
                    await db.SaveChangesAsync();
                    TempData["success"] = "Record Added success!";
                    return RedirectToAction("Index");
                }

                return View(cutOffMasater);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(cutOffMasater);
            }
        }

        // GET: CutOffMasaters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var Vendor = db.VendorMasters.ToList();
            if (Vendor != null)
            {
                ViewBag.Vendor = Vendor;
            }
            ViewBag.TrMode = db.TransModes.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CutOffMasater cutOffMasater = await db.CutOffMasaters.FindAsync(id);
            if (cutOffMasater == null)
            {
                return HttpNotFound();
            }
            return View(cutOffMasater);
        }

        // POST: CutOffMasaters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CityCutOff,Mode,Vendor,PackingCutoff,PlantCutoff,DispatchCutoff")] CutOffMasater cutOffMasater)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var toupdate = db.CutOffMasaters.Find(cutOffMasater.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    db.Entry(toupdate).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["success"] = "Record Updated success!";
                    return RedirectToAction("Index");
                }
                return View(cutOffMasater);
            }
            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(cutOffMasater);
            }
        }

        // GET: CutOffMasaters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CutOffMasater cutOffMasater = await db.CutOffMasaters.FindAsync(id);
            if (cutOffMasater == null)
            {
                return HttpNotFound();
            }
            return View(cutOffMasater);
        }

        // POST: CutOffMasaters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CutOffMasater cutOffMasater = await db.CutOffMasaters.FindAsync(id);
            db.CutOffMasaters.Remove(cutOffMasater);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
