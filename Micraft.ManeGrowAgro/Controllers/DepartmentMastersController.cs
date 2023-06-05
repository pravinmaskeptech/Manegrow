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
using System.Text;

namespace Micraft.ManeGrowAgro.Controllers
{
    [Authorize]
    public class DepartmentMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: DepartmentMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.DepartmentMasters.ToListAsync());
        }
        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.DepartmentMasters.Count();

                var result = db.DepartmentMasters
                    .AsEnumerable()
                    .Take(iDisplayLength)
                    .Select(x => new DepartmentMaster { ID = x.ID, Department = x.Department, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.DepartmentMasters.AsEnumerable()
                        .Where(x => x.Department.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new DepartmentMaster { ID = x.ID, Department = x.Department, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.DepartmentMasters.AsEnumerable()
                        .Take(iDisplayLength)
                        .Select(x => new DepartmentMaster { ID = x.ID, Department = x.Department, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
                }


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
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        [HttpPost]
        public JsonResult AddRecord(DepartmentMaster records)
        {
            try
            {
                db.DepartmentMasters.Add(records);  
                db.SaveChanges();
                TempData["success"] = "Record Added Success!";
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditRecord(DepartmentMaster records) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var toupdate = db.DepartmentMasters.Find(records.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    var id = records.ID;
                    var model = db.DepartmentMasters.Where(x => x.ID == id).FirstOrDefault();
                    model.Department = records.Department;
                    model.IsActive = records.IsActive;
                    db.Entry(toupdate).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["success"] = "Record Update Success!";
                    var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);            
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        
        public JsonResult GetRecord(int ID)
        {
            try
            {
                var result = db.DepartmentMasters.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: DepartmentMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentMaster departmentMaster = await db.DepartmentMasters.FindAsync(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
            return View(departmentMaster);
        }

        // GET: DepartmentMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DepartmentMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(DepartmentMaster departmentMaster)
        {
            if (ModelState.IsValid)
            {
                departmentMaster.CreatedBy = User.Identity.Name;
                departmentMaster.CreatedDate = DateTime.Now;
                departmentMaster.IsActive = true;   
                db.DepartmentMasters.Add(departmentMaster);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Added Success!";

                return RedirectToAction("Index");
            }

            return View(departmentMaster);
        }

        // GET: DepartmentMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentMaster departmentMaster = await db.DepartmentMasters.FindAsync(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
            return View(departmentMaster);
        }

        // POST: DepartmentMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Department")] DepartmentMaster departmentMaster)
        {
            if (ModelState.IsValid)
            {
           
                db.Entry(departmentMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(departmentMaster);
        }

        public ActionResult CheckDuplicate(string Department, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.DepartmentMasters.Where(x => x.Department == Department && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.DepartmentMasters.Where(x => x.Department == Department).ToList();
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


        // GET: DepartmentMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DepartmentMaster departmentMaster = await db.DepartmentMasters.FindAsync(id);
            if (departmentMaster == null)
            {
                return HttpNotFound();
            }
            return View(departmentMaster);
        }

        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var departmentMasters = db.DepartmentMasters.Where(a => a.ID == id).FirstOrDefault();
                db.DepartmentMasters.Remove(departmentMasters);
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }



        // POST: DepartmentMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            DepartmentMaster departmentMaster = await db.DepartmentMasters.FindAsync(id);
            db.DepartmentMasters.Remove(departmentMaster);
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
