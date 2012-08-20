using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class UserZombiePackProgressRepository : IUserZombiePackProgressRetriever, IUserZombiePackProgressSaver
    {
        private string _connectionString;

        public UserZombiePackProgressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region "IUserZombiePackProgress Implementation"

        IUserZombiePackProgress IUserZombiePackProgressRetriever.GetUserZombiePackProgressFor(Guid userId, Guid zombiePackId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.GetUserZombiePackProgressByUserIdAndZombiePackId(userId, zombiePackId);
            }
        }

        bool IUserZombiePackProgressRetriever.IsZombiePackDestroyed(Guid userId, Guid zombiePackId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserZombiePackProgressDto dto = dataContext.GetUserZombiePackProgressByUserIdAndZombiePackId(userId, zombiePackId);

                if (dto != null)
                {
                    return dto.IsDestroyed;
                }

                return false;
            }
        }

        #endregion

        #region "IUserZombiePackProgressSaver Implementation"

        void IUserZombiePackProgressSaver.SaveZombiePackProgress(Guid userId, Guid zombiePackId, IUserZombiePackProgress progress)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                DateTime dateTime = dataContext.GetDate();
                UserZombiePackProgressDto userZombieProgressDto;
                userZombieProgressDto = dataContext.GetUserZombiePackProgressByUserIdAndZombiePackId(userId, zombiePackId);

                if (userZombieProgressDto == null)
                {
                    userZombieProgressDto = new UserZombiePackProgressDto
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
                        ZombiePackId = zombiePackId,
                        LastHuntDate = DateTime.Now,
                        LastRegen = DateTime.Now
                    };

                    dataContext.UserZombiePackProgressDtos.InsertOnSubmit(userZombieProgressDto);

                }

                userZombieProgressDto.IsDestroyed = progress.IsDestroyed;
                userZombieProgressDto.MaxZombies = progress.MaxZombies;
                userZombieProgressDto.ZombiesLeft = progress.ZombiesLeft;
                dataContext.SubmitChanges();
            }
        }

        #endregion
    }
}
