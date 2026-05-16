using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Clinic1.Models
{
    public class ADashboard
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int CancelledAppointments { get; set; }
    }
}