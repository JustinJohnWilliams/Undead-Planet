using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IItemUsageDirector
    {
        /// <summary>
        /// Uses specified item
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        void UseItem(Guid userId, Guid itemId);
    }
}
