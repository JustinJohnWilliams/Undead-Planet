using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UndeadEarth.Web.Models
{
    public class DistanceNode : Node
    {
        public double MinutesAway { get; set; }
        public double MilesAway { get; set; }
    }
}
