using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            INodeCreator nodeCreator = DependancyInjection.Instance.Resolve<INodeCreator>();

            nodeCreator.PurgeZombiePacks();
            nodeCreator.CreateZombiePacks();
            System.Console.WriteLine("Zombie hordes created.  Press enter to continue.");
            System.Console.ReadLine();

            nodeCreator.PurgeShops();
            nodeCreator.CreateShops();
            System.Console.WriteLine("Shops created.  Press enter to continue.");
            System.Console.ReadLine();

            nodeCreator.PurgeSafeHouses();
            nodeCreator.CreateSafeHouses();
            System.Console.WriteLine("Safe houses created.  Press enter to continue.");
            System.Console.ReadLine();

        }
    }
}
