using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class HuntDirector : IHuntDirector
    {
        private IUserRetriever _userRetriever;
        private IZombiePackRetriever _zombiePackRetriever;
        private IUserEnergyProvider _userEnergyProvider;
        private IUserSaver _userSaver;
        private IUserZombiePackProgressSaver _userZombiePackProgressSaver;
        private IUserAttackPowerProvider _userAttackPowerProvider;
        private IZombiePackDifficultyDirector _zombiePackDifficultyDirector;
        private IRandomNumberProvider _randomNumberProvider;
        private IUserLevelService _userLevelService;
        private IUserCountsSaver _userCountsSaver;
        private IHotZoneRetriever _hotZoneRetriever;

        public HuntDirector(IUserRetriever userRetriever,
            IZombiePackRetriever zombiePackRetriever,
            IUserEnergyProvider userEnergyProvider,
            IUserSaver userSaver,
            IUserZombiePackProgressSaver userZombiePackProgressSaver,
            IUserAttackPowerProvider userAttackPowerProvider,
            IZombiePackDifficultyDirector zombiePackDifficultyDirector,
            IRandomNumberProvider randomNumberProvider,
            IUserLevelService userLevelService,
            IUserCountsSaver userCountsSaver,
            IHotZoneRetriever hotZoneRetriever)
        {
            _userRetriever = userRetriever;
            _zombiePackRetriever = zombiePackRetriever;
            _userEnergyProvider = userEnergyProvider;
            _userSaver = userSaver;
            _userZombiePackProgressSaver = userZombiePackProgressSaver;
            _userAttackPowerProvider = userAttackPowerProvider;
            _zombiePackDifficultyDirector = zombiePackDifficultyDirector;
            _randomNumberProvider = randomNumberProvider;
            _userLevelService = userLevelService;
            _userCountsSaver = userCountsSaver;
            _hotZoneRetriever = hotZoneRetriever;
        }


        void IHuntDirector.Hunt(Guid userId, Guid zombiePackId)
        {
            //hunt can only occur if user exists
            if (_userRetriever.UserExists(userId) == false)
            {
                return;
            }

            //and if the zombie pack existss
            if (_zombiePackRetriever.ZombiePackExists(zombiePackId) == false)
            {
                return;
            }

            //and if the user's current location is the location of the zombie pack
            IUser user = _userRetriever.GetUserById(userId);
            IZombiePack zombiePack = _zombiePackRetriever.GetZombiePackById(zombiePackId);

            if (user.Latitude != zombiePack.Latitude || user.Longitude != zombiePack.Longitude)
            {
                return;
            }

            //if the user is indeed at the given zombie pack, make sure to update the users zone id to 
            //be the hotzone the user is located in
            _userSaver.UpdateZone(userId, _zombiePackRetriever.GetHotZoneByZombiePackId(zombiePackId));

            //and if the user has enough energy to hunt
            int userEnergy = _userEnergyProvider.GetUserEnergy(userId, DateTime.Now);
            int energyRequiredToHunt = _zombiePackDifficultyDirector.GetEnergyCost(userId, zombiePackId);
            if (userEnergy < energyRequiredToHunt)
            {
                return;
            }

            //update the progress for the hunt
            IUserZombiePackProgress progress = _zombiePackDifficultyDirector.GetCalculatedZombiePackProgress(userId, zombiePackId);

            if (progress.ZombiesLeft == 0)
            {
                return;
            }

            //update the user's energy
            _userSaver.SaveLastEnergy(userId, userEnergy - energyRequiredToHunt, DateTime.Now);

            int userAttackPower = _userAttackPowerProvider.GetAttackPower(userId);

            UserZombiePackProgress newProgress = new UserZombiePackProgress
            {
                ZombiesLeft = progress.ZombiesLeft - userAttackPower,
                MaxZombies = progress.MaxZombies,
                IsDestroyed = false
            };

            if (newProgress.ZombiesLeft < 0)
            {
                newProgress.ZombiesLeft = 0;
            }

            if (newProgress.ZombiesLeft == 0)
            {
                newProgress.IsDestroyed = true;
            }

            _userZombiePackProgressSaver.SaveZombiePackProgress(userId, zombiePackId, newProgress);
            _userSaver.UpdateLastVisitedHotZone(userId, _zombiePackRetriever.GetHotZoneByZombiePackId(zombiePackId));
            
            int zombiesKilled = userAttackPower;
            if(newProgress.IsDestroyed == true)
            {
                zombiesKilled = progress.ZombiesLeft;
            }

            _userCountsSaver.AddZombiesKilled(userId, zombiesKilled);

            GiveUserArbitraryMoney(userId, zombiesKilled, newProgress);

            Guid hotZoneId = _zombiePackRetriever.GetHotZoneByZombiePackId(zombiePackId);
            if (_hotZoneRetriever.ZombiePacksLeft(userId, hotZoneId) == 0)
            {
                _userCountsSaver.AddHotZonesDestroyed(userId, 1);
            }

            if (newProgress.IsDestroyed == true)
            {
                _userCountsSaver.AddZombiePacksDestroyed(userId, 1);
                GiveMoneyReward(userId);
            }

            _userCountsSaver.RecordPeakZombiesDestroyed(userId, zombiesKilled);
            _userLevelService.CheckForLevelUp(userId);
        }

        private void GiveUserArbitraryMoney(Guid userId, int zombiesKilled, UserZombiePackProgress newProgress)
        {
            //after hunt is complete, user should get some arbitrary amount of money
            int moneyForHunt = zombiesKilled;
            _userSaver.AddMoney(userId, moneyForHunt);
            _userCountsSaver.AddMoney(userId, moneyForHunt);
        }

        private void GiveMoneyReward(Guid userId)
        {
            _userSaver.AddMoney(userId, 100);
            _userCountsSaver.AddMoney(userId, 100);
        }
    }
}