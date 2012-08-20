using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUser
    {
        Guid Id { get; }
        string Email { get; }
        string DisplayName { get; }
        Guid ZoneId { get; }
        Guid LocationId { get; }
        double Latitude { get; }
        double Longitude { get; }
        int Money { get; }
        int PossibleItemAmount { get; }
    }
}
