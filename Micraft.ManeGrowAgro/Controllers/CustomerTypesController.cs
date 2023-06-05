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
    public class CustomerTypesController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: CustomerTypes
        public async Task<ActionResult> Index()
        {
            return View(await db.CustomerTypes.ToListAsync());
        }



        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.CustomerTypes.Count();

                var result = db.CustomerTypes
                    .AsEnumerable()
                    .Select(x => new CustomerType { ID = x.ID, Type = x.Type, Prefix=x.Prefix,StartCode=x.StartCode,IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.CustomerTypes.AsEnumerable()
                        .Where(x => x.Type.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new CustomerType { ID = x.ID, Type = x.Type, Prefix = x.Prefix, StartCode = x.StartCode, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.CustomerTypes.AsEnumerable()
                        .Select(x => new CustomerType { ID = x.ID, Type = x.Type, Prefix = x.Prefix, StartCode = x.StartCode, IsActive = x.IsActive }).OrderByDescending(x => x.ID).ToList();
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
        public JsonResult AddRecord(CustomerType customerType)
        {
            try
            {
                db.CustomerTypes.Add(customerType);
                db.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditRecord(CustomerType customerType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    CustomerType model = db.CustomerTypes.Where(x => x.ID == customerType.ID).FirstOrDefault();
                    model.Type = customerType.Type;
                    model.Prefix= customerType.Prefix;
                    model.StartCode = customerType.StartCode;
                    model.IsActive = customerType.IsActive;
                    model.UpdatedBy = User.Identity.Name;
                    model.UpdatedDate = DateTime.Now;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["success"] = "Record Update success!";
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
                var result = db.CustomerTypes.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CustomerTypes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerType customerType = await db.CustomerTypes.FindAsync(id);
            if (customerType == null)
            {
                return HttpNotFound();
            }
            return View(customerType);
        }

        // GET: CustomerTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerType customerType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    customerType.IsActive = true;
                    customerType.CreatedBy = User.Identity.Name;
                    customerType.CreatedDate = DateTime.Now;
                    db.CustomerTypes.Add(customerType);
                    await db.SaveChangesAsync();
                    TempData["success"] = "Record Added success!";
                    return RedirectToAction("Index");
                }

                return View(customerType);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(customerType);
            }
        }
        // GET: CustomerTypes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerType customerType = await db.CustomerTypes.FindAsync(id);
            if (customerType == null)
            {
                return HttpNotFound();
            }
            return View(customerType);
        }

        // POST: CustomerTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerType customerType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerType).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customerType);
        }
        public ActionResult CheckDuplicate(string Type,string Prefix,Decimal StartCode, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.CustomerTypes.Where(x => x.Type == Type && x.Prefix==Prefix && x.StartCode== StartCode && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.CustomerTypes.Where(x => x.Type == Type && x.Prefix == Prefix && x.StartCode == StartCode && x.ID != ID).ToList();
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

        // GET: CustomerTypes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerType customerType = await db.CustomerTypes.FindAsync(id);
            if (customerType == null)
            {
                return HttpNotFound();
            }
            return View(customerType);
        }


        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var customerTypes = db.CustomerTypes.Where(a => a.ID == id).FirstOrDefault();
                db.CustomerTypes.Remove(customerTypes);
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
