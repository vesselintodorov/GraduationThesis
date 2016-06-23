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

        public EventController(IRepository<Event> events, IRepository<Lecture> coursesLectures, IRepository<EventUser> eventsUsers)
        {
            this.events = events;
            this.lectures = coursesLectures;
            this.eventsUsers = eventsUsers;
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(AddEventInputModel model)
        {
            if (ModelState.IsValid)
            {
                var currentEvent = new Event
                {
                    //Author = User.Identity.GetFirstName() + " " + User.Identity.GetLastName(),
                    Author = User.Identity.GetUserId(),
                    Title = model.Title,
                    Type = model.Type,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate
                };

                this.events.Add(currentEvent);
                this.events.SaveChanges();


                return this.RedirectToAction("Display", new { eventId = currentEvent.EventId });
            }
            return View(model);
        }

        public ActionResult Display(int eventId)
        {
            var currentEvent = this.events.All().FirstOrDefault(x => x.EventId == eventId);

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



        public ActionResult Browse()
        {
            var browseFilterModel = new BrowseFilterInputModel();

            return View(browseFilterModel);
        }

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
        public ActionResult AddLecture(int eventId)
        {
            ViewBag.EventId = eventId;
            return PartialView();
        }

        [HttpPost]
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
            var lectures = GetCourseUsers(eventId);

            return PartialView(lectures);
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
            }

            this.eventsUsers.SaveChanges();
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
                    alertType = "warning";
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

            this.eventsUsers.SaveChanges();
            return Json(new { alertType, alertMsg }, JsonRequestBehavior.AllowGet);
        }

        private List<CourseLectureViewModel> GetCourseLectures(Event currentEvent)
        {
            var courseLectures = new List<CourseLectureViewModel>();
            if (currentEvent.Type == EventType.Course)
            {
                courseLectures = this.lectures.All().Where(x => x.EventId == currentEvent.EventId)
                    .Select(x => new CourseLectureViewModel
                    {
                        EventId = x.EventId,
                        LectureTitle = x.Title,
                        LectureTeacher = x.Teacher,
                        LectureDescription = x.Description,
                        LectureDate = x.Date
                    }).ToList();
            }

            return courseLectures;
        }

        private List<CourseLectureViewModel> GetCourseLectures(int eventId)
        {
            return this.lectures.All().Where(x => x.EventId == eventId)
                .Select(x => new CourseLectureViewModel
                {
                    EventId = x.EventId,
                    LectureTitle = x.Title,
                    LectureTeacher = x.Teacher,
                    LectureDescription = x.Description,
                    LectureDate = x.Date
                }).ToList();

        }

        private List<UserViewModel> GetCourseUsers(int eventId)
        {
            return this.eventsUsers.All().Where(x => x.EventID == eventId)
                .Select(x => new UserViewModel
                {
                    FirstName = x.UserId.FirstName,
                    LastName = x.UserId.LastName
                }).ToList();
        }
    }
}