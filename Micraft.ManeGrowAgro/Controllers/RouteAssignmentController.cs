using iTextSharp.text;
using iTextSharp.text.pdf;
using Micraft.ManeGrowAgro.Models;
using Micraft.ManeGrowAgro.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class RouteAssignmentController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();

        // GET: RouteAssignment
        [CustomAuthorize("You dont have access to View Dispatch", "DispatchEdit,DispatchView, Admin")]
        public ActionResult Index()
        {
            ViewBag.Expence = db.ExpenceTypes.OrderBy(a => a.ExpenceType).ToList();


            try
            {
                string consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(consString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateAmount"))
                    {
                        con.Open();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = con;

                        cmd.CommandTimeout = 1800;
                        cmd.ExecuteNonQuery();
                        con.Close();
                        cmd.Dispose();
                    }
                }
            }

            catch (Exception ex)
            {

            }



            return View();
        }

        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                var dataa = (from x in db.dispatchDetails
                             join c in db.RouteMains
                             on x.Route equals c.ID

                             select new
                             {
                                 x.DispatchDate,
                                 x.DispatchID,
                                 x.DriverName,
                                 x.VehicalNo,
                                 x.NoofBoxes,
                                 x.TotalWeight,
                                 Route = c.MainRouteName,
                                 x.VendorName,


                             }).OrderByDescending(x => x.DispatchID).ToList();





                var totalRecord = dataa.Count();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    dataa = dataa.AsEnumerable()
                        .Where(x => x.DriverName.ToLower().Contains(sSearch) || x.VehicalNo.ToLower().Contains(sSearch) || x.DispatchID.ToString().ToLower().Contains(sSearch) || x.VendorName.ToLower().Contains(sSearch) || x.Route.ToLower().Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new
                        {
                            x.DispatchDate,
                            x.DispatchID,
                            x.DriverName,
                            x.VehicalNo,
                            x.NoofBoxes,
                            x.TotalWeight,
                            x.Route,
                          ///  Route = x.MainRouteName,
                            x.VendorName
                        }).OrderByDescending(x => x.DispatchID).ToList();
                   // totalRecord = dataa.Count();
                }
                else
                {
                    dataa = dataa.AsEnumerable()
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new {
                            x.DispatchDate,
                            x.DispatchID,
                            x.DriverName,
                            x.VehicalNo,
                            x.NoofBoxes,
                            x.TotalWeight,
                            ///  Route = x.MainRouteName,
                              x.Route,
                            x.VendorName
                        }).OrderByDescending(x => x.DispatchID).ToList();
               //     totalRecord = dataa.Count();
                }








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
                sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(dataa));
                sb.Append("}");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [CustomAuthorize("You dont have access to Create Dispatch", "DispatchEdit, Admin")]

        public ActionResult Create()
        {
            ViewBag.Root = db.RouteMains.OrderBy(a => a.MainRouteName).ToList();
            ViewBag.DriverName = db.DriverMaster.Where(a => a.DriverName == "sjdfsfg").OrderBy(a => a.DriverName).ToList();
            ViewBag.VendorName = db.VendorMasters.Where(a => a.UserName == "sdjgfsfgh").OrderBy(a => a.VendorName).ToList();
            ViewBag.SubRoute = db.RouteMains.Where(a => a.MainRouteName == "dhgfsj").OrderBy(a => a.MainRouteName).ToList();
            ViewBag.VendorType = db.VendorTypes.OrderBy(a => a.VendorType).ToList();
            return View();
        }

        [CustomAuthorize("You dont have access to Edit Dispatch", "DispatchEdit, Admin")]
        public ActionResult Edit(int? id)
        {
            var disp = db.dispatchDetails.Where(a => a.DispatchID == id).FirstOrDefault();
            ViewBag.Root = db.RouteMains.OrderBy(a => a.MainRouteName).ToList();
            ViewBag.DriverName = db.DriverMaster.Where(a => a.VendorID == disp.VendorID).OrderBy(a => a.DriverName).ToList();

            ViewBag.SubRoute = db.RouteMains.Where(a => a.ID == disp.Route).OrderBy(a => a.MainRouteName).ToList();
            ViewBag.VendorType = db.VendorTypes.OrderBy(a => a.VendorType).ToList();

            ViewBag.VendorName = db.VendorMasters.Where(a => a.VendorType == disp.VendorType).OrderBy(a => a.VendorName).ToList();


            return View(disp);
        }

        public JsonResult GetDriverDetails(int ID)
        {
            //  var vendorid = db.VendorMasters.Where(a => a.VendorName == VendorName).Select(a => a.ID).SingleOrDefault();
            var Driverdetails = db.DriverMaster.Where(a => a.VendorID == ID).Select(a => a.DriverName).ToList();
            var result = new { Message = "success", Driverdetails };
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetVendors(string VendorType)
        {
            var Vendordetails = db.VendorMasters.Where(a => a.VendorType == VendorType).OrderBy(a => a.VendorName).ToList();
            //  var Vendordetails = db.DriverMaster.Where(a => a.VendorID == vendorid).Select(a => a.DriverName).ToList();
            var result = new { Message = "success", Vendordetails };
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEditData(int ID)
        {
            var Dispatch = db.dispatchDetails.Where(a => a.DispatchID == ID).FirstOrDefault();
            var result = new { Message = "success", Dispatch };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetCustomers(int Root,int SubRoute,string Date) 
        //{
        //    var dt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture); 
        //    var result1 = (from e in db.Order.Where(a => (a.OrderStatus == "Packing Completed" || a.OrderStatus == "Quantity Changed & Packing Completed" || a.OrderStatus == "Merge & Packing Completed") && a.Date == dt)

        //                   join sply in db.MainOrder on e.MasterOrderId equals sply.Id into MainOrder
        //                   from dr in MainOrder.DefaultIfEmpty()

        //                   join Cu in db.CustomerMasters on dr.CustomerId equals Cu.ID into CustomerMasters
        //                   from dp in CustomerMasters.DefaultIfEmpty()

        //                   join sbc in db.SubRouteCustomers on dr.CustomerId equals sbc.CustomerID into SubRouteCustomers
        //                   from src in SubRouteCustomers.Where(a => a.SubRouteID == SubRoute).DefaultIfEmpty()

        //                   join ssrd in db.SubRouteDetails on src.SubRouteID equals ssrd.ID into SubRouteDetails
        //                   from srd in SubRouteDetails.DefaultIfEmpty()

        //                   join pk in db.PackingDetails on e.Id equals pk.OrderNo into PackingDetails
        //                   from pd in PackingDetails.DefaultIfEmpty()
        //                   select new
        //                   {

        //                       ID = e.Id,
        //                       Area = dp.Area,
        //                       City = dp.City,
        //                       SubRouteID = src == null ? 0 : src.SubRouteID,
        //                       CustName = dp == null ? String.Empty : dp.CustName,
        //                       RootName = srd == null ? String.Empty : srd.SubRouteName,
        //                       RouteMainID = srd == null ? 0 : srd.RouteMainID,
        //                       RouteMainName = srd == null ? string.Empty : srd.RouteMainName,

        //                       CustID = dp == null ? 0 : dp.ID,
        //                       NoOfBox = pd == null ? 0 : pd.NoOfBox,
        //                       TotalWeight = pd == null ? 0 : pd.TotalWeight,


        //                   } into p
        //                   group p by new { p.NoOfBox, p.CustID, p.CustName, p.City, p.Area, p.ID, p.SubRouteID, p.RootName, p.RouteMainID, p.RouteMainName,p.TotalWeight } into gcs 
        //                   select new
        //                   {

        //                       SubRouteID = gcs.Key.SubRouteID,
        //                       ID = gcs.Key.ID,
        //                       Area = gcs.Key.Area,
        //                       City = gcs.Key.City,
        //                       CustName = gcs.Key.CustName,
        //                       RootName = gcs.Key.RootName,
        //                       RouteMainID = gcs.Key.RouteMainID,
        //                       RouteMainName = gcs.Key.RouteMainName,
        //                       CustID = gcs.Key.CustID,
        //                       NoOfBox = gcs.Sum(x => x.NoOfBox),
        //                       TotalWeight = gcs.Sum(x => x.TotalWeight), 

        //                   }).ToList();


        //    var abcc = 0; 

        //    //var result1 = (from e in db.Order.Where(a => (a.OrderStatus == "Packing Completed" || a.OrderStatus == "Quantity Changed & Packing Completed" || a.OrderStatus == "Merge & Packing Completed") && a.Date == DateTime.Today)

        //    //               join dr in db.MainOrder
        //    //               on e.MasterOrderId equals dr.Id

        //    //               join dp in db.CustomerMasters
        //    //               on dr.CustomerId equals dp.ID

        //    //               join src in db.SubRouteCustomers
        //    //              on dr.CustomerId equals src.CustomerID

        //    //               join srd in db.SubRouteDetails
        //    //             on src.SubRouteID equals srd.ID


        //    //               //join pd in db.PackingDetails
        //    //               // on e.Id equals pd.OrderNo
        //    //               //group pd by pd.NoOfBox

        //    //               select new
        //    //               {
        //    //                   //    SubRouteName = srd.SubRouteName,
        //    //                   SubRouteID = srd.ID,
        //    //                   ID = e.Id,
        //    //                   Area = dp.Area,
        //    //                   City = dp.City,
        //    //                   CustName = dp.CustName,
        //    //                   //   RootName = dp.RootName,
        //    //                   RootName = srd.SubRouteName,
        //    //                   RouteMainID = srd.RouteMainID,
        //    //                   RouteMainName = srd.RouteMainName,
        //    //                   CustID = dp.ID,
        //    //               }).OrderBy(x => x.CustName).ToList();


        //    var WithOrder = result1.Where(a => a.SubRouteID == SubRoute && a.RouteMainID==Root).ToList();
        //    var WithoutOrder = result1.Where(a => a.SubRouteID != SubRoute && a.RouteMainID != Root).ToList();   




        //    //var orders = db.Order.Where(a=>(a.OrderStatus== "Packing Completed" || a.OrderStatus== "Quantity Changed & Packing Completed") && a.Date==DateTime.Today).Select( a=>a.MasterOrderId ).Distinct().ToList();  
        //    //var mainorderCustomerID = db.MainOrder.Where(a => orders.Contains(a.Id)).Select(a => a.CustomerId ).ToList(); 
        //    //var Customers = db.CustomerMasters.ToList();
        //    //var WithOrder = Customers.Where(a =>a.IsApproved==true && a.RootName== Root && mainorderCustomerID.Contains(a.ID)).ToList();
        //    //var tempCust = WithOrder.Select(a => a.ID).ToList();

        //    //var WithoutOrder  = Customers.Where(a => a.IsApproved == true && mainorderCustomerID.Contains(a.ID) && !tempCust.Contains(a.ID)).ToList();

        //    var result = new { WithOrder, WithoutOrder };
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult GetCustomers(int Root, string SubRoute, string Date)
        {
            try
            {
                var rt = Root;
                var CustID = db.SubRouteCustomers.Where(a => a.RouteMainID == Root && a.CustomerID != null).Select(a => a.CustomerID).Distinct().ToList();
                var dt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var result1 = (from e in db.Order.Where(a => (a.OrderStatus == "Packing Completed" || a.OrderStatus == "Quantity Changed & Packing Completed" || a.OrderStatus == "Merge & Packing Completed") && a.Date == dt)

                               join sply in db.MainOrder on e.MasterOrderId equals sply.Id into MainOrder
                               from dr in MainOrder.DefaultIfEmpty()

                               join Cu in db.CustomerMasters on dr.CustomerId equals Cu.ID into CustomerMasters
                               from dp in CustomerMasters.DefaultIfEmpty()

                               join pk in db.PackingDetails on e.Id equals pk.OrderNo into PackingDetails
                               from pd in PackingDetails.DefaultIfEmpty()
                               select new
                               {

                                   ID = e.Id,
                                   Area = dp.Area,
                                   City = dp.City,
                                   //   SubRouteID = src == null ? 0 : src.RouteDetailsID,
                                   CustName = dp == null ? String.Empty : dp.CustName,
                                   //    RootName = srd == null ? String.Empty : srd.SubRouteName,
                                   //   RouteMainID = src == null ? 0 : src.RouteMainID,
                                   //  RouteMainName = srd == null ? string.Empty : srd.RouteMainName,

                                   CustID = dp == null ? 0 : dp.ID,
                                   NoOfBox = pd == null ? 0 : pd.NoOfBox,
                                   TotalWeight = pd == null ? 0 : pd.TotalWeight,


                               } into p
                               group p by new { p.CustID, p.CustName, p.ID } into gcs
                               select new
                               {


                                   ID = gcs.Key.ID,
                                   Area = gcs.Select(a => a.Area).Distinct(),
                                   City = gcs.Select(a => a.City).Distinct(),
                                   CustName = gcs.Key.CustName,


                                   CustID = gcs.Key.CustID,
                                   NoOfBox = gcs.Sum(x => x.NoOfBox),
                                   TotalWeight = gcs.Sum(x => x.TotalWeight),

                               }).ToList();


                var WithOrder = result1.Where(a => CustID.Contains(a.CustID)).ToList();
                var WithoutOrder = result1.Where(a => !CustID.Contains(a.CustID)).ToList();

                var result = new { WithOrder, WithoutOrder };
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                var result = new { };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //var orders = db.Order.Where(a=>(a.OrderStatus== "Packing Completed" || a.OrderStatus== "Quantity Changed & Packing Completed") && a.Date==DateTime.Today).Select( a=>a.MasterOrderId ).Distinct().ToList();  
            //var mainorderCustomerID = db.MainOrder.Where(a => orders.Contains(a.Id)).Select(a => a.CustomerId ).ToList(); 
            //var Customers = db.CustomerMasters.ToList();
            //var WithOrder = Customers.Where(a =>a.IsApproved==true && a.RootName== Root && mainorderCustomerID.Contains(a.ID)).ToList();
            //var tempCust = WithOrder.Select(a => a.ID).ToList();

            //var WithoutOrder  = Customers.Where(a => a.IsApproved == true && mainorderCustomerID.Contains(a.ID) && !tempCust.Contains(a.ID)).ToList();

        }
        public JsonResult GetEditCustomers(string Root, string Date, int ID)
        {
            var expdt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var orders = db.Order.Where(a => a.DispatchID == ID).Select(a => a.MasterOrderId).Distinct().ToList();
            var mainorderCustomerID = db.MainOrder.Where(a => orders.Contains(a.Id)).Select(a => a.CustomerId).ToList();
            var Customers = db.CustomerMasters.ToList();


            var WithOrder = Customers.Where(a => a.IsApproved == true && a.RootName == Root && mainorderCustomerID.Contains(a.ID)).ToList();
            var tempCust = WithOrder.Select(a => a.ID).ToList();

            var WithoutOrder = Customers.Where(a => a.IsApproved == true && mainorderCustomerID.Contains(a.ID) && !tempCust.Contains(a.ID)).ToList();

            var result = new { WithOrder, WithoutOrder };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCalculatedWeight(List<MainOrder> Cust)
        {
            var TotalBoxes = 0;
            decimal TotalWeight = 0;
            var dataa = Cust.Select(x => new { x.CustomerId }).GroupBy(x => x.CustomerId).Select(x => x.FirstOrDefault()).ToList();
            foreach (var x in dataa)
            {
                var order = db.MainOrder.Where(a => a.CustomerId == x.CustomerId && a.Date == DateTime.Today).ToList();
                foreach (var xx in order)
                {
                    var Orderid = db.Order.Where(a => a.MasterOrderId == xx.Id && (a.OrderStatus == "Packing Completed" || a.OrderStatus == "Quantity Changed & Packing Completed" || a.OrderStatus == "Merge & Packing Completed")).Select(a => a.Id).ToList();
                    if (Orderid.Count != 0)
                    {

                        var xxx = (from p in db.PackingDetails.Where(a => Orderid.Contains(a.OrderNo))
                                   group p by 1 into g
                                   select new
                                   {
                                       NoOfBox = g.Sum(c => c.NoOfBox),
                                       TotalWeight = g.Sum(c => c.TotalWeight)
                                   }).FirstOrDefault();
                        TotalBoxes = TotalBoxes + xxx.NoOfBox;
                        TotalWeight = Convert.ToDecimal(TotalWeight) + Convert.ToDecimal(xxx.TotalWeight);
                    }

                }
            }

            var result = new { Message = "success", TotalBoxes, TotalWeight };
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetSubRoutes(int RouteMainID)
        {

            var SubRoute = db.SubRouteDetails.Where(a => a.RouteMainID == RouteMainID).Select(a => a.SubRouteName).Distinct().ToList();
            var result = new { Message = "success", SubRoute };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFile(int ID)
        {
            var expencedtl = db.ExpenceDetails.Where(a => a.ID == ID).ToList();
            var filename = expencedtl[0].ImageName;
            db.ExpenceDetails.RemoveRange(expencedtl);
            db.SaveChanges();
            var PAth = Path.Combine(Server.MapPath("~/Attachments/ExpenceBill/"), filename);
            if ((System.IO.File.Exists(PAth)))
            {
                System.IO.File.Delete(PAth);
            }

            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveExpence(List<ExpenceDetails> Order, int FromKM, int ToKM, int TotalKM, decimal TotalAmount, int DispatchID, int ExpMainID)
        {
            try
            {
                try
                {


                    var mfst = db.dispatchDetails.Where(a => a.DispatchID == DispatchID).FirstOrDefault();

                    var manifest = db.DispatchTransactions.Where(a => a.DispatchID == DispatchID && a.VendorID == mfst.VendorID).FirstOrDefault();
                    if (manifest != null)
                    {
                        var rate = manifest.Rate;
                        manifest.Amount = Convert.ToDecimal(manifest.Rate) * Convert.ToDecimal(TotalKM);
                        manifest.TotalKM = TotalKM;
                        manifest.UpdatedBy = "ExpenceDetails - " + User.Identity.Name;

                    }
                }
                catch { }

                var maxid = 0;




                //using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                //{
                if (ExpMainID == 0)
                {
                    VendorExpence em = new VendorExpence();
                    em.FromKM = FromKM;
                    em.ToKM = ToKM;
                    em.TotalKM = TotalKM;
                    em.TotalAmount = TotalAmount;
                    em.CreatedBy = User.Identity.Name;
                    em.CreatedDate = DateTime.Today;
                    em.DispatchID = DispatchID;
                    db.VendorExpence.Add(em);
                    db.SaveChanges();
                    maxid = db.VendorExpence.Select(a => a.ID).Max();
                }
                else
                {
                    var em = db.VendorExpence.Where(a => a.ID == ExpMainID).FirstOrDefault();
                    em.FromKM = FromKM;
                    em.ToKM = ToKM;
                    em.TotalKM = TotalKM;
                    em.TotalAmount = TotalAmount;
                    em.UpdatedBy = User.Identity.Name;
                    em.UpdatedDate = DateTime.Today;
                    em.DispatchID = DispatchID;

                }


                ExpenceDetails ed = new ExpenceDetails();
                foreach (var expense in Order)
                {
                    var dispdt = Convert.ToDateTime(expense.strDate);
                    if (expense.ID == 0)
                    {
                        expense.ReceiptDate = dispdt;
                        expense.ExpMainID = maxid;
                        expense.CashMemo = expense.CashMemo;
                        expense.ReceiptNo = expense.ReceiptNo;
                        expense.ExpType = expense.ExpType;
                        expense.Amount = expense.Amount;
                        expense.ImageName = expense.ImageName;
                        expense.DispatchID = DispatchID;
                        expense.ExpMainID = expense.ExpMainID;
                        db.ExpenceDetails.Add(expense);
                    }
                    else
                    {
                        var expdtl = db.ExpenceDetails.Where(a => a.ID == expense.ID).FirstOrDefault();
                        expdtl.ExpType = expense.ExpType;
                        expdtl.Amount = expense.Amount;
                        expdtl.ImageName = expense.ImageName;
                        expdtl.DispatchID = DispatchID;
                        ed.ExpMainID = expense.ExpMainID;
                        ed.ReceiptDate = dispdt;
                        ed.CashMemo = expense.CashMemo;
                        ed.ReceiptNo = expense.ReceiptNo;
                    }
                }
                db.SaveChanges();
                //  dbTran.Commit();
                var result = new { Message = "success", };
                return Json(result, JsonRequestBehavior.AllowGet);

                //  }
            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public virtual ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Attachments/ExpenceBill/"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }
        public JsonResult GetExpencePopupData(int ID)
        {
            var VehicleType = db.dispatchDetails.Where(a => a.DispatchID == ID).Select(a => a.VendorType).SingleOrDefault();
            if (VehicleType != "OWN")
            {
                var results = new { Message = "This Facility Only For OWN Vehicles..." };
                return Json(results, JsonRequestBehavior.AllowGet);
            }

            var order = db.VendorExpence.Where(a => a.DispatchID == ID).ToList();
            var dtl = db.ExpenceDetails.Where(a => a.DispatchID == ID).ToList();

            var result = new { Message = "success", order, dtl };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save(List<MainOrder> Cust, string Date, int Route, string DriverName, string VendorType, string VendorName, string VehicalNo, int TotalBoxes, decimal TotalWT, int? ID, int? VendorID,string RouteName)
        {
            try
            {
                int DispatchID = 0;
                var transaction = new List<DispatchTransactions>();
                var expdt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                {
                    if (ID != null)
                    {
                        //var dispatchdtl = db.dispatchDetails.Where(a => a.DispatchID == ID).FirstOrDefault();
                        //dispatchdtl.TotalWeight = TotalWT;
                        //dispatchdtl.NoofBoxes = TotalBoxes;
                        //dispatchdtl.DriverName = DriverName;
                        //dispatchdtl.VehicalNo = VehicalNo;
                        //dispatchdtl.VendorName = VendorName;
                        //dispatchdtl.Route = Route;
                        //dispatchdtl.VendorType = VendorType;
                        //dispatchdtl.CreatedBy = User.Identity.Name;
                        //dispatchdtl.CreatedDate = DateTime.Today;
                        //dispatchdtl.UpdatedBy = User.Identity.Name;
                        //dispatchdtl.UpdatedDate = DateTime.Today; 
                        //DispatchID =Convert.ToInt32( ID);

                        //foreach (var x in Cust)
                        //{          
                        //            var orderdtl = db.Order.Where(a => a.Id == x.OrderNo).FirstOrDefault();
                        //            orderdtl.OrderStatus = "Order Dispatched";
                        //            orderdtl.OrderStage = 8;
                        //            orderdtl.DispatchID = null;

                        //}
                        db.SaveChanges();
                    }
                    else
                    {

                        var dispatchdtl = new DispatchDetails();
                        dispatchdtl.TotalWeight = TotalWT;
                        dispatchdtl.NoofBoxes = TotalBoxes;
                        dispatchdtl.DispatchDate = expdt;
                        dispatchdtl.DriverName = DriverName;
                        dispatchdtl.VehicalNo = VehicalNo;
                        dispatchdtl.VendorName = VendorName;
                        dispatchdtl.VendorID = VendorID;
                        dispatchdtl.Route = Route;
                        dispatchdtl.VendorType = VendorType;
                        dispatchdtl.CreatedBy = User.Identity.Name;
                        dispatchdtl.CreatedDate = DateTime.Today;
                        db.dispatchDetails.Add(dispatchdtl);
                        db.SaveChanges();
                        DispatchID = db.dispatchDetails.Max(u => u.DispatchID);
                    }

                    var CustDetails = Cust.Select(a => a.CustomerId).Distinct().ToList();



                    var GroupManifestss = db.ManifestGroups.Where(a => a.RouteID == Route).ToList();

                    foreach (var x in CustDetails)
                    {



                        var CustDetailss = Cust.Where(a => a.CustomerId == x)
.GroupBy(_ => _.CustomerId, _ => new { TotalWT = _.TotalWT, TotalBoxes = _.TotalBoxes, CustomerId = _.CustomerId })
.Select(g => new
{
    CustomerId = g.Select(a => a.CustomerId),
    TotalWT = g.Sum(f => f.TotalWT).ToString(),
    TotalBoxes = g.Sum(f => f.TotalBoxes).ToString(),

}).FirstOrDefault();
                        var custID = x;
                        var Custdata = db.SubRouteCustomers.Where(a => a.CustomerID == custID).Select(a => a.RouteDetailsID).Distinct().ToList();
                        if (Custdata.Count == 0)
                        {
                            var cst = db.CustomerMasters.Where(a => a.ID == custID).FirstOrDefault();
                            var resultss = new { Message = "Route Not Assigned For " + cst.CustName };
                            return Json(resultss, JsonRequestBehavior.AllowGet);
                        }

                        var caunt = GroupManifestss.Where(a => a.CustomerID == x).Count();
                        if (caunt == 0)
                        {
                            var cst = db.CustomerMasters.Where(a => a.ID == custID).FirstOrDefault();
                            var resultsss = new { Message = "Group Not Assigned For " + cst.CustName };
                            return Json(resultsss, JsonRequestBehavior.AllowGet);
                        }






                        var TotalWTT = CustDetailss.TotalWT;
                        var TotalBoxess = CustDetailss.TotalBoxes;


                    //    List<DispatchTransactions> dtc = new List<DispatchTransactions>();
                        foreach (var dtlid in Custdata)
                        {
                            var rdd = db.RouteDetails.Where(a => a.ID == dtlid).FirstOrDefault();
                            var transCount = transaction.Where(a => a.VendorID == rdd.VendorID && a.Date == expdt).Count();
                            if (transCount == 0)
                            {
                                var trans = new DispatchTransactions();
                                trans.DispatchID = Convert.ToInt32(DispatchID);
                                trans.LocationFrom = rdd.FromRoute;
                                trans.LocationTo = rdd.ToRoute;
                                trans.VendorID = Convert.ToInt32(rdd.VendorID);
                                trans.VendorName = rdd.VendorName;
                                trans.Date = expdt;
                                trans.LoadINKG = Convert.ToDecimal(TotalWTT);
                                trans.NoOfBoxes = Convert.ToInt32(TotalBoxess);
                                trans.RateType = rdd.RateType;
                                decimal AMT = 0;
                                trans.RouteName = RouteName;

                                if (rdd.RateType == "Trip" || rdd.RateType == "TRIP")
                                {

                                    trans.Rate = rdd.Rate;
                                    trans.Amount = Convert.ToDecimal(rdd.Unit) * Convert.ToInt32(rdd.Rate);
                                }
                                if (rdd.RateType == "WEIGHT")
                                {
                                    // AMT = rdd.Rate * Convert.ToDecimal(TotalWTT);

                                    if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom1 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                    {
                                        trans.Rate = rdd.Rate1;
                                        trans.Amount = Convert.ToDecimal(rdd.Rate1) * Convert.ToDecimal(TotalWTT);
                                    }
                                    if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom2 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                    {
                                        trans.Rate = rdd.Rate2;
                                        trans.Amount = Convert.ToDecimal(rdd.Rate2) * Convert.ToDecimal(TotalWTT);
                                    }
                                    if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom3 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                    {
                                        trans.Rate = rdd.Rate3;
                                        trans.Amount = Convert.ToDecimal(rdd.Rate3) * Convert.ToDecimal(TotalWTT);
                                    }

                                    if (Convert.ToDecimal(TotalWTT) > rdd.WTTo3 && rdd.Rate != 0)
                                    {
                                        trans.Rate = rdd.Rate;
                                        trans.Amount = Convert.ToDecimal(rdd.Rate) * Convert.ToDecimal(TotalWTT);
                                    }

                                }
                                if (rdd.RateType == "BOX")
                                {

                                    if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom1 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                    {
                                        trans.Rate = rdd.Rate1;
                                        trans.Amount = Convert.ToDecimal(rdd.Rate1) * Convert.ToInt32(TotalBoxess);
                                    } else
                                    if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom2 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                    {
                                        trans.Rate = rdd.Rate2;
                                        trans.Amount = Convert.ToDecimal(rdd.Rate2) * Convert.ToInt32(TotalBoxess);
                                    } else
                                    if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom3 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                    {
                                        trans.Rate = rdd.Rate3;
                                        trans.Amount = Convert.ToDecimal(rdd.Rate3) * Convert.ToInt32(TotalBoxess);
                                    } else
                                    if ((Convert.ToInt32(TotalBoxess) > rdd.WTTo3 || rdd.WTTo3 == null) && rdd.Rate != 0)
                                    {
                                        trans.Rate = rdd.Rate;
                                        trans.Amount = rdd.Rate * Convert.ToInt32(TotalBoxess);
                                    }
                                }
                                if (rdd.RateType == "PER KM")
                                {

                                    trans.Rate = rdd.Rate;
                                    //trans.Amount = Convert.ToDecimal(rdd.Unit) * Convert.ToInt32(rdd.Rate);

                                }
                                if (rdd.RateType == "UP TO KM")
                                {

                                }

                                transaction.Add(trans);

                            } else
                            {
                                var trans = transaction.Where(a => a.VendorID == rdd.VendorID && a.Date == expdt).FirstOrDefault();

                                trans.LoadINKG = trans.LoadINKG + Convert.ToDecimal(TotalWTT);
                                trans.NoOfBoxes = trans.NoOfBoxes + Convert.ToInt32(TotalBoxess);
                                trans.RateType = rdd.RateType;
                                trans.CreatedBy = User.Identity.Name;
                                trans.CreatedDate = DateTime.Today;

                                if (rdd.RateType == "Trip" || rdd.RateType == "TRIP")
                                {

                                    trans.Rate = rdd.Rate;
                                    trans.Amount = Convert.ToDecimal(rdd.Unit) * Convert.ToInt32(rdd.Rate);
                                }
                                if (rdd.RateType == "WEIGHT")
                                {
                                    if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom1 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                    {
                                        trans.Rate = rdd.Rate1;
                                        trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate1) * Convert.ToDecimal(TotalWTT));
                                    }
                                    if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom2 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                    {
                                        trans.Rate = rdd.Rate2;
                                        trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate2) * Convert.ToDecimal(TotalWTT));
                                    }
                                    if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom3 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                    {
                                        trans.Rate = rdd.Rate3;
                                        trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate3) * Convert.ToDecimal(TotalWTT));
                                    }

                                    if ((Convert.ToDecimal(TotalWTT) > rdd.WTTo3 || rdd.WTTo3 == null) && rdd.Rate != 0)
                                    {
                                        trans.Rate = rdd.Rate;
                                        trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate) * Convert.ToDecimal(TotalWTT));
                                    }

                                }
                                if (rdd.RateType == "BOX")
                                {

                                    if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom1 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                    {
                                        trans.Rate = rdd.Rate1;
                                        trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate1) * Convert.ToInt32(TotalBoxess));
                                    }
                                    if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom2 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                    {
                                        trans.Rate = rdd.Rate2;
                                        trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate2) * Convert.ToInt32(TotalBoxess));
                                    }
                                    if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom3 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                    {
                                        trans.Rate = rdd.Rate3;
                                        trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate3) * Convert.ToInt32(TotalBoxess));
                                    }

                                    if (Convert.ToInt32(TotalBoxess) > rdd.WTTo3 && rdd.Rate != 0)
                                    {
                                        trans.Rate = rdd.Rate;
                                        trans.Amount = trans.Amount + (rdd.Rate * Convert.ToInt32(TotalBoxess));
                                    }
                                }

                                if (rdd.RateType == "PER KM")
                                {
                                    trans.Rate = rdd.Rate;
                                }
                                if (rdd.RateType == "UP TO KM")
                                {

                                }

                            }
                        }


                        var lstt = Cust.Select(a => a.OrderNo).ToList();


                        var OrderMainID = db.Order.Where(x1 => lstt.Contains(x1.Id)).Select(a => a.MasterOrderId).Distinct().ToList();
                        foreach (var xx in OrderMainID)
                        {
                            var InvNo = db.TableNumbring.Where(a => a.TableName == "InvoiceNo").FirstOrDefault();
                            var invv = InvNo.Prefix + "" + InvNo.SerialNo;
                            InvNo.SerialNo = InvNo.SerialNo + 1;

                            var dbdata = (from om in db.MainOrder.Where(a => a.Id == xx)
                                          join cm in db.CustomerMasters on om.CustomerId equals cm.ID
                                          join qd in db.QuotationDetails on cm.QuotationID equals qd.MainID
                                          select new
                                          {
                                              ProductName = qd.ProductName,
                                              ProductID = qd.ProductID,
                                              UOM = qd.UOM,
                                              WTFrom1 = qd.WTFrom1,
                                              WTTo1 = qd.WTTo1,
                                              Rate1 = qd.Rate1,
                                              WTFrom2 = qd.WTFrom2,
                                              WTTo2 = qd.WTTo2,
                                              Rate2 = qd.Rate2,

                                              WTFrom3 = qd.WTFrom3,
                                              WTTo3 = qd.WTTo3,
                                              Rate3 = qd.Rate3,
                                              AddWt = qd.AddWt,
                                              AddRate = qd.AddRate,

                                              CustomerName=om.CustomerName,
                                              CustomerID = om.CustomerId,


                                          }).ToList();

                            //                           
                            var Orderid = db.Order.Where(a => a.MasterOrderId == xx && lstt.Contains(a.Id)).ToList();
                            foreach (var o in Orderid)
                            {
                                var orderdtl = db.Order.Where(a => a.Id == o.Id).FirstOrDefault();

                                orderdtl.InvoiceNo = invv;
                                orderdtl.InvoiceDate = expdt;
                                orderdtl.OrderStatus = "Order Dispatched";
                                orderdtl.DispatchID = DispatchID;
                                orderdtl.OrderStage = 8;

                                var tempp = true;

                                var dbdatanew = dbdata.Where(a => a.ProductID == orderdtl.ProductId).FirstOrDefault();
                                if (dbdatanew == null)
                                {
                                    tempp = false;
                                    if (orderdtl.TodaysRate !=0 && orderdtl.TodaysRate != null)
                                    {
                                        orderdtl.RateIN = "PCS";
                                        orderdtl.Rate = orderdtl.TodaysRate;
                                        orderdtl.Amount = orderdtl.TodaysRate * orderdtl.Qty; 
                                    }
                                    else
                                    {                                      
                                        orderdtl.RateIN = "PCS";
                                        orderdtl.Rate = 0;
                                        orderdtl.TodaysRate = 0;
                                        orderdtl.Amount = 0;
                                    }


                                }
                                if (tempp == true)
                                {
                                    if (dbdatanew.UOM == "BOX")
                                    {
                                        if (orderdtl.Qty > dbdatanew.WTFrom1 && orderdtl.Qty <= dbdatanew.WTTo1 && dbdatanew.Rate1 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate1;
                                            orderdtl.Amount = dbdatanew.Rate1 * orderdtl.Qty;
                                        }
                                        if (orderdtl.Qty > dbdatanew.WTFrom2 && orderdtl.Qty <= dbdatanew.WTTo2 && dbdatanew.Rate2 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate2;
                                            orderdtl.Amount = dbdatanew.Rate2 * orderdtl.Qty;
                                        }
                                        if (orderdtl.Qty > dbdatanew.WTFrom3 && orderdtl.Qty <= dbdatanew.WTTo3 && dbdatanew.Rate3 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate3;
                                            orderdtl.Amount = dbdatanew.Rate3 * orderdtl.Qty;
                                        }

                                        if (orderdtl.Qty > dbdatanew.WTTo3 && dbdatanew.AddRate != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.AddRate;
                                            orderdtl.Amount = dbdatanew.AddRate * orderdtl.Qty;
                                        }

                                    }
                                    if (dbdatanew.UOM == "KG")
                                    {
                                        if (orderdtl.Weight > dbdatanew.WTFrom1 && orderdtl.Weight <= dbdatanew.WTTo1 && dbdatanew.Rate1 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate1;
                                            orderdtl.Amount = dbdatanew.Rate1 * orderdtl.Weight;
                                        }
                                        else
                                        if (orderdtl.Weight > dbdatanew.WTFrom2 && orderdtl.Weight <= dbdatanew.WTTo2 && dbdatanew.Rate2 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate2;
                                            orderdtl.Amount = dbdatanew.Rate2 * orderdtl.Weight;
                                        }
                                        else
                                        if (orderdtl.Weight > dbdatanew.WTFrom3 && orderdtl.Weight <= dbdatanew.WTTo3 && dbdatanew.Rate3 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate3;
                                            orderdtl.Amount = dbdatanew.Rate3 * orderdtl.Weight;
                                        }
                                        else

                                        if (orderdtl.Weight > dbdatanew.WTTo3 && dbdatanew.AddRate != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.AddRate;
                                            orderdtl.Amount = dbdatanew.AddRate * orderdtl.Weight;
                                        }
                                    }
                                    if (dbdatanew.UOM == "PCS")
                                    {
                                        if (orderdtl.Qty > dbdatanew.WTFrom1 && orderdtl.Qty <= dbdatanew.WTTo1 && dbdatanew.Rate1 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate1;
                                            orderdtl.Amount = dbdatanew.Rate1 * orderdtl.Qty;
                                        }
                                        if (orderdtl.Qty > dbdatanew.WTFrom2 && orderdtl.Qty <= dbdatanew.WTTo2 && dbdatanew.Rate2 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate2;
                                            orderdtl.Amount = dbdatanew.Rate2 * orderdtl.Qty;
                                        }
                                        if (orderdtl.Qty > dbdatanew.WTFrom3 && orderdtl.Qty <= dbdatanew.WTTo3 && dbdatanew.Rate3 != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.Rate3;
                                            orderdtl.Amount = dbdatanew.Rate3 * orderdtl.Qty;
                                        }

                                        if (orderdtl.Qty > dbdatanew.WTTo3 && dbdatanew.AddRate != 0)
                                        {
                                            orderdtl.RateIN = dbdatanew.UOM;
                                            orderdtl.Rate = dbdatanew.AddRate;
                                            orderdtl.Amount = dbdatanew.AddRate * orderdtl.Qty;
                                        }
                                    }
                                }



                             

                                //TrackingDetails track = new TrackingDetails();
                                //    track.Status = orderdtl.OrderStatus;
                                //    track.Action = "Dispatch Details";
                                //    track.Location = "Plant";
                                //    track.OrderNo = orderdtl.Id;
                                //    track.Date = DateTime.Now;
                                //    track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                                //    track.CreatedDate = DateTime.Now;
                                //    track.CreatedBy = User.Identity.Name;
                                //    db.trackingDetails.Add(track);
                                //      temp++;

                            }
                        }

                    }

                    ///Manisfest Code
                    ///
                    var dispatchdetails = db.dispatchDetails.Where(a => a.DispatchID == DispatchID).FirstOrDefault();
                    var driverdtl = db.DriverMaster.Where(a => a.DriverName == dispatchdetails.DriverName).FirstOrDefault();
                    var routedtl = db.RouteDetails.Where(a => a.RouteMainID == dispatchdetails.Route && a.VendorName == dispatchdetails.VendorName).FirstOrDefault();
                    if (routedtl == null)
                    {
                        var result1 = new { Message = "Route Not Added For Vendor" + dispatchdetails.VendorName, };
                        return Json(result1, JsonRequestBehavior.AllowGet);
                    }


                    var MasterOrderId = db.Order.Where(a => a.DispatchID == DispatchID).Select(a => a.MasterOrderId).ToList();
                    var CustomerList = db.MainOrder.Where(a => MasterOrderId.Contains(a.Id)).ToList();


                    var lst = Cust.Select(a => a.OrderNo).ToList();
                    var data2 = (from x1 in db.Order.Where(x1 => lst.Contains(x1.Id))

                                 join c in db.MainOrder
                                on x1.MasterOrderId equals c.Id


                                 join cm in db.CustomerMasters
                                 on c.CustomerId equals cm.ID


                                 // join dd in db.dispatchDetails
                                 //on x1.DispatchID equals dd.DispatchID


                                 select new
                                 {
                                     //DriverName = dd.DriverName,
                                     //Route = dd.Route,
                                     //VehicalNo = dd.VehicalNo,
                                     //VendorType = dd.VendorType,
                                     OrderNo = x1.Id,
                                     CustID = cm.ID,
                                     CustName = cm.CustName,
                                     CustCode = cm.CustID,
                                     ItemName = x1.ProductName,
                                     OrderQty = x1.Qty,
                                     BoxinKG = 0,
                                     Location = cm.Area,
                                     Weight = x1.Weight,
                                     //        DispatchDate = dd.DispatchDate,
                                     InvoiceNo = x1.InvoiceNo,
                                     CustomerID = cm.ID,

                                 }).OrderBy(xx => xx.CustName).ToList();


                    var routedata = db.RouteMains.Where(a => a.ID == Route).FirstOrDefault();
                    var custgrp = data2.Select(a => a.CustID).ToList();
                    var CustGroup = db.ManifestGroups.Where(a => custgrp.Contains(a.CustomerID) && a.RouteID == Route).Select(a => new { a.CustomerID, a.GroupName }).Distinct().ToList();

                    var grpp = CustGroup.Select(a => a.GroupName).Distinct().ToList();


                    foreach (var g1 in grpp)
                    {
                        var InvNo = db.TableNumbring.Where(a => a.TableName == "ManifestNo").FirstOrDefault();
                        var drsno = InvNo.Prefix + "" + InvNo.SerialNo;
                        InvNo.SerialNo = InvNo.SerialNo + 1;


                        ManifestMain main = new ManifestMain();
                        main.GroupName = g1;
                        main.DispatchID = DispatchID;
                        main.DeliveryChallanNo = drsno;

                        main.RouteName = routedata.MainRouteName;
                        main.RouteID = Route;

                        main.Date = expdt.Date;
                        main.Mode = routedtl.Mode;
                        main.TransportName = dispatchdetails.VendorName;
                        main.VehicleNo = dispatchdetails.VehicalNo;
                        main.VehicleType = dispatchdetails.VendorType;

                        main.DriverName = driverdtl.DriverName;
                        main.DriverMobileNo = driverdtl.MobileNo;
                        main.CreatedBy = User.Identity.Name;
                        main.CreatedDate = DateTime.Today;


                        var test = CustGroup.Where(a => a.GroupName == g1).ToList();
                        var Count = 1;
                        decimal TotalOrderWeight = 0;
                        var TotalDispatchQty = 0;
                        var TotalBoxQty = 0;
                        decimal TotalBoxinKG = 0;
                        foreach (var x1 in test)
                        {
                            var dta = data2.Where(a => a.CustID == x1.CustomerID).ToList();

                            foreach (var xx in dta)
                            {
                                var pkg = db.PackingDetails.Where(a => a.OrderNo == xx.OrderNo)
                          .GroupBy(a => a.OrderNo)
                          .Select(a => new { NoOfBox = a.Sum(b => b.NoOfBox), BoxWeight = a.Sum(b => b.BoxWeight), TotalWeight = a.Sum(b => b.TotalWeight), Name = a.Key })
                          .FirstOrDefault();

                                ManifestDetails md = new ManifestDetails();
                                md.DeliveryChallanNo = drsno;
                                md.DispatchID = DispatchID;
                                md.OrderNo = xx.OrderNo;
                                md.CustomerID = xx.CustomerID;
                                md.CustomerCode = xx.CustCode;
                                md.CustomerName = xx.CustName;
                                md.Location = xx.Location;
                                md.ProductName = xx.ItemName;
                                md.OrderWeight = xx.Weight;
                                md.DispatchQty = xx.OrderQty;
                                md.BoxQty = pkg.NoOfBox;
                                md.BoxInKG = Convert.ToDecimal(pkg.TotalWeight);

                                db.ManifestDetails.Add(md);

                                TotalOrderWeight = TotalOrderWeight + Convert.ToDecimal(xx.Weight);
                                TotalDispatchQty = TotalDispatchQty + Convert.ToInt32(xx.OrderQty);
                                TotalBoxQty = TotalBoxQty + pkg.NoOfBox;
                                TotalBoxinKG = TotalBoxinKG + Convert.ToDecimal(pkg.TotalWeight);

                            }
                        }

                        main.TotalOrderWeight = TotalOrderWeight;
                        main.TotalDispatchQty = TotalDispatchQty;
                        main.TotalBoxQty = TotalBoxQty;
                        main.TotalBoxinKG = TotalBoxinKG;
                        db.ManifestMains.Add(main);
                    }
                    db.DispatchTransactions.AddRange(transaction);
                    foreach (var c in Cust)
                    {
                        TrackingDetails track = new TrackingDetails();
                        track.Status = "Order Dispatched";
                        track.Action = "Dispatch Details";
                        track.Location = "Plant";
                        track.OrderNo = c.OrderNo;
                        track.Date = DateTime.Now;
                        track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                        track.CreatedDate = DateTime.Now;
                        track.CreatedBy = User.Identity.Name;
                        db.trackingDetails.Add(track);
                    }

                    db.SaveChanges();
                    dbTran.Commit();
                    var result = new { Message = "success" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            } catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }


        public ActionResult ManifestPrint(int OrderNo)
        {
            try
            {
                var OD = db.Order.Where(a => a.Id == OrderNo).FirstOrDefault();

                if (OD.OrderStatus == "Packing Completed" || OD.OrderStatus == "Quantity Changed & Packing Completed" || OD.OrderStatus == "Merge & Packing Completed")
                {
                    var _OM = db.MainOrder.Where(a => a.Id == OD.MasterOrderId).FirstOrDefault();
                    var customer = db.CustomerMasters.Where(a => a.ID == _OM.CustomerId).FirstOrDefault();
                    var PD = db.PackingDetails.Where(a => a.OrderNo == OrderNo).ToList();


                    Document document;
                    //  var pgSize = new iTextSharp.text.Rectangle(321f, 245f);
                    document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20f, 10f, 20f, 10f);

                    //  document = new Document(iTextSharp.text.PageSize.A4, 10f, 10f, 10f, 0f);
                    string path = Server.MapPath("~/Attachments/LabelPrints/");
                    string filename1 = path + User.Identity.Name + ".pdf";
                    //string filename1 = path11;
                    string StikerPrintName = User.Identity.Name + ".pdf";
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filename1, FileMode.Create));
                    document.Open();



                    var TOTAL = PD.Sum(a => a.NoOfBox);
                    var Count = 1;

                    foreach (var x in PD)
                    {

                        for (int i = 1; i <= x.NoOfBox; i++)
                        {
                            PdfPTable table1 = new PdfPTable(4);
                            float[] width1 = new float[] { 15f, 10, 10, 10 };
                            table1.SetWidths(width1);
                            table1.WidthPercentage = 100f;
                            table1.DefaultCell.Padding = 1;

                            string barcode1 = Server.MapPath("~/dist/img/logo.png");
                            iTextSharp.text.Image barcodejpg1 = iTextSharp.text.Image.GetInstance(barcode1);
                            barcodejpg1.ScaleToFit(85, 45);
                            barcodejpg1.SpacingBefore = 5f;
                            barcodejpg1.SpacingAfter = 1f;
                            barcodejpg1.Alignment = Element.ALIGN_LEFT;


                            var Dims = "";
                            var FullConsigneeAddress = "";


                            Paragraph P117585 = new Paragraph();
                            P117585.Add(new Phrase("", FontFactory.GetFont("Arial", 11, Font.BOLD)));
                            P117585.Add(new Chunk(barcodejpg1, 0, 0, true));
                            PdfPCell prr32987456 = new PdfPCell(P117585);
                            prr32987456.HorizontalAlignment = 1;
                            table1.AddCell(prr32987456);


                            Paragraph pr1 = new Paragraph();
                            pr1.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                            pr1.Add(new Phrase("MANEGROW\n", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                            pr1.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                            pr1.Add(new Phrase("AGRO PRODUCTS ", FontFactory.GetFont("Arial", 12, Font.NORMAL)));
                            pr1.Add(new Phrase("PVT LTD\n", FontFactory.GetFont("Arial", 12, Font.NORMAL)));

                            PdfPCell PC111 = new PdfPCell(pr1);
                            PC111.FixedHeight = 62f;
                            PC111.Colspan = 3;
                            PC111.HorizontalAlignment = 1;
                            table1.AddCell(PC111);

                            try
                            {
                                Paragraph pr132178 = new Paragraph();
                                pr132178.Add(new Phrase("\nCUSTOMER ", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                                PdfPCell PC19785854 = new PdfPCell(pr132178);
                                //    PC19785854.Border = Rectangle.BOTTOM_BORDER;
                                PC19785854.FixedHeight = 50;
                                PC19785854.HorizontalAlignment = 0;
                                table1.AddCell(PC19785854);

                                Paragraph pr13872154 = new Paragraph();
                                pr13872154.Add(new Phrase("" + customer.CustName, FontFactory.GetFont("Arial", 14, Font.BOLD)));
                                pr13872154.Add(new Phrase("\n" + customer.ShortName, FontFactory.GetFont("Arial", 14, Font.BOLD)));
                                PdfPCell PC197858745 = new PdfPCell(pr13872154);
                                PC197858745.Colspan = 3;
                                //   PC197858745.Border = Rectangle.BOTTOM_BORDER;
                                PC197858745.HorizontalAlignment = 1;
                                table1.AddCell(PC197858745);
                            }
                            catch { };
                            //Second Row                                
                            try
                            {
                                Paragraph pr132178 = new Paragraph();
                                pr132178.Add(new Phrase("ORDER No ", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                                PdfPCell PC19785854 = new PdfPCell(pr132178);
                                PC19785854.FixedHeight = 22;
                                //  PC19785854.Border = Rectangle.BOTTOM_BORDER;
                                PC19785854.HorizontalAlignment = 0;
                                table1.AddCell(PC19785854);

                                Paragraph pr13872154 = new Paragraph();
                                pr13872154.Add(new Phrase("" + OrderNo, FontFactory.GetFont("Arial", 14, Font.BOLD)));

                                PdfPCell PC197858745 = new PdfPCell(pr13872154);
                                PC197858745.Colspan = 3;
                                //    PC197858745.Border = Rectangle.BOTTOM_BORDER;
                                PC197858745.HorizontalAlignment = 1;
                                table1.AddCell(PC197858745);
                            }
                            catch
                            { }
                            try
                            {
                                Paragraph pr132178 = new Paragraph();
                                pr132178.Add(new Phrase("DESTINATION", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                                PdfPCell PC19785854 = new PdfPCell(pr132178);
                                //   PC19785854.Border = Rectangle.BOTTOM_BORDER;
                                PC19785854.FixedHeight = 22;
                                PC19785854.HorizontalAlignment = 0;
                                table1.AddCell(PC19785854);

                                Paragraph pr13872154 = new Paragraph();
                                pr13872154.Add(new Phrase("" + customer.City, FontFactory.GetFont("Arial", 14, Font.BOLD)));
                                PdfPCell PC197858745 = new PdfPCell(pr13872154);
                                PC197858745.Colspan = 3;

                                //      PC197858745.Border = Rectangle.BOTTOM_BORDER;
                                PC197858745.HorizontalAlignment = 1;
                                table1.AddCell(PC197858745);
                            }
                            catch { };

                            try
                            {
                                Paragraph pr132178 = new Paragraph();
                                pr132178.Add(new Phrase("PCS", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                                PdfPCell PC19785854 = new PdfPCell(pr132178);
                                PC19785854.FixedHeight = 22;
                                //  PC19785854.Border = Rectangle.BOTTOM_BORDER;
                                PC19785854.HorizontalAlignment = 0;
                                table1.AddCell(PC19785854);

                                Paragraph pr13872154 = new Paragraph();

                                pr13872154.Add(new Phrase("" + Count + "/" + TOTAL, FontFactory.GetFont("Arial", 14, Font.BOLD)));
                                PdfPCell PC197858745 = new PdfPCell(pr13872154);
                                PC197858745.Colspan = 3;
                                //    PC197858745.Border = Rectangle.BOTTOM_BORDER;
                                PC197858745.HorizontalAlignment = 1;
                                table1.AddCell(PC197858745);
                            }
                            catch
                            { }

                            try
                            {
                                Paragraph pr132178 = new Paragraph();
                                pr132178.Add(new Phrase("WEIGHT", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                                PdfPCell PC19785854 = new PdfPCell(pr132178);
                                //     PC19785854.Border = Rectangle.BOTTOM_BORDER;
                                PC19785854.FixedHeight = 22;
                                PC19785854.HorizontalAlignment = 0;
                                table1.AddCell(PC19785854);

                                Paragraph pr13872154 = new Paragraph();
                                pr13872154.Add(new Phrase("" + x.BoxWeight, FontFactory.GetFont("Arial", 14, Font.BOLD)));
                                PdfPCell PC197858745 = new PdfPCell(pr13872154);
                                PC197858745.Colspan = 3;
                                //      PC197858745.Border = Rectangle.BOTTOM_BORDER;
                                PC197858745.HorizontalAlignment = 1;
                                table1.AddCell(PC197858745);
                            }
                            catch { };



                            try
                            {
                                Paragraph pr132178 = new Paragraph();
                                pr132178.Add(new Phrase("Pawana, Gat No 379, At Post Bebad Ohol, Dam Rd, Tal. Maval, Maharashtra 410506    Mob- 09156552009, 09156552032 \nEmail : sales@manegrow.com Website - www.manegrow.com", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                                PdfPCell PC19785854 = new PdfPCell(pr132178);
                                //      PC19785854.Border = Rectangle.NO_BORDER;
                                //PC19785854.FixedHeight = 30;
                                PC19785854.HorizontalAlignment = 0;
                                PC19785854.Colspan = 4;

                                table1.AddCell(PC19785854);

                            }
                            catch { };

                            document.Add(table1);
                            Count++;
                        }
                    }
                    // document.NewPage();
                    document.Close();

                    Session["LabelAWBNO"] = StikerPrintName;
                    var result = new { Message = "success", FileName = StikerPrintName };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var result = new { Message = "This Facility Only For Packing...." };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception EX)
            {
                //document.Close();
                var result = new { Message = EX.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var result11 = new { Message = "success", };
            return Json(result11, JsonRequestBehavior.AllowGet);
        }
        public FileResult GetReportLabel()
        {
            string FileName = Session["LabelAWBNO"].ToString();
            Session["LabelAWBNO"] = null;
            string path = Server.MapPath("~/Attachments/LabelPrints/");
            path = path + FileName;
            string ReportURL = path;
            byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
            return File(FileBytes, "application/pdf", FileName);
        }

        public FileResult GetReport()
        {
            string FileName = Session["DRSNO"].ToString();
            Session["DRSNO"] = null;
            string path = Server.MapPath("~/Reports/DRSPrints/");
            path = path + FileName;
            string ReportURL = path;
            byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
            return File(FileBytes, "application/pdf", FileName);
        }

        [HttpGet]
        public JsonResult DRSPrint(int DispatchID)
        {
            try
            {

                Document document;
                document = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20f, 10f, 20f, 10f);
                long timeSince1970 = DateTimeOffset.Now.ToUnixTimeSeconds();
                string path = Server.MapPath("~/Attachments/LabelPrints/");
                string filename1 = path + User.Identity.Name + timeSince1970 + ".pdf";
                string StikerPrintName = User.Identity.Name + timeSince1970 + ".pdf";
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filename1, FileMode.Create));
                document.Open();

                var ManifestMain = db.ManifestMains.Where(a => a.DispatchID == DispatchID).ToList();

                foreach (var x in ManifestMain)
                {
                    PdfPTable table1 = new PdfPTable(14);
                    float[] width1 = new float[] { 0.1f, 1.5f, 3f, 3f, 6f, 4f, 6f, 2f, 2f, 2f, 4f, 3.5f, 4f, 0.1f };
                    table1.SetWidths(width1);
                    table1.WidthPercentage = 100f;
                    table1.DefaultCell.Padding = 1;

                    string barcode1 = Server.MapPath("~/dist/img/logo.png");
                    iTextSharp.text.Image barcodejpg1 = iTextSharp.text.Image.GetInstance(barcode1);
                    barcodejpg1.ScaleToFit(85, 45);
                    barcodejpg1.SpacingBefore = 5f;
                    barcodejpg1.SpacingAfter = 1f;
                    barcodejpg1.Alignment = Element.ALIGN_LEFT;

                    Paragraph P117585 = new Paragraph();
                    P117585.Add(new Phrase("", FontFactory.GetFont("Arial", 11, Font.BOLD)));
                    P117585.Add(new Chunk(barcodejpg1, 0, 0, true));
                    PdfPCell prr32987456 = new PdfPCell(P117585);
                    prr32987456.HorizontalAlignment = 1;
                    prr32987456.Colspan = 4;
                    table1.AddCell(prr32987456);


                    Paragraph pr1 = new Paragraph();
                    pr1.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1.Add(new Phrase("MANEGROW\n", FontFactory.GetFont("Arial", 20, Font.BOLD)));
                    pr1.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1.Add(new Phrase("AGRO PRODUCTS ", FontFactory.GetFont("Arial", 12, Font.BOLD)));
                    pr1.Add(new Phrase("PVT LTD\n", FontFactory.GetFont("Arial", 12, Font.BOLD)));

                    pr1.Add(new Phrase("Gate No 379, At Post Bebad Ohol, Dam Rd, Tal.Maval, Maharashtra 410506\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));



                    PdfPCell PC111 = new PdfPCell(pr1);
                    PC111.FixedHeight = 62f;
                    PC111.Colspan = 10;
                    PC111.HorizontalAlignment = 1;
                    table1.AddCell(PC111);



                    //2 nd row

                    Paragraph pr1002 = new Paragraph();
                    pr1002.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1002.Add(new Phrase("DELIVERY CHALLAN", FontFactory.GetFont("Arial", 14, Font.BOLD)));
                    PdfPCell PC11001 = new PdfPCell(pr1002);
                    PC11001.FixedHeight = 30f;
                    PC11001.Colspan = 14;
                    PC11001.HorizontalAlignment = 1;
                    table1.AddCell(PC11001);

                    //3rd row

                    PdfPCell PC1100dfx1 = new PdfPCell();
                    PC1100dfx1.Border = Rectangle.LEFT_BORDER;
                    table1.AddCell(PC1100dfx1);

                    Paragraph pr1002sd = new Paragraph();
                    pr1002sd.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1002sd.Add(new Phrase("Delivery Challan No\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    pr1002sd.Add(new Phrase("Route\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                    pr1002sd.Add(new Phrase("Group Name\n\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                    pr1002sd.Add(new Phrase("Out Time\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));

                    PdfPCell PC1100df1 = new PdfPCell(pr1002sd);

                    PC1100df1.Colspan = 2;
                    PC1100df1.HorizontalAlignment = 0;
                    PC1100df1.Border = Rectangle.BOTTOM_BORDER;
                    table1.AddCell(PC1100df1);

                    Paragraph pr1002sd6 = new Paragraph();
                    pr1002sd6.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1002sd6.Add(new Phrase(": " + x.DeliveryChallanNo + "\n\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                    pr1002sd6.Add(new Phrase(": " + x.RouteName + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                    pr1002sd6.Add(new Phrase(": " + x.GroupName + "\n\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                    pr1002sd6.Add(new Phrase(": ", FontFactory.GetFont("Arial", 10, Font.NORMAL)));


                    PdfPCell PC1100df11 = new PdfPCell(pr1002sd6);

                    PC1100df11.Colspan = 5;
                    PC1100df11.HorizontalAlignment = 0;
                    PC1100df11.Border = Rectangle.BOTTOM_BORDER;
                    table1.AddCell(PC1100df11);


                    Paragraph pr1002dfdf = new Paragraph();
                    pr1002dfdf.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1002dfdf.Add(new Phrase("Date  \n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    pr1002dfdf.Add(new Phrase("Mode  \n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    pr1002dfdf.Add(new Phrase("Transport Name  \n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    pr1002dfdf.Add(new Phrase("Vehicle No  \n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    pr1002dfdf.Add(new Phrase("Vehicle Type  \n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    pr1002dfdf.Add(new Phrase("Driver Name  \n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    pr1002dfdf.Add(new Phrase("Mobile No  \n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                    PdfPCell PC11001022 = new PdfPCell(pr1002dfdf);

                    PC11001022.Colspan = 2;
                    PC11001022.HorizontalAlignment = 0;
                    PC11001022.Border = Rectangle.BOTTOM_BORDER;
                    table1.AddCell(PC11001022);


                    Paragraph pr1002dfd22f = new Paragraph();
                    pr1002dfd22f.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase(": " + x.Date.ToString("dd/MM/yyyy") + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase(": " + x.Mode + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase(": " + x.TransportName + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase(": " + x.VehicleNo + " \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase(": " + x.VehicleType + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase(": " + x.DriverName + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase(": " + x.DriverMobileNo + "\n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    PdfPCell PC1100102s2 = new PdfPCell(pr1002dfd22f);
                    PC1100102s2.Colspan = 3;
                    PC1100102s2.HorizontalAlignment = 0;
                    PC1100102s2.Border = Rectangle.BOTTOM_BORDER;
                    table1.AddCell(PC1100102s2);


                    PdfPCell PC1100d102s2 = new PdfPCell();
                    //  PC1100d102s2.Colspan = 3;
                    PC1100d102s2.HorizontalAlignment = 0;
                    PC1100d102s2.Border = Rectangle.RIGHT_BORDER;
                    table1.AddCell(PC1100d102s2);

                    try
                    {


                        PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("Sr No", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p1.HorizontalAlignment = 1;
                        p1.BackgroundColor = BaseColor.WHITE;
                        p1.FixedHeight = 20;
                        p1.Colspan = 2;
                        //    p1.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p1);

                        PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("Order No", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p2dd.HorizontalAlignment = 1;
                        p2dd.BackgroundColor = BaseColor.WHITE;
                        //      p2dd.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p2dd);

                        PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("Customer Code", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p2.HorizontalAlignment = 1;
                        p2.BackgroundColor = BaseColor.WHITE;
                        //        p2.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p2);

                        PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("Party Name", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp5.HorizontalAlignment = 1;
                        pp5.BackgroundColor = BaseColor.WHITE;
                        //        pp5.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp5);

                        PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("Location", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp55.HorizontalAlignment = 1;
                        pp55.BackgroundColor = BaseColor.WHITE;
                        //        pp55.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp55);

                        PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("Item Description", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p29.HorizontalAlignment = 1;
                        p29.BackgroundColor = BaseColor.WHITE;
                        //         p29.Border = Rectangle.BOTTOM_BORDER;
                        //    p29.Colspan = 2;
                        table1.AddCell(p29);

                        PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("Order Weight", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp51.HorizontalAlignment = 1;
                        pp51.BackgroundColor = BaseColor.WHITE;
                        //           pp51.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp51);


                        PdfPCell p6 = new PdfPCell();
                        p6 = new PdfPCell(new Phrase(new Phrase("Disp Qty", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p6.HorizontalAlignment = 1;
                        p6.BackgroundColor = BaseColor.WHITE;
                        //              p6.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p6);

                        PdfPCell pp4 = new PdfPCell(new Phrase(new Phrase("Box Qty", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp4.HorizontalAlignment = 1;
                        pp4.BackgroundColor = BaseColor.WHITE;
                        pp4.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp4);



                        PdfPCell pp41 = new PdfPCell(new Phrase(new Phrase("Box in KG", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp41.HorizontalAlignment = 1;
                        pp41.BackgroundColor = BaseColor.WHITE;
                        //              pp41.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp41);


                        PdfPCell p291 = new PdfPCell(new Phrase(new Phrase("Invoice Code", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p291.HorizontalAlignment = 1;
                        p291.BackgroundColor = BaseColor.WHITE;
                        //             p291.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p291);

                        PdfPCell p292 = new PdfPCell(new Phrase(new Phrase("Signature Of Party", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292.HorizontalAlignment = 1;
                        p292.BackgroundColor = BaseColor.WHITE;
                        //              p292.Border = Rectangle.BOTTOM_BORDER;
                        p292.Colspan = 2;
                        table1.AddCell(p292);


                    }
                    catch
                    {

                    }

                    var Count = 1;


                    //       var manifesrdtl = db.ManifestDetails.Where(a => a.DeliveryChallanNo == x.DeliveryChallanNo).ToList();

                    var data2 = (from x1 in db.ManifestDetails.Where(a => a.DeliveryChallanNo == x.DeliveryChallanNo)
                                 join c in db.Order
                                on x1.OrderNo equals c.Id

                                 select new
                                 {
                                     OrderNo = x1.OrderNo,
                                     CustomerID = x1.CustomerID,
                                     CustomerCode = x1.CustomerCode,
                                     CustomerName = x1.CustomerName,
                                     Location = x1.Location,
                                     ProductName = x1.ProductName,
                                     OrderWeight = x1.OrderWeight,
                                     DispatchQty = x1.DispatchQty,
                                     BoxQty = x1.BoxQty,

                                     InvoiceNo = c.InvoiceNo,
                                     BoxInKG = x1.BoxInKG,


                                 }).OrderBy(xx => xx.CustomerName).ToList();


                    foreach (var xx in data2)
                    {


                        PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("" + Count, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p1.HorizontalAlignment = 1;
                        p1.BackgroundColor = BaseColor.WHITE;
                        p1.FixedHeight = 25;
                        p1.Colspan = 2;
                        //     p1.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p1);

                        PdfPCell p2sd = new PdfPCell(new Phrase(new Phrase("" + xx.OrderNo, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p2sd.HorizontalAlignment = 1;
                        p2sd.BackgroundColor = BaseColor.WHITE;
                        //      p2sd.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p2sd);



                        PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("" + xx.CustomerCode, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p2.HorizontalAlignment = 1;
                        p2.BackgroundColor = BaseColor.WHITE;
                        ///   p2.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p2);

                        PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("" + xx.CustomerName, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        pp5.HorizontalAlignment = 1;
                        pp5.BackgroundColor = BaseColor.WHITE;
                        //    pp5.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp5);


                        PdfPCell p2945 = new PdfPCell(new Phrase(new Phrase("" + xx.Location, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p2945.HorizontalAlignment = 1;
                        p2945.BackgroundColor = BaseColor.WHITE;
                        //      p2945.Border = Rectangle.BOTTOM_BORDER;
                        //   p29.Colspan = 2;
                        table1.AddCell(p2945);


                        PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("" + xx.ProductName, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p29.HorizontalAlignment = 1;
                        p29.BackgroundColor = BaseColor.WHITE;
                        //     p29.Border = Rectangle.BOTTOM_BORDER;
                        //   p29.Colspan = 2;
                        table1.AddCell(p29);

                        PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("" + xx.OrderWeight, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        pp51.HorizontalAlignment = 1;
                        pp51.BackgroundColor = BaseColor.WHITE;
                        //      pp51.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp51);


                        PdfPCell p6 = new PdfPCell();
                        p6 = new PdfPCell(new Phrase(new Phrase("" + xx.DispatchQty, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p6.HorizontalAlignment = 1;
                        p6.BackgroundColor = BaseColor.WHITE;
                        //      p6.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p6);

                        var WtDtl = db.PackingDetails.Where(a => a.OrderNo == xx.OrderNo).OrderBy(a => a.OrderNo).ToList();
                        var strbox = "";
                        foreach (var t in WtDtl)
                        {
                            strbox = strbox + "" + Convert.ToDecimal(t.BoxWeight) + ":" + decimal.ToInt32(t.NoOfBox) + "/";
                        }



                        PdfPCell pp4 = new PdfPCell(new Phrase(new Phrase("" + xx.BoxQty, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        pp4.HorizontalAlignment = 1;
                        pp4.BackgroundColor = BaseColor.WHITE;
                        //        pp4.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp4);

                        string founderMinus1 = strbox.Remove(strbox.Length - 1, 1);

                        //PdfPCell pp4 = new PdfPCell(new Phrase(new Phrase("" + founderMinus1, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        //pp4.HorizontalAlignment = 1;
                        //pp4.BackgroundColor = BaseColor.WHITE;
                        ////        pp4.Border = Rectangle.BOTTOM_BORDER;
                        //table1.AddCell(pp4);


                        PdfPCell pp41 = new PdfPCell(new Phrase(new Phrase("" + founderMinus1, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        pp41.HorizontalAlignment = 1;
                        pp41.BackgroundColor = BaseColor.WHITE;
                        //        pp41.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp41);




                        PdfPCell p291 = new PdfPCell(new Phrase(new Phrase("" + xx.InvoiceNo, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p291.HorizontalAlignment = 1;
                        p291.BackgroundColor = BaseColor.WHITE;
                        //           p291.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p291);

                        PdfPCell p292 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        p292.HorizontalAlignment = 1;
                        p292.BackgroundColor = BaseColor.WHITE;
                        //            p292.Border = Rectangle.BOTTOM_BORDER;
                        p292.Colspan = 2;
                        table1.AddCell(p292);

                        // PdfPCell p292s = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                        // p292s.HorizontalAlignment = 1;
                        // p292s.BackgroundColor = BaseColor.WHITE;
                        // //         p292s.Border = Rectangle.RIGHT_BORDER;
                        //p292.Colspan = 2;
                        // table1.AddCell(p292s);

                        Count++;
                    }

                    try
                    {
                        PdfPCell p292e22 = new PdfPCell(new Phrase(new Phrase("TOTAL", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292e22.HorizontalAlignment = 1;
                        p292e22.BackgroundColor = BaseColor.WHITE;
                        //    p292e22.Border = Rectangle.RIGHT_BORDER;
                        p292e22.Colspan = 7;
                        table1.AddCell(p292e22);

                    }
                    catch (Exception ee) { }
                    try
                    {
                        PdfPCell p292e22 = new PdfPCell(new Phrase(new Phrase("" + x.TotalOrderWeight, FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292e22.HorizontalAlignment = 1;
                        p292e22.BackgroundColor = BaseColor.WHITE;
                        //      p292e22.Border = Rectangle.RIGHT_BORDER;

                        table1.AddCell(p292e22);

                    }
                    catch (Exception ee) { }
                    try
                    {
                        PdfPCell p292e22 = new PdfPCell(new Phrase(new Phrase("" + x.TotalDispatchQty, FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292e22.HorizontalAlignment = 1;
                        p292e22.BackgroundColor = BaseColor.WHITE;
                        //         p292e22.Border = Rectangle.RIGHT_BORDER;

                        table1.AddCell(p292e22);

                    }
                    catch (Exception ee) { }
                    try
                    {
                        PdfPCell p292e22 = new PdfPCell(new Phrase(new Phrase("" + x.TotalBoxQty, FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292e22.HorizontalAlignment = 1;
                        p292e22.BackgroundColor = BaseColor.WHITE;
                        //           p292e22.Border = Rectangle.RIGHT_BORDER;

                        table1.AddCell(p292e22);

                    }
                    catch (Exception ee) { }

                    try
                    {
                        PdfPCell p292e22 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292e22.HorizontalAlignment = 1;
                        p292e22.BackgroundColor = BaseColor.WHITE;
                        //       p292e22.Border = Rectangle.RIGHT_BORDER;
                        p292e22.Colspan = 4;
                        table1.AddCell(p292e22);

                    }
                    catch (Exception ee) { }



                    //  last table

                    try
                    {
                        PdfPCell p292e22 = new PdfPCell(new Phrase(new Phrase("\n\n\n\n                            Prepared By" + "\n                                " + User.Identity.Name, FontFactory.GetFont("Arial", 8, Font.BOLD))));


                        p292e22.HorizontalAlignment = 0;
                        p292e22.BackgroundColor = BaseColor.WHITE;
                        //    p292e22.Border = Rectangle.RIGHT_BORDER;
                        p292e22.Colspan = 7;
                        table1.AddCell(p292e22);

                    }
                    catch (Exception ee) { }

                    try
                    {
                        PdfPCell p292e22 = new PdfPCell(new Phrase(new Phrase("\n\n\n\n\n                                                                                 Received By", FontFactory.GetFont("Arial", 8, Font.BOLD))));


                        p292e22.HorizontalAlignment = 1;
                        p292e22.BackgroundColor = BaseColor.WHITE;
                        //    p292e22.Border = Rectangle.RIGHT_BORDER;
                        p292e22.Colspan = 7;
                        table1.AddCell(p292e22);

                    }
                    catch (Exception ee) { }
                    document.Add(table1);
                    document.NewPage();

                }




                document.Close();

                Session["LabelAWBNO"] = StikerPrintName;
                var result = new { Message = "success", FileName = StikerPrintName };
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception EX)
            {
                //document.Close();
                var result = new { Message = EX.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var result11 = new { Message = "success", };
            return Json(result11, JsonRequestBehavior.AllowGet);
        }

        public JsonResult InvoicePrint(int DispatchID)
        {
            try
            {
                Document document;
                document = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 10f);
                long timeSince1970 = DateTimeOffset.Now.ToUnixTimeSeconds();
                string path = Server.MapPath("~/Attachments/LabelPrints/");
                string filename1 = path + User.Identity.Name + timeSince1970 + ".pdf";
                string StikerPrintName = User.Identity.Name + timeSince1970 + ".pdf";
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filename1, FileMode.Create));
                document.Open();
                var result1 = (from e in db.Order.Where(a => a.DispatchID == DispatchID)

                               join sply in db.MainOrder on e.MasterOrderId equals sply.Id into MainOrder
                               from dr in MainOrder.DefaultIfEmpty()

                               join Cu in db.CustomerMasters on dr.CustomerId equals Cu.ID into CustomerMasters
                               from dp in CustomerMasters.DefaultIfEmpty()

                               join P in db.ProductMasters on e.ProductId equals P.ID into ProductMasters
                               from prd in ProductMasters.DefaultIfEmpty()

                               join Pp in db.ManifestDetails on e.Id equals Pp.OrderNo into ManifestDetails
                               from md in ManifestDetails.DefaultIfEmpty()

                               join dd in db.dispatchDetails on e.DispatchID equals dd.DispatchID into dispachdtl
                               from dsp in dispachdtl.DefaultIfEmpty()

                               select new
                               {
                                   OrderNo = e.Id,
                                   MasterOrderId = e.MasterOrderId,
                                   Product = e.ProductName,
                                   HSN = prd.HSCCode,
                                   Qty = e.Qty,
                                   Rate = e.Rate,
                                   RateIN = e.RateIN,
                                   Amount = e.Amount,
                                   InvoiceNo = e.InvoiceNo,
                                   InvoiceDate = e.InvoiceDate,

                                   DeliveryChallanNo = md.DeliveryChallanNo,

                                   CustName = dp.CustName,
                                   Address = dp.Address,
                                   GSTNumber = dp.GSTNumber,
                                   StateName = dp.State,
                                   City = dp.City,
                                   PinCode = dp.PinCode,
                                   DispatchDate = dsp.DispatchDate,
                                   VendorName = dsp.VendorName,
                                   DriverName = dsp.DriverName,
                                   VehicalNo = dsp.VehicalNo,
                                   MobileNumber = dp.MobileNumber

                               }).ToList();



                var results = result1.Select(a => a.MasterOrderId).Distinct().ToList();

                foreach (var x in results)
                {
                    var BillName = "";
                    for (int i = 1; i <= 3; i++)
                    {

                        if (i == 1)
                        {
                            BillName = "Original Copy";
                        }
                        if (i == 2)
                        {
                            BillName = "Duplicate Copy For Transport";
                        }
                        if (i == 3)
                        {
                            BillName = "Triplicate  Copy For Sale";
                        }
                        var Count = 1;
                        var temp = 1;
                        var data = result1.Where(a => a.MasterOrderId == x).ToList();



                        PdfPTable temptable = new PdfPTable(2);
                        float[] temptablewidth1 = new float[] { 6, 6 };
                        temptable.SetWidths(temptablewidth1);
                        temptable.WidthPercentage = 100f;
                        temptable.DefaultCell.Padding = 1;
                        //1st row
                        try
                        {
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Invoice No : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + data[0].InvoiceNo + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.FixedHeight = 24;
                            tempPC12.HorizontalAlignment = 0;
                            temptable.AddCell(tempPC12);
                        }
                        catch { };

                        try
                        {
                            var dtt = Convert.ToDateTime(data[0].InvoiceDate);
                            var dt = dtt.ToString("dd/MM/yyyy");
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Dated : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + dt + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.HorizontalAlignment = 0;
                            temptable.AddCell(tempPC12);
                        }
                        catch { };

                        //2nd row
                        try
                        {
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Delivery Note : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.FixedHeight = 24;
                            tempPC12.HorizontalAlignment = 0;
                            temptable.AddCell(tempPC12);
                        }
                        catch { };

                        try
                        {

                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Mode/Terms of Payment : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.HorizontalAlignment = 0;
                            temptable.AddCell(tempPC12);
                        }
                        catch { };


                        //3rd row
                        try
                        {
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Reference No & Date : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.FixedHeight = 22;
                            tempPC12.HorizontalAlignment = 0;
                            temptable.AddCell(tempPC12);
                        }
                        catch { };

                        try
                        {

                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Other References : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.HorizontalAlignment = 0;
                            temptable.AddCell(tempPC12);
                        }
                        catch { };






                        //new TAble


                        PdfPTable temptable2 = new PdfPTable(2);
                        float[] temptable2width2 = new float[] { 6, 6 };
                        temptable2.SetWidths(temptable2width2);
                        temptable2.WidthPercentage = 100f;
                        temptable2.DefaultCell.Padding = 1;
                        //1st row
                        try
                        {
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Buyers Order No : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.FixedHeight = 22;
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };

                        try
                        {

                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Dated : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };

                        //2nd row
                        try
                        {
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Dispatch Doc No : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + data[0].DeliveryChallanNo + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.FixedHeight = 22;
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };

                        try
                        {

                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Delivery Note Date : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + data[0].DispatchDate.ToString("dd/MM/yyyy") + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };


                        //3rd row
                        try
                        {
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Dispatch through", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + data[0].VendorName + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.FixedHeight = 22;
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };

                        try
                        {

                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Destination : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + data[0].City + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };


                        //4th row

                        try
                        {
                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Bill of Lading/LR-RR No", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.FixedHeight = 22;
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };

                        try
                        {

                            Paragraph temppr2 = new Paragraph();
                            temppr2.Add(new Phrase("Motor Vehicle No : ", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                            temppr2.Add(new Phrase("\n" + data[0].VehicalNo + "", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            PdfPCell tempPC12 = new PdfPCell(temppr2);
                            tempPC12.HorizontalAlignment = 0;
                            temptable2.AddCell(tempPC12);
                        }
                        catch { };

                        PdfPTable table1 = new PdfPTable(10);
                        float[] width1 = new float[] { 0.1f, 1.5f, 2f, 6f, 2f, 2f, 2f, 2f, 3f, 0.1f };
                        table1.SetWidths(width1);
                        table1.WidthPercentage = 100f;
                        table1.DefaultCell.Padding = 1;


                        Paragraph P11758sd5 = new Paragraph();
                        P11758sd5.Add(new Phrase("Bill of Supply", FontFactory.GetFont("Arial", 11, Font.BOLD)));
                        PdfPCell PC12885d = new PdfPCell(P11758sd5);
                        PC12885d.Colspan = 5;
                        PC12885d.FixedHeight = 30;
                        PC12885d.Border = Rectangle.NO_BORDER;
                        PC12885d.HorizontalAlignment = 2;
                        table1.AddCell(PC12885d);


                        Paragraph P117584sd5 = new Paragraph();
                        P117584sd5.Add(new Phrase("" + BillName, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                        PdfPCell PC12885sd = new PdfPCell(P117584sd5);
                        PC12885sd.Colspan = 5;
                        PC12885sd.FixedHeight = 30;
                        PC12885sd.Border = Rectangle.NO_BORDER;
                        PC12885sd.HorizontalAlignment = 2;
                        table1.AddCell(PC12885sd);




                        string barcode1 = Server.MapPath("~/dist/img/logo.png");
                        iTextSharp.text.Image barcodejpg1 = iTextSharp.text.Image.GetInstance(barcode1);
                        barcodejpg1.ScaleToFit(85, 45);
                        barcodejpg1.SpacingBefore = 5f;
                        barcodejpg1.SpacingAfter = 1f;
                        barcodejpg1.Alignment = Element.ALIGN_LEFT;

                        Paragraph P11758555 = new Paragraph();
                        P11758555.Add(new Phrase("", FontFactory.GetFont("Arial", 11, Font.BOLD)));
                        P11758555.Add(new Chunk(barcodejpg1, 0, 0, true));
                        PdfPCell prr32987456 = new PdfPCell(P11758555);
                        prr32987456.HorizontalAlignment = 1;
                        prr32987456.Colspan = 3;
                        table1.AddCell(prr32987456);


                        Paragraph pr1 = new Paragraph();

                        pr1.Add(new Phrase("MANEGROW AGRO PRODUCTS ", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                        pr1.Add(new Phrase("\n", FontFactory.GetFont("Arial", 7, Font.BOLD)));

                        pr1.Add(new Phrase("Gate No 379, At Post Bebad Ohol, Tal.Maval, Dist Pune \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                        pr1.Add(new Phrase("FSSAI-100200283000197 \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                        pr1.Add(new Phrase("GSTIN-27ABNFM3115G1ZB \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                        pr1.Add(new Phrase("State Name : Maharashtra, Code : 27 \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                        pr1.Add(new Phrase("Email : info@manegrow.com \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));


                        PdfPCell PC111 = new PdfPCell(pr1);
                        PC111.HorizontalAlignment = 0;
                        table1.AddCell(PC111);

                        PdfPCell PC12 = new PdfPCell(temptable);
                        PC12.Colspan = 6;
                        PC12.HorizontalAlignment = 0;
                        table1.AddCell(PC12);


                        //2nd row

                        Paragraph pr4 = new Paragraph();
                        pr4.Add(new Phrase("Consignee (Ship To) :\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr4.Add(new Phrase("" + data[0].CustName + "\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                        pr4.Add(new Phrase("" + data[0].Address + " " + data[0].City + " " + data[0].PinCode + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr4.Add(new Phrase("GSTIN/UIN : " + data[0].GSTNumber + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr4.Add(new Phrase("State : " + data[0].StateName + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr4.Add(new Phrase("Mobile : " + data[0].MobileNumber + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));


                        PdfPCell PC133 = new PdfPCell(pr4);
                        PC133.Colspan = 4;
                        PC133.HorizontalAlignment = 0;
                        table1.AddCell(PC133);


                        PdfPCell PC1248 = new PdfPCell(temptable2);
                        PC1248.Colspan = 6;
                        PC1248.HorizontalAlignment = 0;
                        table1.AddCell(PC1248);



                        //3nd row

                        Paragraph pr444 = new Paragraph();
                        pr444.Add(new Phrase("Buyer (Bill To) :\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr444.Add(new Phrase("" + data[0].CustName + "\n", FontFactory.GetFont("Arial", 10, Font.BOLD)));
                        pr444.Add(new Phrase("" + data[0].Address + " " + data[0].City + " " + data[0].PinCode + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr444.Add(new Phrase("GSTIN/UIN : " + data[0].GSTNumber + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr444.Add(new Phrase("State : " + data[0].StateName + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        pr444.Add(new Phrase("Mobile : " + data[0].MobileNumber + "\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));

                        PdfPCell PC1333 = new PdfPCell(pr444);
                        PC1333.Colspan = 4;
                        PC1333.HorizontalAlignment = 0;
                        table1.AddCell(PC1333);


                        Paragraph pr4444 = new Paragraph();
                        pr4444.Add(new Phrase("Terms of Delivery\n", FontFactory.GetFont("Arial", 10, Font.NORMAL)));
                        PdfPCell PC12488 = new PdfPCell(pr4444);
                        PC12488.Colspan = 6;
                        PC12488.FixedHeight = 70;
                        PC12488.HorizontalAlignment = 0;
                        table1.AddCell(PC12488);


                        try
                        {


                            PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("Sr No", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p1.HorizontalAlignment = 1;
                            p1.BackgroundColor = BaseColor.WHITE;
                            p1.FixedHeight = 25;
                            p1.Colspan = 2;

                            //    p1.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p1);

                            PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("Description of Goods", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p2dd.HorizontalAlignment = 1;
                            p2dd.BackgroundColor = BaseColor.WHITE;
                            p2dd.Colspan = 2;
                            //      p2dd.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p2dd);

                            PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("HSN/SAC", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p2.HorizontalAlignment = 1;
                            p2.BackgroundColor = BaseColor.WHITE;
                            //        p2.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p2);

                            PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("Quantity", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp5.HorizontalAlignment = 1;
                            pp5.BackgroundColor = BaseColor.WHITE;
                            //        pp5.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp5);

                            PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("Rate", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp55.HorizontalAlignment = 1;
                            pp55.BackgroundColor = BaseColor.WHITE;
                            //        pp55.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp55);

                            PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("Per", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p29.HorizontalAlignment = 1;
                            p29.BackgroundColor = BaseColor.WHITE;
                            //         p29.Border = Rectangle.BOTTOM_BORDER;
                            //    p29.Colspan = 2;
                            table1.AddCell(p29);

                            PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("Amount", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp51.HorizontalAlignment = 2;
                            pp51.BackgroundColor = BaseColor.WHITE;
                            //           pp51.Border = Rectangle.BOTTOM_BORDER;
                            pp51.Colspan = 2;
                            table1.AddCell(pp51);


                        }
                        catch
                        {

                        }
                        var cntt = 0;
                        int TotalQty = 0;
                        decimal TotalAMt = 0;
                        var AmtInwords = "";
                        foreach (var xx in data)
                        {

                            cntt++;

                            try
                            {


                                PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("" + cntt, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p1.HorizontalAlignment = 1;
                                p1.BackgroundColor = BaseColor.WHITE;
                                p1.FixedHeight = 30;
                                p1.Colspan = 2;
                                //    p1.Border = Rectangle.BOTTOM_BORDER;
                                table1.AddCell(p1);

                                PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("" + xx.Product, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p2dd.HorizontalAlignment = 0;
                                p2dd.BackgroundColor = BaseColor.WHITE;
                                p2dd.Colspan = 2;
                                //      p2dd.Border = Rectangle.BOTTOM_BORDER;
                                table1.AddCell(p2dd);

                                PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("" + xx.HSN, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p2.HorizontalAlignment = 1;
                                p2.BackgroundColor = BaseColor.WHITE;
                                //        p2.Border = Rectangle.BOTTOM_BORDER;
                                table1.AddCell(p2);

                                PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("" + xx.Qty, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp5.HorizontalAlignment = 1;
                                pp5.BackgroundColor = BaseColor.WHITE;
                                //        pp5.Border = Rectangle.BOTTOM_BORDER;
                                table1.AddCell(pp5);

                                TotalQty = TotalQty + Convert.ToInt32(xx.Qty);

                                PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("" + xx.Rate, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp55.HorizontalAlignment = 1;
                                pp55.BackgroundColor = BaseColor.WHITE;
                                //        pp55.Border = Rectangle.BOTTOM_BORDER;
                                table1.AddCell(pp55);

                                PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("" + xx.RateIN, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p29.HorizontalAlignment = 1;
                                p29.BackgroundColor = BaseColor.WHITE;
                                //         p29.Border = Rectangle.BOTTOM_BORDER;
                                //    p29.Colspan = 2;
                                table1.AddCell(p29);

                                PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("" + xx.Amount, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp51.HorizontalAlignment = 2;
                                pp51.BackgroundColor = BaseColor.WHITE;
                                //           pp51.Border = Rectangle.BOTTOM_BORDER;
                                pp51.Colspan = 2;
                                table1.AddCell(pp51);
                                TotalAMt = TotalAMt + Convert.ToDecimal(xx.Amount);

                            }
                            catch
                            {

                            }
                            try
                            {
                                if (data.Count == cntt)
                                {
                                    var colss = 0;
                                    if (cntt == 1) { colss = 260; };
                                    if (cntt == 2) { colss = 230; };
                                    if (cntt == 3) { colss = 200; };
                                    if (cntt == 4) { colss = 170; };
                                    if (cntt == 5) { colss = 140; };
                                    if (cntt == 6) { colss = 110; };
                                    if (cntt == 7) { colss = 80; };
                                    if (cntt == 8) { colss = 50; };
                                    if (cntt == 9) { colss = 20; };
                                    //   if (cntt >8) { colss = 0; };

                                    try
                                    {
                                        PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p1.HorizontalAlignment = 1;
                                        p1.BackgroundColor = BaseColor.WHITE;
                                        p1.FixedHeight = colss;
                                        p1.Colspan = 2;
                                        //    p1.Border = Rectangle.BOTTOM_BORDER;
                                        table1.AddCell(p1);

                                        PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p2dd.HorizontalAlignment = 0;
                                        p2dd.BackgroundColor = BaseColor.WHITE;
                                        p2dd.Colspan = 2;
                                        //      p2dd.Border = Rectangle.BOTTOM_BORDER;
                                        table1.AddCell(p2dd);

                                        PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p2.HorizontalAlignment = 1;
                                        p2.BackgroundColor = BaseColor.WHITE;
                                        //        p2.Border = Rectangle.BOTTOM_BORDER;
                                        table1.AddCell(p2);

                                        PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        pp5.HorizontalAlignment = 1;
                                        pp5.BackgroundColor = BaseColor.WHITE;
                                        //        pp5.Border = Rectangle.BOTTOM_BORDER;
                                        table1.AddCell(pp5);

                                        PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        pp55.HorizontalAlignment = 1;
                                        pp55.BackgroundColor = BaseColor.WHITE;
                                        //        pp55.Border = Rectangle.BOTTOM_BORDER;
                                        table1.AddCell(pp55);

                                        PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p29.HorizontalAlignment = 1;
                                        p29.BackgroundColor = BaseColor.WHITE;
                                        //         p29.Border = Rectangle.BOTTOM_BORDER;
                                        //    p29.Colspan = 2;
                                        table1.AddCell(p29);

                                        PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        pp51.HorizontalAlignment = 1;
                                        pp51.BackgroundColor = BaseColor.WHITE;
                                        //           pp51.Border = Rectangle.BOTTOM_BORDER;
                                        pp51.Colspan = 2;
                                        table1.AddCell(pp51);

                                    }
                                    catch { };
                                }
                            }
                            catch
                            {

                            }
                        }

                        try
                        {


                            PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("Total", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p1.HorizontalAlignment = 2;
                            p1.BackgroundColor = BaseColor.WHITE;
                            p1.FixedHeight = 20;
                            p1.Colspan = 4;
                            table1.AddCell(p1);


                            PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p2.HorizontalAlignment = 1;
                            p2.BackgroundColor = BaseColor.WHITE;
                            //        p2.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p2);

                            PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("" + TotalQty, FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp5.HorizontalAlignment = 1;
                            pp5.BackgroundColor = BaseColor.WHITE;
                            //        pp5.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp5);

                            PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp55.HorizontalAlignment = 1;
                            pp55.BackgroundColor = BaseColor.WHITE;
                            //        pp55.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp55);

                            PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p29.HorizontalAlignment = 1;
                            p29.BackgroundColor = BaseColor.WHITE;
                            //         p29.Border = Rectangle.BOTTOM_BORDER;
                            //    p29.Colspan = 2;
                            table1.AddCell(p29);
                            var ttotalamt = Math.Round(TotalAMt);
                            AmtInwords = words(Convert.ToInt32(ttotalamt));
                            PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("" + ttotalamt + ".00", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp51.HorizontalAlignment = 2;
                            pp51.BackgroundColor = BaseColor.WHITE;
                            //           pp51.Border = Rectangle.BOTTOM_BORDER;
                            pp51.Colspan = 2;
                            table1.AddCell(pp51);


                        }
                        catch
                        {

                        }
                        try
                        {

                            Paragraph pr44444 = new Paragraph();
                            pr44444.Add(new Phrase("Amount In Word : \n", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                            pr44444.Add(new Phrase("" + AmtInwords, FontFactory.GetFont("Arial", 8, Font.NORMAL)));

                            pr44444.Add(new Phrase("\n\nCompany Name : Manegrow Agro Products Private Limited\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            pr44444.Add(new Phrase("Bank Name: Axis Bank Ltd\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            pr44444.Add(new Phrase("Branch: CBB, Pune\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            pr44444.Add(new Phrase("Account Number: 922030055613057\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));
                            pr44444.Add(new Phrase("IFSC Code: UTIB0001636\n", FontFactory.GetFont("Arial", 9, Font.BOLD)));

                            pr44444.Add(new Phrase("\nCompanys PAN : ", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                            pr44444.Add(new Phrase("AAPCM5068G  \nDeclaration\n---------------", FontFactory.GetFont("Arial", 8, Font.BOLD)));
                            PdfPCell PC124888 = new PdfPCell(pr44444);
                            PC124888.Colspan = 8;
                            PC124888.Border = Rectangle.LEFT_BORDER;

                            PC124888.HorizontalAlignment = 0;
                            table1.AddCell(PC124888);

                            Paragraph pr444445 = new Paragraph();
                            pr444445.Add(new Phrase("E. & O.E \n", FontFactory.GetFont("Arial", 8, Font.NORMAL)));

                            PdfPCell PC1288 = new PdfPCell(pr444445);
                            PC1288.Colspan = 2;
                            PC1288.Border = Rectangle.RIGHT_BORDER;
                            PC1288.HorizontalAlignment = 2;
                            table1.AddCell(PC1288);
                        }
                        catch { }

                        try
                        {
                            Paragraph pr44444 = new Paragraph();

                            pr44444.Add(new Phrase("1:- Exempted goods notified under section 11(1) 02/2017 of GST act,Under serial no: 43.As perd Notification No.02/2017 dated 28/06/2017\n", FontFactory.GetFont("Arial", 7, Font.NORMAL)));
                            pr44444.Add(new Phrase("2:- No E-way bill required for exempted goods transported as per Notification No 27/2017 of GST rule 138(14).Annexure bearing Serial No. 43\n", FontFactory.GetFont("Arial", 7, Font.NORMAL)));
                            pr44444.Add(new Phrase("3:- We declare that this invoice shows the actual price of the goods described and that all particulars are true and correct.", FontFactory.GetFont("Arial", 7, Font.NORMAL)));
                            PdfPCell PC124888 = new PdfPCell(pr44444);
                            PC124888.Colspan = 4;
                            PC124888.Border = Rectangle.LEFT_BORDER;

                            PC124888.HorizontalAlignment = 0;
                            table1.AddCell(PC124888);

                            Paragraph pr444445 = new Paragraph();
                            pr444445.Add(new Phrase("For MANEGROW AGRO PRODUCTS \n\n\n\n\n Authorised Signatury", FontFactory.GetFont("Arial", 8, Font.NORMAL)));

                            PdfPCell PC1288 = new PdfPCell(pr444445);
                            PC1288.Colspan = 6;
                            //   PC1288.Border = Rectangle.RIGHT_BORDER;
                            PC1288.HorizontalAlignment = 2;
                            table1.AddCell(PC1288);
                        }
                        catch { }

                        PdfPCell PC12885 = new PdfPCell();
                        PC12885.Colspan = 10;
                        PC12885.Border = Rectangle.TOP_BORDER;
                        PC12885.HorizontalAlignment = 2;
                        table1.AddCell(PC12885);

                        Paragraph pr44444d5 = new Paragraph();
                        pr44444d5.Add(new Phrase("SUBJECT TO PUNE JURISDICTION\n\n", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                        pr44444d5.Add(new Phrase("This is a Computer Genrated Invoice", FontFactory.GetFont("Arial", 8, Font.NORMAL)));

                        PdfPCell PC12s88 = new PdfPCell(pr44444d5);
                        PC12s88.Colspan = 10;
                        PC12s88.Border = Rectangle.NO_BORDER;
                        PC12s88.HorizontalAlignment = 1;
                        table1.AddCell(PC12s88);
                        document.Add(table1);
                        document.NewPage();

                    }
                }
                document.Close();

                Session["LabelAWBNO"] = StikerPrintName;
                var result = new { Message = "success", FileName = StikerPrintName };
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception EX)
            {
                //document.Close();
                var result = new { Message = EX.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            var result11 = new { Message = "success", };
            return Json(result11, JsonRequestBehavior.AllowGet);
        }


        public string words(int numbers)
        {
            int number = numbers;

            if (number == 0) return "Zero";
            if (number == -2147483648) return "Minus Two Hundred and Fourteen Crore Seventy Four Lakh Eighty Three Thousand Six Hundred and Forty Eight";
            int[] num = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (number < 0)
            {
                sb.Append("Minus ");
                number = -number;
            }
            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
       "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
      "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
       "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };
            num[0] = number % 1000; // units
            num[1] = number / 1000;
            num[2] = number / 100000;
            num[1] = num[1] - 100 * num[2]; // thousands
            num[3] = number / 10000000; // crores
            num[2] = num[2] - 100 * num[3]; // lakhs
            for (int i = 3; i > 0; i--)
            {
                if (num[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (num[i] == 0) continue;
                u = num[i] % 10; // ones
                t = num[i] / 10;
                h = num[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }


        [HttpPost]
        public ActionResult UploadFile()
        {
            var Ext = "";
            var FileNM = "";
            var ID = Request.Form["ID"].ToString();
            HttpFileCollectionBase files = Request.Files;

            List<FileName> list = new List<FileName>();
            var duplicate = "";
            for (int i = 0; i < files.Count; i++)
            {
                FileName F = new FileName();
                var fname = "";
                HttpPostedFileBase file = files[i];
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                    Ext = file.ContentType;
                }

                FileNM = ID + "_" + fname;

                var FilName = System.IO.Path.Combine(Server.MapPath("~/Attachments/ExpenceBill/"), FileNM);
                if (System.IO.File.Exists(FilName))
                {
                    duplicate = duplicate + " , " + fname;
                }
                else
                {

                    var PAth = Path.Combine(Server.MapPath("~/Attachments/ExpenceBill/"), fname);
                    file.SaveAs(PAth);

                    System.IO.FileInfo fi = new System.IO.FileInfo(PAth);
                    if (fi.Exists)
                    {
                        var PAths = Path.Combine(Server.MapPath("~/Attachments/ExpenceBill/"), FileNM);
                        fi.MoveTo(PAths);
                    }
                    F.Name = FileNM;
                    list.Add(F);
                }
            }
            var result = new { Message = "success", list = list, DuplicateImg = duplicate, FileNM };
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        // Edit PAge
        public JsonResult GetEditDispatchCustomers(int? Root, string Date, int ID)
        {
            //var orders = db.Order.Where(a => a.DispatchID == ID).Select(a => a.MasterOrderId).Distinct().ToList();
            //var mainorderCustomerID = db.MainOrder.Where(a => orders.Contains(a.Id)).Select(a => a.CustomerId).ToList();
            //var WithOrder = db.CustomerMasters.Where(a => mainorderCustomerID.Contains(a.ID)).ToList();
            //var WithoutOrder = WithOrder.Where(a => a.CustName == "jhdfgjgdjhsdgfhjfd").ToList();
            //var result = new { WithOrder , WithoutOrder };



            try
            {
                var rt = Root;
                var CustID = db.SubRouteCustomers.Where(a => a.RouteMainID == Root && a.CustomerID != null).Select(a => a.CustomerID).Distinct().ToList();
                var dt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var result1 = (from e in db.Order.Where(a => a.DispatchID == ID)

                               join sply in db.MainOrder on e.MasterOrderId equals sply.Id into MainOrder
                               from dr in MainOrder.DefaultIfEmpty()

                               join Cu in db.CustomerMasters on dr.CustomerId equals Cu.ID into CustomerMasters
                               from dp in CustomerMasters.DefaultIfEmpty()

                               join pk in db.PackingDetails on e.Id equals pk.OrderNo into PackingDetails
                               from pd in PackingDetails.DefaultIfEmpty()
                               select new
                               {

                                   ID = e.Id,
                                   Area = dp.Area,
                                   City = dp.City,
                                   //   SubRouteID = src == null ? 0 : src.RouteDetailsID,
                                   CustName = dp == null ? String.Empty : dp.CustName,
                                   //    RootName = srd == null ? String.Empty : srd.SubRouteName,
                                   //   RouteMainID = src == null ? 0 : src.RouteMainID,
                                   //  RouteMainName = srd == null ? string.Empty : srd.RouteMainName,

                                   CustID = dp == null ? 0 : dp.ID,
                                   NoOfBox = pd == null ? 0 : pd.NoOfBox,
                                   TotalWeight = pd == null ? 0 : pd.TotalWeight,


                               } into p
                               group p by new { p.CustID, p.CustName, p.ID } into gcs
                               select new
                               {


                                   ID = gcs.Key.ID,
                                   Area = gcs.Select(a => a.Area).Distinct(),
                                   City = gcs.Select(a => a.City).Distinct(),
                                   CustName = gcs.Key.CustName,


                                   CustID = gcs.Key.CustID,
                                   NoOfBox = gcs.Sum(x => x.NoOfBox),
                                   TotalWeight = gcs.Sum(x => x.TotalWeight),

                               }).ToList();


                var WithOrder = result1.Where(a => CustID.Contains(a.CustID)).ToList();
                var WithoutOrder = result1.Where(a => !CustID.Contains(a.CustID)).ToList();

                var result = new { WithOrder, WithoutOrder };
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {

                var result = new { };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize("You dont have access to Delete Dispatch", "DispatchEdit, Admin")]
        public JsonResult DeleteDispatchedEntry(int ID)
        {
            var DispatchID = Convert.ToInt32(ID);

            var xx = db.Order.Where(a => a.DispatchID == ID).Select(a =>new { a.Id ,a.PODName}).ToList();
            foreach (var x in xx)
            {
                if(x.PODName !=null)
                {
                    var results = new { Message = "You Cannot delete dispatch because POD Already Updated with Order No "+x.Id };
                    return Json(results, JsonRequestBehavior.AllowGet); 
                }
                var ordern = db.Order.Where(a => a.Id == x.Id).FirstOrDefault();                
                ordern.DispatchID = null;
                ordern.OrderStage = 2;
                ordern.OrderStatus = "Packing Completed";
                ordern.InvoiceNo = null;
                ordern.InvoiceDate = null;
                ordern.Rate = null;
                ordern.RateIN = null;
                ordern.Amount = null;

               
                    TrackingDetails track = new TrackingDetails();
                    track.Status = "Dispatch Entry Deleted by User " + User.Identity.Name; ;
                    track.Action = "Dispatch Details";
                    track.Location = "Plant";
                    track.OrderNo = x.Id;
                    track.Date = DateTime.Now;
                    track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                    track.CreatedDate = DateTime.Now;
                    track.CreatedBy = User.Identity.Name;
                    db.trackingDetails.Add(track);               
            }


            var manifestmain = db.ManifestMains.Where(a => a.DispatchID == ID).ToList();
            db.ManifestMains.RemoveRange(manifestmain);

            var manifestdtl = db.ManifestDetails.Where(a => a.DispatchID == ID).ToList();
            db.ManifestDetails.RemoveRange(manifestdtl);

            var disptrans = db.DispatchTransactions.Where(a => a.DispatchID == ID).ToList();
            db.DispatchTransactions.RemoveRange(disptrans);
           

            var disp = db.dispatchDetails.Where(a => a.DispatchID == ID).ToList(); 
            db.dispatchDetails.RemoveRange(disp); 
            db.SaveChanges();

            var result = new {Message="success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Update(List<MainOrder> Cust, string Date, int Route, string DriverName, string VendorType, string VendorName, string VehicalNo, int TotalBoxes, decimal TotalWT, int? ID, int? VendorID, string RouteName, string strDelete)
        {
            try
            {
                var DispatchID = Convert.ToInt32(ID);
                var transaction = new List<DispatchTransactions>();
                var expdt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                //var invv = "";
                //var drsno = "";
                //  xcx

                var drsno = "";

                var Ordernoss = Cust.Select(a => a.OrderNo).ToList();
                var Ordr = db.Order.Where(a => !Ordernoss.Contains(a.Id) && a.DispatchID == ID).Select(a => a.Id).ToList();


                var manifestmain = db.ManifestMains.Where(a => a.DispatchID == ID).ToList();
                var drssno = manifestmain[0].DeliveryChallanNo;

                drsno= string.Join("", drssno.ToCharArray().Where(Char.IsDigit));
                drsno= drsno + "(-Edited)";


                db.ManifestMains.RemoveRange(manifestmain);


                var manifestdtl = db.ManifestDetails.Where(a => a.DispatchID == ID).ToList();
                db.ManifestDetails.RemoveRange(manifestdtl);

                var disptrans = db.DispatchTransactions.Where(a => a.DispatchID == ID).ToList();
                db.DispatchTransactions.RemoveRange(disptrans);

                foreach (var x in Ordr)
                {

                    var ordern = db.Order.Where(a => a.Id == x).FirstOrDefault();
                  //  invv = ordern.InvoiceNo;
                    ordern.DispatchID = null;
                    ordern.OrderStage = 2;
                    ordern.OrderStatus = "Packing Completed";
                    ordern.InvoiceNo = null;
                    ordern.InvoiceDate = null;
                    ordern.Rate = null;
                    ordern.RateIN = null;
                    ordern.Amount = null;

              
                        TrackingDetails track = new TrackingDetails();
                        track.Status = "Order Removed From Dispatch by User " + User.Identity.Name; ;
                        track.Action = "Dispatch Details";
                        track.Location = "Plant";
                        track.OrderNo = x;
                        track.Date = DateTime.Now;
                        track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                        track.CreatedDate = DateTime.Now;
                        track.CreatedBy = User.Identity.Name;
                        db.trackingDetails.Add(track);
                    

                }
               

                    db.SaveChanges();
                    using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                    {

                        var dispatchdtl = db.dispatchDetails.Where(a => a.DispatchID == ID).FirstOrDefault();
                        dispatchdtl.TotalWeight = TotalWT;
                        dispatchdtl.NoofBoxes = TotalBoxes;
                        dispatchdtl.UpdatedBy = User.Identity.Name;
                        dispatchdtl.UpdatedDate = DateTime.Today;

                        var CustDetails = Cust.Select(a => a.CustomerId).Distinct().ToList();
                        var GroupManifestss = db.ManifestGroups.Where(a => a.RouteID == Route).ToList();
                        foreach (var x in CustDetails)
                        {

                            var CustDetailss = Cust.Where(a => a.CustomerId == x)
                                .GroupBy(_ => _.CustomerId, _ => new { TotalWT = _.TotalWT, TotalBoxes = _.TotalBoxes, CustomerId = _.CustomerId })
                                .Select(g => new
                                {
                                    CustomerId = g.Select(a => a.CustomerId),
                                    TotalWT = g.Sum(f => f.TotalWT).ToString(),
                                    TotalBoxes = g.Sum(f => f.TotalBoxes).ToString(),
                                }).FirstOrDefault();


                            var custID = x;
                            var Custdata = db.SubRouteCustomers.Where(a => a.CustomerID == custID).Select(a => a.RouteDetailsID).Distinct().ToList();
                            if (Custdata.Count == 0)
                            {
                                var cst = db.CustomerMasters.Where(a => a.ID == custID).FirstOrDefault();
                                var resultss = new { Message = "Route Not Assigned For " + cst.CustName };
                                return Json(resultss, JsonRequestBehavior.AllowGet);
                            }

                            var caunt = GroupManifestss.Where(a => a.CustomerID == x).Count();
                            if (caunt == 0)
                            {
                                var cst = db.CustomerMasters.Where(a => a.ID == custID).FirstOrDefault();
                                var resultsss = new { Message = "Group Not Assigned For " + cst.CustName };
                                return Json(resultsss, JsonRequestBehavior.AllowGet);
                            }

                            var TotalWTT = CustDetailss.TotalWT;
                            var TotalBoxess = CustDetailss.TotalBoxes;


                            //   List<DispatchTransactions> dtc = new List<DispatchTransactions>();
                            foreach (var dtlid in Custdata)
                            {
                                var rdd = db.RouteDetails.Where(a => a.ID == dtlid).FirstOrDefault();
                                var transCount = transaction.Where(a => a.VendorID == rdd.VendorID && a.Date == expdt).Count();
                                if (transCount == 0)
                                {
                                    var trans = new DispatchTransactions();
                                    trans.DispatchID = Convert.ToInt32(DispatchID);
                                    trans.LocationFrom = rdd.FromRoute;
                                    trans.LocationTo = rdd.ToRoute;
                                    trans.VendorID = Convert.ToInt32(rdd.VendorID);
                                    trans.VendorName = rdd.VendorName;
                                    trans.Date = expdt;
                                    trans.LoadINKG = Convert.ToDecimal(TotalWTT);
                                    trans.NoOfBoxes = Convert.ToInt32(TotalBoxess);
                                    trans.RateType = rdd.RateType;
                                    decimal AMT = 0;
                                    trans.RouteName = RouteName;

                                    if (rdd.RateType == "Trip" || rdd.RateType == "TRIP")
                                    {

                                        trans.Rate = rdd.Rate;
                                        trans.Amount = Convert.ToDecimal(rdd.Unit) * Convert.ToInt32(rdd.Rate);
                                    }
                                    if (rdd.RateType == "WEIGHT")
                                    {
                                        // AMT = rdd.Rate * Convert.ToDecimal(TotalWTT);

                                        if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom1 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                        {
                                            trans.Rate = rdd.Rate1;
                                            trans.Amount = Convert.ToDecimal(rdd.Rate1) * Convert.ToDecimal(TotalWTT);
                                        }
                                        if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom2 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                        {
                                            trans.Rate = rdd.Rate2;
                                            trans.Amount = Convert.ToDecimal(rdd.Rate2) * Convert.ToDecimal(TotalWTT);
                                        }
                                        if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom3 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                        {
                                            trans.Rate = rdd.Rate3;
                                            trans.Amount = Convert.ToDecimal(rdd.Rate3) * Convert.ToDecimal(TotalWTT);
                                        }

                                        if (Convert.ToDecimal(TotalWTT) > rdd.WTTo3 && rdd.Rate != 0)
                                        {
                                            trans.Rate = rdd.Rate;
                                            trans.Amount = Convert.ToDecimal(rdd.Rate) * Convert.ToDecimal(TotalWTT);
                                        }

                                    }
                                    if (rdd.RateType == "BOX")
                                    {

                                        if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom1 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                        {
                                            trans.Rate = rdd.Rate1;
                                            trans.Amount = Convert.ToDecimal(rdd.Rate1) * Convert.ToInt32(TotalBoxess);
                                        }
                                        else
                                        if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom2 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                        {
                                            trans.Rate = rdd.Rate2;
                                            trans.Amount = Convert.ToDecimal(rdd.Rate2) * Convert.ToInt32(TotalBoxess);
                                        }
                                        else
                                        if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom3 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                        {
                                            trans.Rate = rdd.Rate3;
                                            trans.Amount = Convert.ToDecimal(rdd.Rate3) * Convert.ToInt32(TotalBoxess);
                                        }
                                        else
                                        if ((Convert.ToInt32(TotalBoxess) > rdd.WTTo3 || rdd.WTTo3 == null) && rdd.Rate != 0)
                                        {
                                            trans.Rate = rdd.Rate;
                                            trans.Amount = rdd.Rate * Convert.ToInt32(TotalBoxess);
                                        }
                                    }
                                    if (rdd.RateType == "PER KM")
                                    {

                                        trans.Rate = rdd.Rate;
                                        //trans.Amount = Convert.ToDecimal(rdd.Unit) * Convert.ToInt32(rdd.Rate);

                                    }
                                    if (rdd.RateType == "UP TO KM")
                                    {

                                    }

                                    transaction.Add(trans);

                                }
                                else
                                {
                                    var trans = transaction.Where(a => a.VendorID == rdd.VendorID && a.Date == expdt).FirstOrDefault();

                                    trans.LoadINKG = trans.LoadINKG + Convert.ToDecimal(TotalWTT);
                                    trans.NoOfBoxes = trans.NoOfBoxes + Convert.ToInt32(TotalBoxess);
                                    trans.RateType = rdd.RateType;
                                    trans.CreatedBy = User.Identity.Name;
                                    trans.CreatedDate = DateTime.Today;

                                    if (rdd.RateType == "Trip" || rdd.RateType == "TRIP")
                                    {

                                        trans.Rate = rdd.Rate;
                                        trans.Amount = Convert.ToDecimal(rdd.Unit) * Convert.ToInt32(rdd.Rate);
                                    }
                                    if (rdd.RateType == "WEIGHT")
                                    {
                                        if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom1 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                        {
                                            trans.Rate = rdd.Rate1;
                                            trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate1) * Convert.ToDecimal(TotalWTT));
                                        }
                                        if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom2 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                        {
                                            trans.Rate = rdd.Rate2;
                                            trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate2) * Convert.ToDecimal(TotalWTT));
                                        }
                                        if (Convert.ToDecimal(TotalWTT) > rdd.WTFrom3 && Convert.ToDecimal(TotalWTT) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                        {
                                            trans.Rate = rdd.Rate3;
                                            trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate3) * Convert.ToDecimal(TotalWTT));
                                        }

                                        if ((Convert.ToDecimal(TotalWTT) > rdd.WTTo3 || rdd.WTTo3 == null) && rdd.Rate != 0)
                                        {
                                            trans.Rate = rdd.Rate;
                                            trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate) * Convert.ToDecimal(TotalWTT));
                                        }

                                    }
                                    if (rdd.RateType == "BOX")
                                    {

                                        if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom1 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo1 && rdd.Rate1 != 0)
                                        {
                                            trans.Rate = rdd.Rate1;
                                            trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate1) * Convert.ToInt32(TotalBoxess));
                                        }
                                        if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom2 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo2 && rdd.Rate2 != 0)
                                        {
                                            trans.Rate = rdd.Rate2;
                                            trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate2) * Convert.ToInt32(TotalBoxess));
                                        }
                                        if (Convert.ToInt32(TotalBoxess) > rdd.WTFrom3 && Convert.ToInt32(TotalBoxess) <= rdd.WTTo3 && rdd.Rate3 != 0)
                                        {
                                            trans.Rate = rdd.Rate3;
                                            trans.Amount = trans.Amount + (Convert.ToDecimal(rdd.Rate3) * Convert.ToInt32(TotalBoxess));
                                        }

                                        if (Convert.ToInt32(TotalBoxess) > rdd.WTTo3 && rdd.Rate != 0)
                                        {
                                            trans.Rate = rdd.Rate;
                                            trans.Amount = trans.Amount + (rdd.Rate * Convert.ToInt32(TotalBoxess));
                                        }
                                    }

                                    if (rdd.RateType == "PER KM")
                                    {
                                        trans.Rate = rdd.Rate;
                                    }
                                    if (rdd.RateType == "UP TO KM")
                                    {

                                    }

                                }
                            }


                            var lstt = Cust.Select(a => a.OrderNo).ToList();


                            var OrderMainID = db.Order.Where(x1 => lstt.Contains(x1.Id)).Select(a => a.MasterOrderId).Distinct().ToList();
                            foreach (var xx in OrderMainID)
                            {
                                //var InvNo = db.TableNumbring.Where(a => a.TableName == "InvoiceNo").FirstOrDefault();
                                //var invv = InvNo.Prefix + "" + InvNo.SerialNo;
                                //InvNo.SerialNo = InvNo.SerialNo + 1;

                                var dbdata = (from om in db.MainOrder.Where(a => a.Id == xx)
                                              join cm in db.CustomerMasters on om.CustomerId equals cm.ID
                                              join qd in db.QuotationDetails on cm.QuotationID equals qd.MainID
                                              select new
                                              {
                                                  ProductName = qd.ProductName,
                                                  ProductID = qd.ProductID,
                                                  UOM = qd.UOM,
                                                  WTFrom1 = qd.WTFrom1,
                                                  WTTo1 = qd.WTTo1,
                                                  Rate1 = qd.Rate1,
                                                  WTFrom2 = qd.WTFrom2,
                                                  WTTo2 = qd.WTTo2,
                                                  Rate2 = qd.Rate2,

                                                  WTFrom3 = qd.WTFrom3,
                                                  WTTo3 = qd.WTTo3,
                                                  Rate3 = qd.Rate3,
                                                  AddWt = qd.AddWt,
                                                  AddRate = qd.AddRate,

                                              }).ToList();

                                //                           
                                var Orderid = db.Order.Where(a => a.MasterOrderId == xx && lstt.Contains(a.Id)).ToList();
                                foreach (var o in Orderid)
                                {
                                    var orderdtl = db.Order.Where(a => a.Id == o.Id).FirstOrDefault();

                                  //  orderdtl.InvoiceNo = invv;
                                    orderdtl.InvoiceDate = expdt;
                                    orderdtl.OrderStatus = "Order Dispatched";
                                    orderdtl.DispatchID = DispatchID;
                                    orderdtl.OrderStage = 8;


                                    var dbdatanew = dbdata.Where(a => a.ProductID == orderdtl.ProductId).FirstOrDefault();
                                    if (dbdatanew == null)
                                    {



                                        var resultss = new { Message = "Quotation Not Found For Order No=" + orderdtl.Id };
                                        return Json(resultss, JsonRequestBehavior.AllowGet);
                                    }
                                    else
                                    {

                                        if (dbdatanew.UOM == "BOX")
                                        {
                                            if (orderdtl.Qty > dbdatanew.WTFrom1 && orderdtl.Qty <= dbdatanew.WTTo1 && dbdatanew.Rate1 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate1;
                                                orderdtl.Amount = dbdatanew.Rate1 * orderdtl.Qty;
                                            }
                                            if (orderdtl.Qty > dbdatanew.WTFrom2 && orderdtl.Qty <= dbdatanew.WTTo2 && dbdatanew.Rate2 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate2;
                                                orderdtl.Amount = dbdatanew.Rate2 * orderdtl.Qty;
                                            }
                                            if (orderdtl.Qty > dbdatanew.WTFrom3 && orderdtl.Qty <= dbdatanew.WTTo3 && dbdatanew.Rate3 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate3;
                                                orderdtl.Amount = dbdatanew.Rate3 * orderdtl.Qty;
                                            }

                                            if (orderdtl.Qty > dbdatanew.WTTo3 && dbdatanew.AddRate != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.AddRate;
                                                orderdtl.Amount = dbdatanew.AddRate * orderdtl.Qty;
                                            }

                                        }
                                        if (dbdatanew.UOM == "KG")
                                        {
                                            if (orderdtl.Weight > dbdatanew.WTFrom1 && orderdtl.Weight <= dbdatanew.WTTo1 && dbdatanew.Rate1 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate1;
                                                orderdtl.Amount = dbdatanew.Rate1 * orderdtl.Weight;
                                            }
                                            else
                                            if (orderdtl.Weight > dbdatanew.WTFrom2 && orderdtl.Weight <= dbdatanew.WTTo2 && dbdatanew.Rate2 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate2;
                                                orderdtl.Amount = dbdatanew.Rate2 * orderdtl.Weight;
                                            }
                                            else
                                            if (orderdtl.Weight > dbdatanew.WTFrom3 && orderdtl.Weight <= dbdatanew.WTTo3 && dbdatanew.Rate3 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate3;
                                                orderdtl.Amount = dbdatanew.Rate3 * orderdtl.Weight;
                                            }
                                            else

                                            if (orderdtl.Weight > dbdatanew.WTTo3 && dbdatanew.AddRate != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.AddRate;
                                                orderdtl.Amount = dbdatanew.AddRate * orderdtl.Weight;
                                            }
                                        }
                                        if (dbdatanew.UOM == "PCS")
                                        {
                                            if (orderdtl.Qty > dbdatanew.WTFrom1 && orderdtl.Qty <= dbdatanew.WTTo1 && dbdatanew.Rate1 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate1;
                                                orderdtl.Amount = dbdatanew.Rate1 * orderdtl.Qty;
                                            }
                                            if (orderdtl.Qty > dbdatanew.WTFrom2 && orderdtl.Qty <= dbdatanew.WTTo2 && dbdatanew.Rate2 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate2;
                                                orderdtl.Amount = dbdatanew.Rate2 * orderdtl.Qty;
                                            }
                                            if (orderdtl.Qty > dbdatanew.WTFrom3 && orderdtl.Qty <= dbdatanew.WTTo3 && dbdatanew.Rate3 != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.Rate3;
                                                orderdtl.Amount = dbdatanew.Rate3 * orderdtl.Qty;
                                            }

                                            if (orderdtl.Qty > dbdatanew.WTTo3 && dbdatanew.AddRate != 0)
                                            {
                                                orderdtl.RateIN = dbdatanew.UOM;
                                                orderdtl.Rate = dbdatanew.AddRate;
                                                orderdtl.Amount = dbdatanew.AddRate * orderdtl.Qty;
                                            }
                                        }
                                    }

                                }
                            }

                        }

                        ///Manisfest Code
                        ///
                        var dispatchdetails = db.dispatchDetails.Where(a => a.DispatchID == DispatchID).FirstOrDefault();
                        var driverdtl = db.DriverMaster.Where(a => a.DriverName == dispatchdetails.DriverName).FirstOrDefault();
                        var routedtl = db.RouteDetails.Where(a => a.RouteMainID == dispatchdetails.Route && a.VendorName == dispatchdetails.VendorName).FirstOrDefault();
                        if (routedtl == null)
                        {
                            var result1 = new { Message = "Route Not Added For Vendor" + dispatchdetails.VendorName, };
                            return Json(result1, JsonRequestBehavior.AllowGet);
                        }


                        var MasterOrderId = db.Order.Where(a => a.DispatchID == DispatchID).Select(a => a.MasterOrderId).ToList();
                        var CustomerList = db.MainOrder.Where(a => MasterOrderId.Contains(a.Id)).ToList();


                        var lst = Cust.Select(a => a.OrderNo).ToList();
                        var data2 = (from x1 in db.Order.Where(x1 => lst.Contains(x1.Id))

                                     join c in db.MainOrder
                                    on x1.MasterOrderId equals c.Id


                                     join cm in db.CustomerMasters
                                     on c.CustomerId equals cm.ID


                                     // join dd in db.dispatchDetails
                                     //on x1.DispatchID equals dd.DispatchID


                                     select new
                                     {
                                         //DriverName = dd.DriverName,
                                         //Route = dd.Route,
                                         //VehicalNo = dd.VehicalNo,
                                         //VendorType = dd.VendorType,
                                         OrderNo = x1.Id,
                                         CustID = cm.ID,
                                         CustName = cm.CustName,
                                         CustCode = cm.CustID,
                                         ItemName = x1.ProductName,
                                         OrderQty = x1.Qty,
                                         BoxinKG = 0,
                                         Location = cm.Area,
                                         Weight = x1.Weight,
                                         //        DispatchDate = dd.DispatchDate,
                                         InvoiceNo = x1.InvoiceNo,
                                         CustomerID = cm.ID,

                                     }).OrderBy(xx => xx.CustName).ToList();


                        var routedata = db.RouteMains.Where(a => a.ID == Route).FirstOrDefault();
                        var custgrp = data2.Select(a => a.CustID).ToList();
                        var CustGroup = db.ManifestGroups.Where(a => custgrp.Contains(a.CustomerID) && a.RouteID == Route).Select(a => new { a.CustomerID, a.GroupName }).Distinct().ToList();

                        var grpp = CustGroup.Select(a => a.GroupName).Distinct().ToList();


                        foreach (var g1 in grpp)
                        {
                            //var InvNo = db.TableNumbring.Where(a => a.TableName == "ManifestNo").FirstOrDefault();
                            //var drsno = InvNo.Prefix + "" + InvNo.SerialNo+"(-Edited)";
                            //InvNo.SerialNo = InvNo.SerialNo + 1;


                            ManifestMain main = new ManifestMain();
                            main.GroupName = g1;
                            main.DispatchID = DispatchID;

                            // Update
                            main.DeliveryChallanNo = drsno;

                            main.RouteName = routedata.MainRouteName;
                            main.RouteID = Route;

                            main.Date = expdt.Date;
                            main.Mode = routedtl.Mode;
                            main.TransportName = dispatchdetails.VendorName;
                            main.VehicleNo = dispatchdetails.VehicalNo;
                            main.VehicleType = dispatchdetails.VendorType;

                            main.DriverName = driverdtl.DriverName;
                            main.DriverMobileNo = driverdtl.MobileNo;
                            main.CreatedBy = User.Identity.Name;
                            main.CreatedDate = DateTime.Today;


                            var test = CustGroup.Where(a => a.GroupName == g1).ToList();
                            var Count = 1;
                            decimal TotalOrderWeight = 0;
                            var TotalDispatchQty = 0;
                            var TotalBoxQty = 0;
                            decimal TotalBoxinKG = 0;
                            foreach (var x1 in test)
                            {
                                var dta = data2.Where(a => a.CustID == x1.CustomerID).ToList();

                                foreach (var xx in dta)
                                {
                                    var pkg = db.PackingDetails.Where(a => a.OrderNo == xx.OrderNo)
                              .GroupBy(a => a.OrderNo)
                              .Select(a => new { NoOfBox = a.Sum(b => b.NoOfBox), BoxWeight = a.Sum(b => b.BoxWeight), TotalWeight = a.Sum(b => b.TotalWeight), Name = a.Key })
                              .FirstOrDefault();

                                    ManifestDetails md = new ManifestDetails();
                                    md.DeliveryChallanNo = drsno;
                                    md.DispatchID = DispatchID;
                                    md.OrderNo = xx.OrderNo;
                                    md.CustomerID = xx.CustomerID;
                                    md.CustomerCode = xx.CustCode;
                                    md.CustomerName = xx.CustName;
                                    md.Location = xx.Location;
                                    md.ProductName = xx.ItemName;
                                    md.OrderWeight = xx.Weight;
                                    md.DispatchQty = xx.OrderQty;
                                    md.BoxQty = pkg.NoOfBox;
                                    md.BoxInKG = Convert.ToDecimal(pkg.TotalWeight);

                                    db.ManifestDetails.Add(md);

                                    TotalOrderWeight = TotalOrderWeight + Convert.ToDecimal(xx.Weight);
                                    TotalDispatchQty = TotalDispatchQty + Convert.ToInt32(xx.OrderQty);
                                    TotalBoxQty = TotalBoxQty + pkg.NoOfBox;
                                    TotalBoxinKG = TotalBoxinKG + Convert.ToDecimal(pkg.TotalWeight);

                                }
                            }

                            main.TotalOrderWeight = TotalOrderWeight;
                            main.TotalDispatchQty = TotalDispatchQty;
                            main.TotalBoxQty = TotalBoxQty;
                            main.TotalBoxinKG = TotalBoxinKG;
                            db.ManifestMains.Add(main);
                        }
                        db.DispatchTransactions.AddRange(transaction);
                        foreach (var c in Cust)
                        {
                            TrackingDetails track = new TrackingDetails();
                            track.Status = "Order Dispatch Updated By User " + User.Identity.Name;
                            track.Action = "Dispatch Details";
                            track.Location = "Plant";
                            track.OrderNo = c.OrderNo;
                            track.Date = DateTime.Now;
                            track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                            track.CreatedDate = DateTime.Now;
                            track.CreatedBy = User.Identity.Name;
                            db.trackingDetails.Add(track);
                        }

                        db.SaveChanges();
                        dbTran.Commit();
                        var result = new { Message = "success" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }            
                var results = new { Message = "" };
                return Json(results, JsonRequestBehavior.AllowGet); 
            }
            catch (Exception ex)
            {
                var resultt = new { Message = ex.Message };
                return Json(resultt, JsonRequestBehavior.AllowGet);
            }


        }
    }
    internal class FileName
    {
        public string Name { get; set; }
    }

    internal class SubRoot
    {
        public string RouteFrom { get; set; }
        public string RouteTo { get; set; } 
    }
}