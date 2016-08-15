using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class PanicAlertViewModel
    {
        public WebApplication.Models.panicAttack PanicAttackdetail { get; set; }
        public WebApplication.Models.cPerson UserDetail { get; set; }
    }
}