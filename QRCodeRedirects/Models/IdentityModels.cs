using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QRCodeRedirects.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AccessTraining { get; set; }
        public bool AccessTrainingDenied { get; set; }
        public long TraineeId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentTerms { get; set; }
        public string SalesOrg { get; set; }
        public string Title { get; set; }
        public string CompanyAddress { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public List<string> UserRoles { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}