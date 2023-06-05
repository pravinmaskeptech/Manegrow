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
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Configuration;
using Micraft.ManeGrowAgro.Security;

namespace Micraft.ManeGrowAgro.Controllers
{
    [Authorize]
    public class CustomerMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: CustomerMasters
        [CustomAuthorize("You dont have access to View Customer List", "CustomerOrderView, CustomerOrderEdit, Admin")]
        public async Task<ActionResult> Index()
        {           
            var CustType = db.CustomerTypes.ToList();
            if(CustType != null) { ViewBag.CustType = CustType; }
            var SalesID = db.EmpDesignations.Where(a => a.Designation.Contains("SALES")).Select(a=>a.ID).ToList();
            var SalesPerson = db.EmployeeMasters.Where(a=> SalesID.Contains(a.DesigID)).ToList();
            if(SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }

            return View(await db.CustomerMasters.ToListAsync());
        }

        [HttpPost]
        public JsonResult GenerateCustomerID(int? CustID)
        {
            try
            {
                var customerType = db.CustomerTypes.Where(x => x.ID == CustID).FirstOrDefault();
                var type = customerType.Prefix + "" + customerType.StartCode.ToString();
                return Json(type, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        // GET: CustomerMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerMaster customerMaster = await db.CustomerMasters.FindAsync(id);
            if (customerMaster == null)
            {
                return HttpNotFound();
            }
            return View(customerMaster);
        }


        // GET: CustomerMasters/Create
        [CustomAuthorize("You dont have access to Create Customer","CustomerOrderEdit, Admin")]
        public ActionResult Create()
        {
            var CustType = db.CustomerTypes.ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesID = db.DepartmentMasters.Where(a => a.Department.Contains("SALES")).Select(a => a.ID).ToList();
            var SalesPerson = db.EmployeeMasters.Where(a => SalesID.Contains(a.DepartmentID)).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
            ViewBag.Quotations = db.QuotationMain.OrderBy(a => a.QuotationName).ToList();
            return View();
        }

        // POST: CustomerMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerMaster customerMaster)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var username = (from u in context.Users
                                where u.UserName == customerMaster.Username
                                select new
                                {
                                    Username = u.UserName,
                                    Email = u.Email
                                }).FirstOrDefault(); 


                if (username != null) 
                {
                    TempData["error"] = username.Username + " already taken please enter different username!";
                    return View();
                }
                else
                {
                    customerMaster.IsActive = true;
                    customerMaster.IsApproved = true;
                    customerMaster.CreatedBy = User.Identity.Name;
                    customerMaster.CreatedDate = DateTime.Now; 

                    db.CustomerMasters.Add(customerMaster);

                    var custtype=db.CustomerTypes.Where(a=>a.ID==customerMaster.CustTypeID).FirstOrDefault();
                    custtype.StartCode = custtype.StartCode + 1;

                    await db.SaveChangesAsync();



                    long CustId = db.CustomerMasters.Max(x => x.ID);
                    //ApplicationDbContext context = new ApplicationDbContext();
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var user = new ApplicationUser { UserName = customerMaster.Username, Email = customerMaster.EmailAddress, UserId = CustId, FullName = customerMaster.CustName, RegistrationType = "Customer", Address = customerMaster.Address, PhoneNumber = customerMaster.MobileNumber.ToString() };
                    var adminresult = manager.CreateAsync(user, customerMaster.Password);

                    var UserRoleList  = db.AspNetUserRoles.Where(a => a.UserId == user.Id && a.RoleId == "5").ToList();

                    db.AspNetUserRoles.RemoveRange(UserRoleList);  
                    db.SaveChanges();

                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == user.Id && a.RoleId == "5").FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = user.Id;
                        asprole.RoleId = "5";
                        db.AspNetUserRoles.Add(asprole);
                    }
                   
                    db.SaveChanges();
                    TempData["success"] = "Record successfully added!";
                    return RedirectToAction("Index");

                }
               

            }
            catch (Exception Ex)
            {
                var CustType = db.CustomerTypes.ToList();
                if (CustType != null) { ViewBag.CustType = CustType; }
                var SalesID = db.DepartmentMasters.Where(a => a.Department.Contains("SALES")).Select(a => a.ID).ToList();
                var SalesPerson = db.EmployeeMasters.Where(a => SalesID.Contains(a.DepartmentID)).ToList();
                if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
                ViewBag.Quotations = db.QuotationMain.OrderBy(a => a.QuotationName).ToList();
                TempData["error"] = Ex.Message;
                
                return View(customerMaster);
            }
        }
        // GET: CustomerMasters/Edit/5

        [CustomAuthorize("You dont have access to Edit Customer", "CustomerOrderEdit, Admin")]
        public async Task<ActionResult> Edit(int? id)
        {         

            var CustType = db.CustomerTypes.Take(25).ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesID = db.DepartmentMasters.Where(a => a.Department.Contains("SALES")).Select(a => a.ID).ToList();
            var SalesPerson = db.EmployeeMasters.Where(a => SalesID.Contains(a.DepartmentID)).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
            ViewBag.Quotations = db.QuotationMain.OrderBy(a => a.QuotationName).ToList();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerMaster customerMaster = await db.CustomerMasters.FindAsync(id);
            if (customerMaster == null)
            {
                return HttpNotFound();
            }
            return View(customerMaster);
        }

        // POST: CustomerMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerMaster customerMaster)
        {
            if (ModelState.IsValid)
            {
                customerMaster.UpdatedBy = User.Identity.Name;
                customerMaster.UpdatedDate = DateTime.Now; ;
                db.Entry(customerMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();

                                  ApplicationDbContext context = new ApplicationDbContext();
                var user = (from u in context.Users
                                   where u.UserName == customerMaster.Username
                                   select new
                                   {
                                       Username = u.UserName,
                                       Id = u.Id,     
                                   }).FirstOrDefault();


                if (user == null)
                {

                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var users = new ApplicationUser { UserName = customerMaster.Username, Email = customerMaster.EmailAddress, UserId = customerMaster.ID, FullName = customerMaster.CustName, RegistrationType = "Customer", Address = customerMaster.Address, PhoneNumber = customerMaster.MobileNumber.ToString() };
                    var adminresult = manager.CreateAsync(users, customerMaster.Password);



                    var UserRoleList = db.AspNetUserRoles.Where(a => a.UserId == users.Id && a.RoleId == "5").ToList();
                    db.AspNetUserRoles.RemoveRange(UserRoleList);
                    db.SaveChanges();


                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == users.Id && a.RoleId == "5").FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = users.Id;
                        asprole.RoleId = "5";
                        db.AspNetUserRoles.Add(asprole);
                    }
                }
                else
                {

                    var UserRoleList = db.AspNetUserRoles.Where(a => a.UserId == user.Id && a.RoleId == "5").ToList();
                    db.AspNetUserRoles.RemoveRange(UserRoleList);
                    db.SaveChanges();

                    var IsExistUser = db.AspNetUserRoles.Where(a => a.UserId == user.Id && a.RoleId == "5").FirstOrDefault();
                    if (IsExistUser == null)
                    {
                        var asprole = new AspNetUserRoles();
                        asprole.UserId = user.Id;
                        asprole.RoleId = "5";
                        db.AspNetUserRoles.Add(asprole);
                    }
                }
                db.SaveChanges();
                TempData["success"] = "Record Added Success!";
                return RedirectToAction("Index");
            }
            var CustType = db.CustomerTypes.Take(25).ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesID = db.DepartmentMasters.Where(a => a.Department.Contains("SALES")).Select(a => a.ID).ToList();
            var SalesPerson = db.EmployeeMasters.Where(a => SalesID.Contains(a.DepartmentID)).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
            return View(customerMaster);
        }

        // GET: CustomerMasters/Delete/5

        [CustomAuthorize("You dont have access to Delete Customer", "CustomerOrderEdit, Admin")] 
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerMaster customerMaster = await db.CustomerMasters.FindAsync(id);
            if (customerMaster == null)
            {
                return HttpNotFound();
            }
            return View(customerMaster);
        }

        // POST: CustomerMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            CustomerMaster customerMaster = await db.CustomerMasters.FindAsync(id);
            db.CustomerMasters.Remove(customerMaster);
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
                int totalRecord = 0;

                var result = db.CustomerMasters.Where(a=>a.IsApproved==true && a.CustName=="fusuidfysififyy")
                    .AsEnumerable()
                    .Skip(iDisplayStart)
                    .Select(x => new CustomerMaster { ID=x.ID,CustID = x.CustID, CustName = x.CustName, Username = x.Username, City = x.City, Area = x.Area, ContactPerson = x.ContactPerson, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress }).OrderByDescending(x => x.ID).ToList();

                if (User.IsInRole("Sales"))
                {

                    var CusDtl = db.EmployeeMasters.Where(a => a.Username == User.Identity.Name).Select(a => a.ID).FirstOrDefault();
                    result = db.CustomerMasters.Where(a => a.IsApproved == true && a.SalesPersonID == CusDtl)
               .AsEnumerable()
               .Skip(iDisplayStart)
               .Select(x => new CustomerMaster { ID = x.ID, CustID = x.CustID, CustName = x.CustName, Username = x.Username, City = x.City, Area = x.Area, ContactPerson = x.ContactPerson, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress }).OrderByDescending(x => x.ID).ToList();

                   //  totalRecord = result.Count();

                   
                    if (!string.IsNullOrEmpty(sSearch))
                    {
                        result = db.CustomerMasters.Where(a => a.IsApproved == true && a.SalesPersonID== CusDtl).AsEnumerable()
                            .Where(x => x.CustName.ToLower().Contains(sSearch))
                            .Skip(iDisplayStart)
                            .Select(x => new CustomerMaster { ID = x.ID, CustID = x.CustID, CustName = x.CustName, Username = x.Username, City = x.City, Area = x.Area, ContactPerson = x.ContactPerson, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress }).OrderByDescending(x => x.ID).ToList();
                    }
                    else
                    {
                        result = db.CustomerMasters.Where(a => a.IsApproved == true && a.SalesPersonID == CusDtl).AsEnumerable()
                            .Skip(iDisplayStart)
                            .Select(x => new CustomerMaster { ID = x.ID, CustID = x.CustID, CustName = x.CustName, Username = x.Username, City = x.City, Area = x.Area, ContactPerson = x.ContactPerson, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress }).OrderByDescending(x => x.ID).ToList();
                    }
                }else
                {
                    if (!string.IsNullOrEmpty(sSearch))
                    {
                        result = db.CustomerMasters.Where(a => a.IsApproved == true).AsEnumerable()
                            .Where(x => x.CustName.ToLower().Contains(sSearch))
                            .Skip(iDisplayStart)
                            .Select(x => new CustomerMaster { ID = x.ID, CustID = x.CustID, CustName = x.CustName, Username = x.Username, City = x.City, Area = x.Area, ContactPerson = x.ContactPerson, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress }).OrderByDescending(x => x.ID).ToList();
                    }
                    else
                    {
                        result = db.CustomerMasters.Where(a => a.IsApproved == true).AsEnumerable()
                            .Skip(iDisplayStart)
                            .Select(x => new CustomerMaster { ID = x.ID, CustID = x.CustID, CustName = x.CustName, Username = x.Username, City = x.City, Area = x.Area, ContactPerson = x.ContactPerson, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress }).OrderByDescending(x => x.ID).ToList();
                    }
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
        public JsonResult AddCustomer(CustomerMaster customerMaster)
        {
            try
            {
                db.CustomerMasters.Add(customerMaster);
                db.SaveChanges();

                //var user = new ApplicationUser { UserName = customerMaster.MobileNumber.ToString(), Email = model.Email, UserId = model.UserId, FullName = model.FullName, RegistrationType = model.RegistrationType, Address = model.Address };
                //var result = await UserManager.CreateAsync(user, model.Password);
                //if (result.Succeeded)
                //{
                //    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                //    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                //    // Send an email with this link
                //    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                //    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                //    return RedirectToAction("Index", "Home");
                //}
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult EditCustomer(CustomerMaster customerMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(customerMaster).State = EntityState.Modified;
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
        public JsonResult GetCustomer(int ID)
        {
            try
            {
                var result = db.CustomerMasters.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PincodeAutoComplete(string prefix,string city)
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

        [HttpGet]
        public JsonResult CheckDuplicateCustomerName(string CustName, int? ID,string Type) 
        {
            var Count = 0;
            if(Type=="Create")
            {
                Count = db.CustomerMasters.Where(a => a.CustName == CustName).Count();
            }else
            {
                Count = db.CustomerMasters.Where(a => a.CustName == CustName && a.ID!=ID).Count();
            }
            return Json(Count, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult DuplicteUserName(string Username) 
        {
            var Count = 0;           
                ApplicationDbContext context = new ApplicationDbContext(); 
                Count =context.Users.Where(u=> u.UserName == Username).Count();           
            return Json(Count, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult CheckDuplicateCustomerCode(string Code, int? ID, string Type)  
        {
            var Count = 0;
            if (Type == "Create")
            {
                Count = db.CustomerMasters.Where(a => a.CustID == Code).Count();
            }
            else
            {
                Count = db.CustomerMasters.Where(a => a.CustID == Code && a.ID != ID).Count();
            }
            return Json(Count, JsonRequestBehavior.AllowGet);
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
        [CustomAuthorize("You dont have access to Delete Customer", "CustomerOrderEdit, Admin")]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var customerMasters = db.CustomerMasters.Where(a => a.ID == id).FirstOrDefault();
                db.CustomerMasters.Remove(customerMasters);
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
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
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

        [HttpPost]
        [CustomAuthorize("You dont have access to Delete Customer", "CustomerOrderEdit, Admin")]
        public JsonResult DeleteCustomer(int id)
        {
            CustomerMaster customerMaster = db.CustomerMasters.Find(id);
            var entry = db.CustomerMasters.Remove(customerMaster);
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
        public ActionResult Export()
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DownloadCustomerMaster"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;
                        cmd.CommandTimeout = 1800;
                        sda.SelectCommand = cmd;
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            con.Close();
                            cmd.Dispose();

                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                wb.Worksheets.Add(dt, "SP_MISDownload");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";

                                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
                                Response.ContentType = "application/text";
                                Response.AddHeader("content-disposition", "attachment;filename=CustomerMaster.csv");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                    }
                    RedirectToAction("Index");
                }
            }

            return View();
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
