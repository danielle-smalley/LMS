using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LMS.UI.MVC.Models
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "Ope! Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ope! Email is required")]
        [DataType(DataType.EmailAddress)] //EmailAddress is an option that pops up in intellisense
        public string Email { get; set; }

        [Required(ErrorMessage = "Ope! Subject is required")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Ope! Message is required")]
        [UIHint("MultilineText")]
        public string Message { get; set; }
    }
}