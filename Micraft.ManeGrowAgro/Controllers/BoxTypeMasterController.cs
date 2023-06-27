using Micraft.ManeGrowAgro.Models;
using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class BoxTypeMasterController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: BoxTypeMaster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit()
        {

            return View();
        }

        [HttpPost]
        public string Getdata(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            var boxlist = db.BoxTypeMaster.ToList();
            var result = boxlist;
            int totalRecord = db.BoxTypeMaster.Count();
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            sb.Append("{");
            sb.Append("\"sEcho\": ");
            sb.Append(sEcho);
            sb.Append(",");
            sb.Append("\"iTotalRecords\": ");
            sb.Append(totalRecord);
            sb.Append(",");
            sb.Append("\"iTotalDisplayRecords\": ");
            sb.Append(totalRecord);
            sb.Append(",");
            sb.Append("\"aaData\": ");
            sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result));
            sb.Append("}");
            return sb.ToString();

        }

        [HttpPost]
        public ActionResult Create(BoxTypeMaster boxtypevalue)
        {
            var isexist = db.BoxTypeMaster.ToList();
            isexist = isexist.Where(x => x.BoxType.ToLower().Equals(boxtypevalue.BoxType)).ToList();
            if (isexist.Count == 0)
            {
                if (ModelState.IsValid)
                {
                    boxtypevalue.CreatedBy = User.Identity.Name;
                    boxtypevalue.CreatedDate = DateTime.Now;
                    db.BoxTypeMaster.Add(boxtypevalue);
                    db.SaveChanges();
                    TempData["success"] = "Success";
                    return RedirectToAction("Index");
                }
            }

            return View();
        }


        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var BoxTypes = db.BoxTypeMaster.Where(a => a.Id == id).FirstOrDefault();
                db.BoxTypeMaster.Remove(BoxTypes);
                db.SaveChanges();
                TempData["success"] = "Record Delete Success!";
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetEditData(int Id)
        {
            var isexist = db.BoxTypeMaster.Where(x => x.Id == Id).FirstOrDefault();
            var result = new { Message = "success", isexist };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        public ActionResult Update(BoxTypeMaster box)
        {
            var isexist = db.BoxTypeMaster.Where(x => x.Id == box.Id).FirstOrDefault();
            
                if (ModelState.IsValid)
                {
                    db.BoxTypeMaster.AddOrUpdate(box);
                    db.SaveChanges();
                    TempData["success"] = "Success";
                    return RedirectToAction("Index");
                }
            
            return View();
        }
    }
}