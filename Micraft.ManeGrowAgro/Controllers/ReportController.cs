using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class ReportController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SetDates( string FROMDATE,string TODATE)
        {

            Session["Fromdate"] = FROMDATE;
            Session["Todate"] = TODATE;

            var result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string GetData(string sEcho, int iDisplayStart, int iDisplayLength, string sSearch)
        {
            try
            {

                var FROMDATE=Session["Fromdate"];
                var TODATE= Session["Todate"];

                DateTime? fromd = Convert.ToDateTime(FROMDATE);
                DateTime? tod = Convert.ToDateTime(TODATE);

                string test = string.Empty;
                sSearch = sSearch.ToLower();

                int totalRecord = db.MainOrder.Count();

                var result = (from x in db.MainOrder.Where(w=>w.Date>=fromd && w.Date<=tod)
                              join c in db.CustomerMasters
                              on x.CustomerId equals c.ID
                              join p in db.ProductTypes
                              on x.CategoryId equals p.ID
                              join o in db.Order
                              on x.Id equals o.MasterOrderId
                              
                              select new
                              {
                                  ID = x.Id,
                                  Date = x.Date,
                                  CustomerName = c.CustName,
                                  Location = c.Area,
                                  ShortName=c.ShortName,
                                  City=c.City,
                                  CustomerCode = x.CustomerCode,
                                  CategoryName = p.Type,
                                  CustomerShortCode = x.CustomerShortCode,

                                  ProductName = o.ProductName,
                                  Qty = o.Qty,
                                  UOM = o.UOM,
                                  Weight = o.Weight,
                                  ExpectedDeliveryDate = x.ExpectedDeliveryDate, 
                                  ExpectedDeliveryTime = x.ExpectedDeliveryTime,
                                  Remark = x.Remark


                              }).OrderByDescending(x => x.ID).ToList();


                if (!string.IsNullOrEmpty(sSearch))
                {
                    //result = db.EmployeeMasters.AsEnumerable()
                    //    .Where(x => x.Name.ToLower().Contains(sSearch))
                    //    .Take(iDisplayLength)
                    //    .Select(x => new EmployeeMaster { ID = x.ID, Name = x.Name, Address = x.Address, AdharNumber = x.AdharNumber, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress,DesigID=x.DesigID }).OrderByDescending(x => x.ID).ToList();
                    result = (from x in db.MainOrder.Where(w => w.Date >= fromd && w.Date <= tod)
                              join c in db.CustomerMasters
                              on x.CustomerId equals c.ID
                              join p in db.ProductTypes
                              on x.CategoryId equals p.ID
                              join o in db.Order
                              on x.Id equals o.MasterOrderId
                              select new
                              {
                                  ID = x.Id,
                                  Date = x.Date,
                                  CustomerName = c.CustName,
                                  Location = c.Area,
                                  ShortName = c.ShortName,
                                  City = c.City,
                                  CustomerCode = x.CustomerCode,
                                  CategoryName = p.Type,
                                  CustomerShortCode = x.CustomerShortCode,

                                  ProductName = o.ProductName,
                                  Qty = o.Qty,
                                  UOM = o.UOM,
                                  Weight = o.Weight,
                                  ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                                  ExpectedDeliveryTime = x.ExpectedDeliveryTime,
                                  Remark = x.Remark

                              }).Where(x => x.CustomerName.Contains(sSearch) || x.Date.ToString().Contains(sSearch) || x.Location.ToLower().Contains(sSearch) || x.CategoryName.Contains(sSearch) || x.ID.ToString().Contains(sSearch) || x.ProductName.ToString().Contains(sSearch) || x.Qty.ToString().Contains(sSearch) || x.UOM.ToString().Contains(sSearch) || x.Weight.ToString().Contains(sSearch))
                              .Take(iDisplayLength)
                              .OrderByDescending(x => x.ID).ToList();
                }
                else
                {
                    //result = db.EmployeeMasters.AsEnumerable()
                    //    .Select(x => new EmployeeMaster { ID = x.ID, Name = x.Name, Address = x.Address, AdharNumber = x.AdharNumber, MobileNumber = x.MobileNumber, EmailAddress = x.EmailAddress,DesigID=x.DesigID }).OrderByDescending(x => x.ID).ToList();
                    result = (from x in db.MainOrder.Where(w => w.Date >= fromd && w.Date <= tod)
                              join c in db.CustomerMasters
                              on x.CustomerId equals c.ID
                              join p in db.ProductTypes
                              on x.CategoryId equals p.ID
                              join o in db.Order
                              on x.Id equals o.MasterOrderId
                              select new
                              {
                                  ID = x.Id,
                                  Date = x.Date,
                                  CustomerName = c.CustName,
                                  Location = c.Area,
                                  ShortName = c.ShortName,
                                  City = c.City,
                                  CustomerCode = x.CustomerCode,
                                  CategoryName = p.Type,
                                  CustomerShortCode = x.CustomerShortCode,

                                  ProductName = o.ProductName,
                                  Qty = o.Qty,
                                  UOM = o.UOM,
                                  Weight = o.Weight,
                                  ExpectedDeliveryDate = x.ExpectedDeliveryDate,
                                  ExpectedDeliveryTime = x.ExpectedDeliveryTime,
                                  Remark = x.Remark

                              }).OrderByDescending(x => x.ID).ToList();
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
        public ActionResult MonthlyProdReport()
        {
            ViewBag.RoomNo = db.RoomMaster.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult GetMonthlyReport(int? YEAR, int? MONTH)
        {

            var data = db.MonthlyProdPlan.Where(x => x.Year == YEAR && x.Month == MONTH).ToList();

            foreach (var item in data)
            {

                item.CreatedDateS = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yy") : string.Empty;
                item.UpdatedDateS = item.UpdatedDate.HasValue ? item.UpdatedDate.Value.ToString("dd/MM/yy") : string.Empty;
            }

            return Json(data);
            //return Json(new { data = data }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult WeeklyProdReport()
        {
            ViewBag.RoomNo = db.RoomMaster.ToList();
            return View();
        }

        [HttpPost]
        public ActionResult GetWeeklyReport(string STARTDATE, string ENDDATE, int ROOMNO)
        {

            var data = db.WeeklyProdPlan.ToList();

            return Json(data);
        }

        [HttpGet]
        public JsonResult GetWeeklyReport(string STARTDATE/*, int? ROOMNO*/, string ENDDATE)
        {
            try
            {
                var startdate = Convert.ToDateTime(STARTDATE);
                var enddate = Convert.ToDateTime(ENDDATE);

                var weekdayname = startdate.DayOfWeek.ToString();

                List<DaysW> tbldata = new List<DaysW>();
                List<DaysW> finaldata = new List<DaysW>();

                var count = 0;

                for (DateTime i = startdate; i <= enddate; i = i.AddDays(1))
                {
                    count = count + 1;
                    var Year = i.Year;
                    var month = i.Month;
                    var day = i.Day;
                    var DayName = "D" + day.ToString();

                    var isexist = db.WeeklyProdPlan.Where(x => x.Year == Year && x.Month == month /*&& x.RoomNo == ROOMNO*/).ToList();


                    if (isexist.Count > 0)
                    {
                        //var subcount = 0;
                        //List<int?> prevroomno = new List<int?>();

                        foreach (var item in isexist)
                        {

                            //subcount = subcount + 1;
                            DaysW days = new DaysW();

                            days.CreatedBy = item.CreatedBy;
                            days.CreatedDateS = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yy") : string.Empty;
                            days.UpdatedBy = item.UpdatedBy;
                            days.UpdatedDateS = item.UpdatedDate.HasValue ? item.UpdatedDate.Value.ToString("dd/MM/yy") : string.Empty;
                            days.RoomNo = item.RoomNo;
                            //tbldata.Add(days);

                            if (count == 1)
                            {

                                var columnvalue = item.GetType().GetProperty(DayName).GetValue(item);
                                days.D1 = Convert.ToInt32(columnvalue);

                                if (finaldata.Count > 0)
                                {
                                    var toupdate = finaldata.Where(x => x.RoomNo == days.RoomNo).FirstOrDefault();

                                    if (toupdate != null)
                                    {
                                        toupdate.D1 = days.D1;
                                    }
                                    else
                                    {
                                        finaldata.Add(days);
                                    }
                                }
                                else
                                {
                                    finaldata.Add(days);
                                }
                            }
                            else if (count == 2)
                            {
                                var columnvalue = item.GetType().GetProperty(DayName).GetValue(item);
                                days.D2 = Convert.ToInt32(columnvalue);

                                if (finaldata.Count > 0)
                                {
                                    var toupdate = finaldata.Where(x => x.RoomNo == days.RoomNo).FirstOrDefault();

                                    if (toupdate != null)
                                    {
                                        toupdate.D2 = days.D2;
                                    }
                                    else
                                    {
                                        finaldata.Add(days);
                                    }
                                }
                                else
                                {
                                    finaldata.Add(days);
                                }

                            }
                            else if (count == 3)
                            {
                                var columnvalue = item.GetType().GetProperty(DayName).GetValue(item);
                                days.D3 = Convert.ToInt32(columnvalue);

                                if (finaldata.Count > 0)
                                {
                                    var toupdate = finaldata.Where(x => x.RoomNo == days.RoomNo).FirstOrDefault();

                                    if (toupdate != null)
                                    {
                                        toupdate.D3 = days.D3;
                                    }
                                    else
                                    {
                                        finaldata.Add(days);
                                    }
                                }
                                else
                                {
                                    finaldata.Add(days);
                                }

                            }
                            else if (count == 4)
                            {
                                var columnvalue = item.GetType().GetProperty(DayName).GetValue(item);
                                days.D4 = Convert.ToInt32(columnvalue);

                                if (finaldata.Count > 0)
                                {
                                    var toupdate = finaldata.Where(x => x.RoomNo == days.RoomNo).FirstOrDefault();

                                    if (toupdate != null)
                                    {
                                        toupdate.D4 = days.D4;
                                    }
                                    else
                                    {
                                        finaldata.Add(days);
                                    }
                                }
                                else
                                {
                                    finaldata.Add(days);
                                }

                            }
                            else if (count == 5)
                            {
                                var columnvalue = item.GetType().GetProperty(DayName).GetValue(item);
                                days.D5 = Convert.ToInt32(columnvalue);

                                if (finaldata.Count > 0)
                                {
                                    var toupdate = finaldata.Where(x => x.RoomNo == days.RoomNo).FirstOrDefault();

                                    if (toupdate != null)
                                    {
                                        toupdate.D5 = days.D5;
                                    }
                                    else
                                    {
                                        finaldata.Add(days);
                                    }
                                }
                                else
                                {
                                    finaldata.Add(days);
                                }

                            }
                            else if (count == 6)
                            {
                                var columnvalue = item.GetType().GetProperty(DayName).GetValue(item);
                                days.D6 = Convert.ToInt32(columnvalue);

                                if (finaldata.Count > 0)
                                {
                                    var toupdate = finaldata.Where(x => x.RoomNo == days.RoomNo).FirstOrDefault();

                                    if (toupdate != null)
                                    {
                                        toupdate.D6 = days.D6;
                                    }
                                    else
                                    {
                                        finaldata.Add(days);
                                    }
                                }
                                else
                                {
                                    finaldata.Add(days);
                                }

                            }
                            else if (count == 7)
                            {
                                var columnvalue = item.GetType().GetProperty(DayName).GetValue(item);
                                days.D7 = Convert.ToInt32(columnvalue);

                                if (finaldata.Count > 0)
                                {
                                    var toupdate = finaldata.Where(x => x.RoomNo == days.RoomNo).FirstOrDefault();

                                    if (toupdate != null)
                                    {
                                        toupdate.D7 = days.D7;
                                    }
                                    else
                                    {
                                        finaldata.Add(days);
                                    }
                                }
                                else
                                {
                                    finaldata.Add(days);
                                }

                            }

                        }
                    }

                }

                return Json(finaldata, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ActualProdReport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetActualReport(int? YEAR, int? MONTH)
        {

            var data = db.ActualProduction.Where(x => x.Year == YEAR && x.Month == MONTH).ToList();

            foreach (var item in data)
            {
                item.CreatedDateS = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yy") : string.Empty;
                item.UpdatedDateS = item.UpdatedDate.HasValue ? item.UpdatedDate.Value.ToString("dd/MM/yy") : string.Empty;
            }

            return Json(data);
            //return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DailyProdReport()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetDailyReport(int? YEAR, int? MONTH)
        {

            var data = db.DailyProdPlan.Where(x => x.Year == YEAR && x.Month == MONTH).ToList();

            foreach (var item in data)
            {
                item.CreatedDateS = item.CreatedDate.HasValue ? item.CreatedDate.Value.ToString("dd/MM/yy") : string.Empty;
                item.UpdatedDateS = item.UpdatedDate.HasValue ? item.UpdatedDate.Value.ToString("dd/MM/yy") : string.Empty;
            }

            return Json(data);
            //return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

    }


    public class DaysW
    {
        public string RoomNo { get; set; }
        public int D1 { get; set; }
        public int D2 { get; set; }
        public int D3 { get; set; }
        public int D4 { get; set; }
        public int D5 { get; set; }
        public int D6 { get; set; }
        public int D7 { get; set; }
        public string DayName { get; set; }
        public string Todate { get; set; }

        public string CreatedBy { get; set; }
        public string CreatedDateS { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDateS { get; set; }

    }
}