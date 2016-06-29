using EventSystem.Data.Common.Repository;
using EventSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using EventSystem.Web.Models.Common;
using EventSystem.Web.Models.Event;
using System.Data.Entity;
using EventSystem.Data.Common.Enums;
using Resources;


namespace EventSystem.Web.Controllers
{
    public class CommonController : BaseController
    {
        private IRepository<EventUser> eventsUsers;
        private IRepository<Lecture> lectures;

        public CommonController(IRepository<EventUser> eventsUsers, IRepository<Lecture> lectures)
        {
            this.eventsUsers = eventsUsers;
            this.lectures = lectures;
        }

        public ActionResult GetNotificationsData()
        {
            var currentUserId = User.Identity.GetUserId();

            var notificationsData = new List<NotificationsModel>();

            var currentUserIds = this.eventsUsers.All().Where(x => x.UserID == currentUserId).Select(x => x.EventID).ToList(); ;

            var upcomingEvents = this.eventsUsers.All().Where(x => x.UserID == currentUserId && DbFunctions.DiffHours(DateTime.Now, x.EventId.StartDate) < 24 && DbFunctions.DiffMinutes(DateTime.Now, x.EventId.StartDate) > 0).ToList();

            var upcomingLectures = this.lectures.All().Where(x => currentUserIds.Contains(x.EventId) && DbFunctions.DiffHours(DateTime.Now, x.Date) < 24 && DbFunctions.DiffMinutes(DateTime.Now, x.Date) > 0).Select(x => new CourseLectureViewModel
            {
                Id = x.Id,
                EventId = x.EventId,
                LectureDate = x.Date,
                LectureTitle = x.Title
            });

            foreach (var ev in upcomingEvents)
            {
                notificationsData.Add(new NotificationsModel
                {
                    Id = ev.EventID,
                    Title = ev.EventId.Title,
                    StartDate = ev.EventId.StartDate,
                    LectureId = 0,
                    HoursRemaining = (ev.EventId.StartDate - DateTime.Now).Hours,
                    MinutesRemaining = (ev.EventId.StartDate - DateTime.Now).Minutes,
                    TypeMessage = base.GetLocalizedEventTypeString(ev.EventId.Type)
                });
            }

            foreach (var lecture in upcomingLectures)
            {
                notificationsData.Add(new NotificationsModel
                {
                    Id = lecture.EventId,
                    Title = lecture.LectureTitle,
                    StartDate = lecture.LectureDate,
                    LectureId = lecture.Id,
                    HoursRemaining = (lecture.LectureDate - DateTime.Now).Hours,
                    MinutesRemaining = (lecture.LectureDate - DateTime.Now).Minutes,
                    TypeMessage = string.Format("{0} {1} {2}",EventsTypes.Lecture, Global.From, EventsTypes.Course)
                });
            }

            //var currentUserEvents = this.eventsUsers.All().Where(x => x.UserID == currentUserId).ToList()
            //    .Select(x => new NotificationsModel
            //    {
            //        Id = x.EventID,
            //        Title = x.EventId.Title,
            //        LectureId = x.EventId.Lectures.
            //    });

            return Json(notificationsData.OrderBy(x => x.StartDate).ThenBy(x => x.Title), JsonRequestBehavior.AllowGet);
        }
    }
}