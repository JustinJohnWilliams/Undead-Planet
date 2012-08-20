using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using System.Configuration;
using UndeadEarth.Contract;
using UndeadEarth.Dal;
using UndeadEarth.Model;

namespace UndeadEarth.Test.Dal
{
    public class DalTestContextSpecification
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

        private static string _connectionString;
        public static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = ConfigurationManager.ConnectionStrings["TestConnectionString"].ConnectionString;
                }

                return _connectionString;
            }
        }

        private static IUnityContainer BuildContainer()
        {
            IUnityContainer container = new UnityContainer();

            string connectionString = ConfigurationManager.ConnectionStrings["TestConnectionString"].ConnectionString;

            InjectionConstructor connectionStringConstructor = new InjectionConstructor(connectionString);

            container.RegisterType<IDistanceCalculator, DistanceCalculator>();

            InjectionConstructor zombiePackConstructor =
                new InjectionConstructor(connectionString, typeof(IDistanceCalculator));

            InjectionConstructor storeConstructor =
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

            container.RegisterType<IItemRetriever, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            container.RegisterType<IUserItemRetriever, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            container.RegisterType<IUserItemSaver, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            container.RegisterType<IStoreRetriever, StoreRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<StoreRepository>(storeConstructor);

            container.RegisterType<ISafeHouseRetriever, SafeHouseRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<SafeHouseRepository>(safeHouseConstructor);

            container.RegisterType<ISafeHouseItemSaver, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            container.RegisterType<IUserItemSaver, ItemRepository>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<ItemRepository>(connectionStringConstructor);

            InjectionConstructor zombieCalculatorConstructor =
                new InjectionConstructor(typeof(IUserZombiePackProgressSaver));

            return container;
        }
    }
}
