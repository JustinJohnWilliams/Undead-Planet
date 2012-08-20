using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    /// <summary>
    /// Facilitates the hunting at a zombie pack.
    /// </summary>
    public interface IHuntDirector
    {
        void Hunt(Guid userId, Guid zombiePackId);
    }
}
