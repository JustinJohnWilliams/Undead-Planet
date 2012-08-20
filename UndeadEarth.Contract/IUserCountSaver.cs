using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserCountSaver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="numberOfZombies"></param>
        void AddZombiesKilled(Guid userId, long numberOfZombies);
    }
}
