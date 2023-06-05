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
    public class RegionMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: RegionMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.RegionMasters.ToListAsync());
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.RegionMasters.Count();

                var result = db.RegionMasters
                    .AsEnumerable()
                    .Select(x => new RegionMaster { ID = x.ID, Region = x.Region }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.RegionMasters.AsEnumerable()
                        .Where(x => x.Region.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new RegionMaster { ID = x.ID, Region = x.Region }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.RegionMasters.AsEnumerable()
                        .Select(x => new RegionMaster { ID = x.ID, Region = x.Region }).OrderByDescending(x => x.ID).ToList();
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
        // GET: RegionMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegionMaster regionMaster = await db.RegionMasters.FindAsync(id);
            if (regionMaster == null)
            {
                return HttpNotFound();
            }
            return View(regionMaster);
        }

        // GET: RegionMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RegionMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegionMaster regionMaster)
        {
            if (ModelState.IsValid)
            {
                regionMaster.CreatedBy = User.Identity.Name;
                regionMaster.CreatedDate = DateTime.Now;
                db.RegionMasters.Add(regionMaster);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Added Success!";

                return RedirectToAction("Index");
            }

            return View(regionMaster);
        }

        // GET: RegionMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegionMaster regionMaster = await db.RegionMasters.FindAsync(id);
            if (regionMaster == null)
            {
                return HttpNotFound();
            }
            return View(regionMaster);
        }

        // POST: RegionMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RegionMaster regionMaster)
        {
            if (ModelState.IsValid)
            {
                var toupdate = db.RegionMasters.Find(regionMaster.ID);
                toupdate.UpdatedBy = User.Identity.Name;
                toupdate.UpdatedDate = DateTime.Now;
                db.Entry(toupdate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["success"] = "Record Update Success!";

                return RedirectToAction("Index");
            }
            return View(regionMaster);
        }

        // GET: RegionMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RegionMaster regionMaster = await db.RegionMasters.FindAsync(id);
            if (regionMaster == null)
            {
                return HttpNotFound();
            }
            return View(regionMaster);
        }

        public ActionResult CheckDuplicate(string Region, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.RegionMasters.Where(x => x.Region == Region && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.RegionMasters.Where(x => x.Region == Region).ToList();
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
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var regionMaster = db.RegionMasters.Where(a => a.ID == id).FirstOrDefault();
                db.RegionMasters.Remove(regionMaster);
                db.SaveChanges();
                TempData["success"] = "Record Deleted Success!";
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
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
