using EventSystem.Data.Common.Enums;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventSystem.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        protected string GetLocalizedEventTypeString(EventType item)
        {
            switch (item)
            {
                case EventType.Conference: return EventsTypes.Conference;

                case EventType.Seminar: return EventsTypes.Seminar;

                case EventType.Meeting: return EventsTypes.Meeting;

                case EventType.PressConference: return EventsTypes.PressConference;

                case EventType.Lecture: return EventsTypes.Lecture;

                case EventType.Course: return EventsTypes.Course;

                default: return string.Empty;
            }
        }
    }
}