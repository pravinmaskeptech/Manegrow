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
   // [Authorize(Roles ="Admin")]
   [Authorize]
    public class HarvestingMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: HarvestingMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.HarvestingMasters.ToListAsync());
        }

        // GET: HarvestingMasters/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HarvestingMaster harvestingMaster = await db.HarvestingMasters.FindAsync(id);
            if (harvestingMaster == null)
            {
                return HttpNotFound();
            }
            return View(harvestingMaster);
        }

        // GET: HarvestingMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HarvestingMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,RoomNumber,ProjectionDate,YearlyProjection,MonthlyProjection,DailyPrediction,ActualQuentity,ProjectionFailuarReason,IsProjection")] HarvestingMaster harvestingMaster)
        {
            if (ModelState.IsValid)
            {
                db.HarvestingMasters.Add(harvestingMaster);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(harvestingMaster);
        }

        // GET: HarvestingMasters/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HarvestingMaster harvestingMaster = await db.HarvestingMasters.FindAsync(id);
            if (harvestingMaster == null)
            {
                return HttpNotFound();
            }
            return View(harvestingMaster);
        }

        // POST: HarvestingMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,RoomNumber,ProjectionDate,YearlyProjection,MonthlyProjection,DailyPrediction,ActualQuentity,ProjectionFailuarReason,IsProjection")] HarvestingMaster harvestingMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(harvestingMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(harvestingMaster);
        }

        // GET: HarvestingMasters/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HarvestingMaster harvestingMaster = await db.HarvestingMasters.FindAsync(id);
            if (harvestingMaster == null)
            {
                return HttpNotFound();
            }
            return View(harvestingMaster);
        }

        // POST: HarvestingMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            HarvestingMaster harvestingMaster = await db.HarvestingMasters.FindAsync(id);
            db.HarvestingMasters.Remove(harvestingMaster);
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
