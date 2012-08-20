using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IShopDirector
    {
        /// <summary>
        /// Buys the specified item for the specified user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="itemId"></param>
        void BuyItem(Guid userId, Guid itemId);
    }
}
