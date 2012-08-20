﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserMover
    {
        void MoveUser(Guid userId, decimal latitude, decimal longitude);
    }
}
