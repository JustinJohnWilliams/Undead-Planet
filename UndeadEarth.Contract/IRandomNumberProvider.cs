using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IRandomNumberProvider
    {
        int GetRandomInclusive(int min, int max);
        int GetRandom(int min, int max);
        double GetNextDouble();
    }
}
