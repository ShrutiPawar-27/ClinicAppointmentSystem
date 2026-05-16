using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clinic1.Models
{
    public class BookAppointment
    {
        internal List<SelectListItem> doctorList;

        public int AppointmentID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        
        public List<SelectListItem> DoctorList { get; set; }
       

        [Required]
        public DateTime Date { get; set; }

        [Required]

        public string Time { get; set; }


        [Required]
        public string Disease { get; set; }

        
        public int PatientID { get; set; }

        [Required]
        public string Status { get; set; }

        public int TotalAppointments { get; set; }
    }
}