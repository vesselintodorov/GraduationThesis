using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventSystem.Data.Common.Enums;
using EventSystem.Web.Attributes;
using Resources;
using System.Web.Mvc;
namespace EventSystem.Web.Models.Event
{
    public class BrowseFilterInputModel
    {
        [ResourcesDisplayName("EventName", NameResourceType = typeof(EventsResource))]
        public string SearchedEventName { get; set; }

        [ResourcesDisplayName("Type", NameResourceType = typeof(Global))]
        public EventType? Type { get; set; }

        [ResourcesDisplayName("StartDate", NameResourceType = typeof(Global))]
        public DateTime? StartDate { get; set; }

        [ResourcesDisplayName("EndDate", NameResourceType = typeof(Global))]
        public DateTime? EndDate { get; set; }

        public List<SelectListItem> TypesData { get; set; } 

    }
}