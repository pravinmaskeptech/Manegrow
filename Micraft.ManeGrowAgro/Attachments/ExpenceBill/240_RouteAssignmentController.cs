using iTextSharp.text;
using iTextSharp.text.pdf;
using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class RouteAssignmentController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: RouteAssignment
        public ActionResult Index()
        {
            ViewBag.Expence = db.ExpenceTypes.OrderBy(a => a.ExpenceType).ToList();
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
                                                Route= c.MainRouteName,
                                                x.VendorName

                                            }).OrderByDescending(x => x.DispatchID).ToList();

                var totalRecord = dataa.Count();

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
        public ActionResult Create() 
        {
            ViewBag.Root = db.RouteMains.OrderBy(a => a.MainRouteName).ToList();
            ViewBag.DriverName = db.DriverMaster.Where(a=>a.DriverName=="sjdfsfg").OrderBy(a => a.DriverName).ToList();
            ViewBag.VendorName = db.VendorMasters.Where(a=>a.UserName=="sdjgfsfgh").OrderBy(a => a.VendorName).ToList();
            ViewBag.SubRoute = db.RouteMains.Where(a=>a.MainRouteName=="dhgfsj").OrderBy(a => a.MainRouteName).ToList();
            ViewBag.VendorType = db.VendorTypes.OrderBy(a => a.VendorType).ToList();
            return View();
        }
        public JsonResult GetExpencePopupData(int ID)
        {

            var order = db.ExpenceMains.Where(a => a.DispatchID == ID).ToList();
            var dtl= db.ExpenceDetails.Where(a => a.DispatchID == ID).ToList();

            var result = new { Message = "success", order, dtl };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDriverDetails(int ID) 
        {
          //  var vendorid = db.VendorMasters.Where(a => a.VendorName == VendorName).Select(a => a.ID).SingleOrDefault();
            var Driverdetails = db.DriverMaster.Where(a => a.VendorID == ID).Select(a=>a.DriverName).ToList();
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
            var result = new {Message="success", Dispatch };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPreviousRecords(int? ExpType)
        {

            var main = db.ExpenceTypes.Where(x => x.ID == ExpType).FirstOrDefault();


            List<ExpenceDetails> expences = new List<ExpenceDetails>();

            if (main != null)
            {
                expences = db.ExpenceDetails.Where(x => x.ExpMainID == main.ID).ToList();
            }

            var tbldata = expences;
            return Json(tbldata, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetEditExpense(int ID)
        //{
        //    var ExpenceDetails  = db.ExpenceDetails.Where(a => a.DispatchID == ID).FirstOrDefault();
        //    var result = new { Message = "success", ExpenceDetails };
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

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

            var CustID = db.SubRouteCustomers.Where(a => a.RouteMainID == Root).Select(a => a.CustomerID).Distinct().ToList(); 
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
                           group p by new {  p.CustID, p.CustName,p.ID } into gcs
                           select new
                           {

                              
                               ID = gcs.Key.ID,
                               Area = gcs.Select(a=>a.Area).Distinct(),
                               City = gcs.Select(a => a.City).Distinct(),
                               CustName = gcs.Key.CustName,
                              
                              
                               CustID = gcs.Key.CustID,
                               NoOfBox = gcs.Sum(x => x.NoOfBox),
                               TotalWeight = gcs.Sum(x => x.TotalWeight),

                           }).ToList();

            
            var WithOrder = result1.Where(a => CustID.Contains(a.CustID)).ToList();
            var WithoutOrder = result1.Where(a => !CustID.Contains(a.CustID)).ToList();




            //var orders = db.Order.Where(a=>(a.OrderStatus== "Packing Completed" || a.OrderStatus== "Quantity Changed & Packing Completed") && a.Date==DateTime.Today).Select( a=>a.MasterOrderId ).Distinct().ToList();  
            //var mainorderCustomerID = db.MainOrder.Where(a => orders.Contains(a.Id)).Select(a => a.CustomerId ).ToList(); 
            //var Customers = db.CustomerMasters.ToList();
            //var WithOrder = Customers.Where(a =>a.IsApproved==true && a.RootName== Root && mainorderCustomerID.Contains(a.ID)).ToList();
            //var tempCust = WithOrder.Select(a => a.ID).ToList();

            //var WithoutOrder  = Customers.Where(a => a.IsApproved == true && mainorderCustomerID.Contains(a.ID) && !tempCust.Contains(a.ID)).ToList();

            var result = new { WithOrder, WithoutOrder };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEditCustomers(string Root,string Date,int ID) 
        {
            var expdt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var orders = db.Order.Where(a =>a.DispatchID==ID).Select(a => a.MasterOrderId).Distinct().ToList();
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
            var dataa = Cust.Select(x => new {  x.CustomerId }).GroupBy(x => x.CustomerId).Select(x => x.FirstOrDefault()).ToList();
            foreach (var x in dataa) 
            {
                var order = db.MainOrder.Where(a => a.CustomerId == x.CustomerId && a.Date==DateTime.Today).ToList();
                foreach(var xx in order)
                {
                    var Orderid = db.Order.Where(a => a.MasterOrderId == xx.Id && (a.OrderStatus == "Packing Completed" || a.OrderStatus== "Quantity Changed & Packing Completed" || a.OrderStatus== "Merge & Packing Completed")).Select(a => a.Id).ToList();
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

            var result = new {Message="success", TotalBoxes, TotalWeight };
            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetSubRoutes(int RouteMainID)
        {

            var SubRoute = db.SubRouteDetails.Where(a => a.RouteMainID == RouteMainID).Select(a => a.SubRouteName).Distinct().ToList();
                var result = new {Message="success", SubRoute };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SaveExpence(List<ExpenceDetails> Order, int FromKM, int ToKM, int TotalKM, decimal TotalAmount, int DispatchID,int ExpMainID)
        {
            try
            {
                using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                {
                    if (ExpMainID == 0)
                    {
                        ExpenceMain em = new ExpenceMain();
                        em.FromKM = FromKM;
                        em.ToKM = ToKM;
                        em.TotalKM = TotalKM;
                        em.TotalAmount = TotalAmount;
                        em.CreatedBy = User.Identity.Name;
                        em.CreatedDate = DateTime.Today;
                        em.DispatchID = DispatchID;
                        db.ExpenceMains.Add(em);
                        db.SaveChanges();
                        var maxid = db.ExpenceMains.Select(a => a.ID).Max();
                    }
                    else
                    {
                        var em = db.ExpenceMains.Where(a => a.ID == ExpMainID).FirstOrDefault();
                        em.FromKM = FromKM;
                        em.ToKM = ToKM;
                        em.TotalKM = TotalKM;
                        em.TotalAmount = TotalAmount;
                        em.UpdatedBy = User.Identity.Name;
                        em.UpdatedDate   = DateTime.Today;
                        em.DispatchID = DispatchID;
                      
                    }

                   
                    ExpenceDetails ed = new ExpenceDetails();
                    foreach (var expense in Order)
                    {
                        if (expense.ID == 0)
                        {
                            ed.ExpType = expense.ExpType;
                            ed.Amount = expense.Amount;
                            ed.ImageName = expense.ImageName;
                            ed.DispatchID = DispatchID;
                            ed.ExpMainID = expense.ExpMainID;
                            db.ExpenceDetails.Add(ed);
                        }else
                        {
                            var expdtl = db.ExpenceDetails.Where(a => a.ID == expense.ID).FirstOrDefault();
                            expdtl.ExpType = expense.ExpType;
                            expdtl.Amount = expense.Amount;
                            expdtl.ImageName = expense.ImageName;
                            expdtl.DispatchID = DispatchID;
                            ed.ExpMainID = expense.ExpMainID;
                        }
                    }
                    db.SaveChanges();
                    dbTran.Commit();
                    var result = new { Message = "success", };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            } 
                catch (Exception ex)
                {
                    var result = new { Message =ex.Message };
                    return Json(result, JsonRequestBehavior.AllowGet);
    }
}
        public JsonResult Save(List<MainOrder> Cust,string Date,int Route,string DriverName,string VendorType, string VendorName,string VehicalNo,int TotalBoxes,decimal TotalWT,int? ID)  
        {
            try
            {
                int DispatchID = 0;
                var expdt = DateTime.ParseExact(Date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                {
                    if (ID != null)
                    {
                        var dispatchdtl = db.dispatchDetails.Where(a => a.DispatchID == ID).FirstOrDefault();
                        dispatchdtl.TotalWeight = TotalWT;
                        dispatchdtl.NoofBoxes = TotalBoxes;
                        dispatchdtl.DriverName = DriverName;
                        dispatchdtl.VehicalNo = VehicalNo;
                        dispatchdtl.VendorName = VendorName;
                        dispatchdtl.Route = Route;
                        dispatchdtl.VendorType = VendorType;
                        dispatchdtl.CreatedBy = User.Identity.Name;
                        dispatchdtl.CreatedDate = DateTime.Today;
                        dispatchdtl.UpdatedBy = User.Identity.Name;
                        dispatchdtl.UpdatedDate = DateTime.Today; 
                        DispatchID =Convert.ToInt32( ID);

                        foreach (var x in Cust)
                        {          
                                    var orderdtl = db.Order.Where(a => a.Id == x.OrderNo).FirstOrDefault();
                                    orderdtl.OrderStatus = "Order Dispatched";
                                    orderdtl.OrderStage = 8;
                                    orderdtl.DispatchID = null;

                        }
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
                        dispatchdtl.Route = Route;
                        dispatchdtl.VendorType = VendorType;
                        dispatchdtl.CreatedBy = User.Identity.Name;
                        dispatchdtl.CreatedDate = DateTime.Today;
                        db.dispatchDetails.Add(dispatchdtl);
                        db.SaveChanges();
                        DispatchID = db.dispatchDetails.Max(u => u.DispatchID);
                    }

                    var CustDetails = Cust.Select(a => a.CustomerId).ToList().Distinct();                   

                    foreach (var x in CustDetails)
                    {
                        var CustDetailss = Cust.Where(a=>a.CustomerId==x)
.GroupBy(_ => _.CustomerId, _ => new { TotalWT = _.TotalWT, TotalBoxes = _.TotalBoxes, CustomerId = _.CustomerId })
.Select(g => new
{
CustomerId = g.Select(a => a.CustomerId),
TotalWT = g.Sum(f => f.TotalWT).ToString(),
TotalBoxes = g.Sum(f => f.TotalBoxes).ToString(),

}).FirstOrDefault();
                        var custID =x;  
                        var Custdata = db.SubRouteCustomers.Where(a => a.CustomerID == custID).Select(a=>a.RouteDetailsID).ToList();
                        if(Custdata.Count==0)
                        {
                            var cst = db.CustomerMasters.Where(a => a.ID == custID).FirstOrDefault(); 
                            var resultss = new { Message = "Route Not Assignd For "+ cst.CustName };
                            return Json(resultss, JsonRequestBehavior.AllowGet);
                        }
                        var TotalWTT = CustDetailss.TotalWT;
                        var TotalBoxess = CustDetailss.TotalBoxes;

                        var transaction = new List<DispatchTransactions>(); 
                        foreach (var dtlid in Custdata)
                        {
                            var rdd = db.RouteDetails.Where(a => a.ID == dtlid).FirstOrDefault();
                            var transCount = db.DispatchTransactions.Where(a => a.VendorID == rdd.VendorID && a.Date == expdt).Count();
                            if(transCount==0)
                            {
                                var trans = new DispatchTransactions();
                                trans.DispatchID = Convert.ToInt32(DispatchID);
                                trans.LocationFrom = rdd.FromRoute;
                                trans.LocationTo = rdd.ToRoute;
                                trans.VendorID = Convert.ToInt32(rdd.VendorID);
                                trans.VendorName = rdd.VendorName;
                                trans.Date = expdt;
                                trans.LoadINKG =Convert.ToDecimal(TotalWTT);
                                trans.NoOfBoxes = Convert.ToInt32(TotalBoxess);
                                decimal AMT = 0;
                                if(rdd.RateType== "WEIGHT")
                                {
                                    AMT = rdd.Rate * Convert.ToDecimal(TotalWTT);
                                }
                                if (rdd.RateType == "BOX")
                                {
                                    AMT = rdd.Rate * Convert.ToInt32(TotalBoxess);
                                }
                                if (rdd.RateType == "PER KM")
                                {

                                }
                                if (rdd.RateType == "UP TO KM")
                                {

                                }
                                if (rdd.RateType == "Trip")
                                {
                                    AMT = rdd.Rate;
                                }
                                trans.Amount = AMT;
                                transaction.Add(trans);
                                db.DispatchTransactions.Add(trans); 
                            }
                        }





                     


                        foreach (var xx in Cust)
                        {
                            var Orderid = db.Order.Where(a => a.Id == xx.OrderNo &&( a.OrderStatus== "Packing Completed" ||  a.OrderStatus== "Quantity Changed & Packing Completed" || a.OrderStatus== "Merge & Packing Completed")).ToList();
                            foreach (var o in Orderid)
                            {
                                var orderdtl = db.Order.Where(a => a.Id == o.Id).FirstOrDefault();
                                orderdtl.OrderStatus = "Order Dispatched";
                                orderdtl.DispatchID = DispatchID;
                                orderdtl.OrderStage = 8;
                               
                                    TrackingDetails track = new TrackingDetails();
                                    track.Status = orderdtl.OrderStatus;
                                    track.Action = "Dispatch Details";
                                    track.Location = "Plant";
                                    track.OrderNo = orderdtl.Id;
                                    track.Date = DateTime.Now;
                                    track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                                    track.CreatedDate = DateTime.Now;
                                    track.CreatedBy = User.Identity.Name;
                                    db.trackingDetails.Add(track);
                              //      temp++;
                                
                            }


                            

                        }
                    }
                    db.SaveChanges();
                    dbTran.Commit();
                    var result = new { Message = "success" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }catch(Exception ex)
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
                    var result = new { Message = fname+" Already Added,Please Rename File Name" };
                    return Json(result, JsonRequestBehavior.AllowGet);
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
            var result = new {Message="success", list = list, DuplicateImg = duplicate, FileNM };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        internal class FileName
        {
            public string Name { get; set; }
        }

        public JsonResult GetPopupData(int ID)
        {

            var order = (from e in db.ExpenceMains.Where(a => a.DispatchID == ID)

                         //join dr in db.ExpenceTypes
                         // on e.ExpenceType equals dr.Id

                         // join cus in db.CustomerMasters
                         // on dr.CustomerId equals cus.ID

                         // join pm in db.ProductMasters
                         //on e.ProductId equals pm.ID

                         select new
                         {
                             ID = e.ID,
                             FromKm = e.FromKM,
                             DispatchID = e.DispatchID,
                             ToKm = e.ToKM,
                             TotalKm = e.TotalKM,
                             TotalAmount=e.TotalAmount



                         }).OrderByDescending(x => x.ID).FirstOrDefault();


            var result = new { Message = "success", order };
            return Json(result, JsonRequestBehavior.AllowGet);
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
                    string filename1 = path + User.Identity.Name + timeSince1970+".pdf";                   
                    string StikerPrintName = User.Identity.Name + timeSince1970+".pdf"; 
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filename1, FileMode.Create));
                    document.Open();

                var MasterOrderId = db.Order.Where(a => a.DispatchID == DispatchID).Select(a=>a.MasterOrderId).ToList();
                var CustomerList = db.MainOrder.Where(a => MasterOrderId.Contains(a.Id)).ToList();



                var data = (from x in db.Order.Where(x => x.DispatchID == DispatchID)

                            join c in db.MainOrder
                           on x.MasterOrderId equals c.Id


                            join cm in db.CustomerMasters
                            on c.CustomerId equals cm.ID


                            join dd in db.dispatchDetails 
                           on x.DispatchID equals dd.DispatchID


                            select new
                            {
                                DriverName=  dd.DriverName,
                                Route= dd.Route,
                                VehicalNo= dd.VehicalNo,
                                VendorType=  dd.VendorType,                               
                                OrderNo = x.Id,
                                CustID = cm.ID,  
                                CustName = cm.CustName,
                                CustCode = cm.CustID, 
                                ItemName = x.ProductName, 
                                OrderQty = x.Qty,
                                BoxinKG = 0,
                                Location=cm.City,
                                Weight = x.Weight, 

                            }).OrderBy(xx => xx.CustName).ToList();

                var route = data[0].Route;
                var routedata = db.RouteMains.Where(a => a.ID == route).FirstOrDefault();
                var custgrp = data.Select(a => a.CustID).ToList();
                var CustGroup = db.ManifestGroups.Where(a => custgrp.Contains(a.CustomerID) && a.RouteID == route).Select(a => new {a.CustomerID,a.GroupName}).Distinct().ToList();

                var grpp = CustGroup.Select(a => a.GroupName).Distinct().ToList();


                foreach (var x in grpp)
                {
                    PdfPTable table1 = new PdfPTable(14);
                    float[] width1 = new float[] { 0.1f, 2f, 3f, 4f, 6f, 6f, 4f, 2f, 4f, 3f, 3f, 3f, 3f, 0.1f }; 
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
                    pr1.Add(new Phrase("AGRO PRODUCTS ", FontFactory.GetFont("Arial", 12, Font.NORMAL)));
                    pr1.Add(new Phrase("PVT LTD\n", FontFactory.GetFont("Arial", 12, Font.NORMAL)));

                    PdfPCell PC111 = new PdfPCell(pr1);
                    PC111.FixedHeight = 62f;
                    PC111.Colspan = 10;
                    PC111.HorizontalAlignment = 1;
                    table1.AddCell(PC111);



                    Paragraph pr1002 = new Paragraph();                   
                    pr1002.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL))); 
                    pr1002.Add(new Phrase("MANIFEST", FontFactory.GetFont("Arial", 14, Font.BOLD)));                    
                    PdfPCell PC11001 = new PdfPCell(pr1002);
                    PC11001.FixedHeight = 30f;
                    PC11001.Colspan = 14; 
                    PC11001.HorizontalAlignment = 1;
                    table1.AddCell(PC11001);




                    Paragraph pr1002sd = new Paragraph();
                    pr1002sd.Add(new Phrase("\n", FontFactory.GetFont("Arial", 4, Font.NORMAL)));
                    pr1002sd.Add(new Phrase("", FontFactory.GetFont("Arial", 14, Font.BOLD)));
                    PdfPCell PC1100df1 = new PdfPCell(pr1002sd);
                   
                    PC1100df1.Colspan = 9;
                    PC1100df1.HorizontalAlignment = 1;
                    table1.AddCell(PC1100df1);


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
                    pr1002dfd22f.Add(new Phrase("Date  \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase("Mode \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase("Transport Name \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase("Vehicle No \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase("Vehicle Type  \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase("Driver Name  \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    pr1002dfd22f.Add(new Phrase("Mobile No  \n", FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                    PdfPCell PC1100102s2 = new PdfPCell(pr1002dfd22f);
                    PC1100102s2.Colspan = 2;
                    PC1100102s2.HorizontalAlignment = 0;
                    PC1100102s2.Border = Rectangle.BOTTOM_BORDER;
                    table1.AddCell(PC1100102s2);


                    PdfPCell PC1100d102s2 = new PdfPCell();
                  //  PC1100d102s2.Colspan = 3;
                    PC1100d102s2.HorizontalAlignment = 0;
                    PC1100d102s2.Border = Rectangle.LEFT_BORDER;
                    table1.AddCell(PC1100d102s2);

                    try
                    {

                        PdfPCell p1sd = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p1sd.HorizontalAlignment = 1;
                        p1sd.BackgroundColor = BaseColor.WHITE;
                        p1sd.FixedHeight = 20;
                      //  p1sd.Colspan = 2;
                        p1sd.Border = Rectangle.LEFT_BORDER;
                        table1.AddCell(p1sd);



                        PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("Sr No", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p1.HorizontalAlignment = 1;
                        p1.BackgroundColor = BaseColor.WHITE;
                        p1.FixedHeight = 20;
                     //WW   p1.Colspan = 2;
                        p1.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p1);

                        PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("Order No", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p2dd.HorizontalAlignment = 1;
                        p2dd.BackgroundColor = BaseColor.WHITE;
                        p2dd.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p2dd);



                        PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("Customer Code", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p2.HorizontalAlignment = 1;
                        p2.BackgroundColor = BaseColor.WHITE;
                        p2.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p2);

                        PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("Party Name", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp5.HorizontalAlignment = 0;
                        pp5.BackgroundColor = BaseColor.WHITE;
                        pp5.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp5);

                        PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("Location", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp55.HorizontalAlignment = 0;
                        pp55.BackgroundColor = BaseColor.WHITE;
                        pp55.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp55);

                        PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("Item Description", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p29.HorizontalAlignment = 0;
                        p29.BackgroundColor = BaseColor.WHITE;
                        p29.Border = Rectangle.BOTTOM_BORDER;
                    //    p29.Colspan = 2;
                        table1.AddCell(p29);

                        PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("Order Weight", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp51.HorizontalAlignment = 0;
                        pp51.BackgroundColor = BaseColor.WHITE;
                        pp51.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp51);


                        PdfPCell p6 = new PdfPCell();
                        p6 = new PdfPCell(new Phrase(new Phrase("Dispatch Qty", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p6.HorizontalAlignment = 1;
                        p6.BackgroundColor = BaseColor.WHITE;
                        p6.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p6);

                        PdfPCell pp4 = new PdfPCell(new Phrase(new Phrase("Box Qty", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp4.HorizontalAlignment = 0;
                        pp4.BackgroundColor = BaseColor.WHITE;
                        pp4.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp4);



                        PdfPCell pp41 = new PdfPCell(new Phrase(new Phrase("Box in KG", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        pp41.HorizontalAlignment = 0;
                        pp41.BackgroundColor = BaseColor.WHITE;
                        pp41.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(pp41);

                       
                        PdfPCell p291 = new PdfPCell(new Phrase(new Phrase("Invoice Code", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p291.HorizontalAlignment = 1;
                        p291.BackgroundColor = BaseColor.WHITE;
                        p291.Border = Rectangle.BOTTOM_BORDER;
                        table1.AddCell(p291);

                        PdfPCell p292 = new PdfPCell(new Phrase(new Phrase("Signature Of Party", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292.HorizontalAlignment = 1;
                        p292.BackgroundColor = BaseColor.WHITE;
                        p292.Border = Rectangle.BOTTOM_BORDER;
                    //    p292.Colspan = 2;
                        table1.AddCell(p292);

                        PdfPCell p292e = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD))));
                        p292e.HorizontalAlignment = 1;
                        p292e.BackgroundColor = BaseColor.WHITE;
                        p292e.Border = Rectangle.RIGHT_BORDER;
                     //   p292e.Colspan = 2;
                        table1.AddCell(p292e);

                    }
                    catch
                    {

                    }
                    var test = CustGroup.Where(a => a.GroupName == x).ToList();
                    var Count = 1;

                    foreach (var x1 in test)
                    {
                        var dta = data.Where(a => a.CustID == x1.CustomerID).ToList();
                       
                        foreach (var xx in dta)
                        {                          
                            var pkg = db.PackingDetails.Where(a => a.OrderNo == xx.OrderNo)
                                .GroupBy(a => a.OrderNo)    
                                .Select(a => new { NoOfBox = a.Sum(b => b.NoOfBox), BoxWeight = a.Sum(b => b.BoxWeight), TotalWeight = a.Sum(b => b.TotalWeight), Name = a.Key })   
                                .FirstOrDefault();

                            PdfPCell p292s5 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p292s5.HorizontalAlignment = 1;
                            p292s5.BackgroundColor = BaseColor.WHITE;
                            p292s5.Border = Rectangle.LEFT_BORDER;
                            //   p292.Colspan = 2;
                            table1.AddCell(p292s5);


                            PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("" + Count, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p1.HorizontalAlignment = 1;
                            p1.BackgroundColor = BaseColor.WHITE;
                            p1.FixedHeight = 20;
                         //   p1.Colspan = 2;
                            p1.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p1);

                            PdfPCell p2sd = new PdfPCell(new Phrase(new Phrase("" + xx.OrderNo, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p2sd.HorizontalAlignment = 1;
                            p2sd.BackgroundColor = BaseColor.WHITE;
                            p2sd.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p2sd);



                            PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("" + xx.CustCode, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p2.HorizontalAlignment = 1;
                            p2.BackgroundColor = BaseColor.WHITE;
                            p2.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p2);

                            PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("" + xx.CustName, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            pp5.HorizontalAlignment = 0;
                            pp5.BackgroundColor = BaseColor.WHITE;
                            pp5.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp5);


                            PdfPCell p2945 = new PdfPCell(new Phrase(new Phrase("" + xx.Location, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p2945.HorizontalAlignment = 0;
                            p2945.BackgroundColor = BaseColor.WHITE;
                            p2945.Border = Rectangle.BOTTOM_BORDER;
                            //   p29.Colspan = 2;
                            table1.AddCell(p2945);


                            PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("" + xx.ItemName, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p29.HorizontalAlignment = 0;
                            p29.BackgroundColor = BaseColor.WHITE;
                            p29.Border = Rectangle.BOTTOM_BORDER;
                         //   p29.Colspan = 2;
                            table1.AddCell(p29);

                            PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("" + xx.Weight, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            pp51.HorizontalAlignment = 0;
                            pp51.BackgroundColor = BaseColor.WHITE;
                            pp51.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp51);

                            PdfPCell p6 = new PdfPCell();
                            p6 = new PdfPCell(new Phrase(new Phrase("" + xx.OrderQty, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p6.HorizontalAlignment = 1;
                            p6.BackgroundColor = BaseColor.WHITE;
                            p6.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p6);

                            PdfPCell pp4 = new PdfPCell(new Phrase(new Phrase(""+ pkg.NoOfBox, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            pp4.HorizontalAlignment = 0;
                            pp4.BackgroundColor = BaseColor.WHITE;
                            pp4.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp4);



                            PdfPCell pp41 = new PdfPCell(new Phrase(new Phrase(""+ pkg.TotalWeight, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            pp41.HorizontalAlignment = 0;
                            pp41.BackgroundColor = BaseColor.WHITE;
                            pp41.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(pp41);

                           
                            PdfPCell p291 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p291.HorizontalAlignment = 1;
                            p291.BackgroundColor = BaseColor.WHITE;
                            p291.Border = Rectangle.BOTTOM_BORDER;
                            table1.AddCell(p291);

                            PdfPCell p292 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p292.HorizontalAlignment = 1;
                            p292.BackgroundColor = BaseColor.WHITE;
                            p292.Border = Rectangle.BOTTOM_BORDER;
                          //  p292.Colspan = 2;
                            table1.AddCell(p292);

                            PdfPCell p292s = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                            p292s.HorizontalAlignment = 1;
                            p292s.BackgroundColor = BaseColor.WHITE;
                            p292s.Border = Rectangle.RIGHT_BORDER;
                         //   p292.Colspan = 2;
                            table1.AddCell(p292s);


                        }
                        Count++;

                    }
                      
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

        public virtual ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Attachments/ExpenceBill/"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }

    }

    internal class SubRoot
    {
        public string RouteFrom { get; set; }
        public string RouteTo { get; set; } 
    }
}