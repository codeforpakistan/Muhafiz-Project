using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication.Models
{
    public class panicAttack
    {
        [Display(Name = "Panic Alert ID")]
        public int PanicAttack_Id { get; set; }
        [Display(Name = "Registration ID")]
        public int User_Id { get; set; }
        [Display(Name = "Panic Alert Time")]
        public string panic_date { get; set; }
        [Display(Name = "Read Status")]
        public string Seen_Status { get; set; }
    }
}