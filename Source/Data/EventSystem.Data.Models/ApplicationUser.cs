namespace EventSystem.Data.Models
{
    using EventSystem.Data.Common.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Security.Claims;
    using System.Threading.Tasks;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        private ICollection<EventUser> users;
        public ApplicationUser()
        {
            this.Users = new HashSet<EventUser>();
            this.CreatedOn = DateTime.Now;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("FirstName", this.FirstName));
            userIdentity.AddClaim(new Claim("LastName", this.LastName));


            return userIdentity;
        }

        public virtual ICollection<EventUser> Users
        {
            get { return this.users; }
            set { this.users = value; }
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool PreserveCreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [Index]
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
