
using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Microsoft.Owin.Host.SystemWeb;
using System.Web.Mvc;
using Micraft.ManeGrowAgro.Security;

namespace Micraft.ManeGrowAgro.Security
{
    public class SessionTimeoutAttribute: ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    HttpContext ctx = HttpContext.Current;
        //    if (ctx.Session != null)
        //    {
        //        if(ctx.Session.IsNewSession)
        //        {
        //            string sessionCookie = ctx.Request.Headers["Cookie"];
        //            if ((null != sessionCookie) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0))
        //            {
        //                filterContext.Result = new RedirectToRouteResult(new
        //                           RouteValueDictionary(new { controller = "Account", action = "Login", area = "" }));
        //                return;
        //            }
        //        }
        //    }
         
        //    if (ctx.Response.Status.Substring(0, 3).Equals("4011")) 
        //    {
        //        ctx.Response.ClearContent();
        //        filterContext.Result = new RedirectToRouteResult(new
        //                    RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
        //        return;
        //    }
        //    var eName = "";
        //    var photo = "../photo/EmployeeProfilePic/NoImage.png";
        //    try
        //    {
        //        //BBraunHR db = new BBraunHR();
        //        ApplicationUser currentUser = HttpContext.Current.GetOwinContext().GetUserManager<Inventory.Models.ApplicationUserManager>().FindById(HttpContext.Current.User.Identity.GetUserId());
        //        //var empNm = db.Employee_mstr.FirstOrDefault(x => x.Emp_cd == currentUser.EMP_CODE);
        //        //eName = HttpContext.Current.User.Identity.Name;
        //        //photo = "../photo/EmployeeProfilePic/NoImage.png";
        //        //if (empNm != null)
        //        //{
        //        //    eName = empNm.Emp_nm;
        //        //    photo = empNm.PicturePath;
        //        //}
        //    }
        //    catch (Exception EX)
        //    {
        //        HttpContext.Current.Response.Redirect("~/Account/Login");
        //        return;
        //    }    
        //    base.OnActionExecuting(filterContext);
        //}
    }
}