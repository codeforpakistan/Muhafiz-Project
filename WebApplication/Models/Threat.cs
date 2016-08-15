using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class Threat
    {
        public long ThreatReport_Id { get; set; }
        public int ThreatNotification_Id { get; set; }
        public string Level_Of_Threat { get; set; }
        public string ReportDate { get; set; }
        
    }
}