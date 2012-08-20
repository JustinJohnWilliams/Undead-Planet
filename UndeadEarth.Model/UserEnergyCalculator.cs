using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserEnergyCalculator : IUserEnergyCalculator
    {
        private IUserRetriever _userRetriever;
        private IUserSaver _userSaver;
        public UserEnergyCalculator(IUserRetriever userRetriever, IUserSaver userSaver)
        {
            _userRetriever = userRetriever;
            _userSaver = userSaver;
        }

        int IUserEnergyCalculator.GetUserEnergy(Guid userId, DateTime time)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return 0;
            }

            Tuple<int, DateTime> userEnergy = _userRetriever.GetLastSavedEnergy(userId);
            int? userMaxEnergy = _userRetriever.GetMaxEnergy(userId);
            if (userMaxEnergy == null)
            {
                userMaxEnergy = 100;
                _userSaver.SetMaxEnergy(userId, userMaxEnergy.Value);
            }

            if (userEnergy == null)
            {
                userEnergy = new Tuple<int, DateTime>(userMaxEnergy.Value, time);
                _userSaver.SaveLastEnergy(userId, userMaxEnergy.Value, time);
            }

            userEnergy.Item2 = new DateTime(userEnergy.Item2.Year, userEnergy.Item2.Month, userEnergy.Item2.Day, userEnergy.Item2.Hour, userEnergy.Item2.Minute, 0);
            DateTime truncatedTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);

            TimeSpan span = truncatedTime - userEnergy.Item2;
            int totalMinutes = Convert.ToInt32(span.TotalMinutes);
            if (totalMinutes < 0)
            {
                return 0;
            }

            int lastEnergy = userEnergy.Item1;
            int calculatedEnergy = lastEnergy + totalMinutes;

            if (calculatedEnergy > userMaxEnergy.Value)
            {
                calculatedEnergy = userMaxEnergy.Value;
            }

            return calculatedEnergy;
        }
    }
}
