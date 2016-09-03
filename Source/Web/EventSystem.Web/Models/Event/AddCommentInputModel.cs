using EventSystem.Web.Attributes;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventSystem.Web.Models.Event
{
    public class AddCommentInputModel
    {
        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredTitle")]
        [ResourcesDisplayName("Title", NameResourceType = typeof(Global))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredDescription")]
        [ResourcesDisplayName("Description", NameResourceType = typeof(Global))]
        public string Content { get; set; }

        public int EventId { get; set; }
    }
}