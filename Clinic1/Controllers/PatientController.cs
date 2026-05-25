using Clinic1.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Services.Description;
using System.Xml.Linq;
using static System.Collections.Specialized.BitVector32;

namespace Clinic1.Controllers
{
    public class PatientController : Controller
    {

        string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // GET: Patient
        public ActionResult Pregistration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Pregistration(Pregistration model)
        {
            if (ModelState.IsValid)
            {


                using (SqlConnection con = new SqlConnection(connStr))
                {

                    String query = "INSERT INTO Patients(Fullname,DOB,Email,MobileNo,Gender,Password) Values (@Fullname,@DOB,@Email,@MobileNo,@Gender,@Password)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@Fullname", model.Fullname);
                    cmd.Parameters.AddWithValue("@DOB", model.DOB);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@MobileNo", model.MobileNo);
                    cmd.Parameters.AddWithValue("@Gender", model.Gender);
                    cmd.Parameters.AddWithValue("@Password", model.Password);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    return RedirectToAction("PLogin");

                }

            }
            return View();
        }

        public ActionResult PLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PLogin(PLogin model)

        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = "SELECT PatientID ,FullName FROM Patients WHERE Email = @Email AND Password = @Password";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Email", model.Email);
                    cmd.Parameters.AddWithValue("@Password", model.Password);

                    con.Open();

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Session["PatientID"] = reader["PatientID"].ToString();
                        Session["PatientName"] = reader["FullName"].ToString();

                        TempData["Success"] = "Login successful";
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        ViewBag.Error = "Enter Valid Email or Password";
                    }
                }

            }

            return View(model);
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPassword model)
        {
            using (SqlConnection con = new SqlConnection(connStr))
            {
                string checkQuery = "SELECT COUNT(*) FROM Patients WHERE Email = @Email";
                SqlCommand cmd = new SqlCommand(checkQuery, con);
                cmd.Parameters.AddWithValue("@Email", model.Email);

                con.Open();
                int count = (int)cmd.ExecuteScalar();

                if (count == 1)
                {
                    string tempPassword = "Temp@123"; // You can randomize this
                    string updateQuery = "UPDATE Patients SET Password = @Password WHERE Email = @Email";
                    SqlCommand updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@Password", tempPassword);
                    updateCmd.Parameters.AddWithValue("@Email", model.Email);
                    updateCmd.ExecuteNonQuery();

                    ViewBag.Message = $"Your temporary password is: {tempPassword}";
                }
                else
                {
                    ViewBag.Message = "Email not found.";
                }
            }

            return View(model);
        }



        public ActionResult BookAppointment()
        {
            BookAppointment model = new BookAppointment(); // Fetch Doctor List from database                                                
          using (SqlConnection conn = new SqlConnection(connStr)) 
            {
                conn.Open(); 
                string selectQuery = "SELECT DoctorID, DName FROM Doctors";
                using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
              {
                List<SelectListItem> doctorList = new List<SelectListItem>(); 
                    while (dr.Read())
                    {
                     doctorList.Add(new SelectListItem 
                    { 
                         Value = dr["DoctorID"].ToString(), 
                         Text = dr["DName"].ToString() }); 
                    } 
                    model.DoctorList = doctorList; 
                } 
            } return View(model);
        }
      
        

        [HttpPost]
        public ActionResult BookAppointment(BookAppointment model)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();

                if (ModelState.IsValid)
                {
                    // ✅ Make sure the user is logged in
                    if (Session["PatientID"] == null)
                    {
                        TempData["ErrorMessage"] = "Please log in to book an appointment.";
                        return RedirectToAction("PLogin");
                    }

                    int patientId = Convert.ToInt32(Session["PatientID"]); // ✅ Get from session

                    string insertQuery = @"INSERT INTO Appointments
                (DoctorID, Date, Time, Disease, PatientID)
                VALUES (@DoctorID, @Date, @Time, @Disease, @PatientID)";

                    using (SqlCommand Cmd = new SqlCommand(insertQuery, conn))
                    {
                        Cmd.Parameters.AddWithValue("@DoctorID", model.DoctorID);
                        Cmd.Parameters.AddWithValue("@Date", model.Date);
                        Cmd.Parameters.AddWithValue("@Time", model.Time);
                        Cmd.Parameters.AddWithValue("@Disease", model.Disease);
                        Cmd.Parameters.AddWithValue("@PatientID", patientId); // ✅ Use session value

                        Cmd.ExecuteNonQuery();
                    }

                    TempData["SuccessMessage"] = "Appointment booked successfully!";
                    return RedirectToAction("Dashboard");
                }

                // Reload doctor list if validation fails
                string selectQuery = "SELECT DoctorID, DName FROM Doctors";
                using (SqlCommand cmd = new SqlCommand(selectQuery, conn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<SelectListItem> doctorList = new List<SelectListItem>();
                    while (dr.Read())
                    {
                        doctorList.Add(new SelectListItem
                        {
                            Value = dr["DoctorID"].ToString(),
                            Text = dr["DName"].ToString()
                        });
                    }
                    model.DoctorList = doctorList;
                }
            }

            return View(model);
        }



        public ActionResult Dashboard()
        {

            // Check if patient is logged in
            if (Session["PatientID"] == null)
            {
                return RedirectToAction("PLogin");
            }

            var model = new Dashboard();
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string query = "SELECT FullName FROM Patients WHERE PatientID = @id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", Session["PatientID"]);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            model.PatientName = reader["FullName"].ToString();
                        }
                    }
                }
            }

            return View(model);
        }
        
        
       
        public ActionResult History()
        {
            if (Session["PatientID"] == null)
            {
                return RedirectToAction("PLogin", "Patient");
            }

            List<Appointment> appointments = new List<Appointment>();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = "SELECT Date, DoctorID, Status FROM Appointments WHERE PatientID = @id ORDER BY Date DESC";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", Session["PatientID"]);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    appointments.Add(new Appointment
                    {
                        Date = Convert.ToDateTime(reader["Date"]),
                        Dname = reader["Dname"].ToString(),
                        //DoctorName = reader["DoctorName"].ToString(),
                        Status = reader["Status"].ToString()
                    });
                }
            }

            return View(appointments);
        }


    }


}


    

    





