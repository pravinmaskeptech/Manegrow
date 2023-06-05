using Micraft.ManeGrowAgro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.AspNet.Identity;//For GetUserId()

namespace Micraft.ManeGrowAgro.Security
{
    public class CustomAuthorizeAttribute:AuthorizeAttribute
    {
     //   RoleMgtModel db = new RoleMgtModel(); // my entity  
        ManeGrowContext db = new ManeGrowContext();
        private readonly string[] allowedroles;
        private readonly string Message;
        public CustomAuthorizeAttribute(string message,string roles)
        {
            this.allowedroles = roles.Split(',');
            this.Message=message;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            foreach (var role in allowedroles)
            {
                var UserId=HttpContext.Current.User.Identity.GetUserId();
                var UserRoles = db.AspNetUserRoles.Where(x => x.UserId == UserId).Select(x => x.RoleId).ToList();
                var user = db.AspNetRole.Where(x => x.Name == role.Trim() && UserRoles.Contains(x.Id)).Count();
                if (user > 0)
                {
                    authorize = true; /* return true if Entity has current user(active) with specific role */
                }
            }
            if (authorize == false)
            {
                HttpContext.Current.Session["AuthorizeMsg"]=Message;
            }
            return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {            
            //filterContext.Result = new HttpUnauthorizedResult();
            filterContext.Result = new RedirectToRouteResult(new
                           RouteValueDictionary(new { controller = "Home", action = "Index", area = "" }));
        }
    }
}