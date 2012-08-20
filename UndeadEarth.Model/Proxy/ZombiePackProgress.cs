using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Model.Proxy
{
    public class ZombiePackProgress
    {
        public int ZombiesLeft { get; set; }
        public int MaxZombies { get; set; }
        public int CostPerHunt { get; set; }
    }
}
