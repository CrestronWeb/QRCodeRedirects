using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using QRCodeRedirects.Models;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Web.Security;
using System.Security.Claims;
using System.Net;

namespace QRCodeRedirects
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            MvcHandler.DisableMvcResponseHeader = true;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            if (Context.User != null)
            {
                var authCookie = Request.Cookies[".WebApp.Cookies"];
                if (authCookie != null)
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    ApplicationUser user = JsonConvert.DeserializeObject<ApplicationUser>(authTicket.UserData);
                    string[] rolesArray = user.UserRoles.ToArray();

                    FormsIdentity formsIdentity = new FormsIdentity(authTicket);
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(formsIdentity);
                    claimsIdentity.AddClaim(new Claim("CustomerId", user.CustomerId));
                    claimsIdentity.AddClaim(new Claim("UserName", user.UserName));
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    Context.User = claimsPrincipal;

                    GenericPrincipal gp = new GenericPrincipal(Context.User.Identity, rolesArray);
                    Context.User = gp;
                }
            }
        }
    }
}
