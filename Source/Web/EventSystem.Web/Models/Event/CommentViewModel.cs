using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventSystem.Web.Models.Event
{
    public class CommentViewModel
    {
        public int EventId { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime DateAdded { get; set; }

        public string CreatorName { get; set; }
    }
}