using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class ModeMasterController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();

        // GET: ModeMaster
        public ActionResult Index()
        {
            var model = db.ModeMasters.OrderBy(x => x.ID).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ModeMaster ModeMaster)
        {
            var isexist = db.ModeMasters.Where(x => x.Mode == ModeMaster.Mode).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    ModeMaster.CreatedBy = User.Identity.Name;
                    ModeMaster.CreatedDate = DateTime.Now;
                    db.ModeMasters.Add(ModeMaster);
                    db.SaveChanges();
                    TempData["success"] = "Success";

                    ModelState.Clear();
                    return RedirectToAction("Index");

                }
            }

            return View();
        }

        public ActionResult CheckDuplicateName(string MODE)
        {

            var count = db.ModeMasters.Where(x => x.Mode == MODE).ToList();
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
                var model = db.ModeMasters.Where(x => x.ID == id).FirstOrDefault();
                db.ModeMasters.Remove(model);
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