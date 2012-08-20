using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using System.Diagnostics;

namespace UndeadEarth.Model
{
    public class RandomNumberProvider : IRandomNumberProvider
    {
        //attribute added cause there is no need to test the .net framework
        private static Random _random;

        [DebuggerNonUserCode]
        static RandomNumberProvider()
        {
            _random = new Random();
        }

        //attribute added cause there is no need to test the .net framework
        [DebuggerNonUserCode]
        int IRandomNumberProvider.GetRandomInclusive(int min, int max)
        {
            return _random.Next(min, max + 1);
        }

        [DebuggerNonUserCode]
        double IRandomNumberProvider.GetNextDouble()
        {
            return _random.NextDouble();
        }

        [DebuggerNonUserCode]
        int IRandomNumberProvider.GetRandom(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}
