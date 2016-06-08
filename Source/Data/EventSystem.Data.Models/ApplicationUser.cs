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
        private ICollection<UserTranslation> translations;
        public ApplicationUser()
        {
            this.Users = new HashSet<EventUser>();
            this.Translations = new HashSet<UserTranslation>();
            this.CreatedOn = DateTime.Now;
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public virtual ICollection<EventUser> Users
        {
            get { return this.users; }
            set { this.users = value; }
        }

        public virtual ICollection<UserTranslation> Translations
        {
            get { return this.translations; }
            set { this.translations = value; }
        }


        public DateTime CreatedOn { get; set; }

        public bool PreserveCreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        [Index]
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
