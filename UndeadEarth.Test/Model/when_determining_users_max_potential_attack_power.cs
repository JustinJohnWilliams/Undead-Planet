using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UndeadEarth.Contract;
using UndeadEarth.Model;
using Moq;
using UndeadEarth.Test.Model.Spec;

namespace UndeadEarth.Test.Model
{
    [TestClass]
    public class when_determining_users_max_potential_attack_power : max_potential_spec
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

        public when_determining_users_max_potential_attack_power()
        {

        }

        [TestMethod]
        public void should_return_0_for_attack_power_if_user_does_not_exist()
        {
            Guid userId = Guid.NewGuid();
            _userRetriever.Setup(s => s.UserExists(userId)).Returns(false).Verifiable();
            int attackPower = _userPotentialProvider.GetMaxPotentialAttackPower(userId);
            Assert.AreEqual(0, attackPower);
            _userRetriever.Verify();
        }

        [TestMethod]
        public void should_return_base_attack_power_if_user_does_not_have_any_money_and_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseAttack(10);

            TheAttackPower().ShouldBe(10);
        }

        [TestMethod]
        public void should_return_attack_power_along_with_any_attack_power_increase_from_buying_item_with_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseAttack(10);
            GivenMoney(100);
            GivenAvailableItemSlots(1);
            GivenShopItem(new Item { Price = 100, Attack = 15 });

            TheAttackPower().ShouldBe(10 + 15);
        }

        [TestMethod]
        public void should_return_attack_power_along_with_any_attack_power_increase_from_buying_many_of_the_same_item_with_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseAttack(10);
            GivenMoney(100);
            GivenAvailableItemSlots(9);
            GivenShopItem(new Item { Price = 1, Attack = 1 });

            TheAttackPower().ShouldBe(19);
        }

        [TestMethod]
        public void should_return_attack_power_along_with_any_attack_power_increase_from_buying_different_items_and_no_items_in_safehouse()
        {
            GivenUserExists();
            GivenBaseAttack(10);
            GivenMoney(111);
            GivenAvailableItemSlots(10);
            GivenShopItem(new Item { Price = 100, Attack = 3 })
                .AndItem(new Item { Price = 10, Attack = 2 })
                .AndItem(new Item { Price = 1, Attack = 1 });

            TheAttackPower().ShouldBe(10 + 3 + 2 + 1);
        }

        [TestMethod]
        public void should_return_attack_power_based_on_items_in_safehouse_with_user_not_having_any_money()
        {
            GivenUserExists();
            GivenBaseAttack(5);
            GivenAvailableItemSlots(3);
            GivenMoney(0);
            GivenAvailableItemSlots(5);
            GivenShopItem(new Item { Price = 1, Attack = 100 });
            GivenSafeHouseItem(new Item { Attack = 10 })
                .AndItem(new Item { Attack = 15 });

            TheAttackPower().ShouldBe(5 + 10 + 15);
        }

        [TestMethod]
        public void should_return_attack_power_along_with_any_attack_power_increase_from_multiple_safehouse_items_and_user_has_no_money()
        {
            GivenUserExists();
            GivenBaseAttack(5);
            GivenAvailableItemSlots(3);
            GivenMoney(0);
            GivenAvailableItemSlots(5);

            GivenShopItem(new Item { Price = 1, Attack = 100 });

            for (int i = 0; i < 100; i++)
            {
                GivenSafeHouseItem(new Item { Attack = 1 });
            }

            TheAttackPower().ShouldBe(5 + 1 + 1 + 1 + 1 + 1);
        }

        [TestMethod]
        public void given_user_has_money_and_items_in_safe_house_and_can_buy_items_should_return_best_combination()
        {
            GivenUserExists();
            GivenMoney(111);
            GivenAvailableItemSlots(6);
            GivenBaseAttack(5);

            GivenShopItem(new Item { Attack = 10, Price = 100 })
                .AndItem(new Item { Attack = 9, Price = 10 })
                .AndItem(new Item { Attack = 8, Price = 1 })
                .AndItem(new Item { Attack = 1, Price = 111 });

            GivenSafeHouseItem(new Item { Attack = 2 })
                .AndItem(new Item { Attack = 2 });

            GivenUserItem(new Item { Attack = 12 })
                .AndItem(new Item { Attack = 1 });

            TheAttackPower().ShouldBe(5 + 12 + 10 + 9 + 8 + 2 + 2);
        }

        [TestMethod]
        public void should_disregard_safehouse_items_that_are_one_use()
        {
            GivenUserExists();
            GivenMoney(4);
            GivenBaseAttack(100);
            GivenAvailableItemSlots(5);
            GivenShopItem(new Item { Attack = 1, Price = 1 });
            GivenSafeHouseItem(new Item { Attack = 4, IsOneTimeUse = true })
                .AndItem(new Item { Attack = 3, IsOneTimeUse = true })
                .AndItem(new Item { Attack = 2, IsOneTimeUse = true });

            TheAttackPower().ShouldBe(100 + 1 + 1 + 1 + 1);
        }

        [TestMethod]
        public void should_disregard_one_use_items_that_can_be_bought()
        {
            GivenUserExists();
            GivenMoney(4);
            GivenBaseAttack(100);
            GivenAvailableItemSlots(5);
            GivenShopItem(new Item { Attack = 1, Price = 1, IsOneTimeUse = true });
            GivenSafeHouseItem(new Item { Attack = 4 })
                .AndItem(new Item { Attack = 3 })
                .AndItem(new Item { Attack = 2 });

            TheAttackPower().ShouldBe(100 + 4 + 3 + 2);
        }

        public int TheAttackPower()
        {
            return _userPotentialProvider.GetMaxPotentialAttackPower(_userId);
        }
    }
}
