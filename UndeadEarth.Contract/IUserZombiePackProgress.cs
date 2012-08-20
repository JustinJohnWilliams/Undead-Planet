using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserZombiePackProgress
    {
         bool IsDestroyed { get; }
         int MaxZombies { get; }
         int ZombiesLeft { get; }
    }
}
