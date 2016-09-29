using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public virtual Event EventId { get; set; }

        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser UserId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateAdded { get; set; }

    }
}
