using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.GameSimulation
{
    class Program
    {
        private static IHotZoneRetriever _hotZoneRetriever;
        private static IUser _user;
        private static IUserCreationService _userCreationService;
        private static IUserMoveDirector _userMover;
        private static IUserRetriever _userRetriever;
        private static INodeCreator _nodeCreator;
        private static IUserSaver _userSaver;
        private static IUserEnergyProvider _userEnergyProvider;
        private static IZombiePackRetriever _zombiePackRetriever;
        private static IHuntDirector _huntDirector;
        private static IUserZombiePackProgressRetriever _userZombiePackProgressRetriever;
        private static IUserStatsRetriever _userStatsRetriever;

        static void Main(string[] args)
        {
            ResolveDI();

            SetUpWorld();

            CreateUser();

            HuntWorld();
        }

        private static void ResolveDI()
        {
            _nodeCreator = UndeadEarth.Console.DependancyInjection.Instance.Resolve<INodeCreator>();
            _hotZoneRetriever = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IHotZoneRetriever>();
            _userCreationService = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IUserCreationService>();
            _userRetriever = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IUserRetriever>();
            _userMover = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IUserMoveDirector>();
            _userSaver = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IUserSaver>();
            _userEnergyProvider = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IUserEnergyProvider>();
            _huntDirector = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IHuntDirector>();
            _zombiePackRetriever = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IZombiePackRetriever>();
            _userZombiePackProgressRetriever = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IUserZombiePackProgressRetriever>();
            _userStatsRetriever = UndeadEarth.Controllers.DependancyInjection.Instance.Resolve<IUserStatsRetriever>();

        }

        private static void HuntWorld()
        {
            List<IHotZone> hotZones = _hotZoneRetriever.GetAllHotZones();
            foreach (var hotZone in hotZones)
            {
                MoveToHotZone(hotZone);
                System.Console.WriteLine("Moved to: " + hotZone.Name);
                System.Console.WriteLine();
                System.Console.WriteLine();
                ClearHotZoneByHuntingEachZombie(hotZone);
            }
        }

        private static void ClearHotZoneByHuntingEachZombie(IHotZone hotZone)
        {
            List<IZombiePack> zombiePacks = _zombiePackRetriever.GetAllZombiePacksInHotZone(hotZone.Id);
            int packCount = 0;
            foreach(IZombiePack pack in zombiePacks)
            {
                MoveToZombiePack(pack);

                ClearPack(pack);

                packCount += 1;
                System.Console.WriteLine("Zombie pack " + packCount.ToString() + " destroyed.");
                var stats = _userStatsRetriever.GetStats(_user.Id);

                System.Console.Write("Level: " + stats.Level + ", ");
                System.Console.Write("Zombies Killed: " + stats.ZombiesKilled + ", ");
                System.Console.Write("Money: " + stats.MoneyAccumulated + ", ");
                System.Console.WriteLine("Zones: " + stats.HotZonesDestroyed);
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
        }

        private static void ClearPack(IZombiePack pack)
        {
            int hunts = 1;
            while(_userZombiePackProgressRetriever.IsZombiePackDestroyed(_user.Id, pack.Id) == false)
            {
                _huntDirector.Hunt(_user.Id, pack.Id);

                _user = _userRetriever.GetUserById(_user.Id);

                ReplenishEnergy();
                hunts += 1;
            }

            System.Console.WriteLine("Hunts to destroy: " + hunts);
        }

        private static void MoveToZombiePack(IZombiePack pack)
        {
            while(!IsUserOnZombiePack(pack))
            {
                _userMover.MoveUser(_user.Id, pack.Latitude, pack.Longitude);
                _user = _userRetriever.GetUserById(_user.Id);
                ReplenishEnergy();
            }
        }

        private static void MoveToHotZone(IHotZone hotZone)
        {
            while (!IsUserOnHotZone(hotZone))
            {
                _userMover.MoveUser(_user.Id, hotZone.Latitude, hotZone.Longitude);
                _user = _userRetriever.GetUserById(_user.Id);
                ReplenishEnergy();
            }
        }

        private static void SetUpWorld()
        {
            _nodeCreator.PurgeSafeHouses();
            _nodeCreator.PurgeShops();
            _nodeCreator.PurgeZombiePacks();

            _nodeCreator.CreateZombiePacks();
            _nodeCreator.CreateShops();
            _nodeCreator.CreateSafeHouses();
        }

        private static void CreateUser()
        {
            Guid startingHotZoneId = _hotZoneRetriever.GetStartingHotZones().First().Key;

            long facebookId = DateTime.Now.Ticks;
            _userCreationService.CreateUser(facebookId, "Test User", startingHotZoneId);

            _user = _userRetriever.GetUserByFacebookId(facebookId);
        }

        private static bool IsUserOnHotZone(IHotZone hotZone)
        {
            var user = _userRetriever.GetUserById(_user.Id);
            return hotZone.Latitude == user.Latitude && hotZone.Longitude == user.Longitude;
        }

        private static bool IsUserOnZombiePack(IZombiePack zombiePack)
        {
            var user = _userRetriever.GetUserById(_user.Id);
            return zombiePack.Latitude == user.Latitude && zombiePack.Longitude == user.Longitude;
        }

        private static void ReplenishEnergy()
        {
            _userSaver.SaveLastEnergy(_user.Id, _userEnergyProvider.GetUserMaxEnergy(_user.Id), DateTime.Now);
        }
    }
}
