using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserEnergyProvider
    {
        int GetUserEnergy(Guid userId, DateTime time);
        int GetUserMaxEnergy(Guid userId);
    }
}
