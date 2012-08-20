using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IZombiePackRetriever
    {
        /// <summary>
        /// Retrieves zombie pack by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IZombiePack GetZombiePackById(Guid id);

        /// <summary>
        /// Retrieves all zombie packs inside visible raidus
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        List<IZombiePack> GetAllZombiePacksInRadius(double latitude, double longitude, double radius);

        /// <summary>
        /// Retrieves all zombie packs in a hotzone.
        /// </summary>
        /// <param name="hotZoneId"></param>
        /// <returns></returns>
        List<IZombiePack> GetAllZombiePacksInHotZone(Guid hotZoneId);

        /// <summary>
        /// Returns Hot zone id associated with zombie pack
        /// </summary>
        /// <param name="zombiePackId"></param>
        /// <returns></returns>
        Guid GetHotZoneByZombiePackId(Guid zombiePackId);

        /// <summary>
        /// Returns true if a zombie pack exists.
        /// </summary>
        /// <param name="zombiePackId"></param>
        /// <returns></returns>
        bool ZombiePackExists(Guid zombiePackId);
    }
}
