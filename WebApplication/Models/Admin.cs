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
using System.Security.Policy;
using System.Web.Routing;
using System.Text;
using System.Security.Cryptography;

namespace WebApplication.Models
{
    public class Admin
    {
        public int ID { get; set; }
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
       
        private MySqlConnection connection;

        private bool connection_open;
        public string converttoencryptedPassword(string passwordValue)
        {

            byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(passwordValue);
            byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
            string hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            return hashedString;

        }

        public long GetPanicAlertCount()
        {

            Get_Connection();
            long countrows = 0;
            try
            {

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("SELECT COUNT(*) FROM  panicattack WHERE  seen_status ='NotSeen'");
                try
                {
                    countrows = Convert.ToInt32(cmd.ExecuteScalar());

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

        public bool UpdatUserInfo(cPerson userobj) {
           

            Get_Connection();
            try
            {
                string Userprevpassword = "";
                MySqlCommand cmdUser = new MySqlCommand();
                cmdUser.Connection = connection;
                cmdUser.CommandText = string.Format("select Password from registrationtable where Registration_Id = '{0}'", userobj.Registration_Id);
                MySqlDataReader reader = cmdUser.ExecuteReader();
      

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Userprevpassword = reader["Password"].ToString();

                    }

                }


                reader.Close();
                cmdUser.ExecuteNonQuery();
                cmdUser.Dispose();

                MySqlCommand cmd1 = new MySqlCommand();
                cmd1.Connection = connection;
                cmd1.CommandText = string.Format("UPDATE RegistrationTable SET Email = @em, Name = @nam, WorkedAt = @work, OrganizationName = @orgname, Mobileno = @mob, StationedAt = @station, Role = @rol, Password = @pass  WHERE Registration_Id='{0}'",userobj.Registration_Id);;
                cmd1.Parameters.AddWithValue("@em",userobj.email);
                cmd1.Parameters.AddWithValue("@nam", userobj.UserName);
               
                cmd1.Parameters.AddWithValue("@orgname", userobj.orgname);

                if (userobj.orgname.Equals("Freelancer")) {


                    cmd1.Parameters.AddWithValue("@work", "Freelance");

                }
                else {

                    cmd1.Parameters.AddWithValue("@work", "Organization");

                }
                cmd1.Parameters.AddWithValue("@mob", userobj.mobilde);
                cmd1.Parameters.AddWithValue("@station", userobj.station);
                cmd1.Parameters.AddWithValue("@rol", userobj.role);
                if (string.IsNullOrEmpty(userobj.ConfirmNewPassword))
                {

                    cmd1.Parameters.AddWithValue("@pass",Userprevpassword);
                }
                else {
                   
                   
                    string hashedString = converttoencryptedPassword(userobj.ConfirmNewPassword);
                    cmd1.Parameters.AddWithValue("@pass", hashedString);
                }
                cmd1.ExecuteNonQuery();
                cmd1.Dispose();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("UPDATE profilesettings SET Status='Approved' where RegistrationId = '{0}'",
                   userobj.Registration_Id);
               

                int numRowsUpdated = cmd.ExecuteNonQuery();
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




        public string InsertReportDetails(WebApplication.Models.ThreatReportViewModel viewModel, List<string> namesofsuspects,string Dateval) {

            
            string ConfirmationCode = Guid.NewGuid().ToString();




            Get_Connection();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "INSERT INTO threatdetailedtable(ThreatReportId,Date_of_Inci,Source, Details, Witness,AnyReason, Response,ConfirmationOfDetails) VALUES (@g1d, @g2d, @g3d, @colorChart, @hex1, @hex2,@hex5 ,@hex6)";
                cmd.Parameters.AddWithValue("@g1d", viewModel.Threat.ThreatReport_Id);
                cmd.Parameters.AddWithValue("@g2d", Dateval);
                cmd.Parameters.AddWithValue("@g3d", viewModel.ThreatDetails.Source.ToString());
                cmd.Parameters.AddWithValue("@colorChart", viewModel.ThreatDetails.Details);
                cmd.Parameters.AddWithValue("@hex1", viewModel.ThreatDetails.Witness.ToString());
                cmd.Parameters.AddWithValue("@hex2", viewModel.ThreatDetails.Any_Personal_Reason);
                cmd.Parameters.AddWithValue("@hex5", viewModel.ThreatDetails.Response.ToString());
                cmd.Parameters.AddWithValue("@hex6", "NotConfirmed");
                cmd.ExecuteNonQuery();
                long id = cmd.LastInsertedId;
                cmd.Dispose();

                //=============================Insert Into Suspect table======================
               foreach (var name in namesofsuspects)
                {
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = connection;
                    cmd2.CommandText = "INSERT INTO suspecttable(Name,Contact,ThreatDetailId) VALUES (@namee, @contact, @threatdetailedId)";
                    cmd2.Parameters.AddWithValue("@namee", name);
                    cmd2.Parameters.AddWithValue("@contact", "");
                    cmd2.Parameters.AddWithValue("@threatdetailedId", id);

                    cmd2.ExecuteNonQuery();
                    cmd2.Dispose();

                }
                //=============================Insert Into NotifyTo Table========================
                foreach (var name in viewModel.SelectedCheckBoxes)
                {
                    MySqlCommand cmd4 = new MySqlCommand();
                    cmd4.Connection = connection;
                    cmd4.CommandText = "INSERT INTO notifytotable(Name,ThreatDetailId) VALUES (@namee, @threatdetailedId)";
                    cmd4.Parameters.AddWithValue("@namee", name);
                    cmd4.Parameters.AddWithValue("@threatdetailedId", id);

                    cmd4.ExecuteNonQuery();
                    cmd4.Dispose();

                }

                //===========================Insert Into whoToNotify===================================

                foreach (var name in viewModel.SelectedCheckBoxesNotify)
                {
                    MySqlCommand cmd5 = new MySqlCommand();
                    cmd5.Connection = connection;
                    cmd5.CommandText = "INSERT INTO whotonotifytable(Name,ThreatDetailId) VALUES (@namee, @threatdetailedId)";
                    cmd5.Parameters.AddWithValue("@namee", name);
                    cmd5.Parameters.AddWithValue("@threatdetailedId", id);

                    cmd5.ExecuteNonQuery();
                    cmd5.Dispose();

                }







            //=============================================================================

                MySqlCommand cmd1 = new MySqlCommand();
                cmd1.Connection = connection;
                cmd1.CommandText = "INSERT INTO threatdetailsconfirmationtable(ThreatDetailId,ConfirmationCode,ConfirmByUser) VALUES (@g1d, @g2d,@g3d)";
                cmd1.Parameters.AddWithValue("@g1d", id);
                cmd1.Parameters.AddWithValue("@g2d", ConfirmationCode);
                cmd1.Parameters.AddWithValue("@g3d", '0');

                cmd1.ExecuteNonQuery();
                cmd1.Dispose();
                //===============================Remove Notification=====================
                MySqlCommand cmd3 = new MySqlCommand();
                cmd3.Connection = connection;


                cmd3.CommandText = string.Format("UPDATE threatnotificationtable SET ProcessThreat_status='Approved' where ThreatReportNotification_Id = '{0}'",
                viewModel.ThreatNotification.ThreatNotification_Id);


                int numRowsUpdated = cmd3.ExecuteNonQuery();
                cmd3.Dispose();












                connection.Close();

                return ConfirmationCode;
            }
            catch (MySqlException e)
            {
                string MessageString = "Read error occurred  / entry not found loading the Column details: "
                    + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                //MessageBox.Show(MessageString, "SQL Read Error");
                connection.Close();
                return null;

            }


        }


        public static bool SendEmail(string toAddress, string subject, string body)
        {



            string your_id = "muhafizgroup123@gmail.com";
            string your_password = "muhafiz123";
            try
            {
                SmtpClient client = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(your_id, your_password),
                    Timeout = 10000,
                };
                MailMessage mm = new MailMessage(your_id, toAddress, subject, body);
                mm.IsBodyHtml = true;
                client.Send(mm);
                return true;
            }
            catch (Exception e)
            {


                string MessageString = "The following error occurred loading the Column details: "
                   + " - " + e.Message;
                return false;
            }

        }   
        public cPerson GetUser(int? id, int? param)
        {
            var cperson = new cPerson();
            try
            {
                Get_Connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("select * from registrationtable where Registration_Id = '{0}'", id);
                MySqlDataReader reader = cmd.ExecuteReader();
                var model = new cPerson();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string tmp = reader["Registration_Id"].ToString();
                        cperson.Registration_Id = Int32.Parse(tmp);
                        cperson.UserName = reader["Name"].ToString();
                        cperson.email = reader["Email"].ToString();
                        cperson.orgname = reader["OrganizationName"].ToString();
                        cperson.station = reader["StationedAt"].ToString();
                        cperson.mobilde = reader["Mobileno"].ToString();
                        cperson.role = reader["Role"].ToString();
                        cperson.password = reader["Password"].ToString();
                    }
                }
                cmd.Dispose();
                connection.Close();
                return cperson;
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                cperson.UserName = MessageString;
                cperson.email = null;
                connection.Close();
                return cperson;
            }

        }
      
        public List<ProfileSettingViewModel> GetAllUpdateInfoRequests() {
            Get_Connection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            //cmd.CommandText = "INSERT INTO registereduser(Email,Mobileno,Name,OrganizationName,Password,reg_date,Role,StationedAt,WorkedAt)VALUES('" + email + "','" + mobilde + "','" + name + "','" + orgname + "','" + password + "','" + reg + "','" + role + "','" + station + "','" + workat + "') ;";
            cmd.CommandText = string.Format("SELECT  Email, Name, Mobileno, WorkedAt, OrganizationName,  StationedAt, Role, ProfileSettings_Id,RegistrationId ,ProfileSettings_date FROM registrationtable,profilesettings WHERE Registration_Id = RegistrationId AND Status ='pending' ORDER BY ProfileSettings_date DESC");
            MySqlDataReader reader = cmd.ExecuteReader();
            var model = new List<ProfileSettingViewModel>();
            while (reader.Read())
            {
                var UpdateReq = new ProfileSettingViewModel();
                UpdateReq.UserDetail = new cPerson();
                UpdateReq.UpdateInfodetail = new UpdateInfo();
                UpdateReq.UpdateInfodetail.Profile_Settings_id = Int32.Parse(reader["ProfileSettings_Id"].ToString());
                UpdateReq.UpdateInfodetail.User_Id = Int32.Parse(reader["RegistrationId"].ToString());
                UpdateReq.UpdateInfodetail.ProfileSetting_Request_Time = reader["ProfileSettings_date"].ToString();
                UpdateReq.UserDetail.email = reader["Email"].ToString();
                UpdateReq.UserDetail.UserName = reader["Name"].ToString();
                UpdateReq.UserDetail.mobilde = reader["Mobileno"].ToString();
                UpdateReq.UserDetail.workat = reader["WorkedAt"].ToString();
                UpdateReq.UserDetail.orgname = reader["OrganizationName"].ToString();
                UpdateReq.UserDetail.station = reader["StationedAt"].ToString();
                UpdateReq.UserDetail.role = reader["Role"].ToString();

                model.Add(UpdateReq);
            }

            connection.Close();
            return model;



        }
        public List<PanicAlertViewModel> GetPanicAlerts() {
            var model = new List<PanicAlertViewModel>();
            try
            {
                Get_Connection();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("SELECT  Email, Name, Mobileno, WorkedAt, OrganizationName,  StationedAt, Role, panic_date,seen_status,RegistrationId ,PanicAttack_Id FROM registrationtable,panicattack WHERE Registration_Id = RegistrationId ORDER BY panic_date DESC");

                MySqlDataReader reader = cmd.ExecuteReader();
             
                while (reader.Read())
                {
                    var panic_Attack = new PanicAlertViewModel();
                    panic_Attack.PanicAttackdetail = new panicAttack();
                    panic_Attack.UserDetail = new cPerson();
                    panic_Attack.PanicAttackdetail.PanicAttack_Id = Int32.Parse(reader["PanicAttack_Id"].ToString());
                    panic_Attack.PanicAttackdetail.User_Id = Int32.Parse(reader["RegistrationId"].ToString());
                    
                    var outputTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                    var reportDateTimeRetrieved = Convert.ToDateTime(reader["panic_date"].ToString());
                    var dateTimeInUtc = new DateTime(reportDateTimeRetrieved.Ticks, DateTimeKind.Utc);
                    DateTime reportDateTimeConverted = TimeZoneInfo.ConvertTime(dateTimeInUtc, outputTimeZone);
                    panic_Attack.PanicAttackdetail.panic_date = reportDateTimeConverted.ToString();

                    panic_Attack.PanicAttackdetail.Seen_Status = reader["seen_status"].ToString();
                    panic_Attack.UserDetail.email = reader["Email"].ToString();
                    panic_Attack.UserDetail.UserName = reader["Name"].ToString();
                    panic_Attack.UserDetail.mobilde = reader["Mobileno"].ToString();
                    panic_Attack.UserDetail.workat = reader["WorkedAt"].ToString();
                    panic_Attack.UserDetail.orgname = reader["OrganizationName"].ToString();
                    panic_Attack.UserDetail.station = reader["StationedAt"].ToString();
                    panic_Attack.UserDetail.role = reader["Role"].ToString();

                    model.Add(panic_Attack);
                }
                reader.Close();
                cmd.Dispose();
                //=================================
                MySqlCommand cmd1 = new MySqlCommand();
                cmd1.Connection = connection;
                cmd1.CommandText = string.Format("UPDATE panicattack SET seen_status ='Seen' where seen_status = 'NotSeen'");
                int numRowsUpdated = cmd1.ExecuteNonQuery();
                cmd1.Dispose();

                connection.Close();
                return model;
            }
            catch (MySqlException e) {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                connection.Close();
                return model;

            }

        }
    //    mokahoot@lmkr.com

        public PanicAlertViewModel GetAlertDetails(int ?Attackid, int? Userid) {
            var model = new PanicAlertViewModel();
            try
            {
                Get_Connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("select * from panicattack where PanicAttack_Id = '{0}'", Attackid);
               // string.  Format("select * from panicattack where PanicAttack_Id = '{0}' and ProfileSettings_date = '{1}'", Attackid, Userid);
               // cmd.CommandText = "SELECT * from registrationtable WHERE Registration_Id ='" + id + "'";
                MySqlDataReader reader = cmd.ExecuteReader();

                model.PanicAttackdetail = new panicAttack();
                model.UserDetail = new cPerson();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                       
                        string tmp = reader["RegistrationId"].ToString();
                        model.PanicAttackdetail.User_Id = Int32.Parse(tmp);
                        
                        var outputTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                        var reportDateTimeRetrieved = Convert.ToDateTime(reader["panic_date"].ToString());
                        var dateTimeInUtc = new DateTime(reportDateTimeRetrieved.Ticks, DateTimeKind.Utc);
                        DateTime reportDateTimeConverted = TimeZoneInfo.ConvertTime(dateTimeInUtc, outputTimeZone);
                        model.PanicAttackdetail.panic_date = reportDateTimeConverted.ToString();                      
          
                    }
                }


                  model.UserDetail = GetUser(Userid, 0);

                connection.Close();
                return model;
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                connection.Close();
                return model;
            }





        }
        public List<ThreatReportViewModel> GetAllThreatHistoriesByUserName(string searchstring)
        {
            List<ThreatReportViewModel> modelList = new List<ThreatReportViewModel>();

            try
            {
                Get_Connection();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("SELECT registrationtable.*, threatnotificationtable.*, threatreporttable.*, threatdetailedtable.* FROM registrationtable JOIN threatnotificationtable ON threatnotificationtable.RegistrationId = registrationtable.Registration_Id JOIN threatreporttable ON threatreporttable.ThreatReportNotificationId = threatnotificationtable.ThreatReportNotification_Id JOIN threatdetailedtable ON threatdetailedtable.ThreatReportId = threatreporttable.ThreatReport_Id where registrationtable.Name ='{0}'", searchstring);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var model = new ThreatReportViewModel();
                    model.Users = new cPerson();
                    model.ThreatDetails = new ThreatDetails();


                    model.Users.Registration_Id = Int32.Parse(reader["Registration_Id"].ToString());
                    model.Users.email = reader["Email"].ToString();
                    model.Users.UserName = reader["Name"].ToString();
                    model.Users.mobilde = reader["Mobileno"].ToString();
                    model.ThreatDetails.Threat_Detail_Id = Int32.Parse(reader["ThreatDetail_Id"].ToString());
                    model.ThreatDetails.ThreatReportId = Int32.Parse(reader["ThreatReport_Id"].ToString());
                    model.ThreatDetails.DetailConfirmedStatus = reader["ConfirmationOfDetails"].ToString();
                    model.ThreatDetails.Source = reader["Source"].ToString();

                    modelList.Add(model);
                }

                connection.Close();
                return modelList;

            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;

                connection.Close();
                return null;
            }




        }
        public List<ThreatReportViewModel> GetAllThreatHistories()
        {
            List<ThreatReportViewModel> modelList = new List<ThreatReportViewModel>();

            try
            {
                Get_Connection();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("SELECT registrationtable.*, threatnotificationtable.*, threatreporttable.*, threatdetailedtable.* FROM registrationtable JOIN threatnotificationtable ON threatnotificationtable.RegistrationId = registrationtable.Registration_Id JOIN threatreporttable ON threatreporttable.ThreatReportNotificationId = threatnotificationtable.ThreatReportNotification_Id JOIN threatdetailedtable ON threatdetailedtable.ThreatReportId = threatreporttable.ThreatReport_Id");
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var model = new ThreatReportViewModel();
                    model.Users = new cPerson();
                    model.ThreatDetails = new ThreatDetails();


                    model.Users.Registration_Id = Int32.Parse(reader["Registration_Id"].ToString());
                    model.Users.email = reader["Email"].ToString();
                    model.Users.UserName = reader["Name"].ToString();
                    model.Users.mobilde = reader["Mobileno"].ToString();
                    model.ThreatDetails.Threat_Detail_Id = Int32.Parse(reader["ThreatDetail_Id"].ToString());
                    model.ThreatDetails.ThreatReportId = Int32.Parse(reader["ThreatReport_Id"].ToString());
                    model.ThreatDetails.DetailConfirmedStatus = reader["ConfirmationOfDetails"].ToString();
                    model.ThreatDetails.Source = reader["Source"].ToString();

                    modelList.Add(model);
                }

                connection.Close();
                return modelList;

            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;

                connection.Close();
                return null;
            }




        }
        public List<ThreatReportViewModel> GetConfirmedThreatDetail()
        {
            List<ThreatReportViewModel> modelList = new List<ThreatReportViewModel>();
         
            try
            {
                Get_Connection();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("SELECT registrationtable.*, threatnotificationtable.*, threatreporttable.*, threatdetailedtable.* FROM registrationtable JOIN threatnotificationtable ON threatnotificationtable.RegistrationId = registrationtable.Registration_Id JOIN threatreporttable ON threatreporttable.ThreatReportNotificationId = threatnotificationtable.ThreatReportNotification_Id JOIN threatdetailedtable ON threatdetailedtable.ThreatReportId = threatreporttable.ThreatReport_Id JOIN threatdetailsconfirmationtable ON threatdetailedtable.ThreatDetail_Id = threatdetailsconfirmationtable.ThreatDetailId WHERE threatdetailsconfirmationtable.ConfirmByUser= '1'");
                MySqlDataReader reader = cmd.ExecuteReader();
               
                while (reader.Read())
                {
                     var model = new ThreatReportViewModel();
                     model.Users = new cPerson();
                     model.ThreatDetails = new ThreatDetails();


                     model.Users.Registration_Id = Int32.Parse(reader["Registration_Id"].ToString());
                     model.Users.email = reader["Email"].ToString();
                     model.Users.UserName = reader["Name"].ToString();
                     model.Users.mobilde = reader["Mobileno"].ToString();
                     model.ThreatDetails.Threat_Detail_Id = Int32.Parse(reader["ThreatDetail_Id"].ToString());
                     model.ThreatDetails.ThreatReportId= Int32.Parse(reader["ThreatReport_Id"].ToString());


                    modelList.Add(model);
                }

                connection.Close();
                return modelList;

            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                
                connection.Close();
                return null;
            }


         

        }
        public bool ConfirmDetailsByAdmin(int? id)
        {


            try
            {
                Get_Connection();

                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("UPDATE threatdetailedtable SET ConfirmationOfDetails ='Confirmed' where ThreatDetail_Id = '{0}'",
                id);
                int numRowsUpdated = cmd.ExecuteNonQuery();
                cmd.Dispose();

                MySqlCommand cmd2 = new MySqlCommand();
                cmd2.Connection = connection;
                cmd2.CommandText = string.Format("DELETE FROM threatdetailsconfirmationtable where ThreatDetailId  = '{0}'",
                id);
                int RowsUpdated2 = cmd2.ExecuteNonQuery();
                cmd2.Dispose();
                connection.Close();
                return true;

        }

            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                
                connection.Close();
                return false;
            }









        }
             
        public ThreatReportViewModel GetThreatRequest(int? id, int? param)
        {
            ThreatReportViewModel viewmodel = new ThreatReportViewModel();
            viewmodel.ThreatNotification = new Threatnotification();
            viewmodel.Threat = new Threat();
            var model = new Threatnotification();
            var threatmodel = new Threat();
            try
            {
                Get_Connection();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = string.Format("select * from threatnotificationtable where ThreatReportNotification_Id= '{0}'", id);
                MySqlDataReader reader = cmd.ExecuteReader();


                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string tmp = reader["RegistrationId"].ToString();
                        model.User_Id = Int32.Parse(tmp);
                        model.ThreatNotification_Id = Int32.Parse(reader["ThreatReportNotification_Id"].ToString());
                        model.Level_Of_Threat = reader["Level_of_Threat"].ToString();
                        model.Status = reader["Threat_status"].ToString();
                        
                        var reportDateTimeRetrieved = Convert.ToDateTime(reader["ReportDate"]);
                        model.ReportDate = reportDateTimeRetrieved;
                       


                    }
                   
                    viewmodel.ThreatNotification = model;
                    reader.Close();
                    cmd.Dispose();
                    if (model.Status.Equals("No")) {
                        MySqlCommand cmd3 = new MySqlCommand();
                        cmd3.Connection = connection;
                        cmd3.CommandText = string.Format("select * from threatreporttable where ThreatReportNotificationId= '{0}'", id);
                        MySqlDataReader reader2 = cmd3.ExecuteReader();
                        if (reader2.HasRows)
                        {
                            while (reader2.Read())
                            {
                                string tmp = reader2["ThreatReport_Id"].ToString();
                                threatmodel.ThreatReport_Id = Int32.Parse(tmp);
                                threatmodel.ThreatNotification_Id = Int32.Parse(reader2["ThreatReportNotificationId"].ToString());
                                threatmodel.Level_Of_Threat = reader2["Level_of_Threat"].ToString();
                            }
                            
                            viewmodel.Threat = threatmodel;
                            reader2.Close();
                            cmd3.Dispose();



                        }


                      else {

                            reader2.Close();
                            cmd3.Dispose();
                            MySqlCommand cmd2 = new MySqlCommand();
                            cmd2.Connection = connection;
                            cmd2.CommandText = "INSERT INTO threatreporttable(ThreatReportNotificationId,Level_of_Threat,ReportDate) VALUES (@g1d, @g2d,@g3d)";
                            cmd2.Parameters.AddWithValue("@g1d", id);
                            cmd2.Parameters.AddWithValue("@g2d", model.Level_Of_Threat);
                            cmd2.Parameters.AddWithValue("@g3d", model.ReportDate);
                            cmd2.ExecuteNonQuery();
                            long LastInsid = cmd2.LastInsertedId;
                            cmd2.Dispose();

                            threatmodel.ThreatReport_Id = LastInsid;
                            threatmodel.ThreatNotification_Id = model.User_Id;
                            threatmodel.Level_Of_Threat = model.Level_Of_Threat;
                            threatmodel.ReportDate = model.ReportDate.ToString();
                            viewmodel.Threat = threatmodel;
                        }

                    }
                }
              
                connection.Close();

                return viewmodel;
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                model.Status = MessageString;
                connection.Close();
                return null;
            }

        }
        public bool DiscardthreatReq(int?id) {

            Get_Connection();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;

                cmd.CommandText = string.Format("DELETE FROM threatnotificationtable where ThreatReportNotification_Id = '{0}'",
                   id);
                

                int numRowsUpdated = cmd.ExecuteNonQuery();
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
        public bool ApproveUser(int? id, string param)
        {
            

            Get_Connection();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;

               
                cmd.CommandText = string.Format("UPDATE RegistrationTable SET RegistrationStatus='Approved' where Registration_Id = '{0}'",
                   id);
                

                int numRowsUpdated = cmd.ExecuteNonQuery();
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
        public bool RejectUser(int? id)
        {


            Get_Connection();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;

               
                cmd.CommandText = string.Format("DELETE FROM registrationtable where Registration_Id = '{0}'",
                   id);

                int numRowsUpdated = cmd.ExecuteNonQuery();
                cmd.Dispose();
                connection.Close();
                return true;

            }
            catch (MySqlException e)
            {
                string MessageString = "Read error occurred  / entry not found loading the Column details: "
                    + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                connection.Close();
                return false;

            }

        }

        public List<cPerson> GetAllRequests()
        {
            Get_Connection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = string.Format("select * from registrationtable Where RegistrationStatus='Pending'");
            MySqlDataReader reader = cmd.ExecuteReader();
            var model = new List<cPerson>();
            while (reader.Read())
            {
                var cperson = new cPerson();
                cperson.Registration_Id = Int32.Parse(reader["Registration_Id"].ToString());
                cperson.UserName = reader["Name"].ToString();
                cperson.email = reader["Email"].ToString();
                cperson.mobilde = reader["Mobileno"].ToString();
                cperson.role = reader["Role"].ToString();
                cperson.orgname = reader["OrganizationName"].ToString();
                cperson.station = reader["StationedAt"].ToString();

                model.Add(cperson);
            }
            connection.Close();
           
            return model;

        }
        public List<cPerson> GetAllRegisteredUsers() {
            Get_Connection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText = string.Format("select * from registrationtable Where RegistrationStatus='Approved' ORDER BY reg_date DESC");
            MySqlDataReader reader = cmd.ExecuteReader();
            var model = new List<cPerson>();
            while (reader.Read())
            {
                var cperson = new cPerson();
                cperson.Registration_Id = Int32.Parse(reader["Registration_Id"].ToString());
                cperson.UserName = reader["Name"].ToString();
                cperson.email = reader["Email"].ToString();
                cperson.mobilde = reader["Mobileno"].ToString();
                cperson.role = reader["Role"].ToString();
                cperson.orgname = reader["OrganizationName"].ToString();
                cperson.station = reader["StationedAt"].ToString();

                var dateTimeRetrieved = Convert.ToDateTime(reader["reg_date"]);
                var outputTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time");
                var outputDateTime = TimeZoneInfo.ConvertTime(dateTimeRetrieved, outputTimeZone);

                cperson.RegisterationDate = outputDateTime;
                model.Add(cperson);
            }
            connection.Close();

            return model;




        }
        public List<ThreatReportViewModel> GetThreatRequests()
        {
            Get_Connection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
            cmd.CommandText= string.Format("SELECT  Email, Name, Mobileno, WorkedAt, OrganizationName,  StationedAt, Role, Level_of_Threat,Threat_status,ReportDate,RegistrationId ,ThreatReportNotification_Id FROM registrationtable,threatnotificationtable WHERE Registration_Id = RegistrationId AND ProcessThreat_status='Pending' ORDER BY ReportDate DESC");
            MySqlDataReader reader = cmd.ExecuteReader();
            var model = new List<ThreatReportViewModel>();
            while (reader.Read())
            {
                var objthreat= new ThreatReportViewModel();
                objthreat.ThreatNotification = new Threatnotification();
                objthreat.Users = new cPerson();
                objthreat.ThreatNotification.ThreatNotification_Id = Int32.Parse(reader["ThreatReportNotification_Id"].ToString());
                objthreat.ThreatNotification.User_Id = Int32.Parse(reader["RegistrationId"].ToString());
                objthreat.ThreatNotification.Level_Of_Threat = reader["Level_of_Threat"].ToString();

                var reportDateTimeRetrieved = Convert.ToDateTime(reader["ReportDate"]);
                objthreat.ThreatNotification.ReportDate = reportDateTimeRetrieved;

                objthreat.ThreatNotification.Status = reader["Threat_status"].ToString();
                objthreat.Users.email = reader["Email"].ToString();
                objthreat.Users.UserName = reader["Name"].ToString();
                objthreat.Users.mobilde = reader["Mobileno"].ToString();
                objthreat.Users.workat = reader["WorkedAt"].ToString();
                objthreat.Users.orgname = reader["OrganizationName"].ToString();
                objthreat.Users.station = reader["StationedAt"].ToString();
                objthreat.Users.role = reader["Role"].ToString();


                model.Add(objthreat);
            }
            connection.Close();
            return model;

        }
       
        public Admin IsValidUser(string useremail, string password)
        {

            Get_Connection();
            try
            {
                Admin adminUser = new Admin();
                string pass = converttoencryptedPassword(password);
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = connection;
                
                cmd.CommandText    =  string.Format("select * from admin where Username = '{0}' and  Password = '{1}'", useremail, pass);


                MySqlDataReader reader = cmd.ExecuteReader();

                try
                {
                    
                    if (reader.HasRows)
                    {
                        while (reader.Read()) {
                            string tmp = reader["ID"].ToString();
                            adminUser.ID = Int32.Parse(tmp);
                            adminUser.Email = reader["Email"].ToString();
                            adminUser.UserName = reader["UserName"].ToString();
                          

                        }



                        reader.Dispose();
                        cmd.Dispose();
                        return adminUser;
                    }
                    else
                    {
                        reader.Dispose();
                        cmd.Dispose();
                        connection.Close();
                        return null;
                    }
                    //reader.Close();

                }
                catch (MySqlException e)
                {
                    string MessageString = "Read error occurred  / entry not found loading the Column details: "
                        + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                     //Sql Read Error
                    reader.Close();
                    connection.Close();
                    return null;
                }
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                UserName = MessageString;
                connection.Close();
                return null;
            }
        }

        public ThreatReportViewModel GetUserThreatDetails(int ThreatDetailId) {

            Get_Connection();
            try
            {
               ThreatReportViewModel ThreatDetailModel = new ThreatReportViewModel();
                ThreatDetailModel.ThreatDetails = new ThreatDetails();
              

                try
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = connection;

                    cmd.CommandText = string.Format("select * from threatdetailedtable where ThreatDetail_Id = '{0}'", ThreatDetailId);


                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            string tmp = reader["ThreatDetail_Id"].ToString();
                            ThreatDetailModel.ThreatDetails.Threat_Detail_Id = Int32.Parse(tmp);
                            ThreatDetailModel.ThreatDetails.Date_of_Incidence = reader["Date_of_Inci"].ToString();
                            ThreatDetailModel.ThreatDetails.Source = reader["Source"].ToString();
                            ThreatDetailModel.ThreatDetails.Details = reader["Details"].ToString();
                            string tempWitness = reader["Witness"].ToString();
                            ThreatDetailModel.ThreatDetails.Witness = Convert.ToBoolean(tempWitness);
                            ThreatDetailModel.ThreatDetails.Details = reader["AnyReason"].ToString();
                            ThreatDetailModel.ThreatDetails.Details = reader["Response"].ToString();
                            ThreatDetailModel.ThreatDetails.Details = reader["ConfirmationOfDetails"].ToString();
                        }
                        reader.Dispose();
                        cmd.Dispose();
                       
                    }
                    else
                    {
                        reader.Dispose();
                        cmd.Dispose();
                       
                        
                    }
                    //=============================================Suspect ==========================================================//
                    ThreatDetailModel.SuspectsName = new List<ThreatSuspect>();
                    MySqlCommand cmd1 = new MySqlCommand();
                    cmd1.Connection = connection;

                    cmd1.CommandText = string.Format("select * from  suspecttable where ThreatDetailId = '{0}'", ThreatDetailId);
                    MySqlDataReader reader1 = cmd1.ExecuteReader();
                   
                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            ThreatSuspect SuspectModel = new ThreatSuspect();
                            string tmp = reader1["Suspect_Id"].ToString();
                            SuspectModel.Suspect_Id = Int32.Parse(tmp);
                            SuspectModel.Full_Name = reader1["Name"].ToString();
                            SuspectModel.Contact = reader1["Contact"].ToString();
                            SuspectModel.ThreatDetailId = Int32.Parse(reader1["ThreatDetailId"].ToString());
                            ThreatDetailModel.SuspectsName.Add(SuspectModel);
                        }
                        reader1.Dispose();
                        cmd1.Dispose();

                    }
                    else
                    {
                        reader1.Dispose();
                        cmd1.Dispose();
                       

                    }

                    //=====================================================WhomUserNotified=============================================================//
                    ThreatDetailModel.NotidiedName = new List<MuhafizWebPortal.Models.WhomUserNotified>();
                    MySqlCommand cmd2 = new MySqlCommand();
                    cmd2.Connection = connection;

                    cmd2.CommandText = string.Format("select * from notifytotable where ThreatDetailId = '{0}'", ThreatDetailId);
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    if (reader2.HasRows)
                    {
                        while (reader2.Read())
                        {
                            MuhafizWebPortal.Models.WhomUserNotified WhomUserNotifiedModel = new MuhafizWebPortal.Models.WhomUserNotified();
                            string tmp = reader2["Notified_Id"].ToString();
                            WhomUserNotifiedModel.Notified_Id = Int32.Parse(tmp);
                            WhomUserNotifiedModel.Name = reader2["Name"].ToString();
                            WhomUserNotifiedModel.ThreatDetailId = Int32.Parse(reader2["ThreatDetailId"].ToString());
                            ThreatDetailModel.NotidiedName.Add(WhomUserNotifiedModel);
                        }
                        reader2.Dispose();
                        cmd2.Dispose();

                    }
                    else
                    {
                        reader2.Dispose();
                        cmd2.Dispose();
                        

                    }

                    //=======================================WhoToMuhafizNotify===============================================================//
                    ThreatDetailModel.MuhafizNotify = new List<MuhafizWebPortal.Models.WhomMuhafizNotify>();
                    MySqlCommand cmd3 = new MySqlCommand();
                    cmd3.Connection = connection;

                    cmd3.CommandText = string.Format("select * from whotonotifytable where ThreatDetailId = '{0}'", ThreatDetailId);
                    MySqlDataReader reader3= cmd3.ExecuteReader();
                    if (reader3.HasRows)
                    {
                        while (reader3.Read())
                        {
                            MuhafizWebPortal.Models.WhomMuhafizNotify WhomMuhafizNotifyModel = new MuhafizWebPortal.Models.WhomMuhafizNotify();
                            string tmp = reader3["ToNotify_Id"].ToString();
                            WhomMuhafizNotifyModel.ToNotify = Int32.Parse(tmp);
                            WhomMuhafizNotifyModel.Name = reader3["Name"].ToString();
                            WhomMuhafizNotifyModel.ThreatDetailId = Int32.Parse(reader3["ThreatDetailId"].ToString());
                            ThreatDetailModel.MuhafizNotify.Add(WhomMuhafizNotifyModel);
                        }
                        reader3.Dispose();
                        cmd3.Dispose();

                    }
                    else
                    {
                        reader3.Dispose();
                        cmd3.Dispose();
                       

                    }

                    connection.Close();
                    return ThreatDetailModel;
                    //==============================================================================================================//

                }
                catch (MySqlException e)
                {
                    string MessageString = "Read error occurred  / entry not found loading the Column details: "
                        + e.ErrorCode + " - " + e.Message + "; \n\nPlease Continue";
                    //Sql Read Error
                   
                    connection.Close();
                    return null;
                }
            }
            catch (MySqlException e)
            {
                string MessageString = "The following error occurred loading the Column details: "
                    + e.ErrorCode + " - " + e.Message;
                UserName = MessageString;
                connection.Close();
                return null;
            }



        }
        private void Get_Connection()
        {
            connection_open = false;

            connection = new MySqlConnection();
            
            connection.ConnectionString = ConfigurationManager.ConnectionStrings["MySqlConnectionString"].ConnectionString;

           
            if (Open_Local_Connection())
            {
                connection_open = true;
            }
            else
            {
                //					MessageBox::Show("No database connection connection made...\n Exiting now", "Database Connection Error");
                //					 Application::Exit();
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