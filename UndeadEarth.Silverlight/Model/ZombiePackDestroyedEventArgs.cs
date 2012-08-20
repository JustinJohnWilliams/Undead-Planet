using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Silverlight.Model
{
    public class ZombiePackDestroyedEventArgs : EventArgs
    {
        public Guid ZombePackId { get; set; }
    }
}
