using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EventSystem.Data.Common.Enums;

namespace EventSystem.Web.Models.Event
{
    public class AddEventDatePickerViewModel
    {
        public EventType EventType { get; set; }
        public string DateRange { get; set; }
        public string StartDate { get; set; }
    }
}