using Micraft.ManeGrowAgro.Models;
using Micraft.ManeGrowAgro.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class QuotationMainController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();

        [CustomAuthorize("You dont have access to View Quotation", "CustomerOrderView, CustomerOrderEdit, Admin")]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorize("You dont have access to Create Quotation", "CustomerOrderEdit, Admin")]
        public ActionResult Create() 
        {
            ViewBag.ProductTypes = db.ProductTypes.Where(x => x.IsActive == true).ToList();
            return View();
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
               

                var result = db.QuotationMain
                    .AsEnumerable()
                    .Select(x => new  { ID = x.ID, QuotationName = x.QuotationName, QuotationDate = x.QuotationDate}).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.QuotationMain.AsEnumerable()
                        .Where(x => x.QuotationName.ToLower().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new  { ID = x.ID, QuotationName = x.QuotationName, QuotationDate = x.QuotationDate }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.QuotationMain.AsEnumerable()
                        .Select(x => new { ID = x.ID, QuotationName = x.QuotationName, QuotationDate = x.QuotationDate }).OrderByDescending(x => x.ID).ToList();
                }

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


        


        public JsonResult CheckDuplicateQuotationName(int? ID,string QuotationName)
        {
            if(ID==null)
            {
                var tbldata = db.QuotationMain.Where(x => x.QuotationName == QuotationName).Count();
                return Json(tbldata, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var tbldata = db.QuotationMain.Where(x => x.QuotationName == QuotationName && x.ID != ID).Count();
                return Json(tbldata, JsonRequestBehavior.AllowGet);
            }            
        }

        public JsonResult GetProducts(int PRODUCTTYPE )
        {
            var tbldata = db.ProductMasters.Where(x => x.TypeID == PRODUCTTYPE).OrderBy(a=>a.Name).ToList();
            return Json(tbldata, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CalculateWeight(int? PRODUCT, int? QTY)
        {
            var Product = db.ProductMasters.Where(x => x.ID == PRODUCT).FirstOrDefault();            
            
            var result = new { Message = "success", Product };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save(List<QuotationDetails> recs, string QuotationName, string QuotationDate, int? ID)  
        {
            try
            {                
                using (var dbTransaction = db.Database.BeginTransaction())
                {
                    var QuotationDates = DateTime.ParseExact(QuotationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                    int maxid = 0;
                    if (ID == null)
                    {
                        var mains = new QuotationMain();
                        mains.QuotationName = QuotationName;
                        mains.QuotationDate = QuotationDates;
                        mains.CreatedDate = DateTime.Now;
                        mains.CreatedBy = User.Identity.Name;
                        db.QuotationMain.Add(mains);
                        db.SaveChanges();
                        maxid = db.QuotationMain.Select(x => x.ID).Max();

                    }else
                    {                        
                        maxid =Convert.ToInt32(ID);
                    }
                        foreach (var x in recs)
                        {
                            if (x.DetailsID == 0)
                            {
                                var details = new QuotationDetails();
                                details.ProductID = x.ProductID;
                                details.ProductName = x.ProductName;
                                details.ProductID = x.ProductID;

                                details.UOM = x.UOM;
                                details.WTFrom3 = x.WTFrom3;
                                details.WTTo3 = x.WTTo3;
                                details.Rate3 = x.Rate3;

                                details.WTFrom1 = x.WTFrom1;
                                details.WTTo1 = x.WTTo1;
                                details.Rate1 = x.Rate1;

                                details.WTFrom2 = x.WTFrom2;
                                details.WTTo2 = x.WTTo2;
                                details.Rate2 = x.Rate2;

                                details.AddWt = x.AddWt;
                                details.AddRate = x.AddRate;

                                details.ProdCategoryID = x.ProdCategoryID;
                                details.ProdCategory = x.ProdCategory;
                                details.MainID = maxid;
                                db.QuotationDetails.Add(details);
                            }else
                            {
                                var details = db.QuotationDetails.Where(a => a.DetailsID == x.DetailsID).FirstOrDefault();
                                details.ProductID = x.ProductID;
                                details.ProductName = x.ProductName;
                                details.ProductID = x.ProductID;

                                details.UOM = x.UOM;
                                details.WTFrom3 = x.WTFrom3;
                                details.WTTo3 = x.WTTo3;
                                details.Rate3 = x.Rate3;

                                details.WTFrom1 = x.WTFrom1;
                                details.WTTo1 = x.WTTo1;
                                details.Rate1 = x.Rate1;

                                details.WTFrom2 = x.WTFrom2;
                                details.WTTo2 = x.WTTo2;
                                details.Rate2 = x.Rate2;

                                details.AddWt = x.AddWt;
                                details.AddRate = x.AddRate;

                                details.ProdCategoryID = x.ProdCategoryID;
                                details.ProdCategory = x.ProdCategory;
                                
                            }
                        }
                   
                    db.SaveChanges();
                    dbTransaction.Commit();
                    
                }
                var result = new { Message = "success",};
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message, Max = 0 };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Cities/Delete/5
        [HttpGet]
        [CustomAuthorize("You dont have access to Delete Quotation", "CustomerOrderEdit, Admin")]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var cities = db.QuotationMain.Where(a => a.ID == id).ToList();
                db.QuotationMain.RemoveRange(cities);



                var QuotationDetai = db.QuotationDetails.Where(a => a.MainID == id).ToList();
                db.QuotationDetails.RemoveRange(QuotationDetai);


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

        public JsonResult GetEditData(int ID)
        {
            var data = (from qd in db.QuotationDetails.Where(a=>a.MainID==ID)
                           join qm in db.QuotationMain 
                           on qd.MainID equals qm.ID 
                           select new
                           {

                               //QuotationMain
                               MainID = qm.ID,
                               QuotationName = qm.QuotationName,
                               CreatedDate = qm.CreatedDate,


                               //QuotationDetails
                               DetailsID = qd.DetailsID,
                               ProdCategoryID = qd.ProdCategoryID,
                               ProdCategory = qd.ProdCategory,
                               ProductID = qd.ProductID,
                               ProductName = qd.ProductName,
                               WTFrom1 = qd.WTFrom1,
                               UOM = qd.UOM,
                               WTTo1 = qd.WTTo1,
                               Rate1 = qd.Rate1,
                               WTFrom2 = qd.WTFrom2,
                               WTTo2 = qd.WTTo2,
                               Rate2 = qd.Rate2,


                               WTFrom3 = qd.WTFrom3,
                                WTTo3 = qd.WTTo3,
                               Rate3 = qd.Rate3,
                               AddWt = qd.AddWt,
                               AddRate = qd.AddRate
                                  
                           }).OrderByDescending(x => x.CreatedDate).ToList();
            var result = new { Message = "success", data };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}