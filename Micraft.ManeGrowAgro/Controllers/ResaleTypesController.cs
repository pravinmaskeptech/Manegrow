using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class ResaleTypesController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: ResaleTypes
        public ActionResult Index()
        {
            var model = db.ResaleTypeMasters.OrderBy(x => x.ID).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ResaleTypeMaster ResaleTypes)
        {
            var isexist = db.ResaleTypeMasters.Where(x => x.ResaleType == ResaleTypes.ResaleType).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    ResaleTypes.CreatedBy = User.Identity.Name;
                    ResaleTypes.CreatedDate = DateTime.Now;
                    db.ResaleTypeMasters.Add(ResaleTypes);
                    db.SaveChanges();
                    TempData["success"] = "Success";

                    //   ModelState.Clear();
                    return RedirectToAction("Index");

                }
            }

            return View();
        }

        //public ActionResult CheckDuplicateName(string ResaleType)
        //{

        //    var count = db.ResaleTypeMasters.Where(x => x.ResaleType == ResaleType).ToList();
        //    var flg = 0;
        //    if (count.Count > 0)
        //    {
        //        flg = 1;
        //    }

        //    var result = new { Message = "success", flg };
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult CheckDuplicate(string ResaleType, string Mode, int Id)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.ResaleTypeMasters.Where(x => x.ResaleType == ResaleType && x.ID != Id).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.ResaleTypeMasters.Where(x => x.ResaleType == ResaleType).ToList();
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


        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                var model = db.ResaleTypeMasters.Where(x => x.ID == id).FirstOrDefault();
                db.ResaleTypeMasters.Remove(model);
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