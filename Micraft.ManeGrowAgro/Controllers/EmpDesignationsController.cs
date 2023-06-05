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
    public class EmpDesignationsController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: EmpDesignations
        public async Task<ActionResult> Index()
        {
            return View(await db.EmpDesignations.ToListAsync());
        }

        // GET: EmpDesignations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDesignation empDesignation = await db.EmpDesignations.FindAsync(id);
            if (empDesignation == null)
            {
                return HttpNotFound();
            }
            return View(empDesignation);
        }

        // GET: EmpDesignations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmpDesignations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Designation")] EmpDesignation empDesignation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    empDesignation.CreatedBy = User.Identity.Name;
                    empDesignation.CreatedDate = DateTime.Now;
                    db.EmpDesignations.Add(empDesignation);
                    await db.SaveChangesAsync();
                    TempData["success"] = "Employee Designation Added Success!";
                    return RedirectToAction("Index");
                }

                return View(empDesignation);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View();
            }
        }

        // GET: EmpDesignations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDesignation empDesignation = await db.EmpDesignations.FindAsync(id);
            if (empDesignation == null)
            {
                return HttpNotFound();
            }
            return View(empDesignation);
        }

        // POST: EmpDesignations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Designation")] EmpDesignation empDesignation)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var toupdate = db.EmpDesignations.Find(empDesignation.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    db.Entry(toupdate).State = EntityState.Modified;             
                    await db.SaveChangesAsync();
                    TempData["success"] = "Employee Designation Updated Success!";
                    return RedirectToAction("Index");
                }
                return View(empDesignation);
            }
            catch(Exception ex)
            {
                TempData["error"]=ex.Message;
                return View();
            }
        }


        public ActionResult CheckDuplicate(string Designation, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.EmpDesignations.Where(x => x.Designation == Designation && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.EmpDesignations.Where(x => x.Designation == Designation).ToList();
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


        // GET: EmpDesignations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmpDesignation empDesignation = await db.EmpDesignations.FindAsync(id);
            if (empDesignation == null)
            {
                return HttpNotFound();
            }
            return View(empDesignation);
        }

        // POST: EmpDesignations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EmpDesignation empDesignation = await db.EmpDesignations.FindAsync(id);
            db.EmpDesignations.Remove(empDesignation);
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
