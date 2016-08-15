using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ThreatReportViewModel
    {

        public WebApplication.Models.ThreatDetails ThreatDetails{ get; set; }
        public WebApplication.Models.ThreatSuspect ThreatSuspect { get; set; }
        public WebApplication.Models.Threat Threat { get; set; }
        public WebApplication.Models.Threatnotification ThreatNotification { get; set; }
        public WebApplication.Models.cPerson Users { get; set; }
        public List<WebApplication.Models.ThreatSuspect> SuspectsName { get; set; }
        public List<MuhafizWebPortal.Models.WhomUserNotified> NotidiedName { get; set; }
        public List<MuhafizWebPortal.Models.WhomMuhafizNotify> MuhafizNotify { get; set; }
        // For notified members
        [Required]
        public List<CheckBoxLists> AllCheckBoxes { get; set; }
        public List<CheckBoxLists> UserSelectedCheckBoxes { get; set; }
        [Required]
        public string[] SelectedCheckBoxes { get; set; }

        // for who to notify members
        [Required]
        public List<CheckBoxLists> AllCheckBoxesNotify { get; set; }
        public List<CheckBoxLists> UserSelectedCheckBoxesNotify { get; set; }
        [Required]
        public string[] SelectedCheckBoxesNotify { get; set; }


    }

}