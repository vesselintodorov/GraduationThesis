
namespace EventSystem.Web.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EventSystem.Web.Attributes;
    using Resources;
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [ResourcesDisplayName("Email", NameResourceType = typeof(Global))]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [ResourcesDisplayName("Email", NameResourceType = typeof(Global))]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredEmail")]
        [ResourcesDisplayName("Email", NameResourceType = typeof(Global))]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredPassword")]
        [DataType(DataType.Password)]
        [ResourcesDisplayName("Password", NameResourceType = typeof(Global))]
        public string Password { get; set; }

        [ResourcesDisplayName("RememberMe", NameResourceType = typeof(Global))]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredFirstName")]
        [ResourcesDisplayName("FirstName", NameResourceType = typeof(Global))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Global), ErrorMessageResourceName = "RequiredLastName")]
        [ResourcesDisplayName("LastName", NameResourceType = typeof(Global))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredEmail")]
        [ResourcesDisplayName("Email", NameResourceType = typeof(Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredPassword")]
        [StringLength(100, ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredPassword", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ResourcesDisplayName("Password", NameResourceType = typeof(Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [ResourcesDisplayName("ConfirmPassword", NameResourceType = typeof(Global))]
        [Compare("Password", ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredEmail")]
        [ResourcesDisplayName("Email", NameResourceType = typeof(Global))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredPassword")]
        [StringLength(100, ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredPassword", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ResourcesDisplayName("Password", NameResourceType = typeof(Global))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [ResourcesDisplayName("ConfirmPassword", NameResourceType = typeof(Global))]
        [Compare("Password", ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "PasswordMismatch")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Global), ErrorMessageResourceName = "RequiredEmail")]
        [ResourcesDisplayName("Email", NameResourceType = typeof(Global))]
        public string Email { get; set; }
    }
}
