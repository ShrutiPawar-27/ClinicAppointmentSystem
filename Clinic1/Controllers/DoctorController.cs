using Clinic1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.UI.WebControls;

namespace Clinic1.Controllers
{
    public class DoctorController : Controller
    {
        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Doctor
        public ActionResult Dregistration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Dregistration(Dregistration model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    // Check if email already exists
                    string checkQuery = "SELECT COUNT(*) FROM Doctors WHERE Email = @Email";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@Email", model.Email);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["Message"] = "You are already registered. Please login.";
                        return RedirectToAction("DLogin", "Doctor");
                    }
                    String Query = "insert into Doctors(Dname,Specialization,Email,MobileNo,Availability,Password)Values(@Dname,@Specialization,@Email,@MobileNo,@Availability,@Password)";

                    SqlCommand cmd = new SqlCommand(Query, con);

                    cmd.Parameters.AddWithValue("@Dname", model.Dname);
                    cmd.Parameters.AddWithValue("@Specialization", model.Specialization);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                    cmd.Parameters.AddWithValue("@Availability", model.Availability);
                    cmd.Parameters.AddWithValue("@Password", model.Password);


                    cmd.ExecuteNonQuery();
                    TempData["Message"] = "Registration successful. Please login.";
                    con.Close();

                    return RedirectToAction("DLogin");
                }

            }
            return View(model);
        }
        public ActionResult DLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DLogin(DLogin model)
        {
            if (!ModelState.IsValid)
                return View(model);
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string query = "SELECT DoctorID FROM Doctors WHERE Email = @Email AND Password = @Password";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", model.Email);
                cmd.Parameters.AddWithValue("@Password", model.Password);

                con.Open();
                object result = cmd.ExecuteScalar();

                if (result != null)
                {
                    Session["DoctorID"] = Convert.ToInt32(result);
                    return RedirectToAction("Dashboard");

                }
                else
                {
                    ViewBag.Error = "Invalid Email or Password";
                    return View(model);
                }
            }
        }



        

        public ActionResult Dashboard()
        {
            // 🔐 Check login
            if (Session["DoctorID"] == null)
            {
                return RedirectToAction("DLogin");
            }

            int doctorId = Convert.ToInt32(Session["DoctorID"]);

            List<BookAppointment> appointments = new List<BookAppointment>();
            Dregistration doctor = new Dregistration();

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // ================= Doctor Profile =================
                string doctorQuery = "SELECT * FROM Doctors WHERE DoctorID=@DoctorID";
                using (SqlCommand doctorCmd = new SqlCommand(doctorQuery, con))
                {
                    doctorCmd.Parameters.AddWithValue("@DoctorID", doctorId);

                    using (SqlDataReader dr = doctorCmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            doctor.DoctorID = (int)dr["DoctorID"];
                            doctor.Dname = dr["Dname"].ToString();
                            doctor.Specialization = dr["Specialization"].ToString();
                            doctor.Email = dr["Email"].ToString();
                            doctor.MobileNo = dr["MobileNo"].ToString();
                            doctor.Availability = dr["Availability"].ToString();
                        }
                    }
                }

                // ================= Appointments =================
                string apptQuery = "SELECT * FROM Appointments WHERE DoctorID=@DoctorID";
                using (SqlCommand apptCmd = new SqlCommand(apptQuery, con))
                {
                    apptCmd.Parameters.AddWithValue("@DoctorID", doctorId);

                    using (SqlDataReader ar = apptCmd.ExecuteReader())
                    {
                        while (ar.Read())
                        {
                            appointments.Add(new BookAppointment
                            {
                                AppointmentID = (int)ar["AppointmentID"],
                                DoctorID = (int)ar["DoctorID"],
                                PatientID = (int)ar["PatientID"],
                                Date = Convert.ToDateTime(ar["Date"]),
                                Time = ar["Time"].ToString(),
                                Disease = ar["Disease"].ToString(),
                                Status = ar["Status"].ToString()
                            });
                        }
                    }
                }
            }

            ViewBag.Doctor = doctor;
            return View(appointments);
        }
    }
}
