using Clinic1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clinic1.Controllers
{
    public class AdminController : Controller
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // GET: Admin/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Admin model)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM Admin WHERE Username = @Username AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Username", model.Username);
                cmd.Parameters.AddWithValue("@Password", model.Password);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    Session["AdminId"] = dr["AdminId"];
                    return RedirectToAction("Dashboard");
                }

                ViewBag.Error = "Invalid credentials.";
            }

            return View(model); // Return the same view with error message
        }

        public ActionResult Dashboard()
        {
            ADashboard model = new ADashboard();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                // Total Doctors
                SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM Doctors", conn);
                model.TotalDoctors = (int)cmd1.ExecuteScalar();

                // Total Patients
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM Patients", conn);
                model.TotalPatients = (int)cmd2.ExecuteScalar();

                // Total Appointments
                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Appointments", conn);
                model.TotalAppointments = (int)cmd3.ExecuteScalar();

                // Cancelled Appointments
                SqlCommand cmd4 = new SqlCommand("SELECT COUNT(*) FROM Appointments WHERE Status = 'Cancelled'", conn);
                model.CancelledAppointments = (int)cmd4.ExecuteScalar();
            }

            return View(model);
        }
        

        public ActionResult ManageDoctors()
        {
            List<Dregistration> doctors = new List<Dregistration>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM Doctors";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    doctors.Add(new Dregistration
                    {
                        DoctorID = Convert.ToInt32(dr["DoctorId"]),
                        Dname = dr["Dname"].ToString(),
                        Specialization = dr["Specialization"].ToString(),
                        Email = dr["Email"].ToString(),
                        MobileNo = dr["MobileNo "].ToString(),
                        Availability = dr["Availability"].ToString()
                    });
                }
            }
            return View(doctors);
        }

        public ActionResult Logout()
        {
            Session.Clear(); // or Session.Abandon();
            return RedirectToAction("Login", "Admin");
        }
    
        public ActionResult ManagePatients()
        {
            List<Pregistration> patients = new List<Pregistration>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT * FROM Patients";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    patients.Add(new Pregistration
                    {
                        PatientID = Convert.ToInt32(dr[" PatientID"]),
                        Fullname = dr["Fullname "].ToString(),
                        DOB = Convert.ToDateTime(dr["DOB"]),
                        Email = dr["Email"].ToString(),
                        MobileNo = dr["MobileNo"].ToString()
                    });
                }
            }
            return View(patients);
        }

        //public ActionResult ManageAppointments()
        //{
        //    List<Appointment> appointments = new List<Appointment>();
        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    {
        //        conn.Open();
        //        string query = @"SELECT a.AppointmentId, p.Name AS PatientName, d.Name AS DoctorName, a.Date, a.Time, a.Status
        //                     FROM Appointments a
        //                     JOIN Patients p ON a.PatientId = p.PatientId
        //                     JOIN Doctors d ON a.DoctorId = d.DoctorId";
        //        SqlCommand cmd = new SqlCommand(query, conn);
        //        SqlDataReader dr = cmd.ExecuteReader();
        //        while (dr.Read())
        //        {
        //            appointments.Add(new BookAppointment
        //            {
        //                DoctorID = Convert.ToInt32(dr["DoctorID"]),
        //                Date = Convert.ToDateTime(dr["Date"]),
        //                Time = TimeSpan.TryParse(dr["Time"]?.ToString(), out TimeSpan t) ? t : TimeSpan.Zero,

        //                Disease = dr["Disease"].ToString(),
        //                PatientID = Convert.ToInt32(dr["PatientID"]),


        //                Status = dr["Status"].ToString()
        //            });  
        //        }
        //    }
        //    return View(appointments);
        //}

        public ActionResult ManageAppointments()
        {
            List<BookAppointment> appointments = new List<BookAppointment>();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                string query = @"SELECT a.AppointmentId, a.PatientId, a.DoctorId, a.Disease, 
                                p.Name AS PatientName, d.Name AS DoctorName, 
                                a.Date, a.Time, a.Status
                         FROM Appointments a
                         JOIN Patients p ON a.PatientId = p.PatientId
                         JOIN Doctors d ON a.DoctorId = d.DoctorId";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    appointments.Add(new BookAppointment
                    {
                        DoctorID = Convert.ToInt32(dr["DoctorId"]),
                        PatientID = Convert.ToInt32(dr["PatientId"]),
                        Disease = dr["Disease"].ToString(),
                        Date = Convert.ToDateTime(dr["Date"]),

                        // 👇 Here's where you put the fixed code
                        Time = dr["Time"] != DBNull.Value
                           ? dr["Time"].ToString()
                                      : string.Empty,
                        Status = dr["Status"].ToString()
                    });
                }
            }

            return View(appointments);
        }

        


        public ActionResult CancelAppointment(int? id)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "UPDATE Appointments SET Status = 'Cancelled' WHERE AppointmentId = @id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            TempData["Message"] = "Appointment cancelled.";
            return RedirectToAction("ManageAppointments");
        }

        public ActionResult AppointmentReport()
        {
            List<Appointment> report = new List<Appointment>();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT DoctorId, COUNT(*) AS TotalAppointments FROM Appointments GROUP BY DoctorId";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    report.Add(new Appointment
                    {
                        DoctorID = Convert.ToInt32(dr["DoctorID"]),
                        TotalAppointments = Convert.ToInt32(dr["TotalAppointments"])
                    });
                }
            }
            return View(report);
        }
    }
}