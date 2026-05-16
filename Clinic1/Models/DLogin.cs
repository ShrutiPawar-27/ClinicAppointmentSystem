using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class DLogin
    {
        [Required]
        [RegularExpression("[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }


        [Required]

        public string Password { get; set; }
    }
}