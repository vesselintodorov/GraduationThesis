using EventSystem.Web.Attributes;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventSystem.Web.Models.Manage
{
    public class AccountDetailsViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredCountry")]
        [ResourcesDisplayName("Country", NameResourceType = typeof(Global))]
        public string Country { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredCity")]
        [ResourcesDisplayName("City", NameResourceType = typeof(Global))]
        public string City { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredPhoneNumber")]
        [ResourcesDisplayName("PhoneNumber", NameResourceType = typeof(Global))]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredAddress")]
        [ResourcesDisplayName("Address", NameResourceType = typeof(Global))]
        public string Address { get; set; }
    }
}