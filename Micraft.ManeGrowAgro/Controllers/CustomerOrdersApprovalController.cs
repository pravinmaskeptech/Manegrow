using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    
    public class CustomerOrdersApprovalController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {
                string test = string.Empty;
                sSearch = sSearch.ToLower();
             //   int totalRecord = db.MainOrder.Count();
                if (User.IsInRole("Sales"))
                {
                    var salesID = db.EmployeeMasters.Where(a => a.Username == User.Identity.Name).Select(a => a.ID).SingleOrDefault();
                    var usernames = db.CustomerMasters.Where(a => a.SalesPersonID == salesID).Select(a => a.Username).ToList();

                    var result = (from x in db.MainOrder.Where(a => a.IsApprove != true && usernames.Contains(a.CreatedBy))
                                  join c in db.CustomerMasters
                                  on x.CustomerId equals c.ID
                                  join p in db.ProductTypes
                                  on x.CategoryId equals p.ID
                                  select new
                                  {
                                      ID = x.Id,
                                      Date = x.Date,
                                      CustomerName = c.CustName,
                                      Location = c.Area,
                                      CustomerCode = x.CustomerCode,
                                      CategoryName = p.Type,
                                      CustomerShortCode = x.CustomerShortCode
                                  }).OrderByDescending(x => x.ID).ToList();

                    if (!string.IsNullOrEmpty(sSearch))
                    {
                        result = (from x in db.MainOrder.Where(a => a.IsApprove != true && usernames.Contains(a.CreatedBy))
                                  join c in db.CustomerMasters
                                  on x.CustomerId equals c.ID
                                  join p in db.ProductTypes
                                  on x.CategoryId equals p.ID
                                  select new
                                  {
                                      ID = x.Id,
                                      Date = x.Date,
                                      CustomerName = c.CustName,
                                      Location = c.Area,
                                      CustomerCode = x.CustomerCode,
                                      CategoryName = p.Type,
                                      CustomerShortCode = x.CustomerShortCode

                                  }).Where(x => x.CustomerName.ToLower().Contains(sSearch))
                                  .Take(iDisplayLength)
                                  .OrderByDescending(x => x.ID).ToList();
                    }
                    else
                    {
                        result = (from x in db.MainOrder.Where(a => a.IsApprove != true && usernames.Contains(a.CreatedBy))
                                  join c in db.CustomerMasters
                                  on x.CustomerId equals c.ID
                                  join p in db.ProductTypes
                                  on x.CategoryId equals p.ID
                                  select new
                                  {
                                      ID = x.Id,
                                      Date = x.Date,
                                      CustomerName = c.CustName,
                                      Location = c.Area,
                                      CustomerCode = x.CustomerCode,
                                      CategoryName = p.Type,
                                      CustomerShortCode = x.CustomerShortCode
                                  }).OrderByDescending(x => x.ID).ToList();
                    }


                    StringBuilder sb = new StringBuilder();
                    sb.Clear();
                    sb.Append("{");
                    sb.Append("\"sEcho\": ");
                    sb.Append(sEcho);
                    sb.Append(",");
                    sb.Append("\"iTotalRecords\": ");
                    sb.Append(result.Count);
                    sb.Append(",");
                    sb.Append("\"iTotalDisplayRecords\": ");
                    sb.Append(result.Count);
                    sb.Append(",");
                    sb.Append("\"aaData\": ");
                    sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                    sb.Append("}");
                    return sb.ToString();
                }else
                {
                    var result = (from x in db.MainOrder.Where(a => a.IsApprove != true)
                                  join c in db.CustomerMasters
                                  on x.CustomerId equals c.ID
                                  join p in db.ProductTypes
                                  on x.CategoryId equals p.ID
                                  select new
                                  {
                                      ID = x.Id,
                                      Date = x.Date,
                                      CustomerName = c.CustName,
                                      Location = c.Area,
                                      CustomerCode = x.CustomerCode,
                                      CategoryName = p.Type,
                                      CustomerShortCode = x.CustomerShortCode
                                  }).OrderByDescending(x => x.ID).ToList();

                    if (!string.IsNullOrEmpty(sSearch))
                    {
                        result = (from x in db.MainOrder
                                  join c in db.CustomerMasters
                                  on x.CustomerId equals c.ID
                                  join p in db.ProductTypes
                                  on x.CategoryId equals p.ID
                                  select new
                                  {
                                      ID = x.Id,
                                      Date = x.Date,
                                      CustomerName = c.CustName,
                                      Location = c.Area,
                                      CustomerCode = x.CustomerCode,
                                      CategoryName = p.Type,
                                      CustomerShortCode = x.CustomerShortCode

                                  }).Where(x => x.CustomerName.ToLower().Contains(sSearch))
                                  .Take(iDisplayLength)
                                  .OrderByDescending(x => x.ID).ToList();
                    }
                    else
                    {
                        result = (from x in db.MainOrder.Where(a=>a.IsApprove!=true)
                                  join c in db.CustomerMasters
                                  on x.CustomerId equals c.ID
                                  join p in db.ProductTypes
                                  on x.CategoryId equals p.ID
                                  select new
                                  {
                                      ID = x.Id,
                                      Date = x.Date,
                                      CustomerName = c.CustName,
                                      Location = c.Area,
                                      CustomerCode = x.CustomerCode,
                                      CategoryName = p.Type,
                                      CustomerShortCode = x.CustomerShortCode

                                  }).OrderByDescending(x => x.ID).ToList();
                    }


                    StringBuilder sb = new StringBuilder();
                    sb.Clear();
                    sb.Append("{");
                    sb.Append("\"sEcho\": ");
                    sb.Append(sEcho);
                    sb.Append(",");
                    sb.Append("\"iTotalRecords\": ");
                    sb.Append(result.Count);
                    sb.Append(",");
                    sb.Append("\"iTotalDisplayRecords\": ");
                    sb.Append(result.Count);
                    sb.Append(",");
                    sb.Append("\"aaData\": ");
                    sb.Append(Newtonsoft.Json.JsonConvert.SerializeObject(result));
                    sb.Append("}");
                    return sb.ToString();
                }
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
           
        }

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


            ViewBag.ProductTypes = db.ProductTypes.Where(x => x.IsActive == true).ToList();
            ViewBag.ShortNames = db.CustomerMasters.Select(x => new { x.ShortName, x.ID }).GroupBy(x => x.ShortName).Select(x => x.FirstOrDefault()).ToList();
            ViewBag.Id = maxid;
            return View();
        }

        public JsonResult GetApprovalData(int Id) 
        {
            try
            {
                var main = db.MainOrder.Where(x => x.Id == Id).FirstOrDefault();
                var result = new { Message = "success", main };
                return Json(result, JsonRequestBehavior.AllowGet);
            }catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateStatus(int? Id,Boolean? ISApprove,string Remark)   
        {
            try
            {
                if(ISApprove==true)
                {
                    Remark = "";
                }
                var ordermain = db.MainOrder.Where(a => a.Id == Id).FirstOrDefault();
                ordermain.IsApprove = ISApprove;
                ordermain.Remark = Remark;
                ordermain.ApprovedBy = User.Identity.Name;
                ordermain.ApprovedDate = DateTime.Now;
                db.SaveChanges();
                var result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var result = new { Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }
    }
}