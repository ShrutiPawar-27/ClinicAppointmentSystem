using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class AppointmentHistoryModel
    {
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string Reason { get; internal set; }
    }
}