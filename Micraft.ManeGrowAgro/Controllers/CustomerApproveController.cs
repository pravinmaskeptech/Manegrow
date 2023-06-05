using Micraft.ManeGrowAgro.Models;
using Microsoft.AspNet.Identity.EntityFramework;
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
    public class CustomerApproveController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: CustomerApprove
        public async Task<ActionResult> Index()
        {

            var CustType = db.CustomerTypes.Take(25).ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesPerson = db.EmployeeMasters.Take(25).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }

            return View(await db.CustomerMasters.Where(a=>a.IsApproved==false ||a.IsApproved==null).ToListAsync());
        }


        public ActionResult Create()
        {
            var CustType = db.CustomerTypes.Take(25).ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesPerson = db.EmployeeMasters.Take(25).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerMaster customerMaster)
        {
            if (ModelState.IsValid)
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var username = (from u in context.Users
                                where u.UserName == customerMaster.Username
                                select new
                                {
                                    Username = u.UserName,
                                    Email = u.Email
                                }).First();


                if (username.Username.Length > 0)
                {
                    TempData["error"] = username.Username + " already taken please enter different username!";
                }
                else
                {
                    db.CustomerMasters.Add(customerMaster);
                    await db.SaveChangesAsync();

                    long CustId = db.CustomerMasters.Max(x => x.ID);
                    //ApplicationDbContext context = new ApplicationDbContext();
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var user = new ApplicationUser { UserName = customerMaster.Username, Email = customerMaster.EmailAddress, UserId = CustId, FullName = customerMaster.CustName, RegistrationType = "Customer", Address = customerMaster.Address, PhoneNumber = customerMaster.MobileNumber.ToString() };
                    var adminresult = manager.CreateAsync(user, customerMaster.Password);
                    if (adminresult.Result.Errors.Count() > 0)
                    {
                        TempData["error"] = adminresult.Result.Errors;
                    }
                    //return RedirectToAction("Login", "Account");
                }
                return RedirectToAction("Index");
            }
            var CustType = db.CustomerTypes.Take(25).ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesPerson = db.EmployeeMasters.Take(25).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
            return View(customerMaster);
        }


        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.CustomerMasters.Where(a => a.IsApproved == false || a.IsApproved == null).Count();

                var result = db.CustomerMasters.Where(a => a.IsApproved == false || a.IsApproved == null)
                    .AsEnumerable()
                    .Skip(iDisplayStart).Take(iDisplayLength)
                    .Select(x => new CustomerMaster { ID = x.ID, CustName = x.CustName, City = x.City, State = x.State, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress, IsApproved = x.IsApproved }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.CustomerMasters.Where(a => a.IsApproved == false || a.IsApproved == null).AsEnumerable()
                        .Where(x => x.CustName.ToLower().Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new CustomerMaster { ID = x.ID, CustName = x.CustName, City = x.City, State = x.State, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress, IsApproved = x.IsApproved }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.CustomerMasters.Where(a => a.IsApproved == false || a.IsApproved == null).AsEnumerable()
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new CustomerMaster { ID = x.ID, CustName = x.CustName, City = x.City, State = x.State, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress, IsApproved = x.IsApproved }).OrderByDescending(x => x.ID).ToList();
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


        // GET: CustomerMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

            var CustType = db.CustomerTypes.Take(25).ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesPerson = db.EmployeeMasters.Take(25).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerMaster customerMaster)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerMaster).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            var CustType = db.CustomerTypes.Take(25).ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }
            var SalesPerson = db.EmployeeMasters.Take(25).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
            return View(customerMaster);
        }
    }

}