using Micraft.ManeGrowAgro.Models;
using Syncfusion.XlsIO;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Micraft.ManeGrowAgro.Controllers
{
    public class ManifetPrintExcelController : Controller
    {
        // GET: ManifetPrintExcel
        ManeGrowContext db = new ManeGrowContext();
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetManifestExcel(int DispatchID)
        {
            try
            {
                string fName = "";
                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;
                    IWorkbook workbook = application.Workbooks.Create(1);

                    var ManifestMain = db.ManifestMains.Where(a => a.DispatchID == DispatchID).ToList();
                    var sheetno = 0;
                    foreach (var item in ManifestMain)
                    {
                        IWorksheet worksheet = workbook.Worksheets[sheetno];

                        worksheet.Range["A1:M1"].Merge();
                        worksheet.Range["A1"].Text = "MANEGROW";
                        worksheet.Range["A1"].CellStyle.Font.Bold = true;
                        worksheet.Range["A1"].CellStyle.Font.Size = 20;
                        worksheet.Range["A1"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        worksheet.Range["A2:M2"].Merge();
                        worksheet.Range["A2"].Text = "AGRO PRODUCTS PVT LTD";
                        worksheet.Range["A2"].CellStyle.Font.Bold = true;
                        worksheet.Range["A2"].CellStyle.Font.Size = 16;
                        worksheet.Range["A2"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        worksheet.Range["A3:M3"].Merge();
                        worksheet.Range["A3"].Text = "Gate No 379, At Post Bebad Ohol, Dam Rd, Tal.Maval, Maharashtra 410506";
                        worksheet.Range["A3"].CellStyle.Font.Bold = true;
                        worksheet.Range["A3"].CellStyle.Font.Size = 12;
                        worksheet.Range["A3"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        worksheet.Range["A5:M5"].Merge();
                        worksheet.Range["A5"].Text = "DELIVERY CHALLAN";
                        worksheet.Range["A5"].CellStyle.Font.Bold = true;
                        worksheet.Range["A5"].CellStyle.Font.Size = 20;
                        worksheet.Range["A5"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        worksheet.Range["A6:C6"].Merge();
                        worksheet.Range["A6"].Text = "Delivery Challan No";
                        worksheet.Range["A6"].CellStyle.Font.Bold = true;
                        worksheet.Range["A6"].CellStyle.Font.Size = 13;

                        worksheet.Range["A7:C7"].Merge();
                        worksheet.Range["A7"].Text = "Route";
                        worksheet.Range["A7"].CellStyle.Font.Bold = true;
                        worksheet.Range["A7"].CellStyle.Font.Size = 13;

                        worksheet.Range["A8:C8"].Merge();
                        worksheet.Range["A8"].Text = "Group Name";
                        worksheet.Range["A8"].CellStyle.Font.Bold = true;
                        worksheet.Range["A8"].CellStyle.Font.Size = 13;

                        worksheet.Range["A9:C9"].Merge();
                        worksheet.Range["A9"].Text = "Out Time";
                        worksheet.Range["A9"].CellStyle.Font.Bold = true;
                        worksheet.Range["A9"].CellStyle.Font.Size = 13;

                        worksheet.Range["D6"].Text = item.DeliveryChallanNo;
                        worksheet.Range["D7"].Text = item.RouteName;
                        worksheet.Range["D8"].Text = item.GroupName;
                        worksheet.Range["D9"].Text = "";


                        
                        worksheet.Range["K6"].Text = "Date";
                        worksheet.Range["K6"].CellStyle.Font.Bold = true;
                        worksheet.Range["K6"].CellStyle.Font.Size = 13;

                        
                        worksheet.Range["K7"].Text = "Mode";
                        worksheet.Range["K7"].CellStyle.Font.Bold = true;
                        worksheet.Range["K7"].CellStyle.Font.Size = 13;

                        
                        worksheet.Range["K8"].Text = "Transport Name";
                        worksheet.Range["K8"].CellStyle.Font.Bold = true;
                        worksheet.Range["K8"].CellStyle.Font.Size = 13;

                        
                        worksheet.Range["K9"].Text = "Vehicle No";
                        worksheet.Range["K9"].CellStyle.Font.Bold = true;
                        worksheet.Range["K9"].CellStyle.Font.Size = 13;

                        
                        worksheet.Range["K10"].Text = "Vehicle Type";
                        worksheet.Range["K10"].CellStyle.Font.Bold = true;
                        worksheet.Range["K10"].CellStyle.Font.Size = 13;

                        
                        worksheet.Range["K11"].Text = "Driver Name";
                        worksheet.Range["K11"].CellStyle.Font.Bold = true;
                        worksheet.Range["K11"].CellStyle.Font.Size = 13;

                        
                        worksheet.Range["K12"].Text = "Mobile No";
                        worksheet.Range["K12"].CellStyle.Font.Bold = true;
                        worksheet.Range["K12"].CellStyle.Font.Size = 13;


                        worksheet.Range["L6"].Text = item.Date.ToString("dd/MM/yyyy");
                        worksheet.Range["L7"].Text = item.Mode;
                        worksheet.Range["L8"].Text = item.TransportName;
                        worksheet.Range["L9"].Text = item.VehicleNo;
                        worksheet.Range["L10"].Text = item.VehicleType;
                        worksheet.Range["L11"].Text = item.DriverName;
                        worksheet.Range["L12"].Text = item.DriverMobileNo;

                        var manifesrdtl = db.ManifestDetails.Where(a => a.DeliveryChallanNo == item.DeliveryChallanNo).ToList();

                        var data2 = (from x1 in db.ManifestDetails.Where(a => a.DeliveryChallanNo == item.DeliveryChallanNo)
                                     join c in db.Order
                                    on x1.OrderNo equals c.Id

                                     select new
                                     {
                                         OrderNo = x1.OrderNo,
                                         CustomerID = x1.CustomerID,
                                         CustomerCode = x1.CustomerCode,
                                         CustomerName = x1.CustomerName,
                                         Location = x1.Location,
                                         ProductName = x1.ProductName,
                                         OrderWeight = x1.OrderWeight,
                                         DispatchQty = x1.DispatchQty,
                                         BoxQty = x1.BoxQty,

                                         InvoiceNo = c.InvoiceNo,
                                         BoxInKG = x1.BoxInKG,


                                     }).OrderBy(xx => xx.CustomerName).ToList();


                        worksheet.Range["A13"].Text = "Sr No";
                        worksheet.Range["A13"].CellStyle.Font.Bold = true;
                        worksheet.Range["A13"].CellStyle.Font.Size = 13;

                        worksheet.Range["B13"].Text = "Order No";
                        worksheet.Range["B13"].CellStyle.Font.Bold = true;
                        worksheet.Range["B13"].CellStyle.Font.Size = 13;
                        
                        worksheet.Range["C13"].Text = "Customer Code";
                        worksheet.Range["C13"].CellStyle.Font.Bold = true;
                        worksheet.Range["C13"].CellStyle.Font.Size = 13;
                        
                        worksheet.Range["D13"].Text = "Party Name";
                        worksheet.Range["D13"].CellStyle.Font.Bold = true;
                        worksheet.Range["D13"].CellStyle.Font.Size = 13;
                        
                        worksheet.Range["E13"].Text = "Location";
                        worksheet.Range["E13"].CellStyle.Font.Bold = true;
                        worksheet.Range["E13"].CellStyle.Font.Size = 13;
                        
                        worksheet.Range["F13"].Text = "Item Description";
                        worksheet.Range["F13"].CellStyle.Font.Bold = true;
                        worksheet.Range["F13"].CellStyle.Font.Size = 13;

                        worksheet.Range["G13"].Text = "Order Weight";
                        worksheet.Range["G13"].CellStyle.Font.Bold = true;
                        worksheet.Range["G13"].CellStyle.Font.Size = 13;

                        worksheet.Range["H13"].Text = "Dispatch Qty";
                        worksheet.Range["H13"].CellStyle.Font.Bold = true;
                        worksheet.Range["H13"].CellStyle.Font.Size = 13;

                        worksheet.Range["I13"].Text = "Box Qty";
                        worksheet.Range["I13"].CellStyle.Font.Bold = true;
                        worksheet.Range["I13"].CellStyle.Font.Size = 13;

                        worksheet.Range["J13"].Text = "Box in KG";
                        worksheet.Range["J13"].CellStyle.Font.Bold = true;
                        worksheet.Range["J13"].CellStyle.Font.Size = 13;

                        worksheet.Range["K13"].Text = "Inovice Date";
                        worksheet.Range["K13"].CellStyle.Font.Bold = true;
                        worksheet.Range["K13"].CellStyle.Font.Size = 13;

                        worksheet.Range["L13"].Text = "Signature of Party";
                        worksheet.Range["L13"].CellStyle.Font.Bold = true;
                        worksheet.Range["L13"].CellStyle.Font.Size = 13;

                        var count = 14;
                        var srno = 1;
                        foreach (var x in data2)
                        {
                            worksheet.Range["A" + count].Text = srno.ToString();
                            worksheet.Range["B" + count].Text = x.OrderNo.ToString();
                            worksheet.Range["C" + count].Text = x.CustomerCode.ToString();
                            worksheet.Range["D" + count].Text = x.CustomerName.ToString();
                            worksheet.Range["E" + count].Text = x.Location.ToString();
                            worksheet.Range["F" + count].Text = x.ProductName.ToString();
                            worksheet.Range["G" + count].Text = x.OrderWeight.ToString();
                            worksheet.Range["H" + count].Text = x.DispatchQty.ToString();
                            var WtDtl = db.PackingDetails.Where(a => a.OrderNo == x.OrderNo).OrderBy(a => a.OrderNo).ToList();
                            var strbox = "";
                            foreach (var t in WtDtl)
                            {
                                strbox = strbox + "" + Convert.ToDecimal(t.BoxWeight) + ":" + decimal.ToInt32(t.NoOfBox) + "/";
                            }
                            string founderMinus1 = strbox.Remove(strbox.Length - 1, 1);
                            worksheet.Range["I" + count].Text = x.BoxQty.ToString();
                            worksheet.Range["J" + count].Text = founderMinus1;
                            worksheet.Range["K" + count].Text = x.InvoiceNo.ToString();
                            worksheet.Range["L" + count].Text = "";
                            count = count + 1;
                            srno = srno + 1;
                        }

                        worksheet.Range["A" + count + ":F" + count].Merge();
                        worksheet.Range["A" + count].Text = "Total";
                        worksheet.Range["A"+count].CellStyle.Font.Bold = true;
                        worksheet.Range["A" + count].CellStyle.Font.Size = 16;
                        worksheet.Range["A" + count].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        var totWeightValue = data2.Sum(t => t.OrderWeight ?? 0);
                        worksheet.Range["G" + count].Text = totWeightValue.ToString();
                        worksheet.Range["G" + count].CellStyle.Font.Bold = true;
                        worksheet.Range["G" + count].CellStyle.Font.Size = 13;
                        worksheet.Range["G" + count].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        var totDispatchQty = data2.Sum(t => t.OrderWeight ?? 0);
                        worksheet.Range["H" + count].Text = totDispatchQty.ToString();
                        worksheet.Range["H" + count].CellStyle.Font.Bold = true;
                        worksheet.Range["H" + count].CellStyle.Font.Size = 13;
                        worksheet.Range["H" + count].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        var totBoxQty = data2.Sum(t => t.BoxQty ?? 0);
                        worksheet.Range["I" + count].Text = totBoxQty.ToString();
                        worksheet.Range["I" + count].CellStyle.Font.Bold = true;
                        worksheet.Range["I" + count].CellStyle.Font.Size = 13;
                        worksheet.Range["I" + count].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;

                        sheetno = sheetno + 1;
                    }
                    fName = "ManifestDetails_"+DispatchID+".xlsx";

                    workbook.SaveAs(Server.MapPath(@"\Documents\ExcelFiles\" + fName  ));
                    Session["MainfestExcelFile"] = fName.ToString();

                }


                var result = new { Message = "success", FileName = fName };
                return Json(result, JsonRequestBehavior.AllowGet);


            }
            catch (Exception EX)
            {
                //document.Close();
                var result = new { Message = EX.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult GetReportLabel()
        {
            string FileName = Session["MainfestExcelFile"].ToString();
            string path = Server.MapPath("~/Documents/ExcelFiles/");
            path = path + FileName;
            string ReportURL = path;
            byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
            return File(FileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }
    }
}