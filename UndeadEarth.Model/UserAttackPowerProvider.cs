using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Model
{
    public class UserAttackPowerProvider : IUserAttackPowerProvider
    {
        private IUserRetriever _userRetriever;
        private IUserItemRetriever _userItemRetriever;
        public UserAttackPowerProvider(IUserRetriever userRetriever, IUserItemRetriever userItemRetriever)
        {
            _userRetriever = userRetriever;
            _userItemRetriever = userItemRetriever;
        }

        int IUserAttackPowerProvider.GetAttackPower(Guid userId)
        {
            int userBaseAttackPower = _userRetriever.GetCurrentBaseAttackPower(userId);
            if (_userRetriever.UserExists(userId) == false)
            {
                return 0;
            }

            List<IItem> items = _userItemRetriever.GetUserItems(userId);
            if (items.Count == 0)
            {
                return userBaseAttackPower;
            }

            return items.Sum(s => s.Attack) + userBaseAttackPower;
        }
    }
}
