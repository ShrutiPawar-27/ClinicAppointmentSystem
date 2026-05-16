using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class Appointment
    {
        [Required]
        public int AppointmentId { get; set; }
        [Required]
        public int PatientId { get; set; }


        [Display(Name = "Appointment Date")]
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime Date { get; set; }

        [Display(Name = "Doctor Name")]
        public string Dname { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        public int DoctorID{get;set ;}

        public int TotalAppointments {  get; set; }
    }
}