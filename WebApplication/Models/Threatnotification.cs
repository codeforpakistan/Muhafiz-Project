using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Threatnotification
    {
        public int ThreatNotification_Id { get; set; }
        public string Level_Of_Threat { get; set; }
        public string Status { get; set; }
        public string ProcessThreat_status { get; set; }
        public DateTime ReportDate { get; set; }
        public int User_Id { get; set; }


    }



}