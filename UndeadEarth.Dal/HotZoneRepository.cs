using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class HotZoneRepository : IHotZoneRetriever
    {
        private string _connectionString;

        public HotZoneRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        List<IHotZone> IHotZoneRetriever.GetAllHotZones()
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.HotZoneDtos.Cast<IHotZone>().ToList();
            }
        }

        int IHotZoneRetriever.ZombiePacksLeft(Guid userId, Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                List<KeyValuePair<Guid, int>> result = GetCountsForHotZonesLeft(userId, hotZoneId, dataContext);

                if (result.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return result.First().Value;
                }

            }
        }

        List<KeyValuePair<Guid, int>> IHotZoneRetriever.GetRemainingZombiePacksInHotZones(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                return GetCountsForHotZonesLeft(userId, null, dataContext).ToList();
            }
        }


        bool IHotZoneRetriever.HotZoneExists(Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.HotZoneDtos.Any(s => s.Id == hotZoneId);
            }
        }

        List<KeyValuePair<Guid, string>> IHotZoneRetriever.GetStartingHotZones()
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.HotZoneDtos.Where(h => h.CanStartHere == true).Select(s => new KeyValuePair<Guid, string>(s.Id, s.Name)).ToList();
            }
        }

        bool IHotZoneRetriever.IsStartingHotZone(Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.HotZoneDtos.Any(h => h.CanStartHere == true && h.Id == hotZoneId);
            }
        }

        IHotZone IHotZoneRetriever.GetHotZone(Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.HotZoneDtos.SingleOrDefault(h => h.Id == hotZoneId) as IHotZone;
            }
        }

        private List<KeyValuePair<Guid, int>> GetCountsForHotZonesLeft(Guid userId, Guid? forHotZoneId, UndeadEarthDataContext dataContext)
        {
            dataContext.ReadUncommited();

            var progressPerHotzone =
                from progress in dataContext.UserZombiePackProgressDtos
                join zombiePack in dataContext.ZombiePackDtos
                on progress.ZombiePackId equals zombiePack.Id
                join hotZone in dataContext.HotZoneDtos
                on zombiePack.HotZoneId equals hotZone.Id
                where progress.UserId == userId && progress.IsDestroyed == true
                group hotZone by hotZone.Id into result
                select new
                {
                    HotZoneId = result.Key,
                    Progress = result.Count()
                };

            var zombiePackCounts =
                from zombiePack in dataContext.ZombiePackDtos
                join hotZone in dataContext.HotZoneDtos
                on zombiePack.HotZoneId equals hotZone.Id
                group hotZone by hotZone.Id into result
                select new
                {
                    HotZoneId = result.Key,
                    ZombiePackCount = result.Count()
                };

            var results =
                from zombiePack in zombiePackCounts
                from progress in progressPerHotzone.Where(z => z.HotZoneId == zombiePack.HotZoneId)
                                                   .DefaultIfEmpty()
                select new
                {
                    HotZoneId = zombiePack.HotZoneId,
                    Count = progress == null ? zombiePack.ZombiePackCount : (zombiePack.ZombiePackCount - progress.Progress)
                };

            if (forHotZoneId != null)
            {
                results = results.Where(s => s.HotZoneId == forHotZoneId);
            }

            return results.Where(c => c.Count != 0).Select(c => new KeyValuePair<Guid, int>(c.HotZoneId, c.Count)).ToList();
        }
    }
}
