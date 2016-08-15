using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ThreatDetails
    {
        public int Threat_Detail_Id { get; set; }
        public int ThreatReportId { get; set; }
        //public string Date_of_Incidence { get; set; }
         [Required]
        // [DataType(DataType.DateTime)]
        // [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
         
        public string Date_of_Incidence { get;set;}
        [Required]
        public string Source { get; set; }
        public string Details { get; set; }
        public Boolean Witness { get; set; }
        public string Any_Personal_Reason { get; set; }
        public int NumberOfSuspects { get; set; }
        public string Notify_Any { get; set; }
        public string Response { get; set; }
        public string Muhafiz_Notify_To { get; set; }
        public string  DetailConfirmedStatus { get; set; }
        public ICollection<CheckBoxLists> CheckBox { get; set; }

        public ICollection<CheckBoxLists> CheckBoxes { get; set; }



        private bool connection_open;
        private MySqlConnection connection;

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
                //MessageBox::Show("No database connection connection made...\n Exiting now", "Database Connection Error");
                //Application::Exit();
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

        public bool ConfirmDetailsOfThreat(string ActivationCode)
        {

            Get_Connection();

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection;
          
            cmd.CommandText = string.Format("select * from threatdetailsconfirmationtable where ConfirmationCode = '{0}'",
               ActivationCode);

            MySqlDataReader reader = cmd.ExecuteReader();
            int threatDetailId = 0;
            string ConfirmationCode = "";
            if (reader.HasRows)
            {

                while (reader.Read())
                {

                    threatDetailId = Int32.Parse(reader["ThreatDetailId"].ToString());
                    ConfirmationCode = reader["ConfirmationCode"].ToString();



                }
                reader.Close();
                cmd.Dispose();

                MySqlCommand cmd3 = new MySqlCommand();
                cmd3.Connection = connection;


                cmd3.CommandText = string.Format("UPDATE threatdetailsconfirmationtable SET ConfirmByUser='1' where ConfirmationCode = '{0}'",
                ConfirmationCode);
                int numRowsUpdated = cmd3.ExecuteNonQuery();
                cmd3.Dispose();

            

                connection.Close();
                return true;

            }
            else {


                connection.Close();
                return false;

            }










        }

    }





    

}