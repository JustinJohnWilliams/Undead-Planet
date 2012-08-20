using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserEnergyCalculator
    {
        int GetUserEnergy(Guid userId, DateTime time);
    }
}
