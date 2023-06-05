using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class DriverMasterController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: DriverMaster
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.Vendor = db.VendorMasters.OrderBy(a => a.VendorName).ToList();

           


            return View();
        }


        [HttpPost]
        public ActionResult Create(DriverMaster driverMaster)
        {
            try
            {              
                    driverMaster.CreatedBy = User.Identity.Name;
                    driverMaster.CreatedDate = DateTime.Now;
                    db.DriverMaster.Add(driverMaster);
                     db.SaveChanges();
                    TempData["success"] = "Record Added Success!";
                    return RedirectToAction("Index");
              
            }
            catch (Exception ex)
            {
                ViewBag.Vendor = db.VendorMasters.OrderBy(a => a.VendorName).ToList();
                TempData["error"] = ex.Message;
                return View(driverMaster);
            }
        }


        public JsonResult GetCityArea(string Pincode)
        {
            try
            {
                var pincode = db.PincodeMasters.Where(a => a.Pincode == Pincode).ToList();
                var result = new { Message = "success", pincode, Count = pincode.Count() };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.DriverMaster.Count();

                var result1 = (from dr in db.DriverMaster

                               join a in db.VendorMasters
                              on dr.VendorID equals a.ID

                               select new
                               {
                                   ID = dr.Id,
                                   VendorName = a == null ? string.Empty : a.VendorName,
                                   MobileNo = dr.MobileNo,
                                   DriverName = dr.DriverName, 
                                   AdharNo = dr.AdharNo,
                                   ContactPerson = dr.Pincode,
                                   City = dr.City,
                                   State = dr.State,
                                   Reference = dr.Reference,
                                   ReferenceContact = dr.ReferenceContact,
                                   Address = dr.Address, 

                               }).OrderBy(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result1 = result1.Where(a => a.DriverName.ToLower().Contains(sSearch)).OrderByDescending(a=>a.DriverName).ToList();
                }
                else
                {
                    result1 = result1.OrderByDescending(a => a.DriverName).ToList();
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
                sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result1));
                sb.Append("}");
                return sb.ToString();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Vendor = db.VendorMasters.OrderBy(a => a.VendorName).ToList();
            DriverMaster driverMaster = await db.DriverMaster.FindAsync(id);
            if (driverMaster == null)
            {
                return HttpNotFound();
            }
            return View(driverMaster);
        }

        // POST: BankMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(DriverMaster driverMaster)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var toupdate = db.DriverMaster.Find(driverMaster.Id);
        //            toupdate.UpdatedBy = User.Identity.Name;
        //            toupdate.UpdatedDate = DateTime.Now;
        //            db.Entry(driverMaster).State = EntityState.Modified;
        //             db.SaveChanges();
        //            TempData["success"] = "Record Updated Success";
        //            return RedirectToAction("Index");
        //        }
        //        return View(driverMaster);
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["error"] = ex.Message;
        //        return View(driverMaster);
        //    }
        //}


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DriverMaster driverMaster)
        {
            ViewBag.Vendor = db.VendorMasters.OrderBy(a => a.VendorName).ToList();
            driverMaster.UpdatedBy = User.Identity.Name;
            driverMaster.UpdatedDate = DateTime.Now;

            db.Entry(driverMaster).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Record Edited Successfully";
            return RedirectToAction("Index");

        }


        public ActionResult CheckDuplicate(string DriverName, string Mode, int ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.DriverMaster.Where(x => x.DriverName == DriverName && x.Id != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.DriverMaster.Where(x => x.DriverName == DriverName).ToList();
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
                var regionMaster = db.DriverMaster.Where(a => a.Id == id).FirstOrDefault();
                db.DriverMaster.Remove(regionMaster);
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

    }
}