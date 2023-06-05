using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class DamageReasonsController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: DamageReasons
        public ActionResult Index()
        {
            var model = db.DamageReasons.OrderBy(a => a.ID).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(DamageReasons damageReasons)
        {
            var isexist = db.DamageReasons.Where(x => x.DamageReason == damageReasons.DamageReason).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    db.DamageReasons.Add(damageReasons);
                    db.SaveChanges();
                    TempData["success"] = "Success";

                    return RedirectToAction("Index");

                }
            }
            else
            {
                TempData["error"] = "Reason Already Added";
            }
            

            return View();
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DamageReasons damageReasons = await db.DamageReasons.FindAsync(id);
            if (damageReasons == null)
            {
                return HttpNotFound();
            }
            return View(damageReasons);
        }

        [HttpPost]
        public ActionResult Edit(DamageReasons damageReasons)
        {

            db.Entry(damageReasons).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Record Edited Successfully";
            return RedirectToAction("Index");
        }

        public ActionResult CheckDuplicateName(string REASON)
        {

            var count = db.DamageReasons.Where(x => x.DamageReason == REASON).ToList();
            var flg = 0;
            if (count.Count > 0)
            {
                flg = 1;
            }

            var result = new { Message = "success", flg };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                var model = db.DamageReasons.Where(x => x.ID == id).FirstOrDefault();
                db.DamageReasons.Remove(model);
                db.SaveChanges();
                TempData["delete"] = "Success";
                return RedirectToAction("Index");
            }
            catch (Exception EX)
            {
                TempData["error"] = EX.Message;
                return View();
            }

        }
    }
}