using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using QRCodeRedirects.Models;
using System.Configuration;
using System.Net.Http;
using QRCodeRedirects.BL;
using System.IO;
using System.Data.Entity.Core;
using CrestronClasses.Interfaces.ILogger;
using CrestronClasses.Classes.Logger;

namespace QRCodeRedirects.Controllers
{
    [CustomAuthorize(Roles = "AdminRewardBalances")]
    public class QRCodeRedirectsController : Controller
    {
        private WebAppsEntities db = new WebAppsEntities();
        ILogger logger = new EmailLogger(System.AppDomain.CurrentDomain.FriendlyName, ConfigurationManager.AppSettings["ErrorNotificationEmail"].ToString(), "Aplus Rewards Balance Admin - Error Occured");

        // GET: QRCodeRedirects
        public async Task<ActionResult> Index()
        {
            return View(await db.QRCodeRedirects.ToListAsync());
        }

        // GET: QRCodeRedirects/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            QRCodeRedirect qRCodeRedirect = await db.QRCodeRedirects.FindAsync(id);
            if (qRCodeRedirect == null)
            {
                return HttpNotFound();
            }
            return View(qRCodeRedirect);
        }

        // GET: QRCodeRedirects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: QRCodeRedirects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
 
        public async Task<ActionResult> Create([Bind(Include = "QRCodeId,WidenURL")] QRCodeRedirect qRCodeRedirect)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.QRCodeRedirects.Add(qRCodeRedirect);
                    await db.SaveChangesAsync();
                    String pageQueryURL = (ConfigurationManager.AppSettings["PageQueryURL"]);
                    await PostURI(pageQueryURL, qRCodeRedirect.QRCodeId, qRCodeRedirect.WidenURL);
                    return RedirectToAction("Index");
                }
            }
            catch (DataException except)
            {
                ErrorHandling.SendErrorNotification("Error Occurred During adding of an QR Code: " + except.ToString());
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                ViewBag.Message = except.InnerException.ToString();
            }

            return View(qRCodeRedirect);
        }

        // GET: QRCodeRedirects/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QRCodeRedirect qRCodeRedirect = await db.QRCodeRedirects.FindAsync(id);
            if (qRCodeRedirect == null)
            {
                return HttpNotFound();
            }
            return View(qRCodeRedirect);
        }

        // POST: QRCodeRedirects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
 
        public async Task<ActionResult> Edit([Bind(Include = "QRCodeId,WidenURL")] QRCodeRedirect qRCodeRedirect)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(qRCodeRedirect).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    String pageQueryURL = (ConfigurationManager.AppSettings["PageQueryURL"]);
                    await PostURI(pageQueryURL, qRCodeRedirect.QRCodeId, qRCodeRedirect.WidenURL);
                    return RedirectToAction("Index");
                }
            }
            catch (DataException except)
            {
                ErrorHandling.SendErrorNotification("Error Occurred During updating of an qr code: " + except.ToString());
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                ViewBag.Message = except.InnerException.ToString();
            }
            return View(qRCodeRedirect);
        }

        // GET: QRCodeRedirects/Delete/5
        public async Task<ActionResult> Delete(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QRCodeRedirect qRCodeRedirect = await db.QRCodeRedirects.FindAsync(id);
            if (qRCodeRedirect == null)
            {
                return HttpNotFound();
            }
            return View(qRCodeRedirect);
        }

        // POST: QRCodeRedirects/Delete/5
        [HttpPost, ActionName("Delete")]
 
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            QRCodeRedirect qRCodeRedirect = await db.QRCodeRedirects.FindAsync(id);
            db.QRCodeRedirects.Remove(qRCodeRedirect);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        static async Task<string> PostURI(String u, String QRCodeId, String WidenURL)
        {
            var response = string.Empty;
            //var list = new List<KeyValuePair<string, string>>();
            //list.Add(new KeyValuePair<string, string>("pname", "docs-" + QRCodeId));
            //list.Add(new KeyValuePair<string, string>("path", "/Shortcuts/docs/docs-" + QRCodeId));
            //list.Add(new KeyValuePair<string, string>("rurl", WidenURL));

            //var content = new FormUrlEncodedContent(list);
            //using (var client = new HttpClient())
            //{
            //    HttpResponseMessage result = await client.PostAsync(u, content);
            //    if (result.IsSuccessStatusCode)
            //    {
            //        response = result.StatusCode.ToString();
            //    }
            //}
            //return response;

            string url = "http://localhost:61536/api/page/updatepage"; // Just a sample url
            WebClient wc = new WebClient();

            wc.QueryString.Add("pname", "docs-" + QRCodeId);
            wc.QueryString.Add("path", "/Shortcuts/docs/docs-" + QRCodeId);
            wc.QueryString.Add("rurl", WidenURL);

            var data = wc.UploadValues(u, "POST", wc.QueryString);
            return response;
        }
    }
}
