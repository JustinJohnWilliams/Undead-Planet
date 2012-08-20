using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface INodeCreator
    {
        void CreateZombiePacks();
        void CreateShops();
        void CreateSafeHouses();
        void PurgeZombiePacks();
        void PurgeShops();
        void PurgeSafeHouses();
    }
}
