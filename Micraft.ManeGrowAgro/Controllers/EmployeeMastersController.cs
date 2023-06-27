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
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Micraft.ManeGrowAgro.Controllers
{
    [Authorize]
    public class EmployeeMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: EmployeeMasters
        public async Task<ActionResult> Index()
        {
           
            return View(await db.EmployeeMasters.ToListAsync());
        }

        // GET: EmployeeMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeMaster employeeMaster = await db.EmployeeMasters.FindAsync(id);
            if (employeeMaster == null)
            {
                return HttpNotFound();
            }
            return View(employeeMaster);
        }

        // GET: EmployeeMasters/Create
        public ActionResult Create()
        {




            List<AspNetRole> rulrs = new List<AspNetRole>();
            var x = new AspNetRole(); x.Id = "201"; x.Name = "CustomerOrder"; x.IdView = "301"; x.NameView = "CustomerOrderView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "4"; x.Name = "Harvesting"; x.IdView = "44"; x.NameView = "HarvestingView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "8"; x.Name = "Packing"; x.IdView = "88"; x.NameView = "PackingView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "204"; x.Name = "Dispatch"; x.IdView = "304"; x.NameView = "DispatchView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "205"; x.Name = "PODUpdation"; x.IdView = "305"; x.NameView = "PODUpdationView"; rulrs.Add(x);

            x = new AspNetRole(); x.Id = "206"; x.Name = "Collection"; x.IdView = "306"; x.NameView = "CollectionView"; rulrs.Add(x);

            x = new AspNetRole(); x.Id = "207"; x.Name = "RateUpdation"; x.IdView = "307"; x.NameView = "RateUpdationView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "208"; x.Name = "LocationDetails"; x.IdView = "308"; x.NameView = "LocationDetailsView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "209"; x.Name = "AllMaster"; x.IdView = "309"; x.NameView = "AllMasterEdit"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "210"; x.Name = "Reports"; x.IdView = "310"; x.NameView = "ReportsView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "211"; x.Name = "ResetPassword"; x.IdView = "311"; x.NameView = "ResetPasswordView"; rulrs.Add(x);


            ViewBag.UserRole = rulrs.OrderBy(a => a.NameView).ToList(); 




            var DeptName = db.DepartmentMasters.ToList();
            if (DeptName != null) { ViewBag.DeptName = DeptName; }
            var DesigName = db.EmpDesignations.ToList();
            if (DesigName != null) { ViewBag.DesigName = DesigName; }
            var EmpDetails = db.EmployeeMasters.OrderBy(a => a.Name).ToList();
            if (EmpDetails != null) { ViewBag.EMP = EmpDetails; }





            return View();
        }

        // POST: EmployeeMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmployeeMaster employeeMaster, HttpPostedFileBase PhotoUpload, string PreviousFile)
        {


            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var username = (from u in context.Users
                                where u.UserName == employeeMaster.Username
                                select new
                                {
                                    Username = u.UserName,
                                    Email = u.Email,
                                    Id = u.Id
                                }).Count();


                if (username > 0)
                {
                    TempData["error"] = "Username already taken please enter different username!";
                    return View(employeeMaster);
                }
                else
                {


                    if (PhotoUpload != null && PhotoUpload.ContentLength > 0)
                    {
                        var fileName = PhotoUpload.FileName;
                        string path = Server.MapPath("~/Attachments/EmployeePic/");
                        employeeMaster.PhotoUpload = fileName;
                        PhotoUpload.SaveAs(path + Path.GetFileName(PhotoUpload.FileName));
                    }

                    db.EmployeeMasters.Add(employeeMaster);
                    await db.SaveChangesAsync();
                    long CustId = db.EmployeeMasters.Max(x => x.ID);

                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var user = new ApplicationUser { UserName = employeeMaster.Username, Email = employeeMaster.EmailAddress, UserId = CustId, FullName = employeeMaster.Name, RegistrationType = "Employee", Address = employeeMaster.Address, PhoneNumber = employeeMaster.MobileNumber.ToString() };
                    var adminresult = manager.CreateAsync(user, employeeMaster.Password);
                    db.SaveChanges();

                    //var Userid = context.Users.Where(a => a.UserName == employeeMaster.Username).Select(a => a.Id).SingleOrDefault();

                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == user.Id && a.RoleId == employeeMaster.UserRole).FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = user.Id;
                        asprole.RoleId = employeeMaster.UserRole;
                        db.AspNetUserRoles.Add(asprole);
                    } else
                    {

                        IsExistUser.UserId = user.Id;
                        IsExistUser.RoleId = employeeMaster.UserRole;
                    }
                    db.SaveChanges();
                    TempData["success"] = "Record Added Success!";
                    return RedirectToAction("Index");

                }

            }
            catch (Exception ex)
            {
                var DeptName = db.DepartmentMasters.ToList();
                if (DeptName != null) { ViewBag.DeptName = DeptName; }
                var DesigName = db.EmpDesignations.ToList();
                if (DesigName != null) { ViewBag.DesigName = DesigName; }
                var EmpDetails = db.EmployeeMasters.OrderBy(a => a.Name).ToList();
                if (EmpDetails != null) { ViewBag.EMP = EmpDetails; }
                var UserRoless = db.AspNetRole.OrderBy(a => a.Name).ToList();
                if (EmpDetails != null) { ViewBag.UserRole = UserRoless; }
                TempData["error"] = ex.Message;
                return View(employeeMaster);
            }

        }
        [HttpPost]
        public ActionResult Save(EmployeeMaster employeeMaster, List<EmployeeMaster> UserRoless,List<EmployeeMaster> LSTView)
        {


            ApplicationDbContext context = new ApplicationDbContext();
            try
            {
                long CustId = 0;
                if (employeeMaster.ID == 0)
                {


                    var username = (from u in context.Users
                                    where u.UserName == employeeMaster.Username
                                    select new
                                    {
                                        Username = u.UserName,
                                        Email = u.Email,
                                        Id = u.Id
                                    }).Count();


                    if (username > 0)
                    {
                        TempData["error"] = "Username already taken please enter different username!";
                        return View(employeeMaster);
                    }else
                    {
                        
                        db.EmployeeMasters.Add(employeeMaster);
                        db.SaveChanges();
                        CustId = db.EmployeeMasters.Max(x => x.ID);



                        var store = new UserStore<ApplicationUser>(context);
                        var manager = new ApplicationUserManager(store);
                        var user = new ApplicationUser { UserName = employeeMaster.Username, Email = employeeMaster.EmailAddress, UserId = CustId, FullName = employeeMaster.Name, RegistrationType = "Employee", Address = employeeMaster.Address, PhoneNumber = employeeMaster.MobileNumber.ToString() };
                        var adminresult = manager.Create(user, employeeMaster.Password);
                        db.SaveChanges();
                    }
                    var Userid = context.Users.Where(a => a.UserName == employeeMaster.Username).Select(a => a.Id).SingleOrDefault();
                    db.AspNetUserRoles.RemoveRange(db.AspNetUserRoles.Where(c => c.UserId == Userid));
                    try
                    {
                        foreach (var xx in UserRoless)
                        {

                            var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == Userid && a.RoleId == xx.RoleID).FirstOrDefault();
                            if (IsExistUser == null)
                            {
                                var asprole = new AspNetUserRoles();
                                asprole.UserId = Userid;
                                asprole.RoleId = xx.RoleID;
                                db.AspNetUserRoles.Add(asprole);
                            }
                            else
                            {

                                IsExistUser.UserId = Userid;
                                IsExistUser.RoleId = xx.RoleID;
                            }
                        }
                    }
                    catch { };
                    if(LSTView != null)
                    {
                        try
                        {
                            foreach (var xx in LSTView)
                            {

                                var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == Userid && a.RoleId == xx.RoleID).FirstOrDefault();
                                if (IsExistUser == null)
                                {
                                    var asprole = new AspNetUserRoles();
                                    asprole.UserId = Userid;
                                    asprole.RoleId = xx.RoleID;
                                    db.AspNetUserRoles.Add(asprole);
                                }
                                else
                                {

                                    IsExistUser.UserId = Userid;
                                    IsExistUser.RoleId = xx.RoleID;
                                }
                            }
                        }
                        catch { };

                    }


                    db.SaveChanges();
                    var result = new { Message = "success" };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    CustId = employeeMaster.ID;
                    db.Entry(employeeMaster).State = EntityState.Modified;
                    db.SaveChanges();


                    var username = (from u in context.Users
                                    where u.UserName == employeeMaster.Username
                                    select new
                                    {
                                        Username = u.UserName,
                                        Email = u.Email,
                                        Id = u.Id
                                    }).Count();




                    if (username == 0)
                    {
                        var store = new UserStore<ApplicationUser>(context);
                        var manager = new ApplicationUserManager(store);
                        var user = new ApplicationUser { UserName = employeeMaster.Username, Email = employeeMaster.EmailAddress, UserId = CustId, FullName = employeeMaster.Name, RegistrationType = "Employee", Address = employeeMaster.Address, PhoneNumber = employeeMaster.MobileNumber.ToString() };
                        var adminresult = manager.CreateAsync(user, employeeMaster.Password);
                        db.SaveChanges();
                    }

                  

                    var Userid = context.Users.Where(a => a.UserName == employeeMaster.Username).Select(a => a.Id).SingleOrDefault();
                    db.AspNetUserRoles.RemoveRange(db.AspNetUserRoles.Where(c => c.UserId == Userid));
                
                    db.SaveChanges();
                    try
                    {
                        foreach (var xx in UserRoless)
                        {

                            var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == Userid && a.RoleId == xx.RoleID).FirstOrDefault();
                            if (IsExistUser == null)
                            {
                                var asprole = new AspNetUserRoles();
                                asprole.UserId = Userid;
                                asprole.RoleId = xx.RoleID;
                                db.AspNetUserRoles.Add(asprole);
                            }
                            else
                            {

                                IsExistUser.UserId = Userid;
                                IsExistUser.RoleId = xx.RoleID;
                            }
                        }
                    }
                    catch {; }
                    try
                    {
                        foreach (var xx in LSTView)
                        {

                            var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == Userid && a.RoleId == xx.RoleID).FirstOrDefault();
                            if (IsExistUser == null)
                            {
                                var asprole = new AspNetUserRoles();
                                asprole.UserId = Userid;
                                asprole.RoleId = xx.RoleID;
                                db.AspNetUserRoles.Add(asprole);
                            }
                            else
                            {

                                IsExistUser.UserId = Userid;
                                IsExistUser.RoleId = xx.RoleID;
                            }
                        }
                    }
                    catch {; }
                    db.SaveChanges();

                    var result = new { Message = "successs" };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                var DeptName = db.DepartmentMasters.ToList();
                if (DeptName != null) { ViewBag.DeptName = DeptName; }
                var DesigName = db.EmpDesignations.ToList();
                if (DesigName != null) { ViewBag.DesigName = DesigName; }
                var EmpDetails = db.EmployeeMasters.OrderBy(a => a.Name).ToList();
                if (EmpDetails != null) { ViewBag.EMP = EmpDetails; }
                //  var UserRoless = db.AspNetRole.OrderBy(a => a.Name).ToList();
                //  if (EmpDetails != null) { ViewBag.UserRole = UserRoless; }
                TempData["error"] = ex.Message;
                return View(employeeMaster);
            }

        }


        public JsonResult CheckDuplicateUserCode(string EmpCode)
        {
            var count = db.EmployeeMasters.Where(a => a.EmpCode == EmpCode).Count();
            return Json(count, JsonRequestBehavior.AllowGet);
        }
        
        //[HttpPost]
        //public ActionResult UploadFiles()
        //{
        //    // Checking no of files injected in Request object  
        //    if (Request.Files.Count > 0)
        //    {
        //        try
        //        {
        //            //  Get all files from Request object  
        //            HttpFileCollectionBase files = Request.Files;
        //            for (int i = 0; i < files.Count; i++)
        //            {
        //                //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
        //                //string filename = Path.GetFileName(Request.Files[i].FileName);  

        //                HttpPostedFileBase file = files[i];
        //                string fname;

        //                // Checking for Internet Explorer  
        //                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //                {
        //                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                    fname = testfiles[testfiles.Length - 1];
        //                }
        //                else
        //                {
        //                    fname = file.FileName;
        //                }

        //                // Get the complete folder path and store the file inside it.  
        //                fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
        //                file.SaveAs(fname);
        //            }
        //            // Returns message that successfully uploaded  
        //            return Json("File Uploaded Successfully!");
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json("Error occurred. Error details: " + ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        return Json("No files selected.");
        //    }
        //}

        // GET: EmployeeMasters/Edit/5

        public async Task<ActionResult> Edit(int? id)
        {
            var DeptName = db.DepartmentMasters.ToList();
            if (DeptName != null) { ViewBag.DeptName = DeptName; }
            var DesigName = db.EmpDesignations.ToList();
            if (DesigName != null) { ViewBag.DesigName = DesigName; }
            var EmpDetails = db.EmployeeMasters.OrderBy(a => a.Name).ToList();
            
            
            
            if (EmpDetails != null) { ViewBag.EMP = EmpDetails; }
          


            List<AspNetRole> rulrs = new List<AspNetRole>();
            var x = new AspNetRole();  x.Id = "201";x.Name = "CustomerOrder";x.IdView = "301";x.NameView = "CustomerOrderView"; rulrs.Add(x);
            x = new AspNetRole();  x.Id = "4"; x.Name = "Harvesting";x.IdView = "44"; x.NameView = "HarvestingView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "8"; x.Name = "Packing"; x.IdView = "88"; x.NameView = "PackingView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "204"; x.Name = "Dispatch"; x.IdView = "304"; x.NameView = "DispatchView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "205"; x.Name = "PODUpdation"; x.IdView = "305"; x.NameView = "PODUpdationView"; rulrs.Add(x);

            x = new AspNetRole(); x.Id = "206"; x.Name = "Collection"; x.IdView = "306"; x.NameView = "CollectionView"; rulrs.Add(x);

            x = new AspNetRole(); x.Id = "207"; x.Name = "RateUpdation"; x.IdView = "307"; x.NameView = "RateUpdationView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "208"; x.Name = "LocationDetails"; x.IdView = "308"; x.NameView = "LocationDetailsView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "209"; x.Name = "AllMaster"; x.IdView = "309"; x.NameView = "AllMasterEdit"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "210"; x.Name = "Reports"; x.IdView = "310"; x.NameView = "ReportsView"; rulrs.Add(x);
            x = new AspNetRole(); x.Id = "211"; x.Name = "ResetPassword"; x.IdView = "311"; x.NameView = "ResetPasswordView"; rulrs.Add(x);


            if (EmpDetails != null) { ViewBag.UserRole = rulrs.OrderBy(a=>a.NameView).ToList(); }

           

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeMaster employeeMaster = await db.EmployeeMasters.FindAsync(id);
            if (employeeMaster == null)
            {
                return HttpNotFound();
            }
            ApplicationDbContext context = new ApplicationDbContext();
            var Userid = context.Users.Where(a => a.UserName == employeeMaster.Username).Select(a => a.Id).SingleOrDefault();
            ViewBag.UserRoleID = db.AspNetUserRoles.Where(a => a.UserId == Userid ).ToList();

            return View(employeeMaster);
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // POST: EmployeeMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EmployeeMaster employeeMaster, HttpPostedFileBase PhotoUpload, string PreviousFile )   
        {
            try
            {

                if (PhotoUpload != null && PhotoUpload.ContentLength > 0)
                {
                    var fileName = PhotoUpload.FileName;
                    string path = Server.MapPath("~/Attachments/EmployeePic/");
                    employeeMaster.PhotoUpload = fileName;
                    PhotoUpload.SaveAs(path + Path.GetFileName(PhotoUpload.FileName));
                }
                else
                {
                    employeeMaster.PhotoUpload = PreviousFile;
                }

                db.Entry(employeeMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();

                var aspuserID = "";


                ApplicationDbContext context = new ApplicationDbContext();
                var userdetails = (from u in context.Users
                                   where u.UserName == employeeMaster.Username
                                   select new
                                   {
                                       Username = u.UserName,
                                       Email = u.Email
                                   }).Count();

                if (userdetails == 1)
                {
                    var user = await UserManager.FindByNameAsync(employeeMaster.Username);
                    if (user == null)
                    {
                        TempData["error"] = "Username Not Found....";
                        return View(employeeMaster);
                    }
                    aspuserID = user.Id;
                    var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var result = await UserManager.ResetPasswordAsync(user.Id, code, employeeMaster.Password);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Record Updated Success!";
                        //return RedirectToAction("Index");
                    }
                } else
                {
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var users = new ApplicationUser { UserName = employeeMaster.Username, Email = employeeMaster.EmailAddress, UserId = employeeMaster.ID, FullName = employeeMaster.Name, RegistrationType = "Employee", Address = employeeMaster.Address, PhoneNumber = employeeMaster.MobileNumber.ToString() };
                    var adminresult = manager.CreateAsync(users, employeeMaster.Password);

                    aspuserID = users.Id;

                }
                var UserRoleList = db.AspNetUserRoles.Where(a => a.UserId == aspuserID).ToList();
                db.AspNetUserRoles.RemoveRange(UserRoleList);
                db.SaveChanges();


              


                var Userid = context.Users.Where(a => a.UserName == employeeMaster.Username).Select(a => a.Id).SingleOrDefault();
                var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == Userid && a.RoleId == employeeMaster.UserRole).FirstOrDefault();
                if (IsExistUser == null)
                {
                    var asprole = new AspNetUserRoles();
                    asprole.UserId = Userid;
                    asprole.RoleId = employeeMaster.UserRole;
                    db.AspNetUserRoles.Add(asprole);
                }
                else
                {
                    IsExistUser.UserId = Userid;
                    IsExistUser.RoleId = employeeMaster.UserRole;
                }

                db.SaveChanges();




                var rol = db.AspNetRole.Where(a=>a.Name=="").ToList();

                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                var DeptName = db.DepartmentMasters.ToList();
                if (DeptName != null) { ViewBag.DeptName = DeptName; }


                var DesigName = db.EmpDesignations.ToList();
                if (DesigName != null) { ViewBag.DesigName = DesigName; }
                var EmpDetails = db.EmployeeMasters.OrderBy(a => a.Name).ToList();
                if (EmpDetails != null) { ViewBag.EMP = EmpDetails; }
                var UserRoless = db.AspNetRole.OrderBy(a => a.Name).ToList();
                if (EmpDetails != null) { ViewBag.UserRole = UserRoless; }
                return View(employeeMaster);
            }
        }

        // GET: EmployeeMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EmployeeMaster employeeMaster = await db.EmployeeMasters.FindAsync(id);
            if (employeeMaster == null)
            {
                return HttpNotFound();
            }
            return View(employeeMaster);
        }

        // POST: EmployeeMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EmployeeMaster employeeMaster = await db.EmployeeMasters.FindAsync(id);
            db.EmployeeMasters.Remove(employeeMaster);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.EmployeeMasters.Count();

                //var result = db.EmployeeMasters
                //    .AsEnumerable()
                //    .Select(x => new EmployeeMaster { ID = x.ID, Name = x.Name, Address = x.Address, AdharNumber = x.AdharNumber, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress,DesigID=x.DesigID }).OrderByDescending(x => x.ID).ToList();
                var result = (from e in db.EmployeeMasters
                              join dp in db.DepartmentMasters
                              on e.DepartmentID equals dp.ID
                              join ds in db.EmpDesignations
                              on e.DesigID equals ds.ID
                              select new
                              {
                                  ID = e.ID,
                                  Name = e.Name,
                                  Address = e.Address,
                                  AdharNumber = e.AdharNumber,
                                  MobileNumber = e.MobileNumber,
                                  EmailAddress = e.EmailAddress,
                                  Designation = ds.Designation

                              }).OrderByDescending(x => x.ID).ToList();
                if (!string.IsNullOrEmpty(sSearch))
                {
                    //result = db.EmployeeMasters.AsEnumerable()
                    //    .Where(x => x.Name.ToLower().Contains(sSearch))
                    //    .Take(iDisplayLength)
                    //    .Select(x => new EmployeeMaster { ID = x.ID, Name = x.Name, Address = x.Address, AdharNumber = x.AdharNumber, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress,DesigID=x.DesigID }).OrderByDescending(x => x.ID).ToList();
                    result = (from e in db.EmployeeMasters
                              join dp in db.DepartmentMasters
                              on e.DepartmentID equals dp.ID
                              join ds in db.EmpDesignations
                              on e.DesigID equals ds.ID
                              select new
                              {
                                  ID = e.ID,
                                  Name = e.Name,
                                  Address = e.Address,
                                  AdharNumber = e.AdharNumber,
                                  MobileNumber = e.MobileNumber,
                                  EmailAddress = e.EmailAddress,
                                  Designation = ds.Designation

                              }).Where(x => x.Name.ToLower().Contains(sSearch))
                              .Take(iDisplayLength)
                              .OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    //result = db.EmployeeMasters.AsEnumerable()
                    //    .Select(x => new EmployeeMaster { ID = x.ID, Name = x.Name, Address = x.Address, AdharNumber = x.AdharNumber, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress,DesigID=x.DesigID }).OrderByDescending(x => x.ID).ToList();
                    result = (from e in db.EmployeeMasters
                              join dp in db.DepartmentMasters
                              on e.DepartmentID equals dp.ID
                              join ds in db.EmpDesignations
                              on e.DesigID equals ds.ID
                              select new
                              {
                                  ID = e.ID,
                                  Name = e.Name,
                                  Address = e.Address,
                                  AdharNumber = e.AdharNumber,
                                  MobileNumber = e.MobileNumber,
                                  EmailAddress = e.EmailAddress,
                                  Designation = ds.Designation

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

        public JsonResult GetCityArea(string Pincode)
        {
            try
            {
                var pincode = db.PincodeMasters.Where(a => a.Pincode == Pincode).ToList();
                db.SaveChanges();
                var result = new { Message = "success", pincode };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult AddEmployee(EmployeeMaster employeeMaster)
        {
            try
            {
                db.EmployeeMasters.Add(employeeMaster);
                db.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var emp = db.EmployeeMasters.Where(a => a.ID == id).FirstOrDefault();
                var username = emp.Username;
                db.EmployeeMasters.Remove(emp);
                var Userid = db.AspNetUsers.Where(a => a.UserId == emp.ID).FirstOrDefault();
                var aspuser = db.AspNetUserRoles.RemoveRange(db.AspNetUserRoles.Where(c => c.UserId == Userid.Id));
                db.AspNetUsers.Remove(Userid);
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
