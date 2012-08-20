using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserMover : IUserMover
    {
        private IUserRetriever _userRetriever;
        private IUserEnergyCalculator _userEnergyCalcualtor;
        private IUserSaver _userSaver;
        private IDistanceCalculator _distanceCalculator;

        public UserMover(IUserRetriever userRetriever, IUserEnergyCalculator userEnergyCalculator, IUserSaver userSaver, IDistanceCalculator distanceCalculator)
        {
            _userRetriever = userRetriever;
            _userEnergyCalcualtor = userEnergyCalculator;
            _userSaver = userSaver;
            _distanceCalculator = distanceCalculator;
        }

        void IUserMover.MoveUser(Guid userId, decimal latitude, decimal longitude)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return;
            }

            IUser user = _userRetriever.GetUserById(userId);
            int milesAway = Convert.ToInt32(_distanceCalculator.CalculateMiles(user.Latitude, user.Longitude, latitude, longitude));

            int currentEnergy = _userEnergyCalcualtor.GetUserEnergy(userId, DateTime.Now);
            if (currentEnergy < milesAway)
            {
                return;
            }

            _userSaver.UpdateUserLocation(userId, latitude, longitude);
            _userSaver.SaveLastEnergy(userId, currentEnergy - milesAway, DateTime.Now);
        }
    }
}
