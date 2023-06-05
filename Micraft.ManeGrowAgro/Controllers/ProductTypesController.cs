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
    public class ProductTypesController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: ProductTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.ProductTypes.ToListAsync());
        }
        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.ProductTypes.Count();

                var result = db.ProductTypes
                    .AsEnumerable()
                    .Select(x => new ProductType { ID = x.ID, Type = x.Type, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.ProductTypes.AsEnumerable()
                        .Where(x => x.Type.ToLower().Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new ProductType { ID = x.ID, Type = x.Type, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.ProductTypes.AsEnumerable()
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new ProductType { ID = x.ID, Type = x.Type, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
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
        public JsonResult AddRecord(ProductType productType)
        {
            try
            {
                db.ProductTypes.Add(productType);
                db.SaveChanges();
                TempData["success"] = "Record Added Success!";
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);             
                //  return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateRecord(ProductType productType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var toupdate = db.ProductTypes.Find(productType.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
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

        [HttpPost]
        public JsonResult GetRecord(int ID)
        {
            try
            {
                var result = db.ProductTypes.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        // GET: ProductTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductType productType = await db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // GET: ProductTypes/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: ProductTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                productType.CreatedBy = User.Identity.Name;
                productType.CreatedDate = DateTime.Now;
                productType.IsActive = true;    
                db.ProductTypes.Add(productType);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Added Success!";
                return RedirectToAction("Index");
            }

            return View(productType);
        }

        // GET: ProductTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductType productType = await db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // POST: ProductTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Type")] ProductType productType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productType);
        }

        public ActionResult CheckDuplicate(string Type, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.ProductTypes.Where(x => x.Type == Type && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.ProductTypes.Where(x => x.Type == Type).ToList();
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



        // GET: ProductTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductType productType = await db.ProductTypes.FindAsync(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }




        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var productTypes = db.ProductTypes.Where(a => a.ID == id).FirstOrDefault();
                db.ProductTypes.Remove(productTypes);
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
