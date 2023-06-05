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
    public class UoMeasuresController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: UoMeasures
        public async Task<ActionResult> Index()
        {
            return View(await db.UoMeasures.ToListAsync());
        }

        // GET: UoMeasures/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UoMeasure uoMeasure = await db.UoMeasures.FindAsync(id);
            if (uoMeasure == null)
            {
                return HttpNotFound();
            }
            return View(uoMeasure);
        }

        // GET: UoMeasures/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UoMeasures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,UomName")] UoMeasure uoMeasure)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    uoMeasure.CreatedBy = User.Identity.Name;
                    uoMeasure.CreatedDate = DateTime.Now;
                    db.UoMeasures.Add(uoMeasure);
                    await db.SaveChangesAsync();
                    TempData["success"] = "Unit of Measure Added Success!";
                    return RedirectToAction("Index");
                }

                return View(uoMeasure);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View();
            }
        }

        // GET: UoMeasures/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UoMeasure uoMeasure = await db.UoMeasures.FindAsync(id);
            if (uoMeasure == null)
            {
                return HttpNotFound();
            }
            return View(uoMeasure);
        }

        // POST: UoMeasures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,UomName")] UoMeasure uoMeasure)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var toupdate = db.UoMeasures.Find(uoMeasure.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    db.Entry(toupdate).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["success"] = "Unit of Measure Updated Success!";
                    return RedirectToAction("Index");
                }
                return View(uoMeasure);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View();
            }
        }

        // GET: UoMeasures/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UoMeasure uoMeasure = await db.UoMeasures.FindAsync(id);
            if (uoMeasure == null)
            {
                return HttpNotFound();
            }
            return View(uoMeasure);
        }

        public ActionResult CheckDuplicate(string UomName, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.UoMeasures.Where(x => x.UomName == UomName && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.UoMeasures.Where(x => x.UomName == UomName).ToList();
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
