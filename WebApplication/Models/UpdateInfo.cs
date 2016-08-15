using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class UpdateInfo
    {
        [Display(Name = "Profile Setting ID")]
        public int Profile_Settings_id { get; set; }
        [Display(Name = "Registration ID")]
        public int User_Id { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "ProfileSetting Request Time")]
        public string ProfileSetting_Request_Time { get; set; }

    }
}