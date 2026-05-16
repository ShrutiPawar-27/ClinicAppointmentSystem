using Clinic1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Clinic1.Controllers
{
    public class AppointmentController : Controller
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Appointment
        

        public ActionResult Index()
        {
            int patientId = Convert.ToInt32(Session["PatientId"]); // get from login/session

            List<AppointmentHistoryModel> appointments = new List<AppointmentHistoryModel>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = @"SELECT a.AppointmentId, a.Date, a.Status, 
                               p.Fullname AS PatientName,
                               d.Dname AS DoctorName
                         FROM Appointments a
                         INNER JOIN Patients p ON a.PatientId = p.PatientId
                         INNER JOIN Doctors d ON a.DoctorId = d.DoctorId
                         WHERE a.PatientId = @PatientId
                         ORDER BY a.Date DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    appointments.Add(new AppointmentHistoryModel
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        Date = Convert.ToDateTime(dr["Date"]),
                        Status = dr["Status"].ToString(),
                        PatientName = dr["PatientName"].ToString(),
                        DoctorName = dr["DoctorName"].ToString()
                    });
                }
            }
            return View(appointments);
        }
        public ActionResult MyAppointments()
        {
            List<Appointment> list = new List<Appointment>();

            int patientId = Convert.ToInt32(Session["PatientId"]); // logged-in patient

            using (SqlConnection con = new SqlConnection(connStr))
            {
                 patientId = Convert.ToInt32(Session["PatientId"]);

                string query = @"SELECT A.AppointmentId,
                        A.Date,
                        A.Status,
                        D.Dname
                 FROM Appointments A
                 INNER JOIN Doctors D ON A.DoctorID = D.DoctorID
                 WHERE A.PatientId = @pid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pid", patientId);
                //string query = "SELECT * FROM Appointments WHERE PatientId=@pid";
                //SqlCommand cmd = new SqlCommand(query, con);
                //cmd.Parameters.AddWithValue("@pid", patientId);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    list.Add(new Appointment
                    {
                        AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                        //DoctorID = Convert.ToInt32(dr["DoctorId"]),
                        Dname = dr["Dname"].ToString(),
                        Date = Convert.ToDateTime(dr["Date"]),
                        Status = dr["Status"].ToString()
                    });
                }
            }
            return View(list);
        }
        public ActionResult CancelAppointment(int id)
        {
            int patientId = Convert.ToInt32(Session["PatientId"]);

            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = @"UPDATE Appointments 
                         SET Status='Cancelled' 
                         WHERE AppointmentId=@id AND PatientId=@pid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@pid", patientId);

                con.Open();
                int rows = cmd.ExecuteNonQuery();

                if (rows == 0)
                {
                    return Content("Invalid request"); // security check
                }
            }

            return RedirectToAction("MyAppointments");
        }

    }
}
        
    
