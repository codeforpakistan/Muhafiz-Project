using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class ProfileSettingViewModel
    {
        public WebApplication.Models.UpdateInfo UpdateInfodetail { get; set; }
        public WebApplication.Models.cPerson UserDetail { get; set; }
    }
}