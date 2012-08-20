using System;
using Microsoft.Practices.Unity;
using System.Configuration;
using UndeadEarth.Contract;
using UndeadEarth.Model;
using UndeadEarth.Dal;

namespace UndeadEarth.Controllers
{
    public class DependancyInjection
    {
        private static IUnityContainer _instance;
        public static IUnityContainer Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = BuildContainer();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        private static IUnityContainer BuildContainer()
        {
            IUnityContainer container = new UnityContainer();

            string connectionString = ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString;
            InjectionConstructor connectionStringConstructor = new InjectionConstructor(connectionString);

            container.RegisterType<IDistanceCalculator, DistanceCalculator>();

            InjectionConstructor zombiePackConstructor =
                new InjectionConstructor(connectionString, typeof(IDistanceCalculator));

            InjectionConstructor storesConstructor =
                new InjectionConstructor(connectionString, typeof(IDistanceCalculator));

            InjectionConstructor safeHouseConstructor =
                new InjectionConstructor(connectionString, typeof(IDistanceCalculator));

            container.RegisterType<IZombiePackRetriever, ZombiePackRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ZombiePackRepository>(zombiePackConstructor);

            container.RegisterType<IUserRetriever, UserRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<UserRepository>(connectionStringConstructor);

            container.RegisterType<IUserSaver, UserRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<UserRepository>(connectionStringConstructor);

            container.RegisterType<IHotZoneRetriever, HotZoneRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<HotZoneRepository>(connectionStringConstructor);

            container.RegisterType<IUserZombiePackProgressRetriever, UserZombiePackProgressRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<UserZombiePackProgressRepository>(connectionStringConstructor);

            container.RegisterType<IUserZombiePackProgressSaver, UserZombiePackProgressRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<UserZombiePackProgressRepository>(connectionStringConstructor);

            container.RegisterType<IStoreRetriever, StoreRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<StoreRepository>(storesConstructor);

            container.RegisterType<IItemRetriever, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            container.RegisterType<IUserItemRetriever, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            container.RegisterType<IUserItemSaver, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            container.RegisterType<ISafeHouseRetriever, SafeHouseRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<SafeHouseRepository>(safeHouseConstructor);

            container.RegisterType<ISafeHouseItemSaver, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>();

            container.RegisterType<IUserCountsSaver, UserCountsRepository>()
                     .Configure<InjectedMembers>()
                     .ConfigureInjectionFor<UserCountsRepository>(connectionStringConstructor);

            container.RegisterType<IUserStatsRetriever, UserCountsRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<UserCountsRepository>(connectionStringConstructor);

            InjectionConstructor zombieCalculatorConstructor =
                new InjectionConstructor(typeof(IUserZombiePackProgressSaver));

            container.RegisterType<IUserEnergyProvider, UserEnergyAndSightProvider>();
            container.RegisterType<IUserMoveDirector, UserMoveDirector>();
            container.RegisterType<IHuntDirector, HuntDirector>();
            container.RegisterType<IShopDirector, ShopDirector>();
            container.RegisterType<ISafeHouseDirector, SafeHouseDirector>();
            container.RegisterType<IUserAttackPowerProvider, UserAttackPowerProvider>();
            container.RegisterType<IZombiePackDifficultyDirector, ZombiePackDifficultyDirector>();
            container.RegisterType<IUserPotentialProvider, UserPotentialProvider>();
            container.RegisterType<IRandomNumberProvider, RandomNumberProvider>();
            container.RegisterType<IUserSightRadiusProvider, UserEnergyAndSightProvider>();
            container.RegisterType<IItemUsageDirector, ItemUsageDirector>();
            container.RegisterType<IUserLevelService, UserLevelService>();
            container.RegisterType<IAchievementProvider, AchievementProvider>();
            container.RegisterType<IUserCreationService, UserCreationService>();

            return container;
        }
    }
}
