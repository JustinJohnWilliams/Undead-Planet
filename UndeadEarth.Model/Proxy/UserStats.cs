using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Model.Proxy
{
    public class UserInGameStats
    {
        public UserNode UserNode { get; set; }
        public EnergyResult EnergyResult { get; set; }
        public LevelResult LevelResult { get; set; }
        public List<Item> Items { get; set; }
        public IntResult AttackPower { get; set; }
        public GuidResult Zone { get; set; }
        public IntResult Money { get; set; }
        public LongResult ZombiesDestroyed { get; set; }
        public IntResult MaxItems { get; set; }
    }
}
