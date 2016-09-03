using EventSystem.Data.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Models
{
    [Table("MessageTemplates")]
    public class MessageTemplate
    {
        [Key]
        public int Id { get; set; }

        public MessageType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Parameters { get; set; }

    }
}
