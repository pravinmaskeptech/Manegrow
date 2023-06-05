using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    
    public class VendorRouteAssignmentController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: VendorRouteAssignment
     //   public ActionResult Index()
     //   {
     //       return View();
     //   }
     //   public ActionResult Create() 
     //   {
          
     //       var cats = db.SubRouteDetails 
     //.Select(i => new {  i.SubRoute }) 
     //.Distinct()
     //.OrderByDescending(i => i.SubRoute) 
     //.ToArray();

     //       ViewBag.Route = cats.ToList();
     //       return View();
     //   }


     //   public ActionResult GetVendors() 
     //   {
     //       var vendor = db.VendorMasters.OrderBy(a => a.VendorName).ToList();

     //       var result = new {Message="success" , vendor };
     //       return Json(result, JsonRequestBehavior.AllowGet);
     //   }
     //   public ActionResult GetRouteAssignData(string Root)
     //   {
     //       try
     //       {
     //           var result1 = (from cm in db.CustomerMasters.Where(a=>a.RootName==Root)

     //                         join vendor  in db.vendorRouteAssignment on cm.ID equals vendor.CustomerID into Vendors
     //                         from va in Vendors.DefaultIfEmpty()

     //                         orderby va.CustomerName ascending
     //                         select new
     //                         {
     //                             ID = cm.ID,
     //                             CustName = cm.CustName,

     //                             VendorName = va == null ? string.Empty : va.VendorName,
     //                             VendorID = va == null ? 0 : va.VendorID,
     //                         }
     //                         //select new { GRNId = grn.GRNId, GRNNo = grn.GRNNo, GRNDate = grn.GRNDate, PONo = grn.PONo, BatchNo = grn.BatchNo, ReceivedQty = grn.ReceivedQty, ManufacturingDate = grn.ManufacturingDate, ExpiryDate = grn.ExpiryDate, ProductCode = prd == null ? string.Empty : prd.ProductName, WarehouseID = Whouse == null ? string.Empty : Whouse.WareHouseName, StoreLocationId = store == null ? string.Empty : store.StoreLocation, ReturnQty = grn.ReturnQty, ReplaceQty = grn.ReplaceQty, DamageQty = grn.DamageQty }
     //                                ).ToList();
     //           return Json(result1, JsonRequestBehavior.AllowGet); 
     //       }
     //       catch(Exception  ex)
     //       {

     //       }

            
     //       return Json(null, JsonRequestBehavior.AllowGet);
     //   }



     //   [HttpPost]
     //   public ActionResult Save(List<VendorRouteAssignment> Root)  
     //   {


     //       using (DbContextTransaction dbTran = db.Database.BeginTransaction())
     //       {
                
     //           try
     //           {
     //               IEnumerable<VendorRouteAssignment> entityList = db.vendorRouteAssignment.ToList();
     //               db.vendorRouteAssignment.RemoveRange(entityList);
     //               db.SaveChanges();
                    
     //               foreach(var x in Root)
     //               {
     //                   x.CreatedBy = User.Identity.Name;
     //                   x.CreatedDate = DateTime.Today;
     //                   db.vendorRouteAssignment.Add(x);   

     //               }



     //               db.SaveChanges();
     //               dbTran.Commit();
     //               var result = new {Message="success" };
     //               return Json(result, JsonRequestBehavior.AllowGet);
     //           }
     //           catch(Exception ex)
     //           {
     //                   dbTran.Rollback();
     //               var result = new { Message = ex.Message };
     //               return Json(result, JsonRequestBehavior.AllowGet);
     //           }
     //       }

     //   }

    }
}