using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class PLogin
    {
        [Required]
        [RegularExpression("[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required]
       // [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%&*_])[A-Za-z\\d!@#$%&*_]{8,20}$")]
        public string Password { get; set; }
    }
}