
using Micraft.ManeGrowAgro.Security;
using Micraft.ManeGrowAgro.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Util;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class CollectionMainController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: CollectionMain
        [CustomAuthorize("You dont have access to View Collection List", "CollectionView, CollectionEdit, Admin")]
        public ActionResult Index()
        {
            try
            {
                string consString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                using (SqlConnection con = new SqlConnection(consString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UpdateAmountAfterRejection"))
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
        [CustomAuthorize("You dont have access to create Collection", "CollectionEdit, Admin")]
     //   [CustomAuthorize("You don't have access to create Company",  "Admin")]
        public ActionResult Create()
        {
            ViewBag.CustID = db.CustomerMasters.OrderBy(a => a.ID).ToList();
            return View();
        }

        public ActionResult GetInvoiceDetails(int CustomerID)
        {
            try
            {                
                var orders = db.MainOrder.Where(a => a.CustomerId == CustomerID && a.FinalAmount != a.OldReceiptAmount && a.IsPODUpload==true).ToList();
                var result = new { Message = "success", orders };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(/*List<MainOrder> Cust,*/ List<CollectionDetails> recs, string DATE,int? ID, int? CustomerID, string PaymentMode, decimal? Amount, string PaymentDetails, int? CollectionID, string TransactionID, DateTime? TransactionDate, int? Mainid)
        {

            var paymentdt = DateTime.ParseExact(DATE, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            //var Mainid = db.MainOrder.Where(a => a.CustomerId == CustomerID).Select(a => a.InvoiceNo).ToList();
            //var orders = db.MainOrder.Where(a => Mainid.Contains(a.InvoiceNo)).ToList();

            using (DbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                int maxid = 0;
                if (CollectionID == null)
                {
                    CollectionMain cm = new CollectionMain();
                    cm.CustID = CustomerID;
                    cm.PaymentDate = paymentdt;
                    cm.PaymentMode = PaymentMode;
                    cm.PayAmount = Amount;
                    cm.PaymentDetails = PaymentDetails;
                    cm.TransactionID = TransactionID;
                    cm.TransactionDate = TransactionDate;
                    cm.CreatedBy = User.Identity.Name;
                     cm.CreatedDate = DateTime.Today;

                    db.CollectionMains.Add(cm);
                    db.SaveChanges();
                    maxid = db.CollectionMains.Select(x => x.ID).Max();
                    

                }
                else
                {
                  
                    var cm = db.CollectionMains.Where(a => a.ID == CollectionID).FirstOrDefault();
                    cm.CustID = CustomerID;
                    cm.PaymentDate = paymentdt;
                    cm.PaymentMode = PaymentMode;
                    cm.PayAmount = Amount;
                    cm.PaymentDetails = PaymentDetails;
                    cm.TransactionID = TransactionID;
                    cm.TransactionDate = TransactionDate;
                     //cm.UpdatedBy = User.Identity.Name;
                    // em.UpdatedDate = DateTime.Today;

                }

                foreach (var x in recs)
                {
                    if (CollectionID == null)
                    {
                        var cd = new CollectionDetails();
                        
                        cd.InvoiceNo = x.InvoiceNo;
                        cd.Amount = x.Amount;
                        cd.InvoiceDate = x.InvoiceDate;
                        cd.OldReceiptAmount = x.OldReceiptAmount;
                        cd.ReceivableAmount = x.ReceivableAmount;
                        cd.RecdAmount = x.RecdAmount;
                        cd.AdjustAmount = x.AdjustAmount;
                        cd.CollectionID = maxid;

                        var ordermain = db.MainOrder.Where(a => a.Id == x.Mainid).FirstOrDefault();
                        if(ordermain.OldReceiptAmount==null)
                        {
                            ordermain.OldReceiptAmount = 0;
                        }
                        ordermain.OldReceiptAmount = ordermain.OldReceiptAmount + x.OldReceiptAmount;
                        //ordermain.ReceivableAmount = x.ReceivableAmount;
                        //ordermain.RecdAmount = x.RecdAmount;
                        ordermain.AdjustAmount = x.AdjustAmount;


                        db.CollectionDetails.Add(cd);
                       // db.MainOrder.Add(ordermain);
                        db.Entry(ordermain).State = EntityState.Modified;
                    }
                    else
                    {
                        var cd = db.CollectionDetails.Where(a => a.ID == x.CollectionID).FirstOrDefault();
                        

                        cd.InvoiceNo = x.InvoiceNo;
                        cd.Amount = x.Amount;
                        cd.InvoiceDate = x.InvoiceDate;
                        cd.OldReceiptAmount = x.OldReceiptAmount;
                        cd.ReceivableAmount = x.ReceivableAmount;
                        cd.RecdAmount = x.RecdAmount;
                        cd.AdjustAmount = x.AdjustAmount;

                        var ordermain = db.MainOrder.Where(a => a.CustomerId == x.Mainid).FirstOrDefault();
                        ordermain.OldReceiptAmount = x.OldReceiptAmount;
                        //ordermain.ReceivableAmount = x.ReceivableAmount;
                        //ordermain.RecdAmount = x.RecdAmount;
                        ordermain.AdjustAmount = x.AdjustAmount;

                        // db.Entry(ordermain).State = EntityState.Modified;
                        // x.CollectionID = maxid;
                    }

                }

                db.SaveChanges();
                dbTran.Commit();
            }

            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [CustomAuthorize("You dont have access to Delete Collection Record", "CollectionEdit, Admin")]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var collectionm = db.CollectionMains.Where(a => a.ID == id).FirstOrDefault();
                db.CollectionMains.Remove(collectionm);

                var collectiond = db.CollectionDetails.Where(a => a.CollectionID == id).ToList();
                db.CollectionDetails.RemoveRange(collectiond);


                db.SaveChanges();
                TempData["success"] = "Record Deleted Success!";
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
        

        
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecords = db.CollectionMains.Count();

                var result = (from dr in db.CollectionMains

                              join d in db.CustomerMasters
                              on dr.CustID equals d.ID

                              select new
                              {
                                  ID = dr.ID,
                                  CustID = d == null ? string.Empty : d.CustName,
                                  PaymentDate = dr.PaymentDate,
                                  PaymentMode = dr.PaymentMode,
                                  PayAmount = dr.PayAmount,
                                  PaymentDetails = dr.PaymentDetails,
                                  TransactionID = dr.TransactionID,
                                  TransactionDate = dr.TransactionDate,
                                  CreatedBy = dr.CreatedBy,
                                  //ReceivedQty = dr.ReceivedQty,
                                  //Rejected = dr.RejectedQty,

                              }).OrderBy(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = result.Where(a => a.CustID.ToLower().Contains(sSearch)).OrderByDescending(a => a.ID).ToList();
                }
                else
                {
                    result = result.OrderByDescending(a => a.PaymentDate).ToList();
                }
                if (User.IsInRole("admin"))
                {
                    result = result.Where(a => a.CreatedBy == User.Identity.Name).ToList();
                }

                StringBuilder sb = new StringBuilder();
                sb.Clear();
                sb.Append("{");
                sb.Append("\"sEcho\":");
                sb.Append(sEcho);
                sb.Append(",");
                sb.Append("\"iTotalRecords\":");
                sb.Append(totalRecords);
                sb.Append(",");
                sb.Append("\"iTotalDisplayRecords\":");
                sb.Append(totalRecords);
                sb.Append(",");
                sb.Append("\"aaData\":");
                sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                sb.Append("}");
                return sb.ToString();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public JsonResult GetEditData(int Id)
        {
            //var orders = db.MainOrder.Where(a => a.CustomerId == CustomerID && a.Amount != a.RecdAmount).ToList();
            //var paymentdt = DateTime.ParseExact(DATE, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var data = (from cm in db.CollectionMains.Where(a => a.ID == Id)
                        join cd in db.CollectionDetails
                        on cm.ID equals cd.CollectionID

                        //join d in db.CustomerMasters
                        //     on cm.CustID equals d.ID

                        select new
                        {

                            //CollectionMain
                            ID = cm.ID,
                            //CustID = cm == null ? string.Empty : d.CustName,
                            CustID = cm.CustID,
                            PaymentDate = cm.PaymentDate,
                            PaymentMode = cm.PaymentMode,
                            PayAmount = cm.PayAmount,
                            PaymentDetails = cm.PaymentDetails,
                            TransactionID = cm.TransactionID,
                            TransactionDate =cm.TransactionDate,
                            //CollectionDetails
                            CollectionID = cd.CollectionID,
                            InvoiceNo = cd.InvoiceNo,
                            InvoiceDate = cd.InvoiceDate,
                            Amount = cd.Amount,
                            OldReceiptAmount=cd.OldReceiptAmount,
                            ReceivableAmount=cd.ReceivableAmount,
                            RecdAmount=cd.RecdAmount,
                            AdjustAmount=cd.AdjustAmount,

                            }).OrderByDescending(x => x.CustID).ToList();
                            var result = new { Message = "success", data };
                            return Json(result, JsonRequestBehavior.AllowGet);
    }

   
}
}