﻿using EventSystem.Data.Common.Repository;
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
using System.Resources;
using PagedList;

namespace EventSystem.Web.Controllers
{
    public class EventController : BaseController
    {
        private IRepository<Event> events;
        private IRepository<Lecture> lectures;
        private IRepository<EventUser> eventsUsers;
        private IRepository<Comment> comments;

        public EventController(IRepository<Event> events, IRepository<Lecture> lectures, IRepository<EventUser> eventsUsers, IRepository<Comment> comments)
        {
            this.events = events;
            this.lectures = lectures;
            this.eventsUsers = eventsUsers;
            this.comments = comments;
        }

        public ActionResult Display(int eventId, int? lectureId)
        {
            var currentEvent = this.events.GetById(eventId);

            if (currentEvent == null)
            {
                return this.HttpNotFound(EventsResource.EventNotFound);
            }

            var currentUserId = User.Identity.GetUserId();
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
                IsCreator = currentEvent.Author == currentUserId,
                IsUserEnrolled = CheckIfCurrentUserIsEnrolledInEvent(eventId, currentUserId)
            };

            model.LocalizedType = base.GetLocalizedEventTypeString(model.Type);

            model.ExternallySelectedLectureId = lectureId.HasValue ? lectureId.Value : 0;

            return View(model);
        }


        [HttpGet]
        [Authorize]
        public ActionResult Add()
        {
            var model = new AddEventInputModel();
            model.TypesData = Enum.GetValues(typeof(EventType)).Cast<EventType>().Select(x => new SelectListItem
            {
                Text = base.GetLocalizedEventTypeString(x),
                Value = ((int)x).ToString()
            }).ToList();
            return View(model);
        }



        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
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

        //[Authorize]




        //[Authorize]
        public ActionResult Browse()
        {
            var browseFilterModel = new BrowseFilterInputModel();
            browseFilterModel.TypesData = Enum.GetValues(typeof(EventType)).Cast<EventType>().Select(x => new SelectListItem
            {
                Text = base.GetLocalizedEventTypeString(x),
                Value = ((int)x).ToString()
            }).ToList();

            return View(browseFilterModel);
        }

        //[Authorize]
        public ActionResult List(BrowseFilterInputModel browseFilterModel)
        {
            var events = this.events.All().Where(x => !x.IsFinished);

            if (!string.IsNullOrWhiteSpace(browseFilterModel.SearchedEventName))
            {
                events = events.Where(x => x.Title.ToLower().Contains(browseFilterModel.SearchedEventName.ToLower()));
            }

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
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                Description = x.Description
            }).ToList();

            foreach (var item in model)
            {
                item.ShortDescription = this.GetShortDescription(item.Description);
                item.LocalizedType = base.GetLocalizedEventTypeString(item.Type);
            }

            return PartialView(model);
        }

        private string GetShortDescription(string description)
        {
            return description.Length > 40 ? description.Substring(0, 40) + "..." : description;
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

        [Authorize]
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

        [Authorize]
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
            var courseLectures = new List<CourseLectureViewModel>();
            var currentUserId = User.Identity.GetUserId();
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

        private bool CheckIfCurrentUserIsEnrolledInEvent(int eventId, string userId)
        {
            var currentUserId = User.Identity.GetUserId();
            return this.eventsUsers.All().Any(x => x.EventID == eventId && x.UserID == currentUserId && x.Status == EventUserStatus.Enrolled);
        }


        private List<CourseLectureViewModel> GetCourseLectures(int eventId)
        {
            var currentEvent = this.events.GetById(eventId);
            var currentUserId = User.Identity.GetUserId();
            var allLectures = this.lectures.All().Where(x => x.EventId == eventId)
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

            foreach (var lecture in allLectures)
            {
                lecture.ShortDescription = this.GetShortDescription(lecture.LectureDescription);
            }

            return allLectures;
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

        public ActionResult AddEventDatePicker(int eventId, int eventTypeId)
        {
            //ViewBag.EventTypeId = eventTypeId;
            //ViewBag.DateRange = DateTime.Now.ToString("dd/MM/yyyy") + " - " + DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
            //ViewBag.StartDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
            Event currentEvent = new Event();

            if (eventId > 0)
            {
                currentEvent = this.events.GetById(eventId);
            }
            var model = new AddEventDatePickerViewModel();
            model.EventType = (EventType)eventTypeId;
            model.DateRange = eventId > 0 && (EventType)eventTypeId == EventType.Course
                ? GetDateRange(currentEvent.StartDate, currentEvent.EndDate.Value)
                : DateTime.Now.ToString("dd/MM/yyyy") + " - " + DateTime.Now.AddMonths(1).ToString("dd/MM/yyyy");
            model.StartDate = eventId > 0
                ? currentEvent.StartDate.ToString("dd/MM/yyyy HH:mm")
                : DateTime.Now.ToString("dd/MM/yyyy HH:mm");

            return PartialView(model);
        }


        [Authorize]
        public ActionResult UserEvents()
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUserEvents = this.eventsUsers.All().Where(x => x.UserID == currentUserId && x.Status == EventUserStatus.Enrolled || x.Status == EventUserStatus.Passed).ToList();

            var model = currentUserEvents.Select(x => new EventViewModel
            {
                Id = x.EventId.EventId,
                Title = x.EventId.Title,
                Type = x.EventId.Type,
                Description = x.EventId.Description,
                StartDate = x.EventId.StartDate,
                EndDate = x.EventId.EndDate
            }).ToList();

            foreach (var item in model)
            {
                item.ShortDescription = this.GetShortDescription(item.Description);
                item.LocalizedType = base.GetLocalizedEventTypeString(item.Type);
            }

            return View(model);
        }

        public ActionResult Comments(int eventId, int? page)
        {
            var comments = this.comments.All().Where(x => x.EventID == eventId).Select(x => new CommentViewModel
            {
                EventId = x.EventID,
                Title = x.Title,
                Content = x.Content,
                DateAdded = x.DateAdded,
                CreatorName = x.UserId.FirstName + " " + x.UserId.LastName
            }).OrderByDescending(x => x.DateAdded);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.EventId = eventId;

            return PartialView(comments.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        [Authorize]
        public ActionResult AddComment(int eventId)
        {
            var model = new AddCommentInputModel { EventId = eventId };

            return PartialView(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddComment(AddCommentInputModel model)
        {
            var currentUserId = User.Identity.GetUserId();
            string alertType = "danger";
            string alertMsg = Resources.Global.CommentAddFailed;
            if (ModelState.IsValid)
            {
                try
                {
                    this.comments.Add(new Comment
                    {
                        EventID = model.EventId,
                        Title = model.Title,
                        Content = model.Content,
                        DateAdded = DateTime.Now,
                        UserID = currentUserId
                    });

                    this.comments.SaveChanges();
                    alertType = "success";
                    alertMsg = Resources.Global.CommentAddSuccess;

                }
                catch (Exception ex)
                {
                    // Log the exception somewhere
                }
            }

            return Json(new { alertType, alertMsg }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int eventId)
        {
            var currentEvent = events.GetById(eventId);

            var model = new AddEventInputModel
            {
                Id = currentEvent.EventId,
                Title = currentEvent.Title,
                Type = currentEvent.Type,
                Description = currentEvent.Description,
                TypesData = Enum.GetValues(typeof(EventType)).Cast<EventType>().Select(x => new SelectListItem
                {
                    Text = base.GetLocalizedEventTypeString(x),
                    Value = ((int)x).ToString()
                }).ToList(),

                DateRange = currentEvent.Type == EventType.Course ? GetDateRange(currentEvent.StartDate, currentEvent.EndDate.Value) : string.Empty,
                StartDate = currentEvent.StartDate
            };

            return PartialView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditEvent(AddEventInputModel model)
        {
            string alertType = "danger";
            string alertMsg = Resources.EventsResource.EventEditFailure;
            if (ModelState.IsValid)
            {
                try
                {
                    var currentEvent = this.events.GetById(model.Id);
                    this.events.Update(currentEvent);
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
                    currentEvent.Description = model.Description;
                    currentEvent.Title = model.Title;
                    currentEvent.Type = model.Type;

                    this.events.SaveChanges();
                    alertType = "success";
                    alertMsg = Resources.EventsResource.EventEditSuccess;

                }
                catch (Exception ex)
                {
                    // Log the exception somewhere
                }
            }

            return Json(new { alertType, alertMsg }, JsonRequestBehavior.AllowGet);
        }


        private string GetDateRange(DateTime startDate, DateTime endDate)
        {
            return startDate.ToString("dd/MM/yyyy") + " - " + endDate.ToString("dd/MM/yyyy");
        }
    }
}