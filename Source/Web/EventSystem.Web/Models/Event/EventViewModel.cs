using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventSystem.Data.Common.Enums;
using EventSystem.Web.Attributes;
using Resources;


namespace EventSystem.Web.Models.Event
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [ResourcesDisplayName("Title", NameResourceType = typeof(Global))]
        public string Title { get; set; }

        [ResourcesDisplayName("Type", NameResourceType = typeof(Global))]
        public EventType Type { get; set; }

        [ResourcesDisplayName("Description", NameResourceType = typeof(Global))]
        public string Description { get; set; }

        [ResourcesDisplayName("StartDate", NameResourceType = typeof(Global))]
        public DateTime StartDate { get; set; }

        [ResourcesDisplayName("EndDate", NameResourceType = typeof(Global))]
        public DateTime? EndDate { get; set; }

        [ResourcesDisplayName("Description", NameResourceType = typeof(Global))]
        public string ShortDescription { get; set; }

        public bool IsReadMore { get; set; }

        public bool IsManageLecturesAllowed { get; set; }

        public bool IsManageUsersAllowed { get; set; }

        public bool IsCreator { get; set; }

        public bool IsUserEnrolled { get; set; }

        public List<CourseLectureViewModel> Lectures { get; set; }

        public List<UserViewModel> Users { get; set; }

        public int ExternallySelectedLectureId { get; set; }

        public string LocalizedType { get; set; }
    }
}