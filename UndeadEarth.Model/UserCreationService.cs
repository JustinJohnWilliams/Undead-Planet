using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserCreationService : IUserCreationService
    {
        private IUserRetriever _userRetriever;
        private IUserSaver _userSaver;
        private IUserItemSaver _userItemSaver;
        private ISafeHouseItemSaver _safeHouseItemSaver;
        private IHotZoneRetriever _hotZoneRetriever;
        private IUserCountsSaver _userCountsSaver;

        public UserCreationService(IUserRetriever userRetriever, IUserSaver userSaver, IUserItemSaver userItemSaver, ISafeHouseItemSaver safeHouseItemSaver, IHotZoneRetriever hotZoneRetriever, IUserCountsSaver userCountsSaver)
        {
            _userRetriever = userRetriever;
            _userSaver = userSaver;
            _userItemSaver = userItemSaver;
            _safeHouseItemSaver = safeHouseItemSaver;
            _hotZoneRetriever = hotZoneRetriever;
            _userCountsSaver = userCountsSaver;
        }

        void IUserCreationService.CreateUser(long facebookUserId, string name, Guid startingHotZoneId)
        {
            Guid userId = Guid.NewGuid();

            if(_userRetriever.FacebookUserExists(facebookUserId))
                return;

            if (_hotZoneRetriever.IsStartingHotZone(startingHotZoneId) == false)
                return;

            _userSaver.InsertUser(userId, facebookUserId, name, startingHotZoneId, 1, 100);

            _userCountsSaver.InsertCounts(userId);

            IHotZone hotZone = _hotZoneRetriever.GetHotZone(startingHotZoneId);

            _userSaver.UpdateUserLocation(userId, hotZone.Latitude, hotZone.Longitude);

            _userItemSaver.AddTent(userId, 5);
        }
    }
}
