using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UndeadEarth.Web.Models
{
    public class UserNode : Node
    {
        public bool CanHunt { get; set; }
        public bool IsMoving { get; set; }
        public double MinutesLeftInMove { get; set; }
        public int CurrentSpeed { get; set; }
        public decimal NextDestinationLongitude { get; set; }
        public decimal NextDestinationLatitude { get; set; }
        public string NextNodeName { get; set; }
        public HotZoneNode HotZone { get; set; }
    }
}
