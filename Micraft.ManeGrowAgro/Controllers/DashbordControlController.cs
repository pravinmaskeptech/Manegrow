
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
using System.Data.SqlClient;
using System.Configuration;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Micraft.ManeGrowAgro.Security;
using System.Data.Entity.Core.EntityClient;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class DashbordControlController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: DashbordControl

        [CustomAuthorize("You dont have access to View Packing", "PackingEdit,PackingView, Admin")]
        public ActionResult Index()
        {

            Session["FromDate"] = "";
            Session["ToDate"] = "";
            Session["Type"] = "";
            Session["City"] = "";
            Session["ShortName"] = "";
            Session["SalesPerson"] = "";

            var CustType = db.CustomerTypes.ToList();
            if (CustType != null) { ViewBag.CustType = CustType; }


            var SalesID = db.DepartmentMasters.Where(a => a.Department.Contains("SALES")).Select(a => a.ID).ToList();
            var SalesPerson = db.EmployeeMasters.Where(a => SalesID.Contains(a.DepartmentID)).ToList();
            if (SalesPerson != null) { ViewBag.SalesPerson = SalesPerson; }

            ViewBag.City = db.CustomerMasters.Select(x => new { x.City, x.ID }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
            ViewBag.Shortname = db.CustomerMasters.Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            ViewBag.Product = db.ProductMasters.OrderBy(a => a.Name).ToList();

            ViewBag.ProductType = db.ProductTypes.OrderBy(a => a.Type).ToList();
            ViewBag.BoxType = db.BoxTypeMaster.OrderBy(a => a.BoxType).ToList();
            return View();
        }

        public JsonResult GetProducts(int PrdType)
        {
            var prd = db.ProductMasters.Where(a => a.TypeID == PrdType).ToList();
            var result = new { Message = "success", prd };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public ActionResult SetDates(string FROMDATE, string TODATE, string TYPE, string CITY, string SHORTNAME, string SalesPerson, string Product, int? ProductType)
        {
            Session["FromDate"] = FROMDATE.ToString();
            Session["ToDate"] = TODATE.ToString();
            Session["Type"] = TYPE;
            Session["City"] = CITY;
            Session["ShortName"] = SHORTNAME;
            Session["SalesPerson"] = SalesPerson;
            Session["Product"] = Product;
            Session["ProductType"] = ProductType;

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
                    type = Session["Type"].ToString();
                }

                var Product = "";
                if (Session["Product"] != "")
                {
                    Product = Session["Product"].ToString();
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
                var salespName = "";
                if (Session["SalesPerson"] != "")
                {
                    salespName = Session["SalesPerson"].ToString();
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


                var ProductType = 0;
                if (Session["ProductType"] != "" && Session["ProductType"] != null)
                {
                    var ProductType1 = Session["ProductType"].ToString();
                    ProductType = Convert.ToInt32(ProductType1);
                }

                var Orders = db.Order.ToList();
                int totalRecord = 0;

            
                var result1 = (from dr in db.MainOrder
                               join dp in db.CustomerMasters
                               on dr.CustomerId equals dp.ID

                               join ds in db.CustomerTypes
                               on dp.CustTypeID equals ds.ID

                               join e in db.Order
                               on dr.Id equals e.MasterOrderId

                               join prd in db.ProductMasters
                               on e.ProductId equals prd.ID

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
                                   ProductTypeID = prd.TypeID

                               }).OrderBy(x => x.OrderStage).ToList();
               
                if (Fromdate != null && Todate != null)
                {
                    result1 = result1.Where(a => (a.Date >= Fromdate && a.Date <= Todate)).ToList();
                }
                else
                {
                    var fdt = DateTime.Today;
                    //  var fdt = Tdt.AddDays(-1);
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

                if (salespName != "")
                {
                    result1 = result1.Where(a => a.CreatedBy.ToLower() == salespName.ToLower()).ToList();
                }

                if (Product != "")
                {
                    result1 = result1.Where(a => a.ProductName.ToLower() == Product.ToLower()).ToList();
                }

                if (ProductType != 0)
                {
                    result1 = result1.Where(a => a.ProductTypeID == ProductType).ToList();
                }
                var result = result1;



                StringBuilder reponseBuilder = new StringBuilder();
                reponseBuilder.Clear();
                reponseBuilder.Append("{");
                reponseBuilder.Append("\"sEcho\": ");
                reponseBuilder.Append(sEcho);
                reponseBuilder.Append(",");
                reponseBuilder.Append("\"iTotalRecords\": ");
                reponseBuilder.Append(totalRecord);
                reponseBuilder.Append(",");
                reponseBuilder.Append("\"iTotalDisplayRecords\": ");
                reponseBuilder.Append(totalRecord);
                reponseBuilder.Append(",");
                reponseBuilder.Append("\"aaData\": ");
                reponseBuilder.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                reponseBuilder.Append("}");
                return reponseBuilder.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JsonResult GetPopupData(int ID)
        {

            var order = (from e in db.Order.Where(a => a.Id == ID)

                         join dr in db.MainOrder
                          on e.MasterOrderId equals dr.Id

                         join cus in db.CustomerMasters
                         on dr.CustomerId equals cus.ID

                         join pm in db.ProductMasters
                        on e.ProductId equals pm.ID

                         select new
                         {
                             ID = e.Id,
                             UOM = e.UOM,
                             Qty = e.Qty,
                             Weight = e.Weight,
                             Date = dr.Date,
                             CustName = cus.CustName,
                             CustomerCode = dr.CustomerCode,
                             ProductName = string.Concat(e.ProductName, " ", pm.Weight, " ", pm.ProdUom),
                             ProductId = e.ProductId,
                             OrderStatus = e.OrderStatus,

                         }).OrderByDescending(x => x.ID).FirstOrDefault();


            var result = new { Message = "success", order };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]

        [CustomAuthorize("You dont have access to Update Packing", "PackingEdit, Admin")]
        public JsonResult updateqty(Order order)
        {
            try
            {
                //var entityList = db.PackingDetails.Where(x => x.OrderNo == order.Id).ToList();
                //db.PackingDetails.RemoveRange(entityList);
                //db.SaveChanges();

                var model = db.Order.Where(x => x.Id == order.Id).FirstOrDefault();

                var OrderMain = db.MainOrder.Where(a => a.Id == model.MasterOrderId).FirstOrDefault();
                PackingHistory history = new PackingHistory();
                history.SalesOrderNo = model.Id;
                history.CustomerID = OrderMain.CustomerId;
                history.OrderDate = OrderMain.Date;
                history.CustomerName = order.CustomerName;
                history.ProductName = model.ProductName;
                history.OldQty = order.OldQty;
                history.NewQty = model.Qty;
                history.Remark = order.PackagingRemark;
                history.CreatedBy = User.Identity.Name;
                history.CreatedDate = DateTime.Today;


                db.packingHistory.Add(history);
                db.SaveChanges();

                //TempData["success"] = "Record Update success!";
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CalculateWeight(int? PRODUCT, int? QTY)
        {
            var Product = db.ProductMasters.Where(x => x.ID == PRODUCT).FirstOrDefault();
            decimal? weight = null;
            if (Product != null)
            {
                if (Product.ProdUom == "Gm")
                {
                    weight = QTY * Product.Weight / 1000;
                }
                else if (Product.ProdUom == "" || Product.ProdUom == null)
                {
                    weight = null;
                }
                else
                {
                    weight = QTY;
                }

            }
            var result = new { Message = "success", weight };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [CustomAuthorize("You dont have access to save Packing", "PackingEdit, Admin")]
        public ActionResult Save(List<PackingDetails> recs, int OrderNo, int PrevQty, int OrderQty, int? OldQty, string PackagingRemark, decimal OrderWT)
        {
            try
            {
                var entityList = db.PackingDetails.Where(x => x.OrderNo == OrderNo).ToList();
                db.PackingDetails.RemoveRange(entityList);

                // Save changes
                db.SaveChanges();


                //Loop and insert records.
                //var Order = db.Order.Where(a => a.Id == OrderNo).FirstOrDefault();
                //
                var masterid = db.Order.Where(a => a.Id == OrderNo).FirstOrDefault();
                masterid.OrderStage = 3;

                var isvalidforpacking = false;


                if (masterid != null)
                {
                    masterid.PackagingRemark = null;
                    if (Convert.ToInt32(PrevQty) == Convert.ToInt32(OrderQty) && OrderQty != 0)
                    {
                        masterid.OrderStatus = "Packing Completed";
                        masterid.PackagingRemark = "";
                        masterid.OrderStage = 2;
                        masterid.Qty = OrderQty;
                        masterid.Weight = OrderWT;
                        isvalidforpacking = true;
                    }
                    else
                    {

                        if (Convert.ToInt32(OrderQty) == 0)
                        {
                            masterid.OrderStatus = "Order Cancelled";
                            masterid.PackagingRemark = PackagingRemark;
                            masterid.OldQty = OldQty;
                            masterid.Qty = OrderQty;
                            masterid.Weight = OrderWT;
                            masterid.OrderStage = 7;

                        }
                        else
                        {
                            if (Convert.ToInt32(OrderQty) != Convert.ToInt32(OldQty) && Convert.ToInt32(OrderQty) != 0)
                            {
                                masterid.OrderStatus = "Quantity Changed & Packing Completed";
                                masterid.PackagingRemark = PackagingRemark;
                                masterid.OldQty = OldQty;
                                masterid.Qty = OrderQty;
                                masterid.Weight = OrderWT;
                                masterid.OrderStage = 3;
                                isvalidforpacking = true;
                            }

                        }

                    }

                    if (isvalidforpacking == true)
                    {
                        foreach (var x in recs)
                        {
                            var order = new PackingDetails();
                            order.OrderNo = OrderNo;
                            order.NoOfBox = x.NoOfBox;
                            order.BoxWeight = x.BoxWeight;
                            order.TotalWeight = x.TotalWeight;
                            order.CreatedBy = User.Identity.Name;
                            order.CreatedDate = DateTime.Today;
                            order.UOM = x.UOM;
                            order.BoxType = x.BoxType;
                            db.PackingDetails.Add(order);
                        }
                    }
                }

                TrackingDetails track = new TrackingDetails();
                if (masterid.OrderStatus == "Quantity Changed & Packing Completed")
                {
                    track.Status = masterid.OrderStatus + " " + OldQty + " to " + OrderQty;
                }
                else
                {
                    track.Status = masterid.OrderStatus;
                }

                track.Action = "Packing Details";
                track.Location = "Plant";
                track.OrderNo = masterid.Id;
                track.Date = DateTime.Today;
                track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                track.CreatedDate = DateTime.Now;
                track.CreatedBy = User.Identity.Name;
                db.trackingDetails.Add(track);



                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message, Max = 0 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPreviousRecords(int? OrderNo)
        {

            var main = db.Order.Where(x => x.Id == OrderNo).FirstOrDefault();


            List<PackingDetails> Products = new List<PackingDetails>();

            if (main != null)
            {
                Products = db.PackingDetails.Where(x => x.OrderNo == main.Id).ToList();
            }

            var tbldata = Products;
            return Json(tbldata, JsonRequestBehavior.AllowGet);
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

        public JsonResult GetBulkData(List<MainOrder> OrderData)
        {
            decimal? TotalWT = 0;
            int? TotalQty = 0;
            var data = OrderData.Select(a => a.Id).ToList();
            var Orderlist = db.Order.Where(a => data.Contains(a.Id)).ToList();
            var orders = Orderlist.Where(a => a.OrderStatus == "dispatched" || a.OrderStatus == "Order Dispatched" || a.OrderStatus == "Delivered" || a.OrderStatus == "Order Cancelled" || a.OrderStatus == "Quantity Changed & Packing Completed" || a.OrderStatus == "Packing Completed" || a.OrderStatus == "Merge & Packing Completed").OrderBy(a => a.Id).ToList();
            if (orders.Count == 0)
            {
                //  var lstOrdermainID= Orderlist.GroupBy(x => x.MasterOrderId).Select(x => ( MasterOrderId: x.Select(p => p.MasterOrderId))).ToList();
                var lstOrdermainID = (from table in Orderlist
                                      group new { table.MasterOrderId } by new { table.MasterOrderId } into grp
                                      orderby grp.Key.MasterOrderId descending
                                      select new
                                      {
                                          ID = grp.Key.MasterOrderId
                                      }).ToList();


                var sss = lstOrdermainID.Select(a => a.ID).ToList();
                var results = (from table in db.MainOrder.Where(a => sss.Contains(a.Id))
                               group new { table.CustomerCode } by new { table.CustomerCode } into grp
                               orderby grp.Key.CustomerCode descending
                               select new
                               {
                                   ID = grp.Key.CustomerCode
                               }).ToList();
                if (results.Count() != 1)
                {
                    var result3 = new { Message = "customer name mismatch from selected orders" };
                    return Json(result3, JsonRequestBehavior.AllowGet);
                }
                foreach (var x in OrderData)
                {
                    var order = db.Order.Where(a => a.Id == x.Id).Select(a => new { a.Weight, a.Qty }).FirstOrDefault();
                    TotalWT = TotalWT + order.Weight;
                    TotalQty = TotalQty + order.Qty;
                }
                var result = new { Message = "success", TotalWT, TotalQty };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var message = "";
                foreach (var x in orders)
                {
                    message = message + "," + "Order No : " + x.Id + " Already " + x.OrderStatus + "\n";
                }
                var result = new { Message = message, };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [CustomAuthorize("You dont have access to save Packing", "PackingEdit, Admin")]
        public ActionResult BSave(List<PackingDetails> recs, List<Order> OrderNo, int OrderQty, decimal OrderWT, string MergeOrderNo)
        {
            try
            {
                var temp = false;
                var mergeNo = 0;
                foreach (var xx in OrderNo)
                {
                    var masterid = db.Order.Where(a => a.Id == xx.Id).FirstOrDefault();
                    masterid.OrderStage = 3;

                    masterid.PackagingRemark = null;
                    masterid.OrderStatus = "Merge & Packing Completed";
                    masterid.PackagingRemark = "Merge with Order No " + MergeOrderNo;
                    masterid.OrderStage = 6;
                    if (temp == false)
                    {

                        masterid.MergeOrderNo = xx.Id;
                    }
                    else
                    {
                        masterid.MergeOrderNo = mergeNo;

                    }
                    //     masterid.Qty = OrderQty;
                    //     masterid.Weight = OrderWT;


                    foreach (var x in recs)
                    {

                        var pkj = new PackingDetails();
                        pkj.OrderNo = xx.Id;

                        pkj.CreatedBy = User.Identity.Name;
                        pkj.CreatedDate = DateTime.Today;
                        if (temp == false)
                        {
                            pkj.NoOfBox = x.NoOfBox;
                            pkj.BoxWeight = x.BoxWeight;
                            pkj.TotalWeight = x.TotalWeight;
                            pkj.MergeOrderNo = xx.Id;
                            pkj.BoxType = x.BoxType;
                            pkj.UOM = x.UOM;
                            mergeNo = xx.Id;
                        }
                        else
                        {
                            pkj.MergeOrderNo = mergeNo;
                            pkj.NoOfBox = 0;
                            pkj.BoxWeight = 0;
                            pkj.TotalWeight = 0;
                        }
                        db.PackingDetails.Add(pkj);
                    }
                    temp = true;


                    TrackingDetails track = new TrackingDetails();
                    track.Status = "Packing Completed & Merge with Order No " + MergeOrderNo;
                    track.Action = "Packing Details";
                    track.Location = "Plant";
                    track.OrderNo = masterid.Id;
                    track.Date = DateTime.Today;
                    track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                    track.CreatedDate = DateTime.Now;
                    track.CreatedBy = User.Identity.Name;
                    track.Remark = "Merge with Order No " + MergeOrderNo;
                    db.trackingDetails.Add(track);

                }
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message, Max = 0 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult StickerPrint(int OrderNo)
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
                    var pgSize = new iTextSharp.text.Rectangle(321f, 245f);
                    document = new iTextSharp.text.Document(pgSize, 17f, 1f, 2f, 1f);
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
    }

    public class response
    {
        public int ID { get; set; }
        public string UOM { get; set; }

        public int? Qty { get; set; }
        public string Area { get; set; }
        public string ContactPerson { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerTypes { get; set; }
        public string Remark { get; set; }
        public decimal? Weight { get; set; }
        public string ExpectedDeliveryTime { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string City { get; set; }
        public string ShortName { get; set; }
        public DateTime? Date { get; set; }
        public string CustName { get; set; }
        public string ProductName { get; set; }
        public string PackagingRemark { get; set; }
        public string IsPacked { get; set; }
        public string OrderStatus { get; set; }
        public int? OrderStage { get; set; }
        public string CreatedBy { get; set; }
        public string EmpMobile { get; set; }
        public int? ProductTypeID { get; set; }


    }

}