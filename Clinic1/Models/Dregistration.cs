using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class Dregistration
    {
        public int DoctorID { get; set; }
        [Required]
        public string Dname { get; set; }

        [Required]
        public string Specialization { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string MobileNo { get; set; }

        [Required]
        public string Availability { get; set; }

        public string Password { get; set; }


    }
}