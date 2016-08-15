using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MuhafizWebPortal.Models
{
    public class WhomMuhafizNotify
    {
        public int ToNotify { get; set; }
        public string Name { get; set; }
        public int ThreatDetailId { get; set; }
    }
}