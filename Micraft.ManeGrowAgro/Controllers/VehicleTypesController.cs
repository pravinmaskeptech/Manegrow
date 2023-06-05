using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class VehicleTypesController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: VehicleTypes
        public ActionResult Index()
        {
            var model = db.VehicleTypes.OrderBy(x => x.ID).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(VehicleTypes vehicleTypes)
        {
            var isexist = db.VehicleTypes.Where(x => x.VehicleType == vehicleTypes.VehicleType).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    db.VehicleTypes.Add(vehicleTypes);
                    db.SaveChanges();
                    TempData["success"] = "Success";

                    return RedirectToAction("Index");

                }
            }

            return View();
        }

        public ActionResult CheckDuplicateName(string VEHICLE)
        {

            var count = db.VehicleTypes.Where(x => x.VehicleType == VEHICLE).ToList();
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
                var model = db.VehicleTypes.Where(x => x.ID == id).FirstOrDefault();
                db.VehicleTypes.Remove(model);
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