using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class CaretMasterController : Controller
    {
        // GET: CaretMaster
        private ManeGrowContext db = new ManeGrowContext();
        public ActionResult Index()
        {
            var model = db.CaretMaster.OrderBy(x => x.Id).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CaretMaster caretmaster)
        {
            var isexist = db.CaretMaster.Where(x => x.CaretType == caretmaster.CaretType).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    db.CaretMaster.Add(caretmaster);
                    db.SaveChanges();
                    TempData["success"] = "Success";

                    ModelState.Clear();
                }
            }

            return View();
        }

        public ActionResult CheckDuplicateName(string CARET)
        {

            var count = db.CaretMaster.Where(x => x.CaretType == CARET).ToList();
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
                var model = db.CaretMaster.Where(x => x.Id == id).FirstOrDefault();
                db.CaretMaster.Remove(model);
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