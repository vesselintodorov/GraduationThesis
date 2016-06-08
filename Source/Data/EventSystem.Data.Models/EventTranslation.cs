using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Models
{
    [Table("EventsTranslation")]
    public class EventTranslation
    {
        public int EventTranslationID { get; set; }

        public int EventID { get; set; }

        [ForeignKey("EventID")]
        public virtual Event Recipe { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string LanguageCode { get; set; }
    }
}
