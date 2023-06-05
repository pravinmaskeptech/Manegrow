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
    public class BankMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: BankMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.BankMasters.ToListAsync());
        }
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.EmployeeMasters.Count();

                var result = (from b in db.BankMasters
                              join e in db.EmployeeMasters
                              on b.LinkID equals e.ID
                              select new
                              {
                                  ID = b.ID,
                                  Name = e.Name,
                                  BranchCity = b.BranchCity,
                                  AccName = b.AccName,
                                  AccNumber = b.AccNumber,
                                  BranchName = b.BranchName,
                                  BranchState = b.BranchState,
                                  MICRCode = b.MICRCode,
                                  IFSCCode = b.IFSCCode
                              }).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {

                    result = (from b in db.BankMasters
                              join e in db.EmployeeMasters
                              on b.LinkID equals e.ID
                              select new
                              {
                                  ID = b.ID,
                                  Name = e.Name,
                                  BranchCity = b.BranchCity,
                                  AccName = b.AccName,
                                  AccNumber = b.AccNumber,
                                  BranchName = b.BranchName,
                                  BranchState = b.BranchState,
                                  MICRCode = b.MICRCode,
                                  IFSCCode = b.IFSCCode
                              }).Where(x => x.Name.ToLower().Contains(sSearch))
                             .Take(iDisplayLength).ToList();
                }
                else
                {
                    result = (from b in db.BankMasters
                              join e in db.EmployeeMasters
                              on b.LinkID equals e.ID
                              select new
                              {
                                  ID = b.ID,
                                  Name = e.Name,
                                  BranchCity = b.BranchCity,
                                  AccName = b.AccName,
                                  AccNumber = b.AccNumber,
                                  BranchName = b.BranchName,
                                  BranchState = b.BranchState,
                                  MICRCode = b.MICRCode,
                                  IFSCCode = b.IFSCCode
                              }).ToList();
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
        // GET: BankMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankMaster bankMaster = await db.BankMasters.FindAsync(id);
            if (bankMaster == null)
            {
                return HttpNotFound();
            }
            return View(bankMaster);
        }

        // GET: BankMasters/Create
        public ActionResult Create()
        {
            ViewBag.LinkTo = db.EmployeeMasters.ToList();
            return View();
        }

        // POST: BankMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,LinkID,AccName,AccNumber,BranchName,BranchCity,BranchState,MICRCode,IFSCCode")] BankMaster bankMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bankMaster.CreatedBy = User.Identity.Name;
                    bankMaster.CreatedDate = DateTime.Now;
                    db.BankMasters.Add(bankMaster);
                    await db.SaveChangesAsync();
                    TempData["success"] = "Record Added Success!";
                    return RedirectToAction("Index");
                }

                return View(bankMaster);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(bankMaster);
            }
        }

        // GET: BankMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.LinkTo = db.EmployeeMasters.ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankMaster bankMaster = await db.BankMasters.FindAsync(id);
            if (bankMaster == null)
            {
                return HttpNotFound();
            }
            return View(bankMaster);
        }

        // POST: BankMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,LinkID,AccName,AccNumber,BranchName,BranchCity,BranchState,MICRCode,IFSCCode")] BankMaster bankMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var toupdate = db.BankMasters.Find(bankMaster.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    db.Entry(toupdate).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["success"] = "Record Updated Success";
                    return RedirectToAction("Index");
                }
                return View(bankMaster);
            }
            catch(Exception ex)
            {
                TempData["error"]=ex.Message;
                return View(bankMaster);
            }
        }

        // GET: BankMasters/Delete/5
        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var bankMaster = db.BankMasters.Where(a => a.ID == id).FirstOrDefault();
                db.BankMasters.Remove(bankMaster);
                db.SaveChanges();
                TempData["success"] = "Record Deleted Success";

                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        //   POST: BankMasters/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    BankMaster bankMaster = await db.BankMasters.FindAsync(id);
        //    db.BankMasters.Remove(bankMaster);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
