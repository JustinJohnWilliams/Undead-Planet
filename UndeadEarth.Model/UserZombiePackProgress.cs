using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserZombiePackProgress : IUserZombiePackProgress
    {
        public bool IsDestroyed { get; set; }
        public int MaxZombies { get; set; }
        public int ZombiesLeft { get; set; }
    }
}
