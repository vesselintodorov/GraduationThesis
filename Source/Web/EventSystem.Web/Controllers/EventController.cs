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

namespace EventSystem.Web.Controllers
{
    public class EventController : Controller
    {
        private IRepository<Event> events;
        public EventController(IRepository<Event> events)
        {
            this.events = events;
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


                return this.RedirectToAction("Display", new { id = currentEvent.EventId });
            }
            return View(model);
        }

        public ActionResult Display(int id)
        {
            var currentEvent = this.events.All().FirstOrDefault(x => x.EventId == id);

            if (currentEvent == null)
            {
                return this.HttpNotFound(EventsResource.EventNotFound);
            }


            var model = new DisplayEventViewModel()
            {
                EventInfo = new EventViewModel()
                {
                    Id = id,
                    Title = currentEvent.Title,
                    Type = currentEvent.Type,
                    Description = currentEvent.Description,
                    StartDate = currentEvent.StartDate,
                    EndDate = currentEvent.EndDate
                }
            };

            return View(model);
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
    }
}