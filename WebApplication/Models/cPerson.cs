using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Collections;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Security.Cryptography;

namespace WebApplication.Models
{
    public class cPerson
    {
        [Display(Name = "Registration ID")]
     
        public int Registration_Id { get; set; }
        
       
        [Display(Name = "Email id")]
      
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }
        [Display(Name = "Mobile no")]
        [Required]
        [RegularExpression("^(?!0+$)(\\+\\d{1,3}[- ]?)?(?!0+$)\\d{10,15}$", ErrorMessage = "Must be of formate 923ddddddddd")]
        public string mobilde { get; set; }
        [Display(Name = "Name ")]
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Organization")]

        public string orgname { get; set; }
   
      
        [Required]
       
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[$@$!%*?&])[A-Za-z\\d$@$!%*?&]{8,}", ErrorMessage = "Password must contain: Minimum 8 characters atleast 1 UpperCase Alphabet, 1 LowerCase Alphabet, 1 Number and 1 Special Character")]
        public string password { get; set; }
        [Required]
        [Compare("password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string Confirmpassword { get; set; }
        public DateTime RegisterationDate { get; set; }

        [Display(Name = "Role")]
        [Required]
        public string role { get; set; }
        [Display(Name = "Address")]
        [Required]
        public string station { get; set; }
        [Display(Name = "Worked At")]
        [Required]
        public string workat { get; set; }
        
       
        public string  NewPassword { get; set; }
       
      
        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmNewPassword { get; set; }

        public string RegistrationStatus { get; set; }

        private bool connection_open;

        private MySqlConnection connection;
        public cPerson()
        {

        }
        public string converttoencryptedPassword(string passwordValue)
        {

            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(passwordValue);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedString;

        }
   
        public bool IsPhoneExist(string phoneno) {

            var User = new cPerson();
            Get_Connection();
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("select * from  RegistrationTable where Mobileno = '{0}'",
                    phoneno);

                MySqlDataReader reader = cmd.ExecuteReader();

                try
                {
                    //var reader1 = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {




                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return true;
                    }
                    else
                    {

                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return false;

                    }



                }
                catch (MySqlException e)
                {
                    string MessageString = "Read error occurred  / entry not found loading the Column details: "
                        + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                    //MessageBox.Show(MessageString, "SQL Read Error");

                    reader.Close();
                    connection.Close();
                    return false;


                }
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                connection.Close();

                return false;

            }








        }
        public List<ThreatReportViewModel> GetAllReportHistory(int regId) {
             List<ThreatReportViewModel> model = new List<ThreatReportViewModel>();
            try {
                Get_Connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("SELECT  * FROM threatreporttable,threatnotificationtable WHERE ThreatReportNotificationId = ThreatReportNotification_Id AND RegistrationId = '{0}'",
                        regId);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var threatModel = new Threat();
                   
                  
                    threatModel.ThreatReport_Id = (long)Int32.Parse(reader["ThreatReport_Id"].ToString());
                    threatModel.Level_Of_Threat = reader["Level_of_Threat"].ToString();
                    threatModel.ReportDate=reader["ReportDate"].ToString();


                   var modelobj = new ThreatReportViewModel();
                    modelobj.Threat = threatModel;
                    model.Add(modelobj);
                }

                connection.Close();
                return model;
            }
            catch (MySqlException e)
            {
                string MessageString = "Read error occurred  / entry not found loading the Column details: "
                    + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                //MessageBox.Show(MessageString, "SQL Read Error");
                connection.Close();
                return model;

            }

        }
        public bool reportathreat(ThreatReportViewModel modelobj) {
            DateTime date= DateTime.Now;
            Get_Connection();
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO threatnotificationtable(RegistrationId, Level_of_Threat, Threat_status, ReportDate, ProcessThreat_status) VALUES (@g1d, @g2d,@g3d,@g4d,@g5d)";
                cmd.Parameters.AddWithValue("@g1d", modelobj.Users.Registration_Id);
                cmd.Parameters.AddWithValue("@g2d", modelobj.ThreatNotification.Level_Of_Threat);
                if (modelobj.ThreatNotification.Status.Equals("True"))
                {
                    cmd.Parameters.AddWithValue("@g3d", "Yes");
                }
                else {
                    cmd.Parameters.AddWithValue("@g3d", "No");
                }
               
                cmd.Parameters.AddWithValue("@g4d", date);
                cmd.Parameters.AddWithValue("@g5d", "Pending");
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
                return true;

            }
            catch (MySqlException e)
            {
                string MessageString = "Read error occurred  / entry not found loading the Column details: "
                    + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                //MessageBox.Show(MessageString, "SQL Read Error");
                connection.Close();
                return false;

            }








        }
        public bool PanicAlertRequest(cPerson Userobj) {
            DateTime now = DateTime.Now;
            Get_Connection();
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO panicattack(RegistrationId, panic_date, seen_status) VALUES (@g1d, @g2d, @g3d)";
                cmd.Parameters.AddWithValue("@g1d", Userobj.Registration_Id);
                cmd.Parameters.AddWithValue("@g2d", now);
                cmd.Parameters.AddWithValue("@g3d", "NotSeen");
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
                return true;

            }
            catch (MySqlException e)
            {
                string MessageString = "Read error occurred  / entry not found loading the Column details: "
                    + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                //MessageBox.Show(MessageString, "SQL Read Error");
                connection.Close();
                return false;

            }



        }
        public bool UpdateProfileRequest(cPerson Userobj) {
            DateTime now = DateTime.Now;
            Get_Connection();
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO profilesettings(RegistrationId,Status, ProfileSettings_date) VALUES (@g1d, @g2d, @g3d)";
                cmd.Parameters.AddWithValue("@g1d", Userobj.Registration_Id);
                cmd.Parameters.AddWithValue("@g2d", "pending");
                cmd.Parameters.AddWithValue("@g3d", now);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
                return true;

            }
            catch (MySqlException e)
            {
                string MessageString = "Read error occurred  / entry not found loading the Column details: "
                    + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                //MessageBox.Show(MessageString, "SQL Read Error");
                connection.Close();
                return false;

            }





        }

        public bool IsEmailExist(string useremail) {

            var User = new cPerson();
            Get_Connection();
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("select * from  RegistrationTable where Email = '{0}'",
                    useremail);

                MySqlDataReader reader = cmd.ExecuteReader();

                try
                {
                    //var reader1 = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                       



                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return true;
                    }
                    else
                    {

                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return false;

                    }

                    

                }
                catch (MySqlException e)
                {
                    string MessageString = "Read error occurred  / entry not found loading the Column details: "
                        + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                    //MessageBox.Show(MessageString, "SQL Read Error");

                    reader.Close();
                    connection.Close();
                    return false;


                }
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                connection.Close();

                return false;

            }

           

        }

        public bool RegistrationRequest(cPerson User, string organizationname) {

            DateTime now = DateTime.Now;

            
            Get_Connection();
            try {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO registrationtable(Email,Name,WorkedAt,OrganizationName,Mobileno,StationedAt,Role,Password,RegistrationStatus,reg_date) VALUES (@g1d, @g2d, @g3d, @colorChart, @hex1, @hex2, @hex3,@hex5 ,@hex4,@hex6)";
                cmd.Parameters.AddWithValue("@g1d", User.email.ToString());
                cmd.Parameters.AddWithValue("@g2d", User.UserName.ToString());
                cmd.Parameters.AddWithValue("@g3d", User.workat.ToString());
                if (string.IsNullOrEmpty(User.orgname))
                {
                    cmd.Parameters.AddWithValue("@colorChart", "Freelancer");
                }
               else {
                   cmd.Parameters.AddWithValue("@colorChart", User.orgname);

                }
                cmd.Parameters.AddWithValue("@hex1", User.mobilde.ToString());
                cmd.Parameters.AddWithValue("@hex2", User.station.ToString());
                cmd.Parameters.AddWithValue("@hex3", User.role.ToString());
                byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(User.password.ToString());
                byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
                string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                cmd.Parameters.AddWithValue("@hex5", hashedString);
                cmd.Parameters.AddWithValue("@hex4","Pending");
                cmd.Parameters.AddWithValue("@hex6", now);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
                return true;
            }

            catch (MySqlException e) {
                string MessageString = "Read error occurred  / entry not found loading the Column details: "
                 + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                connection.Close();
                return false;


            }




        }
        public long GetUserAllReportCount(int regID) {
           
            Get_Connection();
            long countrows = 0;
            try
            {
                
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("SELECT  COUNT(ThreatReport_Id) FROM threatreporttable,threatnotificationtable WHERE ThreatReportNotificationId = ThreatReportNotification_Id AND RegistrationId = '{0}'",
                        regID);



                try
                {
                    countrows= Convert.ToInt32(cmd.ExecuteScalar());
                    connection.Close();
                    return countrows;

                }
                catch (MySqlException e)
                {
                    string MessageString = "Read error occurred  / entry not found loading the Column details: "
                        + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                    //MessageBox.Show(MessageString, "SQL Read Error");

                 
                    connection.Close();
                    return countrows;


                }
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                connection.Close();
                return countrows;

            }




          



        }

        public cPerson IsValidUser(string useremail, string password)
        {
            var User = new cPerson();

            Get_Connection();
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                string pass = converttoencryptedPassword(password);
                cmd.CommandText = string.Format("select * from  RegistrationTable where Email = '{0}'and Password = '{1}'", useremail,pass);

                MySqlDataReader reader = cmd.ExecuteReader();

                try
                {
                    //var reader1 = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string tmp = reader["Registration_Id"].ToString();
                            User.Registration_Id = Int32.Parse(tmp);
                            User.email = reader["Email"].ToString();
                            User.UserName= reader["Name"].ToString();
                            User.workat = reader["WorkedAt"].ToString();
                            User.orgname = reader["OrganizationName"].ToString();
                            User.mobilde = reader["Mobileno"].ToString();
                            User.role = reader["Role"].ToString();
                            User.station = reader["StationedAt"].ToString();
                            User.RegisterationDate = Convert.ToDateTime(reader["reg_date"]);
                            User.RegistrationStatus = reader["RegistrationStatus"].ToString();
                        }



                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return User;
                    }
                    else
                    {
                       
                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return null;

                    }

                   

                }
                catch (MySqlException e)
                {
                    string MessageString = "Read error occurred  / entry not found loading the Column details: "
                        + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                    //MessageBox.Show(MessageString, "SQL Read Error");

                    reader.Close();
                    connection.Close();
                    return null;
                
                   
                }
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                connection.Close();
                return null;
               
            }




            


        }

        private void Get_Connection()
        {
            connection_open = false;


            connection = new MySqlConnection();
            //connection = DB_Connect.Make_Connnection(ConfigurationManager.ConnectionStrings["SQLConnection"].ConnectionString);
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

            //            if (db_manage_connnection.DB_Connect.OpenTheConnection(connection))
            if (Open_Local_Connection())
            {
                connection_open = true;
            }
            else
            {
                //	MessageBox::Show("No database connection connection made...\n Exiting now", "Database Connection Error");
                //	Application::Exit();
            }

        }

        private bool Open_Local_Connection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                return false;
            }
        }


    }
}