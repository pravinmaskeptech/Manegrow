using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class RoomMasterController : Controller
    {
        // GET: RoomMaster
        private ManeGrowContext db = new ManeGrowContext();
        public ActionResult Index()
        {
            var model = db.RoomMaster.OrderBy(x => x.Id).ToList();

            return View(model);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(RoomMaster roommaster)
        {
            var isexist=db.RoomMaster.Where(x => x.RoomNo == roommaster.RoomNo).ToList();

            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    db.RoomMaster.Add(roommaster);
                    db.SaveChanges();
                    TempData["success"] = "Success";

                    ModelState.Clear();
                }
            }

            return View();
        }

        public ActionResult CheckDuplicateName(string ROOMNO)
        {
            
            var count = db.RoomMaster.Where(x => x.RoomNo == ROOMNO).ToList();
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
                var model = db.RoomMaster.Where(x => x.Id == id).FirstOrDefault();
                db.RoomMaster.Remove(model);
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