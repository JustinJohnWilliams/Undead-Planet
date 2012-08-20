using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public class UserStats
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public long ZombiesKilled { get; set; }
        public long MilesTraveled { get; set; }
        public long ZombiePacksDestroyed { get; set; }
        public long HotZonesDestroyed { get; set; }
        public long MoneyAccumulated { get; set; }
        public long KillStreak { get; set; }
    }
}
