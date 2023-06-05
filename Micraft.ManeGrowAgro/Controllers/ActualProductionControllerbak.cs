using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class ActualProductionController : Controller
    {
        ManeGrowContext db = new ManeGrowContext();
        // GET: ActualProduction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            try
            {
                ViewBag.RoomNo = db.RoomMaster.ToList();
                ViewBag.Caret = db.CaretMaster.ToList();
                return View();
            }
            catch(Exception e)
            {
                return View();
            }
        }

        public ActionResult GetProjection(string DATE, int ROOMNO)
        {
            try
            {
                var Date = Convert.ToDateTime(DATE).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                DateTime Dt = Convert.ToDateTime(Date);

                var DayName = "D" + Dt.Day.ToString();
                var AllDataDaily = db.DailyProdPlan.ToList();
                var projection = AllDataDaily.Where(x => x.Year == Dt.Year && x.Month == Dt.Month && x.RoomNo == ROOMNO).Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null)).FirstOrDefault();
               
                if (projection == null)
                {
                    projection = 0;
                }
                
                var result = new { Message = "success", projection };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var result = new { Message = "failed" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CalculateActualWeight(string CARET, decimal? GROSSWEIGHT, int? NOOFCARET)
        {
            try
            {
                var weight=db.CaretMaster.Where(x => x.CaretType == CARET).Select(x=>x.Weight).FirstOrDefault();
                decimal? ActualWeight = 0;

                if(weight!=null)
                {

                    ActualWeight = GROSSWEIGHT - (weight* NOOFCARET);

                }

                if (weight == null)
                {
                    ActualWeight = 0;
                }

                var result = new { Message = "success", ActualWeight };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                var result = new { Message = "failed" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetToDAYSData(string DATE)
        {
            try
            {
                List<Production> Product = new List<Production>();
                if (DATE!="")
                {
                    DateTime? Date = Convert.ToDateTime(DATE);
                     Product = db.Production.Where(a => a.Date == Date).OrderByDescending(a => a.Id).ToList();

                    var result = new { Message = "success", Product };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                     Product = db.Production.Where(a => a.Date == DateTime.Today).OrderByDescending(a => a.Id).ToList();

                    var result = new { Message = "success", Product };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

            }
            catch
            {
                var result = new { Message = "failed" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Edit(int? ID)
        {
            try
            {
                var result = db.Production.Where(x => x.Id == ID).FirstOrDefault();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult Save(List<Production> recs,int? ID)
        {
            try
            {

                var result = new { Message = "success" };
                if (recs == null)
                {
                    recs = new List<Production>();
                    result = new { Message = "failed" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                string lastdate = "";

                //Loop and insert records.
                foreach (Production production in recs)
                {

                    ActualProduction actualprodction = new ActualProduction();

                    actualprodction.Year = production.Date.Value.Year;
                    actualprodction.Month = production.Date.Value.Month;
                    actualprodction.RoomNo = production.RoomNo;

                    //dynamic columnname
                    var DayName = "D" + production.Date.Value.Day.ToString();
                    var propertyinfo = actualprodction.GetType().GetProperty(DayName);
                    propertyinfo.SetValue(actualprodction, production.ActualWeight);

                    var ActualData = db.ActualProduction.ToList();
                    var isexist = ActualData.Where(x => x.Year == actualprodction.Year && x.Month == actualprodction.Month && x.RoomNo == actualprodction.RoomNo).FirstOrDefault();

                    if (isexist == null)
                    {
                        actualprodction.CreatedBy = User.Identity.Name;
                        actualprodction.CreatedDate = DateTime.Now;
                        db.ActualProduction.Add(actualprodction);
                    }
                    else if (isexist != null)
                    {

                        var DayName2 = "D" + production.Date.Value.Day.ToString();
                        var propertyinfo2 = actualprodction.GetType().GetProperty(DayName2);
                        //.Select(s => (int?)s.GetType().GetProperty(DayName).GetValue(s, null))
                        decimal? prevproduction = ActualData.Where(x => x.Year == actualprodction.Year && x.Month == actualprodction.Month && x.RoomNo == actualprodction.RoomNo).Select(s => (decimal?)s.GetType().GetProperty(DayName2).GetValue(s, null)).FirstOrDefault();
                        
                        if(ID!=null)
                        {
                            var prod = db.Production.Where(x => x.Id == ID).FirstOrDefault();
                            prevproduction = prevproduction - prod.ActualWeight;
                        }
                        
                        decimal? newproduction = 0;
                        if(prevproduction==null)
                        {
                            prevproduction = 0;
                            prevproduction = prevproduction + production.ActualWeight;
                            newproduction = prevproduction;
                        }
                        else
                        {
                            prevproduction = prevproduction + production.ActualWeight;
                            newproduction = prevproduction;
                        }

                        propertyinfo2.SetValue(isexist, newproduction);

                        //total

                        var ActualData2 = db.ActualProduction.ToList();
                        var isexist2 = ActualData2.Where(x => x.Year == actualprodction.Year && x.Month == actualprodction.Month && x.RoomNo == actualprodction.RoomNo).FirstOrDefault();

                        var total = 0;

                        for (int i = 1; i <= 31; i++)
                        {
                            var tDayName = "D" + i.ToString();
                            var val = isexist2.GetType().GetProperty(tDayName).GetValue(isexist2);

                            var val2 = Convert.ToInt32(val);
                            total = total + val2;
                        }

                        isexist.Total = total;
                        //total

                        isexist.UpdatedBy = User.Identity.Name;
                        isexist.UpdatedDate = DateTime.Now;



                        db.Entry(isexist).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                    //save in production
                    if(ID==null)
                    { 
                    production.CreatedBy = User.Identity.Name;
                    production.CreatedDate = DateTime.Today;
                    db.Production.Add(production);
                    db.SaveChanges();
                    }
                    else if(ID!=null)
                    {
                        var toupdate = db.Production.Where(x => x.Id == ID).FirstOrDefault();
                        toupdate.UpdatedBy = User.Identity.Name;
                        toupdate.UpdatedDate = DateTime.Today;
                        toupdate.RoomNo = production.RoomNo;
                        toupdate.CaretType = production.CaretType;
                        toupdate.GrossWeight = production.GrossWeight;
                        toupdate.ActualWeight = production.ActualWeight;
                        toupdate.NoOfCaret = production.NoOfCaret;
                        db.Entry(toupdate).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    lastdate = production.Date.Value.ToString("dd-MM-yyyy");
                }


               
                  var  results = new { Message = "success",lastdate  };  
                return Json(results, JsonRequestBehavior.AllowGet); 
            }
            catch(Exception e)
            {
                var result = new { Message = e.Message };
                return Json(result, JsonRequestBehavior.AllowGet);

            }

        }
    }
}