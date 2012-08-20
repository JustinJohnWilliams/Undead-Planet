using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserEnergyAndSightProvider : IUserEnergyProvider, IUserSightRadiusProvider
    {
        private IUserRetriever _userRetriever;
        private IUserSaver _userSaver;
        private IUserItemRetriever _userItemRetriever;

        public UserEnergyAndSightProvider(IUserRetriever userRetriever, IUserSaver userSaver, IUserItemRetriever userItemRetriever)
        {
            _userRetriever = userRetriever;
            _userSaver = userSaver;
            _userItemRetriever = userItemRetriever;
        }

        int IUserEnergyProvider.GetUserEnergy(Guid userId, DateTime time)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return 0;
            }

            Tuple<int, DateTime> userEnergy = _userRetriever.GetLastSavedEnergy(userId);
            SetBaseEnergyIfNeeded(userId);
            int userBaseEnergy = _userRetriever.GetCurrentBaseEnergy(userId);

            userBaseEnergy += GetAdditionalEnergyFromItems(userId);

            if (userEnergy == null)
            {
                userEnergy = new Tuple<int, DateTime>(userBaseEnergy, time);
                _userSaver.SaveLastEnergy(userId, userBaseEnergy, time);
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
            int calculatedEnergy = lastEnergy + (totalMinutes * 10);

            if (calculatedEnergy > userBaseEnergy)
            {
                calculatedEnergy = userBaseEnergy;
            }

            return calculatedEnergy;
        }

        int IUserEnergyProvider.GetUserMaxEnergy(Guid userId)
        {
            if (_userRetriever.UserExists(userId) == false) return 0;

            SetBaseEnergyIfNeeded(userId);
            int? baseEnergy = _userRetriever.GetCurrentBaseEnergy(userId);
            return baseEnergy.Value + GetAdditionalEnergyFromItems(userId);
        }

        private void SetBaseEnergyIfNeeded(Guid userId)
        {
            int userBaseEnergy = _userRetriever.GetCurrentBaseEnergy(userId);
            if (userBaseEnergy == 0)
            {
                userBaseEnergy = 100;
                _userSaver.UpdateCurrentBaseEnergy(userId, userBaseEnergy);
            }
        }

        private int GetAdditionalEnergyFromItems(Guid userId)
        {
            List<IItem> items = _userItemRetriever.GetUserItems(userId);
            if (items != null)
            {
                return items.Where(s => s.IsOneTimeUse == false).Select(s => s.Energy).Sum();
            }

            return 0;
        }

        int IUserSightRadiusProvider.GetUserSightRadius(Guid userId, DateTime time)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return 0;
            }

            Tuple<int, DateTime> userSightRadius = _userRetriever.GetLastSavedSightRadius(userId);
            SetBaseSightRadiusIfNeeded(userId);
            int? userBaseSightRadius = _userRetriever.GetCurrentBaseSightRadius(userId);

            userBaseSightRadius += GetAdditionalSightRadiusFromItems(userId);

            if (userSightRadius == null)
            {
                userSightRadius = new Tuple<int, DateTime>(userBaseSightRadius.Value, time);
                _userSaver.SaveLastSightRadius(userId, userBaseSightRadius.Value, time);
            }

            userSightRadius.Item2 = new DateTime(userSightRadius.Item2.Year, userSightRadius.Item2.Month, userSightRadius.Item2.Day, userSightRadius.Item2.Hour, userSightRadius.Item2.Minute, 0);
            DateTime truncatedTime = new DateTime(time.Year, time.Month, time.Day, time.Hour, time.Minute, 0);

            TimeSpan span = truncatedTime - userSightRadius.Item2;
            int totalMinutes = Convert.ToInt32(span.TotalMinutes);
            if (totalMinutes < 0)
            {
                return 0;
            }

            int lastSightRadius = userSightRadius.Item1;
            int calculatedSightRadius = lastSightRadius - totalMinutes;

            if (calculatedSightRadius < userBaseSightRadius.Value)
            {
                calculatedSightRadius = userBaseSightRadius.Value;
            }

            return calculatedSightRadius;
        }

        private int GetAdditionalSightRadiusFromItems(Guid userId)
        {
            List<IItem> items = _userItemRetriever.GetUserItems(userId);
            if (items != null)
            {
                return items.Where(s => s.IsOneTimeUse == false).Select(s => s.Distance).Sum();
            }

            return 0;
        }

        private void SetBaseSightRadiusIfNeeded(Guid userId)
        {
            int? userBaseSightRadius = _userRetriever.GetCurrentBaseSightRadius(userId);
            if (userBaseSightRadius == null)
            {
                userBaseSightRadius = 5;
                _userSaver.SetCurrentBaseSightRadius(userId, userBaseSightRadius.Value);
            }
        }

        int IUserSightRadiusProvider.GetUserMinSightRadius(Guid userId)
        {
            if (_userRetriever.UserExists(userId) == false) return 0;

            SetBaseSightRadiusIfNeeded(userId);
            int? baseSightRadius = _userRetriever.GetCurrentBaseSightRadius(userId);
            return baseSightRadius.Value + GetAdditionalSightRadiusFromItems(userId);
        }
    }
}
