using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserStatsRetriever
    {
        /// <summary>
        /// Retrieves stats for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        UserStats GetStats(Guid userId);

        /// <summary>
        /// Returns users ranks for facebook user ids.  Facebook users that dont exist are diregarded.
        /// </summary>
        /// <param name="facebookUserIds"></param>
        /// <returns></returns>
        List<UserStats> GetUsersRank(List<long> facebookUserIds);
    }
}
