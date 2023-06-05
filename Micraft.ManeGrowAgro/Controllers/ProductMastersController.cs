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
    public class ProductMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: ProductMasters
        public async Task<ActionResult> Index()
        {
            ViewBag.ProductType = db.ProductTypes.ToList();
            ViewBag.ProdUom = db.UoMeasures.ToList();


            return View(await db.ProductMasters.ToListAsync());
        }
        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.ProductMasters.Count();

                //var result = db.ProductMasters
                //    .AsEnumerable()
                //    .Select(x => new ProductMaster { ID = x.ID, Name = x.Name, TypeID = x.TypeID, Size = x.Size, Weight = x.Weight, Rate = x.Rate, Image = x.Image }).OrderByDescending(x => x.ID).ToList();

                var result = (from p in db.ProductMasters
                              join p2 in db.ProductTypes
                              on p.TypeID equals p2.ID
                              select new
                              {
                                  ID = p.ID,
                                  Name = p.Name,
                                  Type = p2.Type,
                                  Size = p.Size,
                                  Weight = p.Weight,
                                  Rate = p.Rate,
                                  Image = p.Image,
                                  IsActive = p.IsActive
                              }).OrderByDescending(x => x.ID).ToList();



                if (!string.IsNullOrEmpty(sSearch))
                {
                    //result = db.ProductMasters.AsEnumerable()
                    //    .Where(x => x.Name.ToLower().Contains(sSearch))
                    //    .Take(iDisplayLength)
                    //    .Select(x => new ProductMaster { ID = x.ID, Name = x.Name, TypeID = x.TypeID, Size = x.Size, Weight = x.Weight, Rate = x.Rate, Image = x.Image }).OrderByDescending(x => x.ID).ToList();
                    result = (from p in db.ProductMasters
                              join p2 in db.ProductTypes
                              on p.TypeID equals p2.ID
                              where p.Name.ToLower() == sSearch.ToLower()
                              select new
                              {
                                  ID = p.ID,
                                  Name = p.Name,
                                  Type = p2.Type,
                                  Size = p.Size,
                                  Weight = p.Weight,
                                  Rate = p.Rate,
                                  Image = p.Image,
                                  IsActive = p.IsActive
                              }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    //result = db.ProductMasters.AsEnumerable()
                    //    .Select(x => new ProductMaster { ID = x.ID, Name = x.Name, TypeID = x.TypeID, Size = x.Size, Weight = x.Weight, Rate = x.Rate, Image = x.Image }).OrderByDescending(x => x.ID).ToList();
                    result = (from p in db.ProductMasters
                              join p2 in db.ProductTypes
                              on p.TypeID equals p2.ID
                              select new
                              {
                                  ID = p.ID,
                                  Name = p.Name,
                                  Type = p2.Type,
                                  Size = p.Size,
                                  Weight = p.Weight,
                                  Rate = p.Rate,
                                  Image = p.Image,
                                  IsActive = p.IsActive
                              }).OrderByDescending(x => x.ID).ToList();
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
        public JsonResult AddRecord(ProductMaster productMaster)
        {
            try
            {
                db.ProductMasters.Add(productMaster);
                db.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdateRecord(ProductMaster productMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ProductMaster model = db.ProductMasters.Where(x => x.ID == productMaster.ID).FirstOrDefault();
                    model.Name = productMaster.Name;
                    model.TypeID = productMaster.TypeID;
                    model.Size = productMaster.Size;
                    model.Weight = productMaster.Weight;
                    model.Rate = productMaster.Rate;
                    model.Image = productMaster.Image;
                    model.ProdUom = productMaster.ProdUom;
                    model.IsActive = productMaster.IsActive;
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
        public JsonResult GetRecord(int ID)
        {
            try
            {
                var result = db.ProductMasters.Where(m => m.ID == ID).FirstOrDefault();

                //ViewBag.ProductType = db.ProductTypes.ToList();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: ProductMasters/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMaster productMaster = await db.ProductMasters.FindAsync(id);
            if (productMaster == null)
            {
                return HttpNotFound();
            }
            return View(productMaster);
        }

        // GET: ProductMasters/Create
        public ActionResult Create()
        {
            var ProdType = db.ProductTypes.ToList();
            ViewBag.ProdType = ProdType;
            ViewBag.ProdUom = db.UoMeasures.ToList();

            return View();
        }

        // POST: ProductMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( ProductMaster productMaster)
        {
           
                db.ProductMasters.Add(productMaster);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
           
        }

        // GET: ProductMasters/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            var ProdType = db.ProductTypes.ToList();
            ViewBag.ProdType = ProdType;
            ViewBag.ProdUom = db.UoMeasures.ToList();


            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMaster productMaster = await db.ProductMasters.FindAsync(id);
            if (productMaster == null)
            {
                return HttpNotFound();
            }
            return View(productMaster);
        }

        // POST: ProductMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductMaster productMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(productMaster);
        }

        // GET: ProductMasters/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMaster productMaster = await db.ProductMasters.FindAsync(id);
            if (productMaster == null)
            {
                return HttpNotFound();
            }
            return View(productMaster);
        }

        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var productMasters = db.ProductMasters.Where(a => a.ID == id).FirstOrDefault();
                db.ProductMasters.Remove(productMasters);
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
