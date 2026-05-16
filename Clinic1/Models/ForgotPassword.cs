using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class ForgotPassword
    {
        [Required]

        public string Email { get; set; }
    }
}