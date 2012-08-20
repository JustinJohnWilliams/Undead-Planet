﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IStore
    {
        Guid Id { get; }
        double Latitude { get; }
        double Longitude { get; }
        string Name { get; }
    }
}
