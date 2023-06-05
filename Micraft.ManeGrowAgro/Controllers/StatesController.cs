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
    public class StatesController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: States
        public async Task<ActionResult> Index()
        {
            return View(await db.States.ToListAsync());
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.States.Count();

                var result = db.States
                    .AsEnumerable()
                    .Select(x => new States { ID = x.ID, State = x.State, StateCode = x.StateCode, IsActive = x.IsActive }).OrderByDescending(x => x.ID).Take(10).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.States.AsEnumerable()
                        .Where(x => x.State.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new States { ID = x.ID, State = x.State, StateCode = x.StateCode, IsActive = x.IsActive }).OrderByDescending(x => x.ID).Take(10).ToList();
                }
                else
                {
                    result = db.States.AsEnumerable()
                        .Select(x => new States { ID = x.ID, State = x.State, StateCode = x.StateCode, IsActive = x.IsActive }).OrderByDescending(x => x.ID).Take(10).ToList();
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
                sb.Append(10);
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
        public JsonResult AddState(States states)
        {
            try
            {
                db.States.Add(states);
                db.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditState(States states)
        {
            try
            {
                
                    States model = db.States.Where(x => x.ID == states.ID).FirstOrDefault();
                    model.State = states.State;
                    model.IsActive = states.IsActive;

                model.StateCode = states.StateCode;
                
                db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json("1", JsonRequestBehavior.AllowGet);
               
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetState(int ID)
        {
            try
            {
                var result = db.States.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult CheckDuplicate(string State, string StateCode, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.States.Where(x => x.State == State || x.StateCode== StateCode && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.States.Where(x => x.State == State ||x.StateCode== StateCode).ToList();
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

        // GET: States/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: States/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(States states)
        {
            if (ModelState.IsValid)
            {
                states.CreatedBy = User.Identity.Name;
                states.CreatedDate = DateTime.Now;
                states.IsActive = true;
                db.States.Add(states);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Added Success!";

                return RedirectToAction("Index");
            }

            return View(states);
        }

        // GET: States/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            States states = await db.States.FindAsync(id);
            if (states == null)
            {
                return HttpNotFound();
            }
            return View(states);
        }

        // POST: States/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(States states)
        {
            if (ModelState.IsValid)
            {
                var toupdate = db.States.Find(states.ID);
                toupdate.UpdatedBy = User.Identity.Name;
                toupdate.UpdatedDate = DateTime.Now;
                db.Entry(toupdate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["success"] = "Record Updated Success!";

                return RedirectToAction("Index");
            }
            return View(states);
        }

        // GET: States/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            States states = await db.States.FindAsync(id);
            if (states == null)
            {
                return HttpNotFound();
            }
            return View(states);
        }

        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var states = db.States.Where(a => a.ID == id).FirstOrDefault();
                db.States.Remove(states);
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
