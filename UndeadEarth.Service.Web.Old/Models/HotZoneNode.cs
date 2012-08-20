using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UndeadEarth.Web.Models
{
    public class HotZoneNode : DistanceNode
    {
        public int ZombiesLeft { get; set; }
        public int MaxZombies { get; set; }
        public int RegenRate { get; set; }
        public int MinutesToNextRegen { get; set; }
        public bool IsDestroyed { get; set; }
        public int MinutesBetweenRegen { get; set; }
    }
}
