using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IItem
    {
        Guid Id { get; }
        string Name { get; }
        string Description { get; }
        int Price { get; }
        int Energy { get; }
        int Distance { get; }
        int Attack { get; }
        bool IsOneTimeUse { get; }
    }
}
