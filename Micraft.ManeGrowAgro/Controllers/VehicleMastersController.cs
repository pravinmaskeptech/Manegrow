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
using System.Globalization;
using System.IO;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class VehicleMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: VehicleMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.VehicleMasters.ToListAsync());
        }

        // GET: VehicleMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleMaster vehicleMaster = await db.VehicleMasters.FindAsync(id);
            if (vehicleMaster == null)
            {
                return HttpNotFound();
            }
            return View(vehicleMaster);
        }

        // GET: VehicleMasters/Create
        public ActionResult Create()
        {
            ViewBag.VehicleType = db.VehicleTypes.OrderBy(a => a.VehicleType).ToList();
            ViewBag.VendorID = db.VendorMasters.OrderBy(a => a.VendorName).ToList();
            return View();
        }

        // POST: VehicleMasters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create( VehicleMaster vehicleMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var vehicle = db.VehicleMasters.Where(a => a.VendorID == vehicleMaster.VendorID && a.VehicleNo == vehicleMaster.VehicleNo).FirstOrDefault();
                    if (vehicle == null)
                    {
                        vehicleMaster.CreatedBy = User.Identity.Name;
                        vehicleMaster.CreatedDate = DateTime.Now;
                        db.VehicleMasters.Add(vehicleMaster);
                        await db.SaveChangesAsync();
                        @ViewBag.ID = db.VehicleMasters.Max(a => a.ID);
                        TempData["success"] = "Record Added Successfully";
                        ViewBag.VehicleType = db.VehicleTypes.OrderBy(a => a.VehicleType).ToList();
                        ViewBag.VendorID = db.VendorMasters.OrderBy(a => a.VendorName).ToList();
                        return View(vehicleMaster);
                    }
                    else
                    {
                        return View(vehicle); 
                    }                       
                   
                }

                return View(vehicleMaster);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(vehicleMaster);
            }
        }

        // GET: VehicleMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            VehicleMaster vehicleMaster = await db.VehicleMasters.FindAsync(id);
            ViewBag.VehicleType = db.VehicleTypes.OrderBy(a => a.VehicleType).ToList();
            ViewBag.RCValidity = Convert.ToDateTime(vehicleMaster.RCValidity).ToString("dd/MM/yyyy");
            ViewBag.VendorID = db.VendorMasters.OrderBy(a => a.VendorName).ToList();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }        
            if (vehicleMaster == null)
            {
                return HttpNotFound();
            }
            return View(vehicleMaster);
        }

        // POST: VehicleMasters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VehicleMaster vehicleMaster)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var toupdate = db.VehicleMasters.Find(vehicleMaster.ID);
                    toupdate.UpdatedBy = User.Identity.Name;
                    toupdate.UpdatedDate = DateTime.Now;
                    db.Entry(toupdate).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    TempData["success"] = "Record Updated Success!";
                    return RedirectToAction("Index");
                }
                return View(vehicleMaster);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(vehicleMaster);
            }

        }

        // GET: VehicleMasters/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleMaster vehicleMaster = await db.VehicleMasters.FindAsync(id);
            if (vehicleMaster == null)
            {
                return HttpNotFound();
            }
            return View(vehicleMaster);
        }

        public ActionResult CheckDuplicate(string VehicleNo, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                    var result = db.VehicleMasters.Where(x => x.VehicleNo == VehicleNo && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.VehicleMasters.Where(x => x.VehicleNo == VehicleNo).ToList();
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult UploadFile()
        {
            var Ext = "";
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

                var FileNM = ID + "_" + fname;

                var FilName = System.IO.Path.Combine(Server.MapPath("~/Attachments/VehicleMaster/"), FileNM);
                if (System.IO.File.Exists(FilName))
                {
                    duplicate = duplicate + " , " + fname;
                }
                else
                {

                    var PAth = Path.Combine(Server.MapPath("~/Attachments/VehicleMaster/"), fname);
                    file.SaveAs(PAth);

                    System.IO.FileInfo fi = new System.IO.FileInfo(PAth);
                    if (fi.Exists)
                    {
                        var PAths = Path.Combine(Server.MapPath("~/Attachments/VehicleMaster/"), FileNM);
                        fi.MoveTo(PAths);
                    }
                    F.Name = FileNM;
                    list.Add(F);
                }
            }
            var result = new { list = list, DuplicateImg = duplicate };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        internal class FileName
        {
            public string Name { get; set; }
        }
        public virtual ActionResult Download(string file)
        {
            string fullPath = Path.Combine(Server.MapPath("~/Attachments/ExpenceBill/"), file);
            return File(fullPath, "application/vnd.ms-excel", file);
        }
        public ActionResult DeleteFiles(int? ID, string FileName, string Mode)
        {
            var result = "";
            try
            {
                if (Mode == "Create" | ID == 0)
                {
                    var PAthss = Path.Combine(Server.MapPath("~/Attachments/VehicleMaster/"), FileName);
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
                    var PAthss = Path.Combine(Server.MapPath("~/Attachments/VehicleMaster/"), FileName);
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
                                var PAth = "/Attachments/VehicleMaster/" + x.Name;
                                DMSFile F = new DMSFile();
                                F.Modulename = "VehicleMaster";
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
                var data = db.DMSFile.Where(x => x.ModuleID == ID && x.Modulename == "VehicleMaster").ToList();
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
    }
}
