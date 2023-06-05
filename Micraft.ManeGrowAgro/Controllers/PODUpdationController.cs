using iTextSharp.text;
using iTextSharp.text.pdf;
using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using System.Drawing.Drawing2D;
using Micraft.ManeGrowAgro.Security;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class PODUpdationController : Controller
    {
           ManeGrowContext db = new ManeGrowContext();
        // GET: PODUpdation

       [CustomAuthorize("You dont have access to View Dispatch", "Vendor,Logistics, Admin,PODUpdationEdit,PODUpdationView")]
        public ActionResult Index()
        {
            ViewBag.ResaleType = db.ResaleTypeMasters.OrderBy(a => a.ResaleType).ToList();
            return View();
        }
        [CustomAuthorize("You dont have access to View Create", "Vendor,Logistics, Admin,PODUpdationEdit")]
        public ActionResult Create()
        {
            ViewBag.CustID = db.CustomerMasters.Where(a => a.CustName=="jsfdahdfahj").ToList();
            if (User.IsInRole("Vendor"))
            {
                var VendorID = db.VendorMasters.Where(a => a.UserName == User.Identity.Name).Select(a => a.ID).SingleOrDefault();
                var SubRouteID = db.RouteDetails.Where(a => a.VendorID == VendorID && (a.Mile == "Last" || a.Mile == "First and Last")).Select(a => a.ID).ToList();
                var SubCust = db.SubRouteCustomers.Where(a => SubRouteID.Contains(a.RouteDetailsID)).Select(a => a.CustomerID).ToList();

                ViewBag.CustID = db.CustomerMasters.Where(a => SubCust.Contains(a.ID)).ToList();
            }
            else
            {
               
                    ViewBag.CustID = db.CustomerMasters.OrderBy(a => a.CustName).ToList();
                
            }
           
            ViewBag.Dmgreason = db.DamageReasons.OrderBy(a => a.ID).ToList();
            return View();
        }

        public ActionResult GetDamageDetails(string DATE, int CustomerID)
        {
            try
            {
                if (User.IsInRole("Vendor"))
                {
                    var damagedt = DateTime.ParseExact(DATE, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var Mainid = db.MainOrder.Where(a => a.Date == damagedt && a.CustomerId == CustomerID).Select(a => a.Id).ToList();
                    var orders = db.Order.Where(a => Mainid.Contains(a.MasterOrderId) && a.OrderStage == 8).ToList();
                    if(orders.Count !=0)
                    {
                        var dispID = orders[0].DispatchID;
                        var MainRouteID = db.dispatchDetails.Where(a => a.DispatchID == dispID).Select(a => a.Route).SingleOrDefault();
                        var vendorid = db.VendorMasters.Where(a => a.UserName == User.Identity.Name).Select(a => a.ID).SingleOrDefault();
                        var routedetails = db.RouteDetails.Where(a => a.VendorID == vendorid && a.RouteMainID == MainRouteID && (a.Mile=="Last" ||  a.Mile== "First and Last")).Count();
                        if(routedetails !=0)
                        {
                            var results = new { Message = "success", orders };
                            return Json(results, JsonRequestBehavior.AllowGet);
                        }
                    }
                    orders = orders.Where(a => a.ProductName == "sajdgahdghgd").ToList();
                    var result = new { Message = "success", orders };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var damagedt = DateTime.ParseExact(DATE, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    var Mainid = db.MainOrder.Where(a => a.Date == damagedt && a.CustomerId == CustomerID).Select(a => a.Id).ToList();
                    var orders = db.Order.Where(a => Mainid.Contains(a.MasterOrderId) && a.OrderStage == 8).ToList();
                    var result = new { Message = "success", orders };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

               



                }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [CustomAuthorize("You dont have access to View Dispatch", "Admin,PODUpdationEdit")]
        
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var regionMaster = db.PODUpdations.Where(a => a.ID == id).FirstOrDefault();
                var order = db.Order.Where(a => a.Id == regionMaster.OrderNo).FirstOrDefault();
                order.PODName = null;
                order.RejectedQty = 0;
                order.OrderStatus = "Order Dispatched";
                order.OrderStage = 8;

                var dmsfiles = db.DMSFile.Where(a => a.ModuleID == regionMaster.OrderNo).ToList();
                db.DMSFile.RemoveRange(dmsfiles);


                var history = db.RejectionSoldHistory.Where(a => a.OrderNo == regionMaster.OrderNo).ToList();
                db.RejectionSoldHistory.RemoveRange(history); 



                db.PODUpdations.Remove(regionMaster);

                TrackingDetails track = new TrackingDetails();
                track.Location = "Vendor Location";           
                track.Status = "POD Updation Entry Deleted By "+User.Identity.Name;
                track.Action = "PODUpdation";
                track.OrderNo = order.Id;
                track.Date = DateTime.Today;
                track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                track.CreatedDate = DateTime.Now;
                track.CreatedBy = User.Identity.Name;
                db.trackingDetails.Add(track);



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

        // Save Data

        [HttpPost]
        public ActionResult Save(List<PODUpdation> recs, string DATE, int CustomerID,string CustomerName)
        {

            if(User.Identity.Name=="" || User.Identity.Name==null)
            {
                var result = new { Message = "Session Timeout Please Login Again..." };
                return Json(result, JsonRequestBehavior.AllowGet);
            }





            try
            {
                var damagedt = DateTime.ParseExact(DATE, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                //var Mainid = db.MainOrder.Where(a => a.Date == damagedt && a.CustomerId == CustomerID).Select(a => a.Id).ToList();
                //var orders = db.Order.Where(a => Mainid.Contains(a.MasterOrderId) && a.OrderStatus == "Order Dispatched").ToList();

                foreach (var x in recs)
                {
                   
                    var Order = db.Order.Where(a => a.Id == x.OrderNo).FirstOrDefault();
                   
                    if (Order.RejectedQty == null || x.RejectedQty==0)
                    {
                        Order.RejectedQty = 0;
                    }
                    Order.RejectedQty = Order.RejectedQty + x.RejectedQty;
                    Order.PODName = x.PODName;

                    PODUpdation model = new PODUpdation();
                    if (x.RejectedQty == 0 && (x.DamageReason== "Select Reject Reason" || x.DamageReason == ""))
                    {
                        x.DamageReason = "All Delivered";
                        x.DamageReasonID = 6;
                    }else
                    {
                        model.DamageReason = x.DamageReason;
                        model.DamageReasonID = x.DamageReasonID;
                    }
                    model.Date = damagedt;
                    model.OrderNo = x.OrderNo;
                    model.CustID = CustomerID;
                    model.CustomerName = CustomerName;
                    model.Qty = x.Qty; 
                    model.UOM = x.UOM;
                    model.Weight = x.Weight;
                    model.ReceivedQty = x.ReceivedQty;
                    model.RejectedQty = x.RejectedQty;
                    model.ProductName = x.ProductName;
                    
                    model.InvoiceNo = Order.InvoiceNo;
                    model.CreatedBy = User.Identity.Name;
                    model.CreatedDate = DateTime.Now;
                    model.PODName = x.PODName;

                    db.PODUpdations.Add(model);



                    TrackingDetails track = new TrackingDetails();
                  
                    track.Location = "Plant";
                    if (model.RejectedQty == 0)
                    {
                        track.Status = "Order Successfully Delivered";
                        track.Action = "POD Updation";
                        Order.OrderStatus = track.Status;
                        Order.OrderStage = 11;
                    }
                    else
                    {
                        if (x.ReceivedQty != 0 && x.RejectedQty != 0)
                        {
                            track.Status = "Order Successfully Delivered With Rejection Qty " + x.RejectedQty;
                            track.Action = "POD Updation";
                            Order.OrderStatus = track.Status;
                            Order.OrderStage = 12;
                        }
                        else
                        {
                            if (Order.Qty == x.RejectedQty)
                            {
                                track.Status = " All Orders Rejected With Rejection Qty " + x.RejectedQty;
                                track.Action = "POD Updation";

                                Order.OrderStatus = track.Status;
                                Order.OrderStage = 13;
                            }
                        }
                    }


                    track.OrderNo = Order.Id;
                    track.Date = DateTime.Today;
                    track.StatusTime = DateTime.Now.ToString("HH:mm tt");
                    track.CreatedDate = DateTime.Now;
                    track.CreatedBy = User.Identity.Name;
                    db.trackingDetails.Add(track);






                    //var order
                }
                db.SaveChanges();
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
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();

                var ISadmin = false;
                if (User.IsInRole("Vendor"))
                {
                    ISadmin = false;
                }
                else
                {
                    ISadmin = true;
                }

                var result = (from dr in db.PODUpdations.Where(a => a.CreatedBy == User.Identity.Name || ISadmin==true)

                                  //join a in db.DamageReasons
                                  //on dr.DamageReasonID equals a.ID

                              join d in db.CustomerMasters 
                              on dr.CustID equals d.ID

                              select new
                              {
                                 
                                  ID = dr.ID,
                                  DamageReasonID= dr.DamageReason,
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
                                  CreatedDate = dr.CreatedDate, 
                                  InvoiceNo = dr.InvoiceNo,
                                  Location = d.Area, 


                              }).OrderByDescending(x => x.CreatedDate).ToList();




                int totalRecords = result.Count();
                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = result
                        .Where(x => x.CustID.ToLower().Contains(sSearch.ToLower()) || x.Location.ToLower().Contains(sSearch.ToLower()) || x.InvoiceNo.ToLower().Contains(sSearch.ToLower()) || x.CreatedBy.ToLower().Contains(sSearch.ToLower()) || x.ProductName.ToLower().Contains(sSearch.ToLower()) || x.OrderNo.ToString().ToLower().Contains(sSearch.ToLower()))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new
                        {
                            x.ID ,
                            x.DamageReasonID,
                            x.Date ,
                            x.OrderNo,
                            x.CustID,
                            x.ProductName,
                            x.Qty,
                            x.UOM,
                            x.Weight,
                            x.ReceivedQty,
                            x.Rejected,
                            x.CreatedBy,
                            x.CreatedDate,
                            x.InvoiceNo,
                            x.Location,

                        }).OrderByDescending(x => x.CreatedDate).ToList();
                     
                }
                else
                {
                    totalRecords = result.Count();
                    result = result.AsEnumerable()
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new  {  x.ID,
                            x.DamageReasonID,
                           x.Date,
                           x.OrderNo,
                           x. CustID,
                            x.ProductName,
                            x.Qty,
                            x.UOM,
                            x.Weight,
                            x.ReceivedQty,
                            x.Rejected,
                            x.CreatedBy,
                            x.CreatedDate,
                            x.InvoiceNo,
                          
                            x.Location, }).OrderByDescending(x => x.CreatedDate).ToList();
                     
                }



                //if (User.IsInRole("Vendor"))
                //{
                //    result = result.Where(a => a.CreatedBy == User.Identity.Name).ToList();
                //}
                

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

        public JsonResult GetPODName(int ID)
        {
            var PODNAMEs = db.Order.Where(a => a.Id == ID).Select(a => a.PODName).SingleOrDefault(); 
            if(PODNAMEs=="" || PODNAMEs == null)
            {
                var resultss = new { Message = "POD Not Found"};
                return Json(resultss, JsonRequestBehavior.AllowGet);
            }
            
           // var PAths = Path.Combine(Server.MapPath("~/Attachments/POD/"), PODNAMEs); 
            var result = new {Message="success",PODNAME= PODNAMEs };  

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult UploadPhoto()
        //{
        //    //var Ext = "";
        //    var OrderNo = Request.Form["OrderNo"].ToString();
        //    HttpFileCollectionBase files = Request.Files;

        //    var duplicate = "";
        //    for (int i = 0; i < files.Count; i++)
        //    {
        //        FileName F = new FileName();
        //        var fname = "";
        //        HttpPostedFileBase file = files[i];
        //        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //        {
        //            string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //            fname = testfiles[testfiles.Length - 1];
        //        }
        //        else
        //        {
        //            //   fname = file.FileName;
        //            //Ext = file.ContentType;
        //        }

        //        var FileNM = OrderNo + ".png";

        //        var FilName = System.IO.Path.Combine(Server.MapPath("~/Photo/DamagePhoto/"), FileNM);
        //        if (System.IO.File.Exists(FilName))
        //        {
        //            duplicate = duplicate + " _ " + fname;
        //        }
        //        else
        //        {
        //            var PAth = Path.Combine(Server.MapPath("~/Photo/DamagePhoto/"), FileNM);
        //            file.SaveAs(PAth);
        //            TempData["dmgphoto"] = "~/Photo/DamagePhoto/" + FileNM;
        //            TempData.Keep();

        //            //System.IO.FileInfo fi = new System.IO.FileInfo(PAth);
        //            //if (fi.Exists)
        //            //{
        //            //    var PAths = Path.Combine(Server.MapPath("~/Photo/DamagePhoto/"), FileNM);
        //            //    fi.MoveTo(PAths);
        //            //}

        //        }

        //        var result = new { fname, duplicate };
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        [CustomAuthorize("You dont have access to View Create", "Vendor,Logistics, Admin,PODUpdationEdit")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PODUpdation productDamage = await db.PODUpdations.FindAsync(id);
            if (productDamage == null)
            {
                return HttpNotFound();
            }
            return View(productDamage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PODUpdation productDamageDetails)
        {

            db.Entry(productDamageDetails).State = EntityState.Modified;
            db.SaveChanges();
            TempData["success"] = "Record Edited Successfully";
            return RedirectToAction("Index");
        }

        //[HttpGet]
        //public JsonResult GetImageName(int Id)
        //{
        //    var Dmgimg = db.ProductDamageDetails.Where(a => a.ID == Id).Select(a => a.DamagePhoto).FirstOrDefault();
        //    if (Dmgimg == null)
        //    {
        //        var result = new { Message = "Damage Photo Not Found..." };
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var result = new { Message = "success", DamagePhoto = Dmgimg };
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //}

        [HttpPost]
        public JsonResult DmgReason()
        {
            try
            {
                // var orders = db.DamageReasons.Where(a => a.ID == Id).Select(a => a.DamageReason).FirstOrDefault();
                var orders = db.DamageReasons.OrderBy(a => a.DamageReason).ToList();
                //var result = new { Message = "success", orders };

                return Json(orders, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult UploadFile(string imageData, string OrderNo)
        {
            var FileName = "";
            try
            {
                string[] values = OrderNo.Split(',');

                for (int s = 0; s < values.Length; s++)
                {
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;

                    var sec = currentTime.Seconds; // 30 

                    OrderNo = values[s].Trim();
                    if (OrderNo != "")
                    {
                        FileName = OrderNo + "_" + sec + ".jpg";

                        string fileNameWithPath = Server.MapPath("~/Attachments/DamagePhoto/" + FileName);
                        if (!Directory.Exists(fileNameWithPath))
                        {
                            using (FileStream fs = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                using (BinaryWriter bw = new BinaryWriter(fs))
                                {
                                    if (imageData != "")
                                    {
                                        byte[] data = Convert.FromBase64String(imageData);
                                        bw.Write(data);
                                        bw.Close();
                                    }
                                }
                            }



                        }


                        var results = new { Message = "success", fname = FileName };
                        return Json(results, JsonRequestBehavior.AllowGet);

                    }
                }

            }
            catch (Exception ex)
            {
                var result11 = new { Message = ex.Message };
                return Json(result11, JsonRequestBehavior.AllowGet);
            }
            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);














            //var Ext = "";
            //var OrderNo = Request.Form["OrderNo"].ToString();
            //HttpFileCollectionBase files = Request.Files;

            //List<FileName> list = new List<FileName>();
            //var duplicate = "";
            //for (int i = 0; i < files.Count; i++)
            //{
            //    FileName F = new FileName();
            //    var fname = "";
            //    HttpPostedFileBase file = files[i];
            //    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
            //    {
            //        string[] testfiles = file.FileName.Split(new char[] { '\\' });
            //        fname = testfiles[testfiles.Length - 1];
            //    }
            //    else
            //    {
            //        fname = file.FileName;
            //        Ext = file.ContentType;
            //    }

            //    var FileNM = OrderNo + "_" + fname;

            //    var FilName = System.IO.Path.Combine(Server.MapPath("~/Attachments/DamagePhoto/"), FileNM);
            //    if (System.IO.File.Exists(FilName))
            //    {
            //        duplicate = duplicate + " , " + fname;
            //    }
            //    else
            //    {
            //        Stream strm = file.InputStream;
            //        var PAth = Path.Combine(Server.MapPath("~/Attachments/DamagePhoto/"), FileNM); 
            //        ReduceImageSize(0.2, strm, PAth);

            //        //var PAth = Path.Combine(Server.MapPath("~/Attachments/DamagePhoto/"), fname);
            //        //file.SaveAs(PAth);

            //        System.IO.FileInfo fi = new System.IO.FileInfo(PAth);
            //        if (fi.Exists)
            //        {
            //            var PAths = Path.Combine(Server.MapPath("~/Attachments/DamagePhoto/"), FileNM);
            //            fi.MoveTo(PAths);
            //        }
            //        F.Name = FileNM;
            //        list.Add(F);
            //    }
            //}
            //var result = new { list = list, DuplicateImg = duplicate };
            //return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UploadSignature(string imageData, string OrderNo)
        {

            var FileName = "";
            try
            {

                string[] values = OrderNo.Split(',');
               
                for (int s = 0; s < values.Length; s++)
                {
                    TimeSpan currentTime = DateTime.Now.TimeOfDay;
                   
                  var sec=  currentTime.Seconds; // 30 

                    OrderNo = values[s].Trim();
                    if (OrderNo != "")
                    {
                      FileName = OrderNo+"_" + sec+".jpg"; 

                        string fileNameWithPath = Server.MapPath("~/Attachments/POD/" + FileName);
                        if (!Directory.Exists(fileNameWithPath))
                        {
                            using (FileStream fs = new FileStream(fileNameWithPath, FileMode.Create))
                            {
                                using (BinaryWriter bw = new BinaryWriter(fs))
                                {
                                    if (imageData != "")
                                    {
                                        byte[] data = Convert.FromBase64String(imageData);
                                        bw.Write(data);
                                        bw.Close();
                                    }                                    
                                }
                            }
                           
                        }


                        var results = new { Message = "success", fname=FileName };
                        return Json(results, JsonRequestBehavior.AllowGet);

                    }
                }
               
            }
            catch (Exception ex)
            {
                var result11 = new { Message = ex.Message };
                return Json(result11, JsonRequestBehavior.AllowGet);
            }
            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        //public ActionResult UploadPOD() 
        //{
        //    var Ext = "";
        //    var OrderNo = Request.Form["OrderNo"].ToString();
        //    HttpFileCollectionBase files = Request.Files;

        //    List<FileName> list = new List<FileName>();
        //    var duplicate = "";
        //    for (int i = 0; i < files.Count; i++)
        //    {
        //        FileName F = new FileName();
        //        var fname = "";
        //        HttpPostedFileBase file = files[i];
        //        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //        {
        //            string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //            fname = testfiles[testfiles.Length - 1];
        //        }
        //        else
        //        {
        //            fname = file.FileName;
        //            Ext = file.ContentType;
        //        }

        //        var FileNM = OrderNo + "_" + fname;

        //        var FilName = System.IO.Path.Combine(Server.MapPath("~/Attachments/POD/"), FileNM);
        //        if (System.IO.File.Exists(FilName))
        //        {
        //            duplicate = duplicate + " , " + fname;
        //        }
        //        else
        //        {

                   
        //            Stream strm = file.InputStream;
        //            var PAth = Path.Combine(Server.MapPath("~/Attachments/POD/"), FileNM);
        //            ReduceImageSize(0.2, strm, PAth);




        //            System.IO.FileInfo fi = new System.IO.FileInfo(PAth);
        //            if (fi.Exists)
        //            {
        //                var PAths = Path.Combine(Server.MapPath("~/Attachments/POD/"), FileNM);
        //                fi.MoveTo(PAths);
        //            }
        //            F.Name = FileNM;
        //            list.Add(F);
        //        }

                
        //            var results = new { Message="success", fname };
        //        return Json(results, JsonRequestBehavior.AllowGet); 
        //    }
        //    var result = new { list = list, DuplicateImg = duplicate };
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

          private void ReduceImageSize(double scaleFactor, Stream sourcePath, string targetPath)
        {
            using (var image = System.Drawing.Image.FromStream(sourcePath))
            {
                var newWidth = (int)(image.Width * scaleFactor);
                var newHeight = (int)(image.Height * scaleFactor);
                var thumbnailImg = new System.Drawing.Bitmap(newWidth, newHeight);
                var thumbGraph = System.Drawing.Graphics.FromImage(thumbnailImg);
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                var imageRectangle = new System.Drawing.Rectangle(0, 0, newWidth, newHeight);
                thumbGraph.DrawImage(image, imageRectangle);
                thumbnailImg.Save(targetPath, image.RawFormat);
            }
        }

        internal class FileName
        {
            public string Name { get; set; }
        }
        public virtual ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Attachments/DamagePhoto/"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }
        public virtual ActionResult DownloadNew(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Attachments/POD/"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }

        
        public ActionResult DeleteFiles(int? ID, string FileName, string Mode)
        {
            var result = "";
            try
            {
                if (Mode == "Create" | ID == 0)
                {
                    var PAthss = Path.Combine(Server.MapPath("~/Attachments/DamagePhoto/"), FileName);
                    if (System.IO.File.Exists(PAthss))
                    {
                        System.IO.File.Delete(PAthss);
                        result = "success";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        result = "file not Found";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    var PAthss = Path.Combine(Server.MapPath("~/Attachments/DamagePhoto/"), FileName);
                    if (System.IO.File.Exists(PAthss))
                    {
                        System.IO.File.Delete(PAthss);
                        result = "success";

                    }
                    else
                    {
                        result = "file not Found";
                    }

                    db.DMSFile.RemoveRange(db.DMSFile.Where(a => a.ID == ID));
                    db.SaveChanges();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception EX)
            {
                result = EX.Message;
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult SaveList(List<VehicleMaster> FilesList)
        {
            if (FilesList.Count > 0)
            {
                try
                {
                    using (var dbcxtransaction = db.Database.BeginTransaction())
                    {
                        foreach (var x in FilesList)
                        {
                            if (x.DmsFileID == 0)
                            {
                                var PAth = "/Attachments/DamagePhoto/" + x.Name;
                                DMSFile F = new DMSFile();
                                F.Modulename = "DamagePhoto";
                                F.ModuleID = Convert.ToInt32(x.ID);
                                F.Name = x.Name;
                                F.AttachPath = PAth;
                                F.IsActive = true;
                                F.DocumentNo = x.DocumentNo;
                                F.FileDetails = x.Attachments;
                                F.CreatedBy = User.Identity.Name;
                                F.CreatedDate = DateTime.Today;
                                db.DMSFile.Add(F);
                                db.SaveChanges();
                                PAth = "";
                            }
                        }

                        dbcxtransaction.Commit();
                        var result = new { Message = "success" };
                        return Json(result);
                    }

                }
                catch (Exception EX)
                {
                    var result = new { Message = EX.Message };
                    if (EX.InnerException != null)
                    {
                        if (EX.InnerException.InnerException.Message != null)
                            result = new { Message = EX.InnerException.InnerException.Message };
                        else
                            result = new { Message = EX.Message };
                    }
                    else
                        result = new { Message = EX.Message };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var status = "No data Found... ";
                return new JsonResult { Data = new { status } };
            }
        }

        public ActionResult GetDMSData(int ID)
        {
            try
            {
                var data = db.DMSFile.Where(x => x.ModuleID == ID && x.Modulename == "DamagePhoto").ToList();
                var result = new { dmsList = data };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var msg = Ex.Message.ToString();
                var result = new { error = "error", msg = msg };
                TempData["ErrorMsg"] = "Error";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckDuplicate(int OrderNo, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.PODUpdations.Where(x => x.OrderNo == OrderNo && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.PODUpdations.Where(x => x.OrderNo == OrderNo).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception Ex)
            {
                var str = Ex.Message.ToString();
                return Json(str, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetRejectionHistory(int ID)
        {
            var PODDetails = (from E in db.PODUpdations.Where(a =>a.ID== ID)                            
                           select new
                           {
                               ProductName = E.ProductName,
                               OrderNo = E.OrderNo,
                               RejectedQty = E.RejectedQty,
                               CustomerName = E.CustomerName,
                               CustID=E.CustID,
                           }).FirstOrDefault();
            var History = db.RejectionSoldHistory.Where(a => a.RejectionID == ID).ToList();
            var result = new {Message="success", PODDetails, History }; 
            return Json(result, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public JsonResult SaveData(List<RejectionSoldHistory> recs, int OrderNo,  int RejectedQty, int ID, int CustID, string CustomerNames, string ProductName)
        {
            try
            {                
                
                using (DbContextTransaction dbTran = db.Database.BeginTransaction())
                {

                    var DistinctOrderNo = recs.Select(a => a.OrderNo).Distinct();
                    foreach (var xx in DistinctOrderNo)
                    {
                        var temp = 0;
                        foreach (var x in recs)
                        {
                            x.OrderNo = OrderNo;
                            x.CustomerName = CustomerNames;
                            x.RejectionID = ID;
                            x.CustID = CustID;
                            x.ProductName = ProductName;
                            x.CreatedBy = User.Identity.Name;
                            x.CreatedDate = DateTime.Now;
                            db.RejectionSoldHistory.Add(x);
                        }
                    }
                    db.SaveChanges();
                    dbTran.Commit();
                    var result = new { Message = "success" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }


        }

        
        public JsonResult InvoicePrint(int OrderNo)
        {
            try
            {
                var order = db.Order.Where(a => a.Id == OrderNo).Select(a=>new { a.RejectedQty, a.InvoiceNo }).FirstOrDefault();
                if (order.RejectedQty == 0)
                {
                    var resultsss = new { Message = "Invoice Not Generated..Because Rejected Qty is 0" };
                    return Json(resultsss, JsonRequestBehavior.AllowGet);
                }

                var result1 = (from e in db.Order.Where(a => a.InvoiceNo == order.InvoiceNo)

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
                                   RejectedQty = e.RejectedQty, 
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
              
               

                Document document;
                document = new iTextSharp.text.Document(PageSize.A4, 10f, 10f, 10f, 10f);
                long timeSince1970 = DateTimeOffset.Now.ToUnixTimeSeconds();
                string path = Server.MapPath("~/Attachments/LabelPrints/");
                string filename1 = path + User.Identity.Name + timeSince1970 + ".pdf";
                string StikerPrintName = User.Identity.Name + timeSince1970 + ".pdf";
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filename1, FileMode.Create));
                document.Open();

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
                        PC12885d.FixedHeight = 20;
                        PC12885d.Border = Rectangle.NO_BORDER;
                        PC12885d.HorizontalAlignment = 2;
                        table1.AddCell(PC12885d);


                        Paragraph P117584sd5 = new Paragraph();
                        P117584sd5.Add(new Phrase("" + BillName, FontFactory.GetFont("Arial", 9, Font.NORMAL)));
                        PdfPCell PC12885sd = new PdfPCell(P117584sd5);
                        PC12885sd.Colspan = 5;
                        PC12885sd.FixedHeight = 20;
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


                        PdfPTable table2 = new PdfPTable(12);
                        float[] width2 = new float[] { 0.1f, 1.5f, 2f, 6f, 2f, 2f, 2f, 2f, 2f, 2f, 3f, 0.1f };
                        table2.SetWidths(width2);
                        table2.WidthPercentage = 100f;
                        table2.DefaultCell.Padding = 1;



                        try
                        {


                            PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("Sr No", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p1.HorizontalAlignment = 1;
                            p1.BackgroundColor = BaseColor.WHITE;
                            p1.FixedHeight = 25;
                            p1.Colspan = 2;

                            //    p1.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(p1);

                            PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("Description of Goods", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p2dd.HorizontalAlignment = 1;
                            p2dd.BackgroundColor = BaseColor.WHITE;
                            p2dd.Colspan = 2;
                            //      p2dd.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(p2dd);

                            PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("HSN/SAC", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p2.HorizontalAlignment = 1;
                            p2.BackgroundColor = BaseColor.WHITE;
                            //        p2.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(p2);

                            PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("Order Qty", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp5.HorizontalAlignment = 1;
                            pp5.BackgroundColor = BaseColor.WHITE;
                            //        pp5.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(pp5);

                            PdfPCell pp5gf = new PdfPCell(new Phrase(new Phrase("Rejt Qty", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp5gf.HorizontalAlignment = 1;
                            pp5gf.BackgroundColor = BaseColor.WHITE;
                            //        pp5.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(pp5gf);

                            PdfPCell pp2155 = new PdfPCell(new Phrase(new Phrase("Qty", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp2155.HorizontalAlignment = 1;
                            pp2155.BackgroundColor = BaseColor.WHITE;
                            //        pp5.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(pp2155);

                            PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("Rate", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp55.HorizontalAlignment = 1;
                            pp55.BackgroundColor = BaseColor.WHITE;
                            //        pp55.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(pp55);

                            PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("Per", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p29.HorizontalAlignment = 1;
                            p29.BackgroundColor = BaseColor.WHITE;
                            //         p29.Border = Rectangle.BOTTOM_BORDER;
                            //    p29.Colspan = 2;
                            table2.AddCell(p29);

                            PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("Amount", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp51.HorizontalAlignment = 2;
                            pp51.BackgroundColor = BaseColor.WHITE;
                            //           pp51.Border = Rectangle.BOTTOM_BORDER;
                            pp51.Colspan = 2;
                            table2.AddCell(pp51);


                        }
                        catch
                        {

                        }
                        var cntt = 0;
                        int TotalQty = 0;
                        decimal TotalAMt = 0;
                        var AmtInwords = "";
                        int RejectedQty = 0;
                        int BalanceQty = 0;
                        foreach (var xx in data)
                        {

                            cntt++;

                            try
                            {


                                PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("" + cntt, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p1.HorizontalAlignment = 1;
                                p1.BackgroundColor = BaseColor.WHITE;
                                p1.FixedHeight = 28;
                                p1.Colspan = 2;
                                //    p1.Border = Rectangle.BOTTOM_BORDER;
                                table2.AddCell(p1);

                                PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("" + xx.Product, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p2dd.HorizontalAlignment = 0;
                                p2dd.BackgroundColor = BaseColor.WHITE;
                                p2dd.Colspan = 2;
                                //      p2dd.Border = Rectangle.BOTTOM_BORDER;
                                table2.AddCell(p2dd);

                                PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("" + xx.HSN, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p2.HorizontalAlignment = 1;
                                p2.BackgroundColor = BaseColor.WHITE;
                                //        p2.Border = Rectangle.BOTTOM_BORDER;
                                table2.AddCell(p2);

                                PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("" + xx.Qty, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp5.HorizontalAlignment = 1;
                                pp5.BackgroundColor = BaseColor.WHITE;
                                //        pp5.Border = Rectangle.BOTTOM_BORDER;
                                table2.AddCell(pp5);


                                PdfPCell pp5sd = new PdfPCell(new Phrase(new Phrase("" + xx.RejectedQty, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp5sd.HorizontalAlignment = 1;
                                pp5sd.BackgroundColor = BaseColor.WHITE;
                                //        pp5.Border = Rectangle.BOTTOM_BORDER;


                                RejectedQty = RejectedQty + Convert.ToInt32( xx.RejectedQty); 
                                table2.AddCell(pp5sd);
                                var rejctedqtyy = (Convert.ToInt32(xx.Qty) - Convert.ToInt32(xx.RejectedQty));
                                BalanceQty = BalanceQty + rejctedqtyy;
                                PdfPCell pp5dsd = new PdfPCell(new Phrase(new Phrase("" + rejctedqtyy, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp5dsd.HorizontalAlignment = 1;
                                pp5dsd.BackgroundColor = BaseColor.WHITE;
                                //        pp5.Border = Rectangle.BOTTOM_BORDER;
                                table2.AddCell(pp5dsd);

                                TotalQty = TotalQty + Convert.ToInt32(xx.Qty);

                                PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("" + xx.Rate, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp55.HorizontalAlignment = 1;
                                pp55.BackgroundColor = BaseColor.WHITE;
                                //        pp55.Border = Rectangle.BOTTOM_BORDER;
                                table2.AddCell(pp55);

                                PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("" + xx.RateIN, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                p29.HorizontalAlignment = 1;
                                p29.BackgroundColor = BaseColor.WHITE;
                                //         p29.Border = Rectangle.BOTTOM_BORDER;
                                //    p29.Colspan = 2;
                                table2.AddCell(p29);

                                var Tamt = rejctedqtyy * xx.Rate;
                                PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("" + Tamt, FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                pp51.HorizontalAlignment = 2;
                                pp51.BackgroundColor = BaseColor.WHITE;
                                //           pp51.Border = Rectangle.BOTTOM_BORDER;
                                pp51.Colspan = 2;
                                table2.AddCell(pp51);
                                TotalAMt = TotalAMt + Convert.ToDecimal(Tamt);

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
                                        PdfPCell p1dfdf = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p1dfdf.HorizontalAlignment = 1;
                                        p1dfdf.BackgroundColor = BaseColor.WHITE;
                                        p1dfdf.FixedHeight = colss;
                                        //    p1.Border = Rectangle.BOTTOM_BORDER;
                                        p1dfdf.Colspan = 2;
                                        table2.AddCell(p1dfdf);


                                        PdfPCell p1 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p1.HorizontalAlignment = 1;
                                        p1.BackgroundColor = BaseColor.WHITE;
                                        p1.FixedHeight = colss;
                                        p1.Colspan = 2;
                                        //    p1.Border = Rectangle.BOTTOM_BORDER;
                                        table2.AddCell(p1);

                                        PdfPCell p2dd = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p2dd.HorizontalAlignment = 0;
                                        p2dd.BackgroundColor = BaseColor.WHITE;
                                      //  p2dd.Colspan = 2;
                                        //      p2dd.Border = Rectangle.BOTTOM_BORDER;
                                        table2.AddCell(p2dd);

                                        PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p2.HorizontalAlignment = 1;
                                        p2.BackgroundColor = BaseColor.WHITE;
                                        //        p2.Border = Rectangle.BOTTOM_BORDER;
                                        table2.AddCell(p2);

                                        PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        pp5.HorizontalAlignment = 1;
                                        pp5.BackgroundColor = BaseColor.WHITE;
                                        //        pp5.Border = Rectangle.BOTTOM_BORDER;
                                        table2.AddCell(pp5);

                                        PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        pp55.HorizontalAlignment = 1;
                                        pp55.BackgroundColor = BaseColor.WHITE;
                                        //        pp55.Border = Rectangle.BOTTOM_BORDER;
                                        table2.AddCell(pp55);

                                        PdfPCell p29 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p29.HorizontalAlignment = 1;
                                        p29.BackgroundColor = BaseColor.WHITE;
                                        //         p29.Border = Rectangle.BOTTOM_BORDER;
                                           // p29.Colspan = 2;
                                        table2.AddCell(p29);

                                        PdfPCell p29ssd = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        p29ssd.HorizontalAlignment = 1;
                                        p29ssd.BackgroundColor = BaseColor.WHITE;
                                        //         p29.Border = Rectangle.BOTTOM_BORDER;
                                        // p29.Colspan = 2;
                                        table2.AddCell(p29ssd);

                                        PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.NORMAL))));
                                        pp51.HorizontalAlignment = 1;
                                        pp51.BackgroundColor = BaseColor.WHITE;
                                        //           pp51.Border = Rectangle.BOTTOM_BORDER;
                                        pp51.Colspan = 2;
                                        table2.AddCell(pp51);

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
                            p1.HorizontalAlignment = 4;
                            p1.BackgroundColor = BaseColor.WHITE;
                            p1.FixedHeight = 20;
                            p1.Colspan = 4;
                            table2.AddCell(p1);


                            PdfPCell p2 = new PdfPCell(new Phrase(new Phrase("", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p2.HorizontalAlignment = 1;
                            p2.BackgroundColor = BaseColor.WHITE;
                            //        p2.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(p2);

                            PdfPCell pp5 = new PdfPCell(new Phrase(new Phrase("" + TotalQty, FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp5.HorizontalAlignment = 1;
                            pp5.BackgroundColor = BaseColor.WHITE;
                            //        pp5.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(pp5);

                            PdfPCell pp55 = new PdfPCell(new Phrase(new Phrase(""+RejectedQty, FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp55.HorizontalAlignment = 1;
                            pp55.BackgroundColor = BaseColor.WHITE;
                            //        pp55.Border = Rectangle.BOTTOM_BORDER;
                            table2.AddCell(pp55);

                            PdfPCell p29 = new PdfPCell(new Phrase(new Phrase(""+BalanceQty, FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p29.HorizontalAlignment = 1;
                            p29.BackgroundColor = BaseColor.WHITE;
                            //         p29.Border = Rectangle.BOTTOM_BORDER;
                            //p29.Colspan = 3;
                            table2.AddCell(p29);


                            PdfPCell p29sdsd = new PdfPCell(new Phrase(new Phrase("" , FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p29sdsd.HorizontalAlignment = 1;
                            p29sdsd.BackgroundColor = BaseColor.WHITE;
                            //         p29.Border = Rectangle.BOTTOM_BORDER;
                            //p29.Colspan = 3;
                            table2.AddCell(p29sdsd);

                            PdfPCell p2dfsf9 = new PdfPCell(new Phrase(new Phrase("" , FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            p2dfsf9.HorizontalAlignment = 1;
                            p2dfsf9.BackgroundColor = BaseColor.WHITE;
                            //         p29.Border = Rectangle.BOTTOM_BORDER;
                            //p29.Colspan = 3;
                            table2.AddCell(p2dfsf9);


                            var ttotalamt = Math.Round(TotalAMt);
                            AmtInwords = words(Convert.ToInt32(ttotalamt));
                            PdfPCell pp51 = new PdfPCell(new Phrase(new Phrase("" + ttotalamt + ".00", FontFactory.GetFont("Arial", 10, Font.BOLD, BaseColor.BLACK))));
                            pp51.HorizontalAlignment = 2;
                            pp51.BackgroundColor = BaseColor.WHITE;
                            //           pp51.Border = Rectangle.BOTTOM_BORDER;
                            pp51.Colspan = 2;
                            table2.AddCell(pp51);


                        }
                        catch
                        {

                        }
                        PdfPTable table3 = new PdfPTable(10);
                        float[] width33 = new float[] { 0.1f, 1.5f, 2f, 6f, 2f, 2f, 2f, 2f, 3f, 0.1f };
                        table3.SetWidths(width33);
                        table3.WidthPercentage = 100f;
                        table3.DefaultCell.Padding = 1;
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
                            table3.AddCell(PC124888);

                            Paragraph pr444445 = new Paragraph();
                            pr444445.Add(new Phrase("E. & O.E \n", FontFactory.GetFont("Arial", 8, Font.NORMAL)));

                            PdfPCell PC1288 = new PdfPCell(pr444445);
                            PC1288.Colspan = 2;
                            PC1288.Border = Rectangle.RIGHT_BORDER;
                            PC1288.HorizontalAlignment = 2;
                            table3.AddCell(PC1288);
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
                            table3.AddCell(PC124888);

                            Paragraph pr444445 = new Paragraph();
                            pr444445.Add(new Phrase("For MANEGROW AGRO PRODUCTS \n\n\n\n\n Authorised Signatury", FontFactory.GetFont("Arial", 8, Font.NORMAL)));

                            PdfPCell PC1288 = new PdfPCell(pr444445);
                            PC1288.Colspan = 6;
                            //   PC1288.Border = Rectangle.RIGHT_BORDER;
                            PC1288.HorizontalAlignment = 2;
                            table3.AddCell(PC1288);
                        }
                        catch { }

                        PdfPCell PC12885 = new PdfPCell();
                        PC12885.Colspan = 10;
                        PC12885.Border = Rectangle.TOP_BORDER;
                        PC12885.HorizontalAlignment = 2;
                        table3.AddCell(PC12885);

                        Paragraph pr44444d5 = new Paragraph();
                        pr44444d5.Add(new Phrase("SUBJECT TO PUNE JURISDICTION\n\n", FontFactory.GetFont("Arial", 8, Font.NORMAL)));
                        pr44444d5.Add(new Phrase("This is a Computer Genrated Invoice", FontFactory.GetFont("Arial", 8, Font.NORMAL)));

                        PdfPCell PC12s88 = new PdfPCell(pr44444d5);
                        PC12s88.Colspan = 10;
                        PC12s88.Border = Rectangle.NO_BORDER;
                        PC12s88.HorizontalAlignment = 1;
                        table3.AddCell(PC12s88);
                        document.Add(table1);
                        document.Add(table2);
                        document.Add(table3);
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
}
