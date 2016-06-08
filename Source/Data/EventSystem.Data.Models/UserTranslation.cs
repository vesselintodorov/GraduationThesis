using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Data.Models
{
    [Table("UsersTranslations")]
    public class UserTranslation
    {
        public int UserTranslationID { get; set; }

        public string UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string LanguageCode { get; set; }
    }
}
