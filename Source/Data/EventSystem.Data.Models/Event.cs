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
        private ICollection<Lecture> lectures;

        public Event()
        {
            this.Users = new HashSet<EventUser>();
            this.Lectures = new HashSet<Lecture>();
        }

        [Key]
        public int EventId { get; set; }

        public EventType Type { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsFeatured { get; set; }

        public virtual ICollection<EventUser> Users
        {
            get { return this.users; }
            set { this.users = value; }
        }

        public virtual ICollection<Lecture> Lectures
        {
            get { return this.lectures; }
            set { this.lectures = value; }
        }

        [Index]
        public bool IsFinished { get; set; }

        [Index]
        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        

    }
}
