using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Net;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class ExpenceTypesController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: ExpenceTypes
        public ActionResult Index()
        {
            var model = db.ExpenceTypes.OrderBy(a => a.ID).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ExpenceTypes expenceTypes)
        {
            var isexist = db.ExpenceTypes.Where(x => x.ExpenceType == expenceTypes.ExpenceType).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    db.ExpenceTypes.Add(expenceTypes);
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
            ExpenceTypes expenceTypes = await db.ExpenceTypes.FindAsync(id);
            if (expenceTypes == null)
            {
                return HttpNotFound();
            }
            return View(expenceTypes);
        }

        [HttpPost]
        public ActionResult Edit(ExpenceTypes expenceTypes)
        {

            db.Entry(expenceTypes).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Record Edited Successfully";
            return RedirectToAction("Index");
        }

        public ActionResult CheckDuplicateName(string EXPENCE)
        {

            var count = db.ExpenceTypes.Where(x => x.ExpenceType == EXPENCE).ToList();
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
                var model = db.ExpenceTypes.Where(x => x.ID == id).FirstOrDefault();
                db.ExpenceTypes.Remove(model);
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