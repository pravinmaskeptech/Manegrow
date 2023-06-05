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
    public class CitiesController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: Cities
        public async Task<ActionResult> Index()
        {
            return View(await db.Cities.ToListAsync());
        }
        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.Cities.Count();

                var result = db.Cities
                    .AsEnumerable()
                   .Skip(iDisplayStart).Take(iDisplayLength)
                    .Select(x => new City { ID = x.ID, State = x.State, CityName = x.CityName, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.Cities.AsEnumerable()
                        .Where(x => x.CityName.ToLower().Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new City { ID = x.ID, State = x.State, CityName = x.CityName, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.Cities.AsEnumerable()
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new City { ID = x.ID, State = x.State, CityName = x.CityName, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
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
        public JsonResult AddCity(City city)
        {
            try
            {

                db.Cities.Add(city);
                db.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditCity(City city)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    City model = db.Cities.Where(x => x.ID == city.ID).FirstOrDefault();
                    model.CityName = city.CityName;
                    model.State = city.State;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetCity(int ID)
        {
            try
            {
                var result = db.Cities.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult DeleteCity(int id)
        {
            City city = db.Cities.Find(id);
            var entry = db.Cities.Remove(city);
            db.SaveChanges();
            if (entry.ID == id)
            {
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
        // GET: Cities/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // GET: Cities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "State,CityName")] City city)
        {
            if (ModelState.IsValid)
            {
                city.CreatedBy = User.Identity.Name;
                city.CreatedDate = DateTime.Now;
                city.IsActive=true;
                db.Cities.Add(city);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Saved Success!";
                return RedirectToAction("Index");

            }

            return View(city);
        }

        // GET: Cities/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // POST: Cities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(City city)
        {
            if (ModelState.IsValid)
            {
                var toupdate = db.Cities.Find(city.ID);
                toupdate.UpdatedBy = User.Identity.Name;
                toupdate.UpdatedDate = DateTime.Now;
                db.Entry(toupdate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["success"] = "Record Saved Success!";

                return RedirectToAction("Index");
            }
            return View(city);
        }

        // GET: Cities/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var cities = db.Cities.Where(a => a.ID == id).FirstOrDefault();
                db.Cities.Remove(cities);
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
