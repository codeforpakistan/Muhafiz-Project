using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ThreatSuspect
    {
        public int Suspect_Id { get; set; }
        public int ThreatDetailId { get; set; }
        public string Full_Name { get; set; }
        public string Contact { get; set; }

    }
}