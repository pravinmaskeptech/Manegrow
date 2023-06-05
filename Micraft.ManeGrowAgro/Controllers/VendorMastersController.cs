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
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class VendorMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: VendorMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.VendorMasters.ToListAsync());
        }

        // GET: VendorMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorMaster vendorMaster = await db.VendorMasters.FindAsync(id);
            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
            return View(vendorMaster);
        }

        // GET: VendorMasters/Create
        public ActionResult Create()
        {
            ViewBag.VendorType = db.VendorTypes.OrderBy(a => a.VendorType).ToList();
            ViewBag.Quotation = db.QuotationMain.ToList();
            return View();
        }


        // POST: VendorMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VendorMaster vendorMaster, HttpPostedFileBase VendorPhoto)
        {
            try
            {
                //Dattatrymulik
                ViewBag.Quotation = db.QuotationMain.ToList();

                var duplicateUsername = db.VendorMasters.Where(a => a.UserName == vendorMaster.UserName).Count();
                if (duplicateUsername != 0)
                {
                    TempData["error"] = "Username Already added!";
                    return View(vendorMaster);
                }
                //if (ModelState.IsValid)
                //{
                if (VendorPhoto != null)
                {
                    vendorMaster.VendorPhoto = VendorPhoto.FileName;
                }

                vendorMaster.CreatedBy = User.Identity.Name;
                vendorMaster.CreatedDate = DateTime.Now;
                db.VendorMasters.Add(vendorMaster);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Added Successfully!";



                long CustId = db.VendorMasters.Max(x => x.ID);






                ApplicationDbContext context = new ApplicationDbContext();
                var user = (from u in context.Users
                            where u.UserName == vendorMaster.UserName
                            select new
                            {
                                Username = u.UserName,
                                Id = u.Id,
                            }).FirstOrDefault();


                if (user == null)
                {

                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var users = new ApplicationUser { UserName = vendorMaster.UserName, Email = vendorMaster.EmailId, UserId = CustId, FullName = vendorMaster.VendorName, RegistrationType = "Vendor", Address = vendorMaster.Address, PhoneNumber = vendorMaster.MobileNo.ToString() };
                    var adminresult = manager.CreateAsync(users, vendorMaster.Password);



                    var UserRoleList = db.AspNetUserRoles.Where(a => a.UserId == users.Id && a.RoleId == "99").ToList();
                    db.AspNetUserRoles.RemoveRange(UserRoleList);
                    db.SaveChanges();


                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == users.Id && a.RoleId == "99").FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = users.Id;
                        asprole.RoleId = "99";
                        db.AspNetUserRoles.Add(asprole);
                    }
                }
                else
                {

                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == user.Id && a.RoleId == "99").FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = user.Id;
                        asprole.RoleId = "99";
                        db.AspNetUserRoles.Add(asprole);
                    }
                }
                db.SaveChanges();
                TempData["success"] = "Record successfully added!";
                return RedirectToAction("Index");


            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(vendorMaster);
            }
        }

        // GET: VendorMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            ViewBag.Quotation = db.QuotationMain.ToList();
            ViewBag.VendorType = db.VendorTypes.OrderBy(a => a.VendorType).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorMaster vendorMaster = await db.VendorMasters.FindAsync(id);
            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
            return View(vendorMaster);
        }

        // POST: VendorMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VendorMaster vendorMaster, HttpPostedFileBase VendorPhoto, string OldVendorPhoto)
        {
            try
            {
                var duplicateUsername = db.VendorMasters.Where(a => a.UserName == vendorMaster.UserName && a.ID != vendorMaster.ID).Count();
                if (duplicateUsername != 0)
                {
                    TempData["error"] = "Username Already added!";
                    return View(vendorMaster);
                }
                //if (ModelState.IsValid)
                //{
                //        VendorMaster vendorMaster1 = db.VendorMasters.Find(vendorMaster.ID);

                try
                {
                    if (VendorPhoto != null)
                    {
                        vendorMaster.VendorPhoto = VendorPhoto.FileName;
                    }
                    else
                    {
                        vendorMaster.VendorPhoto = OldVendorPhoto;
                    }
                }
                catch (Exception Ex)
                { }

                vendorMaster.UpdatedBy = User.Identity.Name;
                vendorMaster.UpdatedDate = DateTime.Now;
                db.Entry(vendorMaster).State = EntityState.Modified;


                db.SaveChanges();
                ApplicationDbContext context = new ApplicationDbContext();
                var user = (from u in context.Users
                            where u.UserName == vendorMaster.UserName
                            select new
                            {
                                Username = u.UserName,
                                Id = u.Id,
                            }).FirstOrDefault();


                if (user == null)
                {

                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var users = new ApplicationUser { UserName = vendorMaster.UserName, Email = vendorMaster.EmailId, UserId = vendorMaster.ID, FullName = vendorMaster.VendorName, RegistrationType = "Vendor", Address = vendorMaster.Address, PhoneNumber = vendorMaster.MobileNo.ToString() };
                    var adminresult = manager.CreateAsync(users, vendorMaster.Password);



                    var UserRoleList = db.AspNetUserRoles.Where(a => a.UserId == users.Id && a.RoleId == "99").ToList();
                    db.AspNetUserRoles.RemoveRange(UserRoleList);
                    db.SaveChanges();


                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == users.Id && a.RoleId == "99").FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = users.Id;
                        asprole.RoleId = "99";
                        db.AspNetUserRoles.Add(asprole);
                    }
                }
                else
                {



                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == user.Id && a.RoleId == "99").FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = user.Id;
                        asprole.RoleId = "99";
                        db.AspNetUserRoles.Add(asprole);
                    }
                }
                db.SaveChanges();
                TempData["success"] = "Record Updated Success!";
                return RedirectToAction("Index");
                // }
                //   return View(vendorMaster);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(vendorMaster);
            }
        }

        // GET: VendorMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VendorMaster vendorMaster = await db.VendorMasters.FindAsync(id);
            if (vendorMaster == null)
            {
                return HttpNotFound();
            }
            return View(vendorMaster);
        }

        // POST: VendorMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            VendorMaster vendorMaster = await db.VendorMasters.FindAsync(id);
            db.VendorMasters.Remove(vendorMaster);
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

        //upload photo
        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Attachments/VendorImages/"), fname);
                        file.SaveAs(fname);
                        string extension = Path.GetExtension(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        //Duplicate Name

        [HttpGet]
        public JsonResult CheckDuplicateVendorName(string VendorName, int? ID, string Type)
        {
            var Count = 0;
            if (Type == "Create")
            {
                Count = db.VendorMasters.Where(a => a.VendorName == VendorName).Count();
            }
            else
            {
                Count = db.VendorMasters.Where(a => a.VendorName == VendorName && a.ID != ID).Count();
            }
            return Json(Count, JsonRequestBehavior.AllowGet);
        }




       // Auto fill

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
        public JsonResult PincodeAutoComplete(string prefix, string city)
        {
            var customers = (from pin in db.PincodeMasters
                             where pin.Pincode.StartsWith(prefix) && pin.City.Equals(city)
                             select new
                             {
                                 label = pin.Pincode,
                                 val = pin.Area
                             }).Take(10).ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult AutoComplete(string prefix, string city)
        {
            var customers = (from pin in db.PincodeMasters
                             where pin.Area.StartsWith(prefix) && pin.City.Equals(city)
                             select new
                             {
                                 label = pin.Area,
                                 val = pin.Pincode
                             }).Take(10).ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoCity(string prefix)
        {
            var customers = (from c in db.Cities
                             where c.CityName.StartsWith(prefix)
                             select new
                             {
                                 label = c.CityName,
                                 val = c.State
                             }).Take(10).ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AutoState(string prefix)
        {
            var customers = (from s in db.States
                             where s.State.StartsWith(prefix)
                             select new
                             {
                                 label = s.State,
                                 val = s.ID
                             }).Take(10).ToList();

            return Json(customers, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult DuplicteUserName(string Username)
        {
            var Count = 0;
            ApplicationDbContext context = new ApplicationDbContext();
            Count = context.Users.Where(u => u.UserName == Username).Count();
            return Json(Count, JsonRequestBehavior.AllowGet);
        }


    }
}
