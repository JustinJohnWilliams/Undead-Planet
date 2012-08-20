using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Model.Proxy
{
    public class ZombiePackNode : DistanceNode
    {
        public int ZombiesLeft { get; set; }
        public int MaxZombies { get; set; }
        public int RegenRate { get; set; }
        public int MinutesToNextRegen { get; set; }
        public bool IsDestroyed { get; set; }
        public int MinutesBetweenRegen { get; set; }
        public Guid HotZoneId { get; set; }
    }
}
