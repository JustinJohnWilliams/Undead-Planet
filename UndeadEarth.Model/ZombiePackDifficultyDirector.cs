using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class ZombiePackDifficultyDirector : IZombiePackDifficultyDirector
    {
        private IUserRetriever _userRetriever;
        private IZombiePackRetriever _zombiePackRetriever;
        private IUserZombiePackProgressRetriever _userZombiePackProgressRetriever;
        private IUserSaver _userSaver;
        private IRandomNumberProvider _randomNumberProvider;
        private IUserPotentialProvider _userPotentialProvider;
        private IUserZombiePackProgressSaver _userZombiePackProgressSaver;
        private IUserStatsRetriever _userStatsRetriever;

        public ZombiePackDifficultyDirector(IUserRetriever userRetriever,
            IZombiePackRetriever zombiePackRetriever,
            IUserZombiePackProgressRetriever userZombiePackRetriever,
            IUserSaver userSaver,
            IRandomNumberProvider randomNumberProvider,
            IUserPotentialProvider userPotentialProvider,
            IUserZombiePackProgressSaver userZombiePackProgressSaver,
            IUserStatsRetriever userStatsRetriever)
        {
            _userRetriever = userRetriever;
            _zombiePackRetriever = zombiePackRetriever;
            _userZombiePackProgressRetriever = userZombiePackRetriever;
            _userSaver = userSaver;
            _randomNumberProvider = randomNumberProvider;
            _userPotentialProvider = userPotentialProvider;
            _userZombiePackProgressSaver = userZombiePackProgressSaver;
            _userStatsRetriever = userStatsRetriever;
        }

        IUserZombiePackProgress IZombiePackDifficultyDirector.GetCalculatedZombiePackProgress(Guid userId, Guid zombiePackId)
        {
            UserZombiePackProgress progress = new UserZombiePackProgress
            {
                IsDestroyed = false,
                ZombiesLeft = 0,
                MaxZombies = 0
            };

            if (_userRetriever.UserExists(userId) == false)
            {
                return progress;
            }

            if (_zombiePackRetriever.ZombiePackExists(zombiePackId) == false)
            {
                return progress;
            }

            IUserZombiePackProgress progressFromRepository = _userZombiePackProgressRetriever.GetUserZombiePackProgressFor(userId, zombiePackId);
            if (progressFromRepository != null)
            {
                return progressFromRepository;
            }

            DetermineBaseLineRecalc(userId, zombiePackId);

            int baselineAttack = _userRetriever.GetAttackPowerForDifficultyCalculation(userId);
            int numberOfHunts = GetNumberOfHuntsToDestroy();
            int zombieCount = baselineAttack * numberOfHunts;
            progress = new UserZombiePackProgress
            {
                IsDestroyed = false,
                ZombiesLeft = zombieCount,
                MaxZombies = zombieCount
            };

            _userZombiePackProgressSaver.SaveZombiePackProgress(userId, zombiePackId, progress);

            return progress;
        }

        int IZombiePackDifficultyDirector.GetEnergyCost(Guid userId, Guid zombiePackId)
        {
            DetermineBaseLineRecalc(userId, zombiePackId);
            return Convert.ToInt32(_userRetriever.GetEnergyForDifficultyCalculation(userId) * .05);
        }

        private int GetNumberOfHuntsToDestroy()
        {
            int difficulty = _randomNumberProvider.GetRandomInclusive(1, 4);
            if (difficulty == 1)
            {
                return _randomNumberProvider.GetRandomInclusive(1, 4);
            }
            else if (difficulty == 2)
            {
                return _randomNumberProvider.GetRandomInclusive(5, 10);
            }
            else if (difficulty == 3)
            {
                return _randomNumberProvider.GetRandomInclusive(11, 15);
            }
            else
            {
                return _randomNumberProvider.GetRandomInclusive(16, 20);
            }
        }

        private void DetermineBaseLineRecalc(Guid userId, Guid zombiePackId)
        {
            Guid? lastVisitedHotZoneForUser = _userRetriever.GetLastVisitedHotZone(userId);
            Guid hotzoneAssociatedWithZombiePack = _zombiePackRetriever.GetHotZoneByZombiePackId(zombiePackId);


            if ((lastVisitedHotZoneForUser != hotzoneAssociatedWithZombiePack && _userStatsRetriever.GetStats(userId).HotZonesDestroyed % 3 == 0) || lastVisitedHotZoneForUser == null)
            {
                int maxAttackPower = _userPotentialProvider.GetMaxPotentialAttackPower(userId);
                int maxEnergy = _userPotentialProvider.GetMaxPotentialEnergy(userId) ;

                _userSaver.UpdateBaseLine(userId, maxAttackPower, maxEnergy);
            }
        }
    }
}
