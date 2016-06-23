using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Models
{
    public class Lecture
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Teacher { get; set; }

        public DateTime Date { get; set; }
    }
}
