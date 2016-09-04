using EventSystem.Web.Attributes;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventSystem.Web.Models.Manage
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredCurrentPassword")]
        [DataType(DataType.Password)]
        [ResourcesDisplayName("CurrentPassword", NameResourceType = typeof(Global))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredNewPassword")]
        [DataType(DataType.Password)]
        [ResourcesDisplayName("NewPassword", NameResourceType = typeof(Global))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [ResourcesDisplayName("ConfirmPassword", NameResourceType = typeof(Global))]
        [Compare("NewPassword", ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredConfirmPassword")]
        public string ConfirmPassword { get; set; }
    }
}