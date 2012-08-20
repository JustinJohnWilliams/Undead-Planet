using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserMoveDirector : IUserMoveDirector
    {
        private IUserRetriever _userRetriever;
        private IUserEnergyProvider _userEnergyCalcualtor;
        private IUserSaver _userSaver;
        private IDistanceCalculator _distanceCalculator;
        private IUserCountsSaver _userCountsSaver;

        public UserMoveDirector(IUserRetriever userRetriever, IUserEnergyProvider userEnergyCalculator, IUserSaver userSaver, IDistanceCalculator distanceCalculator, IUserCountsSaver userCountsSaver)
        {
            _userRetriever = userRetriever;
            _userEnergyCalcualtor = userEnergyCalculator;
            _userSaver = userSaver;
            _distanceCalculator = distanceCalculator;
            _userCountsSaver = userCountsSaver;
        }

        void IUserMoveDirector.MoveUser(Guid userId, double latitude, double longitude)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return;
            }

            IUser user = _userRetriever.GetUserById(userId);
            int milesAway = Convert.ToInt32(_distanceCalculator.CalculateMiles(user.Latitude, user.Longitude, latitude, longitude));

            int currentEnergy = _userEnergyCalcualtor.GetUserEnergy(userId, DateTime.Now);
            double newLatitude;
            double newLongitude;
            int newEnergy;

            if (currentEnergy < milesAway)
            {
                double percentageComplete = (double)currentEnergy / (double)milesAway;

                double difference = latitude - user.Latitude;
                newLatitude = user.Latitude + (difference * percentageComplete);

                difference = longitude - user.Longitude;
                newLongitude = user.Longitude + (difference * percentageComplete);

                newEnergy = 0;

                milesAway = Convert.ToInt32(_distanceCalculator.CalculateMiles(user.Latitude, user.Longitude, newLatitude, newLongitude));
            }
            else
            {
                newLatitude = latitude;
                newLongitude = longitude;
                newEnergy = Convert.ToInt32(currentEnergy - milesAway);
            }

            _userSaver.UpdateUserLocation(userId, newLatitude, newLongitude);
            _userSaver.SaveLastEnergy(userId, newEnergy, DateTime.Now);
            _userCountsSaver.AddMiles(userId, milesAway);
        }
    }
}
