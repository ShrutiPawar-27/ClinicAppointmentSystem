using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class Pregistration
    {
        public int PatientID { get; set; }
        [Required(ErrorMessage = "Please Enter Full Name")]
        public string Fullname { get; set; }

        [Required(ErrorMessage = "Please Enter Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Please Enter Email")]
        [RegularExpression("[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,}$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter MobileNo")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit mobile number")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Please Enter Gender")]
        public string Gender { get; set; }



        [Required]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[!@#$%&*_])[A-Za-z\\d!@#$%&*_]{8,20}$")]
        public string Password { get; set; }

    }
}