using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class VendorTypesController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: VendorTypes
        public ActionResult Index()
        {
            var model = db.VendorTypes.OrderBy(x => x.ID).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(VendorTypes VendorTypes)
        {
            var isexist = db.VendorTypes.Where(x => x.VendorType == VendorTypes.VendorType).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    VendorTypes.CreatedBy = User.Identity.Name;
                    VendorTypes.CreatedDate = DateTime.Now;
                    db.VendorTypes.Add(VendorTypes);
                    db.SaveChanges();
                    TempData["success"] = "Success";

                 //   ModelState.Clear();
                   return RedirectToAction("Index");

                }
            }

            return View();
        }

        public ActionResult CheckDuplicateName(string VENDOR)
        {

            var count = db.VendorTypes.Where(x => x.VendorType == VENDOR).ToList();
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
                var model = db.VendorTypes.Where(x => x.ID == id).FirstOrDefault();
                db.VendorTypes.Remove(model);
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