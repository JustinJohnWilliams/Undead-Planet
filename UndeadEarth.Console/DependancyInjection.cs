using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;
using UndeadEarth.Contract;
using System.Configuration;
using UndeadEarth.Model;

namespace UndeadEarth.Console
{
    public class DependancyInjection
    {
        private static IUnityContainer _instance;
        public static IUnityContainer Instance
        {
            get 
            {
                if(_instance == null)
                {
                    _instance = BuildContainer();
                }

                return _instance;
            }
        }

        private static IUnityContainer BuildContainer()
        {
            IUnityContainer container = new UnityContainer();

            string connectionString = ConfigurationManager.ConnectionStrings["UndeadEarthConnectionString"].ConnectionString;


            container.RegisterType<IDistanceCalculator, DistanceCalculator>();
            container.RegisterType<IRandomNumberProvider, RandomNumberProvider>();

            InjectionConstructor nodesContrsuctor =
                new InjectionConstructor(connectionString, typeof(IDistanceCalculator), typeof(IRandomNumberProvider));

            container.RegisterType<INodeCreator, NodeCreator>()
                .Configure<InjectedMembers>()
                .ConfigureInjectionFor<NodeCreator>(nodesContrsuctor);

            return container;
        }

    }
}
