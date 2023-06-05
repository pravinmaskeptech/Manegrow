using ClosedXML.Excel;
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
    public class CustomerOrderController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: CustomerOrder

        [CustomAuthorize("You dont have access to View Order List", "CustomerOrderView, CustomerOrderEdit, Admin")]
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


            var SalesID = db.DepartmentMasters.Where(a => a.Department.Contains("SALES")).Select(a => a.ID).ToList();
            var SalesPerson = db.EmployeeMasters.Where(a => SalesID.Contains(a.DepartmentID)).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }
            return View();
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
                    type = Session["Type"].ToString();
                }

                var city = "";
                if (Session["City"] != "")
                {
                    city = Session["City"].ToString();
                }

                var shrtname = "";
                if (Session["ShortName"] != "")
                {
                    shrtname = Session["ShortName"].ToString();
                }

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
                string test = string.Empty;
                sSearch = sSearch.ToLower();
              
                var Unames = db.CustomerMasters.Where(a => a.CustName == "kjhfkjghfkdjhg").Select(a=>a.Username).ToList();

                var dt = DateTime.Today;
                dt = dt.AddDays(-1);
             

                var result = (from dr in db.MainOrder
                              join dp in db.CustomerMasters
                              on dr.CustomerId equals dp.ID

                              join ds in db.CustomerTypes
                              on dp.CustTypeID equals ds.ID

                              join e in db.Order
                              on dr.Id equals e.MasterOrderId

                              join a in db.EmployeeMasters 
                             on dr.CreatedBy equals a.Username


                              select new
                              {
                                  ID = e.Id,
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
                                  ProductName = e.ProductName,
                                  PackagingRemark = e.PackagingRemark,
                                  IsPacked = e.OrderStatus,
                                  OrderStatus = e.OrderStatus,
                                  OrderStage = e.OrderStage,
                                  CreatedBy = a == null ? string.Empty : a.Name,
                                  EmpMobile = a == null ? string.Empty : a.MobileNumber,

                              }).OrderBy(x => x.OrderStage).ToList();


                

                if (Fromdate != null && Todate != null)
                {
                    result = result.Where(a => (a.Date >= Fromdate && a.Date <= Todate)).ToList();
                }
                else
                {
                    var fdt = DateTime.Today;
                    //  var fdt = Tdt.AddDays(-1);
                    result = result.Where(a => (a.Date == fdt)).ToList();
                }
                if (type != "")
                {
                    result = result.Where(a => a.CustomerTypes.ToLower() == type.ToLower()).ToList();
                }
                if (city != "")
                {
                    result = result.Where(a => a.City.ToLower() == city.ToLower()).ToList();
                }
                if (shrtname != "")
                {
                    result = result.Where(a => a.ShortName.ToLower() == shrtname.ToLower()).ToList(); 
                }

                if(sSearch !="")
                {
                    try
                    {
                        result = result.Where(x => x.ID == Convert.ToInt32(sSearch)
                        || x.OrderStatus.ToLower().Contains(sSearch.ToLower())
                        || x.CustName.ToLower().Contains(sSearch.ToLower())
                        || x.CustName.ToLower().Contains(sSearch.ToLower())
                         || x.ShortName.ToLower().Contains(sSearch.ToLower())
                          || x.CustomerTypes.ToLower().Contains(sSearch.ToLower())
                           || x.ProductName.ToLower().Contains(sSearch.ToLower())
                            || x.City.ToLower().Contains(sSearch.ToLower())
                             || x.Area.ToLower().Contains(sSearch.ToLower())
                              || x.CreatedBy.ToLower().Contains(sSearch.ToLower())
                              ).ToList();
                    }
                    catch
                    {

                    }
                        
                }

              //  int totalRecord = db.MainOrder.Count();

                StringBuilder sb = new StringBuilder();
                sb.Clear();
                sb.Append("{");
                sb.Append("\"sEcho\": ");
                sb.Append(sEcho);
                sb.Append(",");
                sb.Append("\"iTotalRecords\": ");
                sb.Append(result.Count());
                sb.Append(",");
                sb.Append("\"iTotalDisplayRecords\": ");
                sb.Append(result.Count());
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


        [CustomAuthorize("You dont have access to Edit Order","CustomerOrderEdit, Admin")]
        public ActionResult Edit() 
        {
            ViewBag.ProductTypes = db.ProductTypes.Where(x => x.IsActive == true).ToList();
            ViewBag.ShortNames = db.CustomerMasters.Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
   
            return View();
        }
        [CustomAuthorize("You dont have access to Create Order", "CustomerOrderEdit, Admin")]
        public ActionResult Create()
        {


            int maxid = 0;
            try
            {
                maxid = db.MainOrder.Select(x => x.Id).Max();
            }
            catch (Exception e)
            {
                maxid = 0;
            }

            if (maxid == null || maxid == 0)
            {
                maxid = 0;
            }

            maxid = maxid + 1;


            ViewBag.ProductTypes= db.ProductTypes.Where(x=>x.IsActive==true).ToList();
            ViewBag.ShortNames = db.CustomerMasters.Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            //if(User.IsInRole("Admin"))
            //ViewBag.ShortNames = db.CustomerMasters.Select(x=> new { x.ShortName ,x.ID }).GroupBy(x=>x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            //if (User.IsInRole("Customer"))
            //{
            //    ViewBag.ShortNames = db.CustomerMasters.Where(a => a.Username == User.Identity.Name).Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            //}
            //if (User.IsInRole("Sales"))
            //{


            //    var salesID = db.EmployeeMasters.Where(a => a.Username == User.Identity.Name).Select(a => a.ID).SingleOrDefault();

            //    ViewBag.ShortNames = db.CustomerMasters.Where(a=>a.SalesPersonID== salesID).Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            //}




            ViewBag.Id = maxid;
            return View();
        }

        public JsonResult GetLocations(string SHORTNAME)
        {
            var  xxx = db.CustomerMasters.Where(x => x.ShortName == SHORTNAME && x.IsActive == true).Count();
            var Areas = db.CustomerMasters.Where(x => x.ShortName == SHORTNAME && x.IsActive==true).Select(x=> new { x.Area,x.City,x.CustID,x.ID,x.CustName }).ToList();

            var tbldata = Areas;
            //var result = new { Message = "success", tbldata };
            return Json(tbldata, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProducts(int PRODUCTTYPE,string DATE)
        {
            var Products = db.ProductMasters.Where(x => x.TypeID == PRODUCTTYPE).ToList();

            var dt = Convert.ToDateTime(DATE);
           
            var tbldata = Products;
            //var result = new { Message = "success", tbldata };
            return Json(tbldata, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetEditData(int ID)
        {
            var order = (from e in db.Order.Where(a => a.Id == ID) 

                         join x in db.MainOrder 
                              on e.MasterOrderId equals x.Id
                         select new
                          {
                              ID = e.Id, 
                              Date = x.Date,                              
                              Location = x.Location,
                              CustomerCode = x.CustomerCode,
                              CategoryName = x.CategoryId,
                              CustomerShortCode = x.CustomerShortCode,
                              ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                              ExpectedDeliveryTime = x.ExpectedDeliveryTime,
                              Remark = x.Remark,
                             
                          }).OrderByDescending(x => x.ID).FirstOrDefault();

            var result = new { Message = "success", order };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPreviousRecords(string DATE, int CATEGORY,string SHORTNAME,string LOCATION,string CCODE,int ID)  
        {
            // var Products = db.ProductMasters.Where(x => x.TypeID == PRODUCTTYPE).ToList();
            var dt = Convert.ToDateTime(DATE);

           // var main = db.MainOrder.Where(x =>x.Id==ID).FirstOrDefault();

              var OrderCount = db.Order.Where(x => x.Id == ID  && x.OrderStage==1).ToList();  
           
          
            if(OrderCount.Count!=0)
            {
                var tbldata = OrderCount;
                var result = new { Message = "success", tbldata };
                return Json(result, JsonRequestBehavior.AllowGet);
            }else
            {
                var result = new { Message = "You cannot edit this order..beacause order in processing", };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            


        
        }


        public JsonResult GetCustomerCode(string SHORTNAME,string LOCATION)
        {
            var CustCode = db.CustomerMasters.Where(x => x.ShortName == SHORTNAME && x.Area== LOCATION).Select(x => x.CustID).FirstOrDefault();
            var CustId= db.CustomerMasters.Where(x => x.ShortName == SHORTNAME && x.Area == LOCATION).Select(x => x.ID).FirstOrDefault();
            var result = new { Message = "success", CustCode,CustId };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CalculateWeight(int? PRODUCT, int? QTY)
        {
            var Product = db.ProductMasters.Where(x => x.ID == PRODUCT).FirstOrDefault();
            decimal? weight=null;
            var UOM = "";
            if (Product != null)
            {
                if (Product.ProdUom == "Gm")
                {
                    weight = QTY * Product.Weight / 1000;
                }
                else if(Product.ProdUom==""|| Product.ProdUom==null)
                {
                    weight = null;
                }
                else
                {
                    weight = QTY;
                }

                UOM = Product.ProdUom;
            }
            var result = new { Message = "success", weight, UOM };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save(List<CustomerMaster> Cust,List<Order> recs, string DATE, int CATEGORYID,string CUSTOMERSHORTNAME,  string REMARK, string ExpectedDeliveryDate, string ExpectedDeliveryTime)
        {
            try
            {
                var dt = Convert.ToDateTime(DATE);
                var expdt = DateTime.ParseExact(ExpectedDeliveryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                foreach (var x in Cust)
                {
                    int maxid = 0;

                    var CustomerNM = db.CustomerMasters.Where(a => a.ID == x.ID).OrderBy(a=>a.ID).FirstOrDefault();

                    MainOrder model = new MainOrder();
                    model.CustomerName = CustomerNM.CustName;
                    model.CustomerCode = x.CustID;
                    model.Date = dt;
                    //     model.Id = maxid;
                    model.CustomerId = x.ID;
                    model.Location = x.Area;
                    model.CategoryId = CATEGORYID;
                    model.CustomerShortCode = CUSTOMERSHORTNAME;
                    model.Remark = REMARK;
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;

                    model.ExpectedDeliveryDate = expdt;
                    model.ExpectedDeliveryTime = ExpectedDeliveryTime;
                   
                    model.IsApprove = true;
                    model.ApprovedBy = User.Identity.Name;
                    model.ApprovedDate = DateTime.Now;

                    db.MainOrder.Add(model);
                    db.SaveChanges();
                    maxid = (from p in db.MainOrder
                             select p.Id).Max();


                    var results = new { Message = "success", Max = maxid };
                    if (recs == null)
                    {
                        recs = new List<Order>();
                        results = new { Message = "failed", Max = maxid };
                        return Json(results, JsonRequestBehavior.AllowGet);
                    }

                    //Loop and insert records.
                    foreach (Order order in recs)
                    {
                        TrackingDetails track = new TrackingDetails();
                        track.Status = "Order Created with Qty "+order.Qty;
                        track.Action = "Customer Order";
                        track.Location = "Plant";
                      
                        order.Date = dt;
                        
                        order.MasterOrderId = maxid;
                        order.OrderStage = 1;
                        if (order.Qty == 0)
                        {
                            order.OrderStage = 7;
                            order.OrderStatus = "Order Cancelled";
                            order.PackagingRemark = "Order Cancelled by " + User.Identity.Name; ;
                            track.Status = order.OrderStatus;
                        }
                        order.RejectedQty = 0;
                        db.Order.Add(order);
                        db.SaveChanges();
                       var  Ordermaxid = (from p in db.Order
                                 select p.Id).Max();

                        
                        track.OrderNo = Convert.ToInt32(Ordermaxid);
                        track.Date = DateTime.Today;
                        track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                        track.CreatedDate = DateTime.Now;
                        track.CreatedBy = User.Identity.Name;
                        db.trackingDetails.Add(track);

                    }

                    db.SaveChanges();
                    //     dbTran.Commit();
                }


                   var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);

          //  }
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message, Max = 0 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Update(List<Order> recs, string DATE, int CATEGORYID, string CUSTOMERSHORTNAME, string LOCATION, string CUSTID, int? ID, string REMARK, string ExpectedDeliveryDate, string ExpectedDeliveryTime, int MainID) 
        {
            try
            {
                foreach (Order x in recs) 
                {
                    var Orders = db.Order.Where(a => a.Id == MainID).FirstOrDefault();
                    var OldQty = Orders.Qty;
                    var dt = Orders.Date;
                 //   Orders.OrderStage = 1;
                    if (x.Qty == 0)
                    {
                        Orders.Qty = x.Qty;
                        Orders.ProductId = x.ProductId;
                        Orders.ProductName = x.ProductName;
                        Orders.UOM = x.UOM;
                        Orders.Weight = x.Weight;
                        Orders.OrderStage = 7;
                        Orders.OrderStatus = "Order Cancelled";
                        Orders.PackagingRemark = "Order Cancelled by " + User.Identity.Name; ;


                        TrackingDetails track = new TrackingDetails();
                        track.Status = Orders.OrderStatus;
                        track.Remark = Orders.PackagingRemark;
                        track.Action = "Customer Order";
                        track.Location = "Plant";
                        track.OrderNo = Orders.Id;
                        track.Date = DateTime.Today;
                        track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                        track.CreatedDate = DateTime.Now;
                        track.CreatedBy = User.Identity.Name;
                        db.trackingDetails.Add(track);


                    }
                    else
                    {
                        if (OldQty != x.Qty)
                        {
                            Orders.Qty = x.Qty;
                            Orders.ProductId = x.ProductId;
                            Orders.ProductName = x.ProductName;
                            Orders.UOM = x.UOM;
                            Orders.Weight = x.Weight;
                            Orders.OrderStage = 1;
                            Orders.OrderStatus = null;
                            Orders.PackagingRemark = null;

                            TrackingDetails track = new TrackingDetails();
                            track.Status = "Quantity Changed "+ OldQty +" to "+x.Qty;
                            track.Action = "Customer Order";
                            track.Location = "Plant";
                            track.OrderNo = Orders.Id;
                            track.Date = DateTime.Today;
                            track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                            track.CreatedDate = DateTime.Now;
                            track.CreatedBy = User.Identity.Name;
                            db.trackingDetails.Add(track);

                        }                       
                    }
         
                    //db.Order.Add(x);


                }
                db.SaveChanges();
                //     dbTran.Commit();

               var result = new { Message = "success" }; 
                return Json(result, JsonRequestBehavior.AllowGet);

                //  }
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message, Max = 0 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        //[HttpPost]
        //public ActionResult Save(List<Order> recs,string DATE, int CATEGORYID, string CUSTOMERSHORTNAME,string LOCATION,string CUSTID,int? ID, string REMARK, string ExpectedDeliveryDate,string ExpectedDeliveryTime,int MainID)
        //{
        //    try
        //    {
        //        if (MainID == 0)
        //        {
        //            var dt = Convert.ToDateTime(DATE);
        //            var expdt = DateTime.ParseExact(ExpectedDeliveryDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //            int maxid = 0;
        //            try
        //            {
        //                maxid = db.MainOrder.Select(x => x.Id).Max();
        //            }
        //            catch (Exception e)
        //            {
        //                maxid = 0;
        //            }

        //            if (maxid == null || maxid == 0)
        //            {
        //                maxid = 0;
        //            }

        //            maxid = maxid + 1;
        //            var Custid = db.CustomerMasters.Where(a => a.CustID == CUSTID).Select(a => a.ID).SingleOrDefault();
        //            MainOrder model = new MainOrder();
        //            model.CustomerCode = CUSTID;
        //            model.Date = dt;
        //            model.Id = maxid;
        //            model.CustomerId = Custid;
        //            model.Location = LOCATION;
        //            model.CategoryId = CATEGORYID;
        //            model.CustomerShortCode = CUSTOMERSHORTNAME;
        //            model.Remark = REMARK;
        //            model.CreatedBy = User.Identity.Name;
        //            model.CreatedDate = DateTime.Now;

        //            model.ExpectedDeliveryDate = expdt;
        //            model.ExpectedDeliveryTime = ExpectedDeliveryTime;
        //            if (!User.IsInRole("Customer"))
        //            {
        //                model.IsApprove = true;
        //                model.ApprovedBy = User.Identity.Name;
        //                model.ApprovedDate = DateTime.Now;
        //            }

        //            db.MainOrder.Add(model);
        //            db.SaveChanges();


        //            List<Order> Products = new List<Order>();

        //            var result = new { Message = "success", Max = maxid };
        //            if (recs == null)
        //            {
        //                recs = new List<Order>();
        //                result = new { Message = "failed", Max = maxid };
        //                return Json(result, JsonRequestBehavior.AllowGet);
        //            }

        //            //Loop and insert records.
        //            foreach (Order order in recs)
        //            {

        //                order.Date = dt;
        //                order.MasterOrderId = maxid;
        //                order.OrderStage = 1;
        //                order.Weight = order.Weight;
        //                db.Order.Add(order);
        //                db.SaveChanges();

        //            }
        //            var results = new { Message = "success", Max = maxid };
        //            return Json(results, JsonRequestBehavior.AllowGet); 

        //        }
        //        else
        //        {
        //            var entityList = db.Order.Where(x => x.MasterOrderId == MainID).ToList();
        //            db.Order.RemoveRange(entityList);
        //            db.SaveChanges();
        //            var maindata = db.MainOrder.Where(a => a.Id == MainID).FirstOrDefault(); 
        //            foreach (Order order in recs)
        //            {
        //                order.Date = maindata.Date;
        //                order.MasterOrderId = MainID;
        //                order.OrderStage = 1;
        //                order.Weight = order.Weight;
        //                db.Order.Add(order);
        //                db.SaveChanges();
        //            }
        //            var result = new { Message = "success" };
        //            return Json(result, JsonRequestBehavior.AllowGet);
        //        }


        //    }
        //    catch (Exception Ex)
        //    {
        //        var result = new { Message = Ex.Message, Max = 0 };
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //}
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

        public ActionResult Export()
        {
            var frmdate = Session["FromDate"];
            var todate = Session["ToDate"];

            var type = "";
            if (Session["Type"] != "")
            {
                type = Session["Type"].ToString();
            }

            var city = "";
            if (Session["City"] != "")
            {
                city = Session["City"].ToString();
            }

            var shrtname = "";
            if (Session["ShortName"] != "")
            {
                shrtname = Session["ShortName"].ToString();
            }

            DateTime? Fromdate = null;
            DateTime? Todate = null;


            if (todate != "")
            {
                Todate = Convert.ToDateTime(todate);
            }
            else
            {
                Todate = DateTime.Today;
            }
            if (todate != "")
            {
                Fromdate = Convert.ToDateTime(frmdate);
            }
            else
            {
                Fromdate = DateTime.Today;
            }
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DownloadPackingDetails"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@FromDt", Fromdate);
                        cmd.Parameters.AddWithValue("@ToDt", Todate);
                        cmd.Parameters.AddWithValue("@CustType", type);
                        cmd.Parameters.AddWithValue("@City", city);
                        cmd.Parameters.AddWithValue("@ShortName", shrtname);
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
                                wb.Worksheets.Add(dt, "SP_DownloadPackingDetails");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";

                                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
                                Response.ContentType = "application/text";
                                Response.AddHeader("content-disposition", "attachment;filename=PackagingDetails.csv");
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

        //public ActionResult Export()
        //{

        //    //using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
        //    //{
        //    //    using (SqlCommand cmd = new SqlCommand("SP_DownloadCustomerOrderMaster"))
        //    //    {
        //    //        using (SqlDataAdapter sda = new SqlDataAdapter())
        //    //        {
        //    //            cmd.CommandType = CommandType.StoredProcedure;
        //    //            cmd.Connection = con;
        //    //            cmd.CommandTimeout = 1800;
        //    //            sda.SelectCommand = cmd;
        //    //            using (DataTable dt = new DataTable())
        //    //            {
        //    //                sda.Fill(dt);
        //    //                con.Close();
        //    //                cmd.Dispose();

        //    //                using (XLWorkbook wb = new XLWorkbook())
        //    //                {
        //    //                    wb.Worksheets.Add(dt, "SP_DownloadCustomerOrderMaster");

        //    //                    Response.Clear();
        //    //                    Response.Buffer = true;
        //    //                    Response.Charset = "";

        //    //                    //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //    //                    //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
        //    //                    Response.ContentType = "application/text";
        //    //                    Response.AddHeader("content-disposition", "attachment;filename=CustomerOrders.csv");
        //    //                    using (MemoryStream MyMemoryStream = new MemoryStream())
        //    //                    {
        //    //                        wb.SaveAs(MyMemoryStream);
        //    //                        MyMemoryStream.WriteTo(Response.OutputStream);
        //    //                        Response.Flush();
        //    //                        Response.End();
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //        RedirectToAction("Index");
        //    //    }
        //    //}

        //    return View();
        //}
    }
}