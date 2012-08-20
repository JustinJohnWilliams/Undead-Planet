using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IFacebookUser
    {
        long UserId { get; }
        string Name { get; }
        string Email { get; }
    }
}
