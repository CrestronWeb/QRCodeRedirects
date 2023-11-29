using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QRCodeRedirects.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult login(string returnUrl)
        {
            var authCookie = Request.Cookies[".WebApp.Cookies"];

            if (authCookie == null)
                return RedirectToAction("Index", "AccessDenied");
            else
                return Redirect(returnUrl);

        }
    }
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //filterContext.Result = new HttpUnauthorizedResult(); // Try this but i'm not sure
            filterContext.Result = new RedirectResult("~/AccessDenied");
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                this.HandleUnauthorizedRequest(filterContext);
            }
        }

    }
}