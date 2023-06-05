using Micraft.ManeGrowAgro.Models;
using Micraft.ManeGrowAgro.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class WeeklyProdPlanController : Controller
    {
        // GET: WeeklyProdPlan
        private ManeGrowContext db = new ManeGrowContext();

        [CustomAuthorize("You dont have access to View Weekly Prod Plan", "HarvestingEdit,HarvestingView, Admin")]
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
                int totalRecord = db.WeeklyProdPlan.Count();

                var result = db.WeeklyProdPlan
                    .AsEnumerable()
                    .Select(x => new { Id = x.Id, Year = x.Year, Month = x.Month, RoomNo = x.RoomNo, Total = x.Total }).OrderByDescending(x => x.Id).ToList();

                if (!string.IsNullOrEmpty(sSearch))
                {
                    result = db.WeeklyProdPlan.AsEnumerable()
                        .Where(x => x.Id.ToString().Contains(sSearch) || x.Year.ToString().Contains(sSearch) || x.RoomNo.ToString().Contains(sSearch) || x.Total.ToString().Contains(sSearch))
                        .Take(iDisplayLength)
                        .Select(x => new { Id = x.Id, Year = x.Year, Month = x.Month, RoomNo = x.RoomNo, Total = x.Total }).OrderByDescending(x => x.Id).ToList();
                }
                else
                {
                    result = db.WeeklyProdPlan.AsEnumerable()
                        .Select(x => new { x.Id, x.Year, x.Month, x.RoomNo, x.Total }).OrderByDescending(x => x.Id).ToList();
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

        [CustomAuthorize("You dont have access to Create Weekly Prod Plan", "HarvestingEdit,Admin")]
        public ActionResult Create()
        {

            ViewBag.RoomNo = db.RoomMaster.ToList();
            return View();

        }

        [HttpPost]
        public JsonResult Save(List<WeeklyProdPlan> recs,string FROMDATE,string TODATE)
        {
            try
            {
                //save without excel.
                var result = new { Message = "success" };
                if (recs == null)
                {
                    recs = new List<WeeklyProdPlan>();
                    result = new { Message = "failed" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var startdate=Convert.ToDateTime(FROMDATE);
                var enddate = Convert.ToDateTime(TODATE);
               
                

                //Loop and insert records.
                foreach (WeeklyProdPlan weeklyprodplan in recs)
                {
                    var count = 0;

                    for (DateTime i = startdate; i <= enddate; i = i.AddDays(1))
                    {
                        count = count + 1;

                        weeklyprodplan.Year = i.Year;
                        weeklyprodplan.Month = i.Month;
                       
                        var isexist = db.WeeklyProdPlan.Where(x => x.Year == weeklyprodplan.Year && x.Month == weeklyprodplan.Month && x.RoomNo == weeklyprodplan.RoomNo).FirstOrDefault();

                        if(isexist!=null)
                        {
                            //clear column values

                            isexist.D1 = null;
                            isexist.D2 = null;
                            isexist.D3 = null;
                            isexist.D4 = null;
                            isexist.D5 = null;
                            isexist.D6 = null;
                            isexist.D7 = null;
                            isexist.D8 = null;
                            isexist.D9 = null;
                            isexist.D10 = null;
                            isexist.D11 = null;
                            isexist.D12 = null;
                            isexist.D13 = null;
                            isexist.D14 = null;
                            isexist.D15 = null;
                            isexist.D16 = null;
                            isexist.D17 = null;
                            isexist.D18 = null;
                            isexist.D19 = null;
                            isexist.D20 = null;
                            isexist.D21 = null;
                            isexist.D22 = null;
                            isexist.D23 = null;
                            isexist.D24 = null;
                            isexist.D25 = null;
                            isexist.D26 = null;
                            isexist.D27 = null;
                            isexist.D28 = null;
                            isexist.D29 = null;
                            isexist.D30 = null;
                            isexist.D31 = null;

                            db.Entry(isexist).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }

                    count = 0;
                    int? total = 0;
                    int? total2 = 0;
                    for (DateTime i= startdate;i<= enddate;i=i.AddDays(1))
                    {
                        count = count + 1;

                        weeklyprodplan.Year = i.Year;
                        weeklyprodplan.Month = i.Month;
                        var day = i.Day;
                        var DayName = "D" + day.ToString();
                        

                        var isexist = db.WeeklyProdPlan.Where(x => x.Year == weeklyprodplan.Year && x.Month == weeklyprodplan.Month && x.RoomNo == weeklyprodplan.RoomNo).FirstOrDefault();

                        if(isexist==null)
                        {
                            var propertyinfo = weeklyprodplan.GetType().GetProperty(DayName);
                            //save
                            if (count == 1)
                            {
                                propertyinfo.SetValue(weeklyprodplan, weeklyprodplan.Day1);
                                total = total + weeklyprodplan.Day1;
                            }
                            else if (count == 2)
                            {
                                propertyinfo.SetValue(weeklyprodplan, weeklyprodplan.Day2);
                                total = total + weeklyprodplan.Day2;
                            }
                            else if (count == 3)
                            {
                                propertyinfo.SetValue(weeklyprodplan, weeklyprodplan.Day3);
                                total = total + weeklyprodplan.Day3;
                            }
                            else if (count == 4)
                            {
                                propertyinfo.SetValue(weeklyprodplan, weeklyprodplan.Day4);
                                total = total + weeklyprodplan.Day4;
                            }
                            else if (count == 5)
                            {
                                propertyinfo.SetValue(weeklyprodplan, weeklyprodplan.Day5);
                                total = total + weeklyprodplan.Day5;
                            }
                            else if (count == 6)
                            {
                                propertyinfo.SetValue(weeklyprodplan, weeklyprodplan.Day6);
                                total = total + weeklyprodplan.Day6;
                            }
                            else if (count == 7)
                            {
                                propertyinfo.SetValue(weeklyprodplan, weeklyprodplan.Day7);
                                total = total + weeklyprodplan.Day7;
                            }
                            weeklyprodplan.Total = total;
                            weeklyprodplan.CreatedBy = User.Identity.Name;
                            weeklyprodplan.CreatedDate = DateTime.Now;

                            db.WeeklyProdPlan.Add(weeklyprodplan);
                            db.SaveChanges();

                        }
                        else
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            if(columnvalue==null)
                            {
                                //save
                                var propertyinfo2 = weeklyprodplan.GetType().GetProperty(DayName);
                                if (count == 1)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day1);
                                    total2 = total2 + weeklyprodplan.Day1;
                                }
                                else if (count == 2)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day2);
                                    total2 = total2 + weeklyprodplan.Day2;
                                }
                                else if (count == 3)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day3);
                                    total2 = total2 + weeklyprodplan.Day3;
                                }
                                else if (count == 4)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day4);
                                    total2 = total2 + weeklyprodplan.Day4;
                                }
                                else if (count == 5)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day5);
                                    total2 = total2 + weeklyprodplan.Day5;
                                }
                                else if (count == 6)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day6);
                                    total2 = total2 + weeklyprodplan.Day6;
                                }
                                else if (count == 7)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day7);
                                    total2 = total2 + weeklyprodplan.Day7;
                                }
                            }
                            else
                            {
                                var propertyinfo2 = weeklyprodplan.GetType().GetProperty(DayName);
                                if (count == 1)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day1);
                                    total2 = total2 + weeklyprodplan.Day1;
                                }
                                else if(count==2)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day2);
                                    total2 = total2 + weeklyprodplan.Day2;
                                }
                                else if (count == 3)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day3);
                                    total2 = total2 + weeklyprodplan.Day3;
                                }
                                else if (count == 4)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day4);
                                    total2 = total2 + weeklyprodplan.Day4;
                                }
                                else if (count == 5)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day5);
                                    total2 = total2 + weeklyprodplan.Day5;
                                }
                                else if (count == 6)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day6);
                                    total2 = total2 + weeklyprodplan.Day6;
                                }
                                else if (count == 7)
                                {
                                    propertyinfo2.SetValue(isexist, weeklyprodplan.Day7);
                                    total2 = total2 + weeklyprodplan.Day7;
                                }
                            }
                            isexist.Total = total2;
                            isexist.UpdatedBy = User.Identity.Name;
                            isexist.UpdatedDate = DateTime.Now;

                            db.Entry(isexist).State = EntityState.Modified;
                            db.SaveChanges();


                        }


                    }
                    

                }

                result = new { Message = "success" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetMonthlyData(string STARTDATE,string ROOMNO,string ENDDATE)
        {
            try
            {
                var startdate = Convert.ToDateTime(STARTDATE);
                var enddate = Convert.ToDateTime(ENDDATE);

                var weekdayname = startdate.DayOfWeek.ToString();

                var count = 0;
                Days days = new Days();
                for (DateTime i = startdate; i <= enddate; i = i.AddDays(1))
                {
                    count = count + 1;

                    var Year = i.Year;
                    var month = i.Month;
                    var day = i.Day;
                    var DayName = "D" + day.ToString();

                    var isexist = db.MonthlyProdPlan.Where(x => x.Year == Year && x.Month == month && x.RoomNo == ROOMNO).FirstOrDefault();
                    
                    
                    if (isexist!=null)
                    {
                        
                        if(count==1)
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            days.D1 = Convert.ToInt32(columnvalue);
                        }
                        else if(count==2)
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            days.D2 = Convert.ToInt32(columnvalue);
                        }
                        else if (count == 3)
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            days.D3 = Convert.ToInt32(columnvalue);
                        }
                        else if (count == 4)
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            days.D4 = Convert.ToInt32(columnvalue);
                        }
                        else if (count == 5)
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            days.D5 = Convert.ToInt32(columnvalue);
                        }
                        else if (count == 6)
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            days.D6 = Convert.ToInt32(columnvalue);
                        }
                        else if (count == 7)
                        {
                            var columnvalue = isexist.GetType().GetProperty(DayName).GetValue(isexist);
                            days.D7 = Convert.ToInt32(columnvalue);
                        }

                    }

                }

                //days.DayName = weekdayname;

                return Json(days, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                var result = new { Message = Ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [CustomAuthorize("You dont have access to Delete Weekly Prod Plan", "HarvestingEdit,Admin")]
        public JsonResult DeleteConfirmed(int? id)
        {
            try
            {
                var WeeklyProdPlan = db.WeeklyProdPlan.Where(a => a.Id == id).FirstOrDefault();
                db.WeeklyProdPlan.Remove(WeeklyProdPlan);
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

        [HttpPost]
        public JsonResult GetRecord(int ID)
        {
            try
            {
                var result = db.WeeklyProdPlan.Where(m => m.Id == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetNextSevenDays(string STARTDATE)
        {
            var strdate = Convert.ToDateTime(STARTDATE).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
            var startdate = Convert.ToDateTime(strdate);

            var lastdate = startdate.AddDays(6);

            var thelastday= lastdate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

            var count = 0;
            var dayname = startdate.DayOfWeek.ToString();

            Days days = new Days();

            for (var i = startdate; i <= lastdate; i.AddDays(1))
            {
                count += 1;

                var day = i.Day;
                
                if (count <= 7)
                {
                    if (count == 1)
                    {
                        days.D1 = day;
                    }
                    else if (count == 2)
                    {
                        days.D2 = day;
                    }
                    else if (count == 3)
                    {
                        days.D3 = day;
                    }
                    else if (count == 4)
                    {
                        days.D4 = day;
                    }
                    else if (count == 5)
                    {
                        days.D5 = day;
                    }
                    else if (count == 6)
                    {
                        days.D6 = day;
                    }
                    else if (count == 7)
                    {
                        days.D7 = day;
                    }

                }
                i = i.AddDays(1);
            }

            days.DayName= dayname;
            days.Todate = thelastday;

            var tbldata = days;

            return Json(tbldata, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRoomNumbers()
        {

            var roomnos = db.RoomMaster.ToList();

            var tbldata = roomnos;

            return Json(tbldata, JsonRequestBehavior.AllowGet);
        }

    }
}

public class Days
{
    public int D1 { get; set; }
    public int D2 { get; set; }
    public int D3 { get; set; }
    public int D4 { get; set; }
    public int D5 { get; set; }
    public int D6 { get; set; }
    public int D7 { get; set; }
    public string DayName { get; set; }
    public string Todate { get; set; }

}