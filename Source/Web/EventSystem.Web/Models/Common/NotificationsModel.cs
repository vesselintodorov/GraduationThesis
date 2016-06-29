using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventSystem.Web.Models.Common
{
    public class NotificationsModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int LectureId { get; set; }

        public DateTime StartDate { get; set; }

        public int HoursRemaining { get; set; }

        public int MinutesRemaining { get; set; }

    }
}