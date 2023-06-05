using Micraft.ManeGrowAgro.Models;
using Micraft.ManeGrowAgro.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class DailyProdPlanController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        [CustomAuthorize("You dont have access to View Daily Prod Plan", "HarvestingEdit,HarvestingView, Admin")]
        public ActionResult Index()
        {
            ViewBag.RoomNo=db.RoomMaster.OrderBy(a=>a.RoomNo).ToList();
            
            return View();
        }


        public ActionResult GetMonthlyAndWeekly(string DATE,string ROOMNO)
        {
            try
            {
                var Date = Convert.ToDateTime(DATE).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime Dt = Convert.ToDateTime(Date);

                var DayName ="D"+Dt.Day.ToString();
                var AllDataMonth = db.MonthlyProdPlan.ToList();
                var AllDataWeek = db.WeeklyProdPlan.ToList();
                var monthly = AllDataMonth.Where(x => x.Year == Dt.Year && x.Month == Dt.Month && x.RoomNo == ROOMNO).Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null)).FirstOrDefault();
                var weekly= AllDataWeek.Where(x => x.Year == Dt.Year && x.Month == Dt.Month && x.RoomNo == ROOMNO).Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null)).FirstOrDefault();

                if(monthly==null)
                {
                    monthly = 0;
                }
                if(weekly==null)
                {
                    weekly = 0;
                }

                var result = new { Message = "success", monthly, weekly };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var result = new { Message = "failed" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Save(List<DailyProdPlan> recs)
        {
            var result = new { Message = "success" };
            if (recs == null)
            {
                recs = new List<DailyProdPlan>();
                result = new { Message = "failed" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //var DBobj = db.AccountsConfigDatas.Find(0);
            //var propertyInfo = DBobj.GetType().GetProperty(name);
            //propertyInfo.SetValue(DBobj, NewValue);
            //db.Entry(DBobj).State = EntityState.Modified;
            //db.SaveChanges();

            //Loop and insert records.
            foreach (DailyProdPlan dailyprodplan in recs)
            {
                dailyprodplan.Year = dailyprodplan.Date.Value.Year;
                dailyprodplan.Month = dailyprodplan.Date.Value.Month;

                //dynamic columnname
                var DayName = "D" + dailyprodplan.Date.Value.Day.ToString();
                var propertyinfo = dailyprodplan.GetType().GetProperty(DayName);
                propertyinfo.SetValue(dailyprodplan,dailyprodplan.Projection);

                var DailyData = db.DailyProdPlan.ToList();
                var isexist= DailyData.Where(x => x.Year == dailyprodplan.Year && x.Month == dailyprodplan.Month && x.RoomNo==dailyprodplan.RoomNo).FirstOrDefault();

                if(isexist==null)
                {
                    dailyprodplan.CreatedBy= User.Identity.Name;
                    dailyprodplan.CreatedDate = DateTime.Now;
                    db.DailyProdPlan.Add(dailyprodplan);
                }
                else if(isexist!=null)
                {

                    var DayName2 = "D" + dailyprodplan.Date.Value.Day.ToString();
                    var propertyinfo2 = dailyprodplan.GetType().GetProperty(DayName2);
                    propertyinfo2.SetValue(isexist, dailyprodplan.Projection);

                    isexist.UpdatedBy= User.Identity.Name;
                    isexist.UpdatedDate = DateTime.Now;

                    db.Entry(isexist).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            

            result = new { Message = "success" };
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        [CustomAuthorize("You dont have access to Delete Daily Prod Plan", "HarvestingEdit,Admin")]
        public JsonResult Delete(int ID)
        {
           

            ////dynamic columnname
            //var DayName = "D" + dailyprodplan.Date.Value.Day.ToString();
            //var propertyinfo = dailyprodplan.GetType().GetProperty(DayName);
            //propertyinfo.SetValue(dailyprodplan, dailyprodplan.Projection);

            //var DailyData = db.DailyProdPlan.ToList();
            //var isexist = DailyData.Where(x => x.Id==ID).FirstOrDefault();


            //    var DayName2 = "D" + isexist.CreatedBy.Value.Day.ToString();
            //    var propertyinfo2 = dailyprodplan.GetType().GetProperty(DayName2);
            //    propertyinfo2.SetValue(isexist, dailyprodplan.Projection);

            //    isexist.UpdatedBy = User.Identity.Name;
            //    isexist.UpdatedDate = DateTime.Now;

            //    db.Entry(isexist).State = EntityState.Modified;
            
            //db.SaveChanges();


            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetToDAYSData(string DATE)
        {
            try
            {
                List<DailyProdPlan> DailyProduction = new List<DailyProdPlan>();
                if (DATE != "")
                {
                   
                    DateTime? Date = Convert.ToDateTime(DATE);
                    
                    DailyProduction = db.DailyProdPlan.Where(a => a.Year == Date.Value.Year && a.Month==Date.Value.Month).OrderByDescending(a => a.Id).ToList();

                    var DayName = "D" + Date.Value.Day.ToString();
                    //var propertyinfo2 = DailyProduction.GetType().GetProperty(DayName);

                    var AllDataMonth = db.MonthlyProdPlan.ToList();
                    var AllDataWeek = db.WeeklyProdPlan.ToList();

                    foreach (var item in DailyProduction)
                    {
                        var dayproduction = DailyProduction.Where(x=>x.Year==item.Year && x.Month==item.Month && x.RoomNo==item.RoomNo).Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null)).FirstOrDefault();

                        if(dayproduction != null)
                        {
                            item.Projection = dayproduction.Value;
                        }
                        else
                        {
                            item.Projection = 0;
                        }

                        var monthly = AllDataMonth.Where(x => x.Year == item.Year && x.Month == item.Month && x.RoomNo == item.RoomNo).Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null)).FirstOrDefault();
                        var weekly = AllDataWeek.Where(x => x.Year == item.Year && x.Month == item.Month && x.RoomNo == item.RoomNo).Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null)).FirstOrDefault();

                        if (monthly == null)
                        {
                            monthly = 0;
                        }
                        if (weekly == null)
                        {
                            weekly = 0;
                        }
                        item.Monthlyh = monthly;
                        item.Weeklyh = weekly;

                    }

                    var result = new { Message = "success", DailyProduction };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    DateTime? Date = DateTime.Now;

                    DailyProduction = db.DailyProdPlan.Where(a => a.Year == Date.Value.Year && a.Month == Date.Value.Month).OrderByDescending(a => a.Id).ToList();

                    var DayName = "D" + Date.Value.Day.ToString();
                    //var propertyinfo2 = DailyProduction.GetType().GetProperty(DayName);


                    foreach (var item in DailyProduction)
                    {
                        var dayproduction = DailyProduction.Where(x => x.Year == item.Year && x.Month == item.Month && x.RoomNo == item.RoomNo).Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null)).FirstOrDefault();

                        if (dayproduction != null)
                        {
                            item.Projection = dayproduction.Value;
                        }
                        else
                        {
                            item.Projection = 0;
                        }

                    }

                    var result = new { Message = "success", DailyProduction };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                var result = new { Message = "failed" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}