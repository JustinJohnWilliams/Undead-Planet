using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IHotZoneRetriever
    {
        /// <summary>
        /// Returns a list of all hot zones in the world
        /// </summary>
        /// <returns></returns>
        List<IHotZone> GetAllHotZones();

        /// <summary>
        /// Checks to see if the hot zone is cleared for user.
        ///     Hot Zone is cleared when all zombie packs assocated with hot zone are destroyed.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotZoneId"></param>
        /// <returns></returns>
        int ZombiePacksLeft(Guid userId, Guid hotZoneId);

        /// <summary>
        /// Returns all cleared hot zones for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<KeyValuePair<Guid, int>> GetRemainingZombiePacksInHotZones(Guid userId);

        /// <summary>
        /// Returns true if hotzone exists
        /// </summary>
        /// <param name="_startingLocation"></param>
        /// <returns></returns>
        bool HotZoneExists(Guid hotZoneId);

        /// <summary>
        /// Retrives the id and name for hotzones that are valid as a starting position.
        /// </summary>
        /// <returns></returns>
        List<KeyValuePair<Guid, string>> GetStartingHotZones();

        /// <summary>
        /// Returns true if hotzone is a starting hotzone.
        /// </summary>
        /// <param name="hotZoneId"></param>
        /// <returns></returns>
        bool IsStartingHotZone(Guid hotZoneId);

        /// <summary>
        /// Returns hotzone given hotzone id.  Returns null if hotzone doesn't exist.
        /// </summary>
        /// <returns></returns>
        IHotZone GetHotZone(Guid hotZoneId);
    }
}
