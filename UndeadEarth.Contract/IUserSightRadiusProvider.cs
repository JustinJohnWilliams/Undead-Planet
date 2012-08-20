using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserSightRadiusProvider
    {
        int GetUserSightRadius(Guid userId, DateTime time);
        int GetUserMinSightRadius(Guid userId);
    }
}
