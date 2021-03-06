﻿using EventSystem.Data.Common.Enums;
using EventSystem.Web.Attributes;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventSystem.Web.Models.Event
{
    public class AddEventInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredTitle")]
        [ResourcesDisplayName("Title", NameResourceType = typeof(Global))]
        public string Title { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredType")]
        [ResourcesDisplayName("Type", NameResourceType = typeof(Global))]
        public EventType Type { get; set; }

        [Required(ErrorMessageResourceType = typeof(EventsResource), ErrorMessageResourceName = "RequiredDescription")]
        [ResourcesDisplayName("Description", NameResourceType = typeof(Global))]
        public string Description { get; set; }


        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredStartDate")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy hh:ii}")]
        [DataType(DataType.Date)]
        [ResourcesDisplayName("StartDate", NameResourceType = typeof(Global))]
        public DateTime StartDate { get; set; }

        public List<SelectListItem> TypesData { get; set; } 

        public string DateRange { get; set; }


    }
}