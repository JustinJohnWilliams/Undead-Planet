using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Model.Proxy
{
    public class LevelResult
    {
        public int CurrentLevel { get; set; }
        public long ZombiesKilled { get; set; }
        public long ZombiesNeededForNextLevel { get; set; }
        public long ZombiesKilledLastLevel { get; set; }
    }
}
