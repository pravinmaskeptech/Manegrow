using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class PODReportController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: PODReport
        public ActionResult Index()
        {
            return View();
        }
       
        public ActionResult SetDates(string FROMDATE, string TODATE,string PODOption)
        {

            Session["Fromdate"] = FROMDATE;
            Session["Todate"] = TODATE;
              Session["PODOption"] = PODOption;

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
                var PODopt = Session["PODOption"].ToString();

                DateTime? fromd = Convert.ToDateTime(FROMDATE);
                DateTime? tod = Convert.ToDateTime(TODATE);
                if (PODopt == "PODUploaded")
                {
                    //  var result = db.PODUpdations.Where(a => a.Date >= fromd && a.Date <= tod).ToList();
                    var result = (from dr in db.PODUpdations.Where(a => a.Date >= fromd && a.Date <= tod && a.PODName != null) 

                                  //join a in db.DamageReasons
                                  //on dr.DamageReasonID equals a.ID

                                  join d in db.CustomerMasters
                                  on dr.CustID equals d.ID

                                  select new
                                  {
                                      Location = d.Area,
                                      ID = dr.ID,
                                      DamageReasonID="",
                                      Date = dr.Date,
                                      OrderNo = dr.OrderNo,
                                      CustID = d == null ? string.Empty : d.CustName,
                                      ProductName = dr.ProductName,
                                      Qty = dr.Qty,
                                      UOM = dr.UOM,
                                      Weight = dr.Weight,
                                      ReceivedQty = dr.ReceivedQty,
                                      Rejected = dr.RejectedQty,
                                      CreatedBy = dr.CreatedBy,
                                      PODName = dr.PODName,

                                  }).OrderByDescending(x => x.Date).ToList();
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


                } else
                {
                   
                    var xx = db.PODNotUploadedVendorList.ToList();
                    
                   
                    var result = (from dr in db.Order.Where(a => a.Date >= fromd && a.Date <= tod && a.PODName == null && a.OrderStatus== "Order Dispatched")

                                  join a in db.MainOrder
                                  on dr.MasterOrderId equals a.Id

                                  join d in db.dispatchDetails
                                  on dr.DispatchID equals d.DispatchID

                                 

                                  select new
                                  {
                                     Location=a.Location,
                                      ID = dr.Id,
                                      DamageReasonID = "",
                                      Date = dr.Date,
                                      OrderNo = dr.Id,
                                      CustID = a.CustomerName,
                                      ProductName = dr.ProductName,
                                      Qty = dr.Qty,
                                      UOM = dr.UOM,
                                      Weight = dr.Weight,
                                      ReceivedQty = 0,
                                      Rejected = 0,
                                      CreatedBy="",
                                      PODName = dr.PODName,
                                      CustomerID = a.CustomerId,
                                  }).ToList();




                   
                    List<PODDATA> P = new List<PODDATA>();

                    foreach (var x in result) 
                    {
                        var vendor = xx.Where(a => a.CustID == x.CustomerID).FirstOrDefault();
                        var prd = new PODDATA();
                        prd.ID = x.ID;
                        prd.DamageReasonID = x.DamageReasonID;
                        prd.Date = x.Date;
                        prd.OrderNo = x.OrderNo;
                        prd.CustID = x.CustID;
                        prd.ProductName = x.ProductName;
                        prd.Qty = x.Qty;
                        prd.UOM = x.UOM;
                        prd.Weight = x.Weight;
                        prd.ReceivedQty = x.ReceivedQty;
                        prd.Rejected = x.Rejected;
                        if (vendor != null)
                        {
                            prd.CreatedBy = vendor.VendorName;
                        }
                           
                        prd.PODName = x.PODName;
                        prd.CustomerID = x.CustomerID;
                        prd.Location = x.Location; 
                        P.Add(prd);

                    }



                    var totalRecord = P.Count();
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
                    sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(P.OrderByDescending(a => a.Date).ToList())) ;
                    sb.Append("}");
                    return sb.ToString();

                }

                    

                

            }
            catch (Exception ex)
            {
                return ex.Message;

            }
        }

    }

    internal class PODDATA
    {
        public int ID { get; set; }
        public string DamageReasonID { get; set; }
        public DateTime? Date { get; set; }
        public int OrderNo { get; set; }
        public string CustID { get; set; }
        public string ProductName { get; set; }
        public int? Qty { get; set; }
        public string UOM { get; set; }
        public decimal? Weight { get; set; }
        public int? ReceivedQty { get; set; }
        public int? Rejected { get; set; }
        public string CreatedBy { get; set; }
        public string PODName { get; set; }
        public int? CustomerID { get; set; }
        public string Location { get; set; } 
    }
}