using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IAchievementProvider
    {
        List<string> GetAchievementsForUser(Guid userId);
    }
}
