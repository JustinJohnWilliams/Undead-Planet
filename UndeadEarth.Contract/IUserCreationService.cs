using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserCreationService
    {
        void CreateUser(long facebookUserId, string name, Guid startingHotZoneId);

    }
}
