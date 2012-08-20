using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserMoveDirector
    {
        void MoveUser(Guid userId, double latitude, double longitude);
    }
}
