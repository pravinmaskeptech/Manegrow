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
using ClosedXML.Excel;
using System.Data.SqlClient;
using System.Configuration;

namespace Micraft.ManeGrowAgro.Controllers
{
    [Authorize]
    public class PincodeMastersController : Controller
    {
        private ManeGrowContext db = new ManeGrowContext();

        // GET: PincodeMasters
        public async Task<ActionResult> Index()
        {
            return View(await db.PincodeMasters.ToListAsync());
        }
        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
                int totalRecord = db.PincodeMasters.Count();

                var result = db.PincodeMasters
                    .AsEnumerable()
                    .Select(x => new PincodeMasters { ID = x.ID, State = x.State, City = x.City, Pincode = x.Pincode, Area = x.Area }).OrderByDescending(x => x.ID).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.PincodeMasters.AsEnumerable()
                        .Where(x => x.Pincode.ToLower().Contains(sSearch) || x.City.ToLower().Contains(sSearch))
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new PincodeMasters { ID = x.ID, State = x.State, City = x.City, Pincode = x.Pincode, Area = x.Area }).OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    result = db.PincodeMasters.AsEnumerable()
                        .Skip(iDisplayStart).Take(iDisplayLength)
                        .Select(x => new PincodeMasters { ID = x.ID, State = x.State, City = x.City, Pincode = x.Pincode, Area = x.Area }).OrderByDescending(x => x.ID).ToList();
                }


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

        [HttpPost]
        public JsonResult AddPincode(PincodeMasters pincodeMasters)
        {
            try
            {
                db.PincodeMasters.Add(pincodeMasters);
                db.SaveChanges();
                return Json("1", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult UpdatePincode(PincodeMasters pincodeMasters)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    PincodeMasters pincode = db.PincodeMasters.Where(x => x.ID == pincodeMasters.ID).FirstOrDefault();
                    pincode.Area = pincodeMasters.Area;
                    pincode.Pincode = pincodeMasters.Pincode;
                    pincode.City = pincodeMasters.City;
                    pincode.State = pincodeMasters.State;
                    db.Entry(pincode).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json("1", JsonRequestBehavior.AllowGet);
                }
                return Json("0", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("0", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetPincode(int ID)
        {
            try
            {
                var result = db.PincodeMasters.Where(m => m.ID == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: PincodeMasters/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PincodeMasters pincodeMasters = await db.PincodeMasters.FindAsync(id);
            if (pincodeMasters == null)
            {
                return HttpNotFound();
            }
            return View(pincodeMasters);
        }

        // GET: PincodeMasters/Create
        public ActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(PincodeMasters pincodeMasters)
        {
            if (ModelState.IsValid)
            {
                pincodeMasters.CreatedBy = User.Identity.Name;
                pincodeMasters.CreatedDate = DateTime.Now;
                db.PincodeMasters.Add(pincodeMasters);
                await db.SaveChangesAsync();
                TempData["success"] = "Record Added Success!";

                return RedirectToAction("Index");
            }

            return View(pincodeMasters);
        }

        // GET: PincodeMasters/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PincodeMasters pincodeMasters = await db.PincodeMasters.FindAsync(id);
            if (pincodeMasters == null)
            {
                return HttpNotFound();
            }
            return View(pincodeMasters);
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(PincodeMasters pincodeMasters)
        {
            if (ModelState.IsValid)
            {
                var toupdate = db.PincodeMasters.Find(pincodeMasters.ID);
                toupdate.UpdatedBy = User.Identity.Name;
                toupdate.UpdatedDate = DateTime.Now;
                db.Entry(toupdate).State = EntityState.Modified;
                await db.SaveChangesAsync();
                TempData["success"] = "Record Update Success!";

                return RedirectToAction("Index");
            }
            return View(pincodeMasters);
        }


        public ActionResult CheckDuplicate(string Pincode, string City,string Zone,string Area,string State, string StateCode, string Mode, long ID)
        {
            try
            {
                if (Mode == "Create")
                {
                  var result = db.PincodeMasters.Where(x => x.Area == Area && x.ID != ID).ToList();
                    if (result.Count == 0)
                        return Json("0", JsonRequestBehavior.AllowGet);
                    else
                        return Json("1", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = db.PincodeMasters.Where(x => x.Pincode == Pincode && x.ID != ID).ToList();
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


        // POST: PincodeMasters/Delete/5
        [HttpGet]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var pincodeMasters = db.PincodeMasters.Where(a => a.ID == id).FirstOrDefault();
                db.PincodeMasters.Remove(pincodeMasters);
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Export()
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DownloadPincodeMaster"))
                {
                    using (SqlDataAdapter sda = new SqlDataAdapter())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
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
                                wb.Worksheets.Add(dt, "SP_MISDownload");

                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";

                                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                //Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
                                Response.ContentType = "application/text";
                                Response.AddHeader("content-disposition", "attachment;filename=PincodeMaster.csv");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
