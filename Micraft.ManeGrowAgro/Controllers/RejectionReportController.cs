using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class RejectionReportController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: RejectionReport
        public ActionResult Index()
        {
            //  ViewBag.VendorName = db.DispatchTransactions.Where(a => a.VendorName == "ashdfahdfahdgs").ToList();
            return View();
        }
        public ActionResult SetDates(string FROMDATE, string TODATE,string RejectionReason) 
        {

            Session["Fromdate"] = FROMDATE;
            Session["Todate"] = TODATE;
            Session["RejectionReason"] = RejectionReason;

            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {

                var FROMDATE = Session["Fromdate"];
                var TODATE = Session["Todate"];
                var RejectionReason = Session["RejectionReason"]; 

                DateTime? fromd = Convert.ToDateTime(FROMDATE);
                DateTime? tod = Convert.ToDateTime(TODATE);

                var result = (from p in db.PODUpdations.Where(a => a.Date >= fromd && a.Date <= tod &&( a.RejectedQty !=0 && a.RejectedQty !=null))
                
                                    join History in db.RejectionSoldHistory on p.ID equals History.RejectionID into RejectionSoldHistory
                                    from x in RejectionSoldHistory.DefaultIfEmpty()

                                    join orderdetails in db.Order on p.OrderNo equals orderdetails.Id into orderdetail
                                    from vr in orderdetail.DefaultIfEmpty()

                                    join VendorMasters in db.VendorMasters on p.CreatedBy equals VendorMasters.UserName into VendorMaster 
                                    from v in VendorMaster.DefaultIfEmpty()

                                    join CustomerMasters in db.CustomerMasters on p.CustID equals CustomerMasters.ID into CustomerMaster
                                    from c in CustomerMaster.DefaultIfEmpty() 

                                    join CustomerTypes in db.CustomerTypes on c.CustTypeID equals CustomerTypes.ID into CustomerType
                                    from ct in CustomerType.DefaultIfEmpty()

                              join Productss in db.ProductMasters on vr.ProductId equals Productss.ID into Products
                              from prd in Products.DefaultIfEmpty() 

                              orderby p.ID descending
                                    select new
                                    {          
                                        CustName = c.CustName,
                                        CustCode = c.CustID,
                                        CustType = ct.Type,
                                        Location = c.Area,
                                        ProductName = p.ProductName,
                                        DispatchQty = p.Qty,
                                        DeliveredQty = p.ReceivedQty,
                                        RejectedQty = p.RejectedQty,
                                        SoldQty = x == null ? 0 : x.Qty,
                                        // SoldQty = x.Qty,
                                        Rate = x == null ? 0 : x.Rate,  
                                       // Rate = x.Rate,
                                      //  SoldCustomerName = x.SoldCustomerName,
                                        SoldCustomerName = x == null ? string.Empty : x.SoldCustomerName,
                                        RejectReason = x == null ? string.Empty : x.ResaleType, 
                                      //  RejectReason = x.ResaleType,
                                        BillNo = p.InvoiceNo,
                                        Remark = "",

                                        DispatchRate = vr.Rate,
                                        DispatchAmount = vr.Amount,
                                        DeliveredAmount = vr.Rate * (vr.Qty - vr.RejectedQty),
                                        RejectedAmount = vr.Rate * vr.RejectedQty,
                                      //  SoldAmount = x.TotalAmount,
                                        SoldAmount = x == null ? 0 : x.TotalAmount, 
                                        Date = vr.Date,
                                        HCreatedDate = x.CreatedDate, 
                                        HCreatedBy = x == null ? string.Empty : x.CreatedBy, 

                                        CreatedDate = p.CreatedDate,
                                        CreatedBy = p == null ? string.Empty : p.CreatedBy, 
                                        OrderNo = p.OrderNo,
                                        DamageReason = p.DamageReason,
                                        ScrapQty="",
                                        ProdWT = prd == null ? 0 : prd.Weight,

                                    }).ToList();

                if(RejectionReason.ToString()== "Rejection History Added")
                {
                    result = result.Where(a => a.RejectReason != "" && a.RejectReason != null).ToList();


                    var Orderss = result.Where(a => a.Date >= fromd && a.Date <= tod).ToList();

                    var distorders = Orderss.Select(a => a.OrderNo).Distinct().ToList();
                    List<TempOrderDetails> tempOrderDetails = new List<TempOrderDetails>();
                    foreach (var xx in distorders)
                    {
                        var temp = 0;
                        var allorders = Orderss.Where(a => a.OrderNo == xx).ToList();
                        foreach (var x in allorders)
                        {
                            var tmp = new TempOrderDetails();
                            if (temp != 0)
                            {
                                tmp.DispatchQty = 0;
                                tmp.DeliveredQty = 0;
                                tmp.RejectedQty = 0;

                                //tmp.Rate = 0;
                                tmp.Rate = x.Rate;
                                tmp.DispatchAmount = 0;
                                tmp.DeliveredAmount = 0;
                                tmp.RejectedAmount = 0;


                            }
                            else
                            {
        //                         
       
                                tmp.DispatchQty = x.DispatchQty;
                                tmp.DispWt = x.ProdWT * x.DispatchQty;

                                tmp.DeliveredQty = x.DeliveredQty;
                                tmp.DelWt = x.ProdWT * x.DeliveredQty;
                                tmp.RejectedQty = x.RejectedQty;
                                tmp.RejWt = x.ProdWT * x.RejectedQty; 

                                tmp.Rate = x.Rate;
                                tmp.DispatchAmount = x.DispatchAmount;
                                tmp.DeliveredAmount = x.DeliveredAmount;
                                tmp.RejectedAmount = x.RejectedAmount;
                            }
                            temp++;
                            tmp.BillNo = x.BillNo;
                            tmp.CustName = x.CustName;
                            tmp.CustCode = x.CustCode;
                            tmp.CustType = x.CustType;
                            tmp.Location = x.Location;
                            tmp.ProductName = x.ProductName;
                          
                            if(x.RejectReason=="SOLD")
                            {
                                tmp.SoldQty = x.SoldQty;
                                tmp.SoldWt = x.ProdWT * x.SoldQty;

                            }
                            else
                            {
                                tmp.ScrapQty = x.SoldQty; 
                            }
                            tmp.RejectReason = x.RejectReason;
                            tmp.Remark = x.Remark;
                            tmp.DispatchRate = x.DispatchRate;
                            tmp.SoldCustomerName = x.SoldCustomerName;
                            tmp.SoldAmount = x.SoldAmount;
                            tmp.Date = x.Date;
                            tmp.CreatedDate = x.HCreatedDate;
                            tmp.CreatedBy = x.HCreatedBy;
                            tmp.OrderNo = x.OrderNo;
                            tmp.DamageReason = x.DamageReason; 

                            tempOrderDetails.Add(tmp);
                        }
                    }


                    var totalRecord1 = tempOrderDetails.Count();
                    StringBuilder sb1 = new StringBuilder();

                    sb1.Clear();
                    sb1.Append("{");
                    sb1.Append("\"sEcho\": ");
                    sb1.Append(sEcho);
                    sb1.Append(",");
                    sb1.Append("\"iTotalRecords\": ");
                    sb1.Append(totalRecord1);
                    sb1.Append(",");
                    sb1.Append("\"iTotalDisplayRecords\": ");
                    sb1.Append(totalRecord1);
                    sb1.Append(",");
                    sb1.Append("\"aaData\": ");
                    sb1.Append(Newtonsoft.Json.JsonConvert.SerializeObject(tempOrderDetails));
                    sb1.Append("}");
                    return sb1.ToString(); 

                }

                if (RejectionReason.ToString() == "Rejection History Not Added")
                {
                    result = result.Where(a => a.RejectReason == "" || a.RejectReason == null).ToList();
                }

                var totalRecord = result.Count();
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

      
       
    }

    internal class TempOrderDetails
    {
      
         public string CustName { get; set; }
        public string CustCode { get; set; }
        public string CustType { get; set; }

        public string Location { get; set; }
        public string ProductName { get; set; }
        public int? DispatchQty { get; set; }
        public int? DeliveredQty { get; set; }
        public int? RejectedQty { get; set; }
        public int? SoldQty { get; set; }
        public decimal? Rate { get; set; }
        public string SoldCustomerName { get; set; }

        public string RejectReason { get; set; }
        public string BillNo { get; set; }
        public string Remark { get; set; }
        public decimal? DispatchRate { get; set; }
        public decimal? DispatchAmount { get; set; }
        public decimal? DeliveredAmount { get; set; }
        public decimal? RejectedAmount { get; set; }

        public decimal? SoldAmount { get; set; }

        public DateTime? Date { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public int? OrderNo { get; set; }



        public DateTime? HCreatedDate { get; set; }

        public string HCreatedBy { get; set; }

        public string DamageReason { get; set; }

        public int? ScrapQty { get; set; }
        public decimal? ProdWT { get; set; }


        public decimal? DispWt { get; set; }

        public decimal? DelWt { get; set; }


        public decimal? RejWt { get; set; } 

        public decimal? SoldWt { get; set; }

        public decimal? TRO_ScrapWt { get; set; }

    }
}