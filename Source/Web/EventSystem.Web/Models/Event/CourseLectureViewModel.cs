using EventSystem.Web.Attributes;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventSystem.Web.Models.Event
{
    public class CourseLectureViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredLectureTitle")]
        [ResourcesDisplayName("Title", NameResourceType = typeof(Global))]
        public string LectureTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredLectureDescription")]
        [ResourcesDisplayName("Description", NameResourceType = typeof(Global))]
        public string LectureDescription { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredLectureTeacher")]
        [ResourcesDisplayName("LectureTeacher", NameResourceType = typeof(EventsResource))]
        public string LectureTeacher { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredLectureDate")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy hh:ii}")]
        [DataType(DataType.Date)]
        [ResourcesDisplayName("Date", NameResourceType = typeof(Global))]
        public DateTime LectureDate { get; set; }

        public int EventId { get; set; }
    }
}