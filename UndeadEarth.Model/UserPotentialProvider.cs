using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserPotentialProvider : IUserPotentialProvider
    {
        private IUserRetriever _userRetriever;
        private IItemRetriever _itemRetriever;
        private ISafeHouseRetriever _safeHouseRetriever;
        private IUserItemRetriever _userItemRetriever;

        public UserPotentialProvider(IUserRetriever userRetriever, IItemRetriever itemRetriever, ISafeHouseRetriever safeHouseRetriever, IUserItemRetriever userItemRetriever)
        {
            _userRetriever = userRetriever;
            _itemRetriever = itemRetriever;
            _safeHouseRetriever = safeHouseRetriever;
            _userItemRetriever = userItemRetriever;
        }

        private List<IItem> ShopItems(Guid userId, int moneyForUser, Func<IItem, int> orderBy, int maxItemsForCarry)
        {
            List<IItem> shopItems = new List<IItem>();
            List<IItem> items = _itemRetriever.GetAllBelowPrice(moneyForUser);

            do
            {
                items = items.Where(i => i.Price <= moneyForUser && i.IsOneTimeUse == false)
                             .OrderByDescending(orderBy)
                             .ThenBy(i => i.Price)
                             .ToList();

                if (items.Count > 0)
                {
                    moneyForUser -= items.First().Price;
                    shopItems.Add(items.First());
                }

            } while (items.Count > 0 && shopItems.Count <= maxItemsForCarry);

            return shopItems;
        }

        private List<IItem> AllItems(Guid userId, int money, Func<IItem, int> orderBy, int itemLimit)
        {
            List<IItem> allItems = new List<IItem>();
            allItems.AddRange(_userItemRetriever.GetUserItems(userId).Where(s => s.IsOneTimeUse == false));
            allItems.AddRange(_safeHouseRetriever.GetItems(userId).Where(s => s.IsOneTimeUse == false));
            allItems.AddRange(ShopItems(userId, money, orderBy, itemLimit));

            return allItems;
        }

        private int FindOptimum(Guid userId, int baseLine, int money, int itemLimit, Func<IItem, int> propertyFilter)
        {
            return AllItems(userId, money, propertyFilter, itemLimit).OrderByDescending(propertyFilter)
                                                          .Take(itemLimit)
                                                          .Sum(propertyFilter) + baseLine;
        }

        int IUserPotentialProvider.GetMaxPotentialEnergy(Guid userId)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return 0;
            }

            int moneyForUser = _userRetriever.GetCurrentMoney(userId);
            int maxItemsUserCanCarry = _userRetriever.GetCurrentBaseSlots(userId);
            int baseEnergy = _userRetriever.GetEnergyForDifficultyCalculation(userId);

            int limitedItems = CalcLimitedItems(maxItemsUserCanCarry);

            return FindOptimum(userId, baseEnergy, moneyForUser, limitedItems, i => i.Energy);
        }

        int IUserPotentialProvider.GetMaxPotentialAttackPower(Guid userId)
        {
            if (_userRetriever.UserExists(userId) == false)
            {
                return 0;
            }

            int moneyForUser = _userRetriever.GetCurrentMoney(userId);
            int maxItemsUserCanCarry = _userRetriever.GetCurrentBaseSlots(userId);
            int baseAttack = _userRetriever.GetAttackPowerForDifficultyCalculation(userId);

            int limitedItems = CalcLimitedItems(maxItemsUserCanCarry);

            return FindOptimum(userId, baseAttack, moneyForUser, limitedItems, i => i.Attack);
        }

        private static int CalcLimitedItems(int maxItemsUserCanCarry)
        {
            int minusAFewForOneTimeUse = Convert.ToInt32((double)maxItemsUserCanCarry - ((double)maxItemsUserCanCarry * .2));

            int limitedItems = Convert.ToInt32((double)minusAFewForOneTimeUse / (double)2);
            return limitedItems;
        }
    }
}
