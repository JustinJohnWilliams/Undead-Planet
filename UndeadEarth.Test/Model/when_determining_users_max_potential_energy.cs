using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using Moq;
using UndeadEarth.Model;
using UndeadEarth.Test.Model.Spec;

namespace UndeadEarth.Test.Model
{
    /// <summary>
    /// Summary description for when_determining_users_max_potential_energy
    /// </summary>
    [TestClass]
    public class when_determining_users_max_potential_energy : max_potential_spec
    {
        public class Item : IItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int Price { get; set; }
            public int Energy { get; set; }
            public int Distance { get; set; }
            public int Attack { get; set; }
            public bool IsOneTimeUse { get; set; }
        }

        public when_determining_users_max_potential_energy()
        {
            
        }

        [TestMethod]
        public void should_return_0_for_energy_if_user_does_not_exist()
        {
            _userRetriever.Setup(s => s.UserExists(_userId)).Returns(false).Verifiable();
            TheEnergy().ShouldBe(0);
        }

        [TestMethod]
        public void should_return_base_energy_if_user_does_not_have_any_money_and_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseEnergy(100);

            TheEnergy().ShouldBe(100);
        }

        [TestMethod]
        public void should_return_energy_along_with_any_energy_increase_from_buying_item_with_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseEnergy(100);
            GivenMoney(100);
            GivenShopItem(new Item { Energy = 15, Price = 100 });
            GivenAvailableItemSlots(1);

            TheEnergy().ShouldBe(100 + 15);
        }

        [TestMethod]
        public void should_return_energy_along_with_any_energy_increase_from_buying_many_of_the_same_item_with_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseEnergy(100);
            GivenMoney(3);
            GivenAvailableItemSlots(3);
            GivenShopItem(new Item { Energy = 15, Price = 1 });


            TheEnergy().ShouldBe(100 + 15 + 15 + 15);
        }

        [TestMethod]
        public void should_return_energy_along_with_any_energy_increase_from_buying_different_items_and_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseEnergy(100);
            GivenMoney(111);
            GivenAvailableItemSlots(3);

            GivenShopItem(new Item { Energy = 3, Price = 100 })
                .AndItem(new Item { Energy = 2, Price = 10 })
                .AndItem(new Item { Energy = 1, Price = 1 });

            TheEnergy().ShouldBe(100 + 1 + 2 + 3);
        }

        [TestMethod]
        public void should_return_energy_based_on_items_in_safehouse_with_user_not_having_any_money()
        {
            GivenUserExists();
            GivenBaseEnergy(100);
            GivenMoney(0);
            GivenAvailableItemSlots(3);

            GivenSafeHouseItem(new Item { Energy = 3 })
                .AndItem(new Item { Energy = 2 })
                .AndItem(new Item { Energy = 1 });

            TheEnergy().ShouldBe(100 + 1 + 2 + 3);
        }

        [TestMethod]
        public void should_return_energy_along_with_any_energy_increase_from_many_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseEnergy(100);
            GivenMoney(0);
            GivenAvailableItemSlots(10);

            for (int i = 0; i < 100; i++)
            {
                GivenSafeHouseItem(new Item { Energy = 1 });
            }

            TheEnergy().ShouldBe(100 + 10);
        }

        [TestMethod]
        public void given_user_has_money_and_items_in_safe_house_and_can_buy_items_should_return_the_energy_that_is_greater_between_safehouse_items_and_store_items()
        {
            GivenUserExists();
            GivenMoney(4);
            GivenBaseEnergy(100);
            GivenAvailableItemSlots(5);
            GivenShopItem(new Item { Energy = 1, Price = 1 });
            GivenSafeHouseItem(new Item { Energy = 4 })
                .AndItem(new Item { Energy = 3 })
                .AndItem(new Item { Energy = 2 });

            TheEnergy().ShouldBe(100 + 4 + 3 + 2 + 1 + 1);
        }

        [TestMethod]
        public void should_disregard_safehouse_items_that_are_one_use()
        {
            GivenUserExists();
            GivenMoney(4);
            GivenBaseEnergy(100);
            GivenAvailableItemSlots(5);
            GivenShopItem(new Item { Energy = 1, Price = 1 });
            GivenSafeHouseItem(new Item { Energy = 4, IsOneTimeUse = true })
                .AndItem(new Item { Energy = 3, IsOneTimeUse = true })
                .AndItem(new Item { Energy = 2, IsOneTimeUse = true });

            TheEnergy().ShouldBe(100 + 1 + 1 + 1 + 1);
        }

        [TestMethod]
        public void should_disregard_one_use_items_that_can_be_bought()
        {
            GivenUserExists();
            GivenMoney(4);
            GivenBaseEnergy(100);
            GivenAvailableItemSlots(5);
            GivenShopItem(new Item { Energy = 1, Price = 1, IsOneTimeUse = true });
            GivenSafeHouseItem(new Item { Energy = 4 })
                .AndItem(new Item { Energy = 3 })
                .AndItem(new Item { Energy = 2 });

            TheEnergy().ShouldBe(100 + 4 + 3 + 2);
        }

        [TestMethod]
        public void should_give_energy_based_on_best_items_from_user_usersafehouse_and_items_that_can_be_bought()
        {
            GivenUserExists();
            GivenMoney(111);
            GivenAvailableItemSlots(6);
            GivenBaseEnergy(5);

            GivenShopItem(new Item { Energy = 10, Price = 100 })
                .AndItem(new Item { Energy = 9, Price = 10 })
                .AndItem(new Item { Energy = 8, Price = 1 })
                .AndItem(new Item { Energy = 1, Price = 111 });

            GivenSafeHouseItem(new Item { Energy = 2 })
                .AndItem(new Item { Energy = 2 });
            
            GivenUserItem(new Item { Energy = 12 })
                .AndItem(new Item { Energy = 1 });

            TheEnergy().ShouldBe(5 + 12 + 10 + 9 + 8 + 2 + 2);
        }

        public int TheEnergy()
        {
            return _userPotentialProvider.GetMaxPotentialEnergy(_userId);
        }
    }

    public static class when_determining_users_max_potential_energy_extensions
    {
        
    }
}
