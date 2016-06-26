using EventSystem.Data.Common.Repository;
using EventSystem.Data.Models;
using EventSystem.Web.Models.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Resources;
using Microsoft.AspNet.Identity;
using EventSystem.Web.Models;
using System.Net;
using EventSystem.Data.Common.Enums;

namespace EventSystem.Web.Controllers
{
    public class EventController : Controller
    {
        private IRepository<Event> events;
        private IRepository<Lecture> lectures;
        private IRepository<EventUser> eventsUsers;

        public EventController(IRepository<Event> events, IRepository<Lecture> lectures, IRepository<EventUser> eventsUsers)
        {
            this.events = events;
            this.lectures = lectures;
            this.eventsUsers = eventsUsers;
        }

        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Add(AddEventInputModel model)
        {
            if (ModelState.IsValid)
            {
                var currentEvent = new Event
                {
                    Author = User.Identity.GetUserId(),
                    Title = model.Title,
                    Type = model.Type,
                    Description = model.Description
                };

                if (model.Type == EventType.Course)
                {
                    var dateRange = model.DateRange.Split(new char[] { '-', }, StringSplitOptions.RemoveEmptyEntries);
                    currentEvent.StartDate = Convert.ToDateTime(dateRange[0]);
                    currentEvent.EndDate = Convert.ToDateTime(dateRange[1]);
                }
                else
                {
                    currentEvent.StartDate = model.StartDate;
                }

                this.events.Add(currentEvent);
                this.events.SaveChanges();


                return this.RedirectToAction("Display", new { eventId = currentEvent.EventId });
            }
            return View(model);
        }

        [Authorize]
        public ActionResult Display(int eventId)
        {
            //var currentEvent = this.events.All().FirstOrDefault(x => x.EventId == eventId);
            var currentEvent = this.events.GetById(eventId);

            if (currentEvent == null)
            {
                return this.HttpNotFound(EventsResource.EventNotFound);
            }


            var model = new EventViewModel()
                {
                    Id = eventId,
                    Title = currentEvent.Title,
                    Type = currentEvent.Type,
                    Description = currentEvent.Description,
                    StartDate = currentEvent.StartDate,
                    EndDate = currentEvent.EndDate,
                    Lectures = GetCourseLectures(currentEvent),
                    Users = new List<UserViewModel>(),
                    IsManageLecturesAllowed = true,
                    IsManageUsersAllowed = true,
                    IsCreator = currentEvent.Author == User.Identity.GetUserId(),
                    IsUserEnrolled = CheckIfCurrentUserIsEnrolledInEvent(eventId, User.Identity.GetUserId())
                };

            return View(model);
        }

        private bool CheckIfCurrentUserIsEnrolledInEvent(int eventId, string userId)
        {
            var currentUserId = User.Identity.GetUserId();
            return this.eventsUsers.All().Any(x => x.EventID == eventId && x.UserID == currentUserId);
        }


        [Authorize]
        public ActionResult Browse()
        {
            var browseFilterModel = new BrowseFilterInputModel();

            return View(browseFilterModel);
        }

        [Authorize]
        public ActionResult List(BrowseFilterInputModel browseFilterModel)
        {
            var events = this.events.All().Where(x => !x.IsFinished);

            if (browseFilterModel.Type != null)
            {
                events = events.Where(x => x.Type == browseFilterModel.Type);
            }

            if (browseFilterModel.StartDate != null)
            {
                events = events.Where(x => x.StartDate >= browseFilterModel.StartDate);
            }

            if (browseFilterModel.EndDate != null)
            {
                events = events.Where(x => x.EndDate <= browseFilterModel.EndDate);
            }


            var model = events.Select(x => new EventViewModel
            {
                Id = x.EventId,
                Title = x.Title,
                Type = x.Type,
                ShortDescription = x.Description,
                StartDate = x.StartDate,
                EndDate = x.EndDate
            }).ToList();

            return PartialView(model);
        }

        [HttpGet]
        [Authorize]
        public ActionResult AddLecture(int eventId)
        {
            ViewBag.EventId = eventId;
            return PartialView();
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddLecture(CourseLectureViewModel model)
        {
            if (ModelState.IsValid)
            {


                var lecture = new Lecture
                {
                    EventId = model.EventId,
                    Title = model.LectureTitle,
                    Description = model.LectureDescription,
                    Teacher = model.LectureTeacher,
                    Date = model.LectureDate

                };

                this.lectures.Add(lecture);
                this.lectures.SaveChanges();
            }
            return this.RedirectToAction("AddLecture", new { eventId = model.EventId });
        }

        public ActionResult LecturesGrid(int eventId)
        {
            var lectures = GetCourseLectures(eventId);

            return PartialView(lectures);
        }

        public ActionResult UsersGrid(int eventId)
        {
            var users = GetCourseUsers(eventId);

            return PartialView(users);
        }

        public ActionResult SubscribeUser(int eventId)
        {
            string alertType = string.Empty;
            string alertMsg = string.Empty;
            var currentUserId = User.Identity.GetUserId();

            var currentEventUser = this.eventsUsers.All().FirstOrDefault(x => x.EventID == eventId && x.UserID == currentUserId);

            if (currentEventUser != null)
            {
                if (currentEventUser.Status == EventUserStatus.Unenrolled)
                {
                    this.eventsUsers.Update(currentEventUser);
                    currentEventUser.Status = EventUserStatus.Enrolled;
                    this.eventsUsers.SaveChanges();

                    alertType = "success";
                    alertMsg = EventsResource.EventEnrolled;
                }
                else if (currentEventUser.Status == EventUserStatus.Passed)
                {
                    alertType = "info";
                    alertMsg = EventsResource.EventPassed;
                }
                else
                {
                    alertType = "danger";
                    alertMsg = EventsResource.EventExpelled;
                }
            }
            else
            {
                this.eventsUsers.Add(new EventUser
                {
                    EventID = eventId,
                    Status = EventUserStatus.Enrolled,
                    UserID = User.Identity.GetUserId()
                });
                alertType = "success";
                alertMsg = EventsResource.EventEnrolled;
                this.eventsUsers.SaveChanges();
            }

            
            return Json(new { alertType, alertMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UnsubscribeUser(int eventId)
        {
            string alertType = string.Empty;
            string alertMsg = string.Empty;
            var currentUserId = User.Identity.GetUserId();

            var currentEventUser = this.eventsUsers.All().FirstOrDefault(x => x.EventID == eventId && x.UserID == currentUserId);

            if (currentEventUser != null)
            {
                if (currentEventUser.Status == EventUserStatus.Enrolled)
                {
                    this.eventsUsers.Update(currentEventUser);
                    currentEventUser.Status = EventUserStatus.Unenrolled;
                    this.eventsUsers.SaveChanges();

                    alertType = "info";
                    alertMsg = EventsResource.EventUnenrolled;
                }
                else
                {
                    alertType = "danger";
                    alertMsg = EventsResource.EventNotPart;
                }
            }
            else
            {
                alertType = "danger";
                alertMsg = EventsResource.EventNotPart;
            }

            return Json(new { alertType, alertMsg }, JsonRequestBehavior.AllowGet);
        }

        private List<CourseLectureViewModel> GetCourseLectures(Event currentEvent)
        {
            var currentUserId = User.Identity.GetUserId();

            var courseLectures = new List<CourseLectureViewModel>();
            if (currentEvent.Type == EventType.Course)
            {
                courseLectures = this.lectures.All().Where(x => x.EventId == currentEvent.EventId)
                    .Select(x => new CourseLectureViewModel
                    {
                        Id = x.Id,
                        EventId = x.EventId,
                        LectureTitle = x.Title,
                        LectureTeacher = x.Teacher,
                        LectureDescription = x.Description,
                        LectureDate = x.Date,
                        IsCreator = currentEvent.Author == currentUserId
                    }).ToList();
            }

            return courseLectures;
        }

        private List<CourseLectureViewModel> GetCourseLectures(int eventId)
        {
            var currentEvent = this.events.GetById(eventId);
            var currentUserId = User.Identity.GetUserId();

            return this.lectures.All().Where(x => x.EventId == eventId)
                .Select(x => new CourseLectureViewModel
                {
                    Id = x.Id,
                    EventId = x.EventId,
                    LectureTitle = x.Title,
                    LectureTeacher = x.Teacher,
                    LectureDescription = x.Description,
                    LectureDate = x.Date,
                    IsCreator = currentEvent.Author == currentUserId,
                }).ToList();

        }

        private List<UserViewModel> GetCourseUsers(int eventId)
        {
            var currentEvent = this.events.GetById(eventId);
            var currentUserId = User.Identity.GetUserId();

            return this.eventsUsers.All().Where(x => x.EventID == eventId && x.Status == EventUserStatus.Enrolled)
                .Select(x => new UserViewModel
                {
                    Id = x.Id,
                    FirstName = x.UserId.FirstName,
                    LastName = x.UserId.LastName,
                    IsCreator = currentEvent.Author == currentUserId,
                }).ToList();
        }

        public ActionResult DeleteLecture(int eventId, int lectureId)
        {
            string alertType = string.Empty;
            string alertMsg = string.Empty;
            var currentUserId = User.Identity.GetUserId();

            var currentEventAuthorUserId = this.events.GetById(eventId).Author;

            if (this.lectures.All().Any(x => x.Id == lectureId))
            {
                if (currentEventAuthorUserId == currentUserId)
                {
                    this.lectures.Delete(lectureId);
                    alertType = "success";
                    alertMsg = EventsResource.DeleteLectureSuccess;
                    this.lectures.SaveChanges();
                }
                else
                {
                    alertType = "danger";
                    alertMsg = EventsResource.DeleteEventContentNotAuthor;
                }
            }
            else
            {
                alertType = "danger";
                alertMsg = EventsResource.DeleteLectureFailure;
            }
            return Json(new { alertType, alertMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExpellUser(int eventId, int eventUserId)
        {

            //change logic to set status of user to expelled, not deleting the row
            string alertType = string.Empty;
            string alertMsg = string.Empty;
            var currentUserId = User.Identity.GetUserId();

            var currentEventAuthorUserId = this.events.GetById(eventId).Author;

            if (this.eventsUsers.All().Any(x => x.Id == eventUserId))
            {
                if (currentEventAuthorUserId == currentUserId)
                {
                    this.eventsUsers.GetById(eventUserId).Status = EventUserStatus.Expelled;
                    alertType = "success";
                    alertMsg = EventsResource.ExpellUserSuccess;
                    this.eventsUsers.SaveChanges();
                }
                else
                {
                    alertType = "danger";
                    alertMsg = EventsResource.DeleteEventContentNotAuthor;
                }
            }
            else
            {
                alertType = "danger";
                alertMsg = EventsResource.ExpellUserFailure;
            }
            return Json(new { alertType, alertMsg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DisplayLecture(int lectureId)
        {
            var lecture = lectures.GetById(lectureId);

            var model = new CourseLectureViewModel
            {
                Id = lecture.Id,
                LectureTitle = lecture.Title,
                LectureDate = lecture.Date,
                LectureDescription = lecture.Description,
                LectureTeacher = lecture.Teacher
            };

            return PartialView(model);
        }

        public ActionResult AddEventDatePicker(int eventTypeId)
        {
            ViewBag.EventTypeId = eventTypeId;
            return PartialView();
        }

    }
}