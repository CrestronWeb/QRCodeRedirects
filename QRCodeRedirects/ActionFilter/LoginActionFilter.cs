using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using System.Net.Http;
using CrestronClasses.Classes.Global;
using System.Configuration;
using CrestronClasses.Interfaces.ILogger;
using CrestronClasses.Classes.Logger;
using CrestronClasses.Classes.QueryBuilder;
using Newtonsoft.Json;

namespace QRCodeRedirects.ActionFilter
{
    public class LoginActionFilter : ActionFilterAttribute
    {
        ILogger logger = new EmailLogger(System.AppDomain.CurrentDomain.FriendlyName, ConfigurationManager.AppSettings["ErrorNotificationEmail"].ToString(), "Aplus Rewards Balance Admin - Error Occured");
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["Debug"]))
            {
                User.Name = ConfigurationManager.AppSettings["ImpersonatedUser"];
                //logger.LogException(new Exception("In debug mode, impersonating - " + (string.IsNullOrEmpty(User.Name) ? "No user to impersonate" : User.Name)));
            }
            else
            {
                if (filterContext.HttpContext.Request.Cookies["uid"] == null) // UID cookie is valid for one hour.
                {
                    //logger.LogException(new Exception("UID expired or is null"));
                    //System.Web.HttpContext.Current.Response.Write("<script type=text/javascript> var url = '" + ConfigurationManager.AppSettings["Homepage"] + ConfigurationManager.AppSettings["ApplicationsPageURL"] + "'; window.parent.location.href = url;</script>");
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", Controller = "AccessDenied" }));
                    return;
                    //User lost his access while the app was open - return the user to home page or a generic error page
                }
                else
                {
                    var usercookie = filterContext.HttpContext.Request.Cookies["uid"].Value;
                    User.Name = usercookie;
                    if (!HasUserApplicationAccess())
                    {
                        //System.Web.HttpContext.Current.Response.Write("<script type=text/javascript> var url = '" + ConfigurationManager.AppSettings["Homepage"] + ConfigurationManager.AppSettings["ApplicationsPageURL"] + "'; window.parent.location.href = url;</script>");
                        filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new { action = "Index", Controller = "AccessDenied" }));
                        return;
                        //logger.LogException(new Exception("In live, UID is not null- " + (string.IsNullOrEmpty(User.Name) ? "No user to impersonate" : User.Name)));
                    }

                    //logger.LogException(new Exception("In live, UID is not null- " + (string.IsNullOrEmpty(User.Name) ? "No user to impersonate" : User.Name)));
                }
            }
        }

        public bool HasUserApplicationAccess()
        {
            try
            {
                bool hasUserAccess = false;
                var userQueryKey = (ConfigurationManager.AppSettings["UserQueryKey"]);
                var userQueryURL = (ConfigurationManager.AppSettings["UserHasAccessQueryURL"]);

                using (var client = new HttpClient())
                {
                    LoggedInUserQueryBuilder token = new LoggedInUserQueryBuilder(userQueryKey);
                    token.UserName = User.Name;
                    var response = client.PostAsync(userQueryURL, new StringContent(JsonConvert.SerializeObject(token).ToString(), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        hasUserAccess = Convert.ToBoolean(response.Content.ReadAsStringAsync().Result);
                    }


                    logger.LogException(new Exception("HasUserApplicationAccess- " + userQueryKey.ToString() + ' ' + token.UserName));
                }
                return hasUserAccess;
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
                return false;
            }

        }
    }
}