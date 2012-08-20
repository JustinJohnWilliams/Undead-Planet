using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UndeadEarth.Test.Model
{
    public class UserLevelService : IUserLevelService
    {
        private IUserSaver
        public UserLevelService()
        {
            
        }

        void IUserLevelService.RecordZombieKillsForUser(Guid userId, int zombiesKilled)
        {
            
        }
    }

    public class when_determining_if_user_gains_level
    {
        private IUserLevelService _userLevelService;
        private Mock<IUserSaver> _userSaver;

        /// <summary>
        /// Initializes a new instance of the when_determining_if_user_gains_level class.
        /// </summary>
        public when_determining_if_user_gains_level()
        {
            _userLevelService = new UserLevelService();
            _userSaver = new Mock<IUserSaver>();
        }

        [TestMethod]
        public void should_increment_level_if_zombie_kill_count_is_met()
        {
            _userLevelService.RecordZombieKillsForUser(Guid.NewGuid(), 100);

            
        }
    }
}
