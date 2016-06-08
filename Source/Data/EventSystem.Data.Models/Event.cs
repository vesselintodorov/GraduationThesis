using EventSystem.Data.Common.Enums;
using EventSystem.Data.Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Models
{
    public class Event : AuditInfo, IDeletableEntity
    {
        private ICollection<EventUser> users;
        private ICollection<EventTranslation> translations;

        public Event()
        {
            this.Users = new HashSet<EventUser>();
            this.Translations = new HashSet<EventTranslation>();
        }

        [Key]
        public int EventId { get; set; }

        public EventType Type { get; set; }

        //public string Title { get; set; }

        //public string Content { get; set; }

        public string Author { get; set; }

        public bool IsFeatured { get; set; }

        public virtual ICollection<EventUser> Users
        {
            get { return this.users; }
            set { this.users = value; }
        }

        public virtual ICollection<EventTranslation> Translations
        {
            get { return this.translations; }
            set { this.translations = value; }
        }

        [Index]
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool IsApproved { get; set; }

        

    }
}
