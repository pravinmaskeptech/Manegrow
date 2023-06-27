using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Micraft.ManeGrowAgro.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class HomeController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
      //  [InitializeSimpleMembership]
        public ActionResult Index()
        {
            Session["FromDate"] = "";
            Session["ToDate"] = "";
            Session["Type"] = "";
            Session["City"] = "";
            Session["ShortName"] = "";

            var CustType = db.CustomerTypes.ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }


            ViewBag.City = db.CustomerMasters.Select(x => new { x.City, x.ID }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
            ViewBag.Shortname = db.CustomerMasters.Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();


            if (User.IsInRole("Vendor"))
            {
                ViewBag.PackingWT = 0;
                ViewBag.WeekData = 0;
                ViewBag.Todays = 0;
                ViewBag.ActualData = 0;
                ViewBag.TodaysVsActualDiff = 0;
                ViewBag.NextDay = 0;
                ViewBag.PackingWT = 0;
                ViewBag.Dispatch = 0;
            }
            else
            {
                ViewBag.PackingWT = 0;
                ViewBag.WeekData = 0;
                ViewBag.Todays = 0;
                ViewBag.ActualData = 0;
                ViewBag.TodaysVsActualDiff = 0;
                ViewBag.NextDay = 0;
                ViewBag.PackingWT = 0;
                ViewBag.Dispatch = 0;
                //using (SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
                //{
                //    using (SqlCommand cmd = new SqlCommand("SP_DashbordDetails", myConnection))
                //    {
                //        cmd.CommandType = CommandType.StoredProcedure;
                //        myConnection.Open();

                //        //Declare @tblResult table(MonthData,WeekData,Todays,ActualData,TodaysVsActualDiff,TotOrderQty,OrderWeight,ActualDispatchQty,OrderDiff)
                //        using (SqlDataReader dr = cmd.ExecuteReader())
                //        {
                //            if (dr.Read())
                //            {
                //                ViewBag.MonthData = dr["MonthData"].ToString();
                //                ViewBag.WeekData = dr["WeekData"].ToString();
                //                ViewBag.Todays = dr["Todays"].ToString();
                //                ViewBag.ActualData = dr["ActualData"].ToString();
                //                ViewBag.TodaysVsActualDiff = dr["TodaysVsActualDiff"].ToString();
                //                var TotOrderQty = dr["TotOrderQty"].ToString();
                //                ViewBag.NextDay = dr["NextDay"].ToString();
                //                ViewBag.PackingWT = dr["PackingWT"].ToString();

                //                ViewBag.Dispatch = dr["Dispatch"].ToString();



                //                if (TotOrderQty == "")
                //                {
                //                    ViewBag.TotOrderQty = 0;
                //                }
                //                else
                //                {
                //                    ViewBag.TotOrderQty = TotOrderQty;
                //                }

                //                ViewBag.OrderWeight = dr["OrderWeight"].ToString();
                //                var ActualDispatchQty = dr["ActualDispatchQty"].ToString();
                //                if (ActualDispatchQty == "")
                //                {
                //                    ViewBag.ActualDispatchQty = 0;
                //                }
                //                else
                //                {
                //                    ViewBag.ActualDispatchQty = ActualDispatchQty;
                //                }
                //                var OrderDiff = dr["OrderDiff"].ToString();
                //                if (OrderDiff == "")
                //                {
                //                    ViewBag.OrderDiff = 0;
                //                }
                //                else
                //                {
                //                    ViewBag.OrderDiff = OrderDiff;
                //                }
                //            }

                //        }
                //    }
                //}
            }
            //ViewBag.Customer = db.ConsignorDetails.Select(x => new { x.ConsignorID, x.ConsignorName }).ToList();
            if (Session["AuthorizeMsg"] != null)
            {
                try
                {
                    TempData["Message"] = Session["AuthorizeMsg"].ToString();
                    Session["AuthorizeMsg"] = null;
                }
                catch (Exception EX)
                {
                    ViewBag.Message = null;
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult SetDates(string FROMDATE, string TODATE, string TYPE, string CITY, string SHORTNAME)
        {

            Session["FromDate"] = FROMDATE.ToString();
            Session["ToDate"] = TODATE.ToString();
            Session["Type"] = TYPE;
            Session["City"] = CITY;
            Session["ShortName"] = SHORTNAME;

            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {

                var frmdate = Session["FromDate"];
                var todate = Session["ToDate"];

                var type = "";
                if (Session["Type"] != "")
                {
                    try{  type = Session["Type"].ToString();  }catch { }
                }

                var city = "";
                if (Session["City"] != "")
                {
                    try{ city = Session["City"].ToString(); } catch { }
                }

                var shrtname = "";
                if (Session["ShortName"] != "")
                {
                    try { shrtname = Session["ShortName"].ToString(); }catch { }
                }

                //   var type = Session["Type"].ToString() ?? "";
                //    var city = Session["City"].ToString() ?? "";
                //     var shrtname = Session["ShortName"].ToString() ?? "";

                DateTime? Fromdate = null;
                DateTime? Todate = null;


                if (todate != "")
                {
                    Todate = Convert.ToDateTime(todate);
                }
                if (todate != "")
                {
                    Fromdate = Convert.ToDateTime(frmdate);
                }



                int totalRecord = 0;
                //var result1 = (from dr in db.MainOrder

                //               join Customer in db.CustomerMasters on dr.CustomerCode equals Customer.CustID into Customerdata
                //               from dp in Customerdata.DefaultIfEmpty()


                //               join CustomerTypes in db.CustomerTypes on dp.CustTypeID equals CustomerTypes.ID into CustomerTypesdata
                //               from ds in CustomerTypesdata.DefaultIfEmpty()


                //               join Order in db.Order on dr.Id equals Order.MasterOrderId into Orderdata
                //               from ee in Orderdata.DefaultIfEmpty()



                //               join emp in db.EmployeeMasters on dr.CreatedBy equals emp.Username into empdata
                //               from a in empdata.DefaultIfEmpty()
                //               select new
                //               {
                //                   ID = ee.Id,
                //                   UOM = ee == null ? string.Empty : ee.UOM,
                //                   Qty = ee == null ? 0 : ee.Qty,
                //                   ProductName = ee == null ? string.Empty : ee.ProductName,
                //                   PackagingRemark = ee == null ? string.Empty : ee.PackagingRemark,
                //                   IsPacked = ee == null ? string.Empty : ee.OrderStatus,
                //                   OrderStatus = ee == null ? string.Empty : ee.OrderStatus,
                //                   OrderStage = ee == null ? 0 : ee.OrderStage,
                //                   Weight = ee == null ? 0 : ee.Weight,
                //                   Area = dp == null ? string.Empty : dp.Area,
                //                   ContactPerson = dp == null ? string.Empty : dp.ContactPerson,
                //                   MobileNumber = dp == null ? string.Empty : dp.MobileNumber,
                //                   CustomerTypes = ds == null ? string.Empty : ds.Type,
                //                   Remark = dr.Remark,

                //                   ExpectedDeliveryTime = dr.ExpectedDeliveryTime,
                //                   ExpectedDeliveryDate = dr.ExpectedDeliveryDate,
                //                   ShortName = dp == null ? string.Empty : dp.ShortName,
                //                   City = dp == null ? string.Empty : dp.City,
                //                   Date = dr.Date,
                //                   CustName = dp == null ? string.Empty : dp.CustName,
                //                   CreatedBy = a == null ? string.Empty : a.Name,
                //                   EmpMobile = a == null ? string.Empty : a.MobileNumber,



                //               }).OrderByDescending(x => x.Date).ToList();
                var result1 = (from dr in db.MainOrder
                               join dp in db.CustomerMasters
                               on dr.CustomerId equals dp.ID

                               join ds in db.CustomerTypes
                               on dp.CustTypeID equals ds.ID

                               join e in db.Order
                               on dr.Id equals e.MasterOrderId

                               select new
                               {
                                   ID = dr.Id,
                                   UOM = e.UOM,
                                   Qty = e.Qty,
                                   Area = dp.Area,
                                   ContactPerson = dp.ContactPerson,
                                   MobileNumber = dp.MobileNumber,
                                   CustomerTypes = ds.Type,
                                   Remark = dr.Remark,
                                   Weight = e.Weight,
                                   ExpectedDeliveryTime = dr.ExpectedDeliveryTime,
                                   ExpectedDeliveryDate = dr.ExpectedDeliveryDate,
                                   City = dp.City,
                                   ShortName = dp.ShortName,
                                   Date = dr.Date,
                                   CustName = dp.CustName,
                                   ProductName = e.ProductName
                               }).OrderByDescending(x => x.Date).ToList();
                if (!User.IsInRole("Vendor"))
                {
                    if (Fromdate != null && Todate != null)
                    {
                        result1 = result1.Where(a => (a.Date >= Fromdate && a.Date <= Todate)).ToList();
                    }
                    else
                    {
                        var fdt = DateTime.Today;
                        // var fdt = Tdt.AddDays(-1);
                        result1 = result1.Where(a => (a.Date == fdt)).ToList();
                    }
                    if (type != "")
                    {
                        result1 = result1.Where(a => a.CustomerTypes.ToLower() == type.ToLower()).ToList();
                    }
                    if (city != "")
                    {
                        result1 = result1.Where(a => a.City.ToLower() == city.ToLower()).ToList();
                    }
                    if (shrtname != "")
                    {
                        result1 = result1.Where(a => a.ShortName.ToLower() == shrtname.ToLower()).ToList();
                    }
                }else
                {
                    result1 = result1.Where(a => a.CustName == "A011-ARIANT VEG PRAIVAITE LIMITED").ToList();
                }

                var result = result1;

                Session["FromDate"] = "";
                Session["ToDate"] = "";
                Session["Type"] = "";
                Session["City"] = "";
                Session["ShortName"] = "";

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





        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Register()
        {
            ViewBag.City = db.Cities.Take(25).ToList();
            ViewBag.State = db.States.Take(25).ToList();
            ViewBag.PinCode = db.PincodeMasters.Take(25).ToList();
            ViewBag.Region = db.RegionMasters.Take(25).ToList();
            ViewBag.CustType = db.CustomerTypes.ToList();
            ViewBag.SalesPerson = db.EmployeeMasters.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AddCustomer(CustomerMaster customerMaster)
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


                if (username !=null) 
                {
                    TempData["error"] = username.Username + " already taken please enter different username!";
                }
                else
                {
                    db.CustomerMasters.Add(customerMaster);
                    db.SaveChanges();
                    long CustId = db.CustomerMasters.Max(x => x.ID);
                    //ApplicationDbContext context = new ApplicationDbContext();
                    var store = new UserStore<ApplicationUser>(context);
                    var manager = new ApplicationUserManager(store);
                    var user = new ApplicationUser { UserName = customerMaster.Username, Email = customerMaster.EmailAddress, UserId = CustId, FullName = customerMaster.CustName, RegistrationType = "Customer", Address = customerMaster.Address, PhoneNumber = customerMaster.MobileNumber.ToString() };
                    var adminresult = manager.CreateAsync(user, customerMaster.Password);

                    //return RedirectToAction("Login", "Account");
                    TempData["userid"] = CustId;
                    ViewBag.CustType = db.CustomerTypes.ToList();
                    ViewBag.SalesPerson = db.EmployeeMasters.ToList();
                }
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // return RedirectToAction("Register", "Home");
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
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
            catch(Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }
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

        public FileResult DownloadFile(string fileName)
        {
            fileName = "Manegrow Agro New_1_1.0.apk";         
            string path = Server.MapPath("~/AndroidApp/"+fileName);
            byte[] bytes = System.IO.File.ReadAllBytes(path);
            return File(bytes, "application/octet-stream", fileName);
        }
    }
}