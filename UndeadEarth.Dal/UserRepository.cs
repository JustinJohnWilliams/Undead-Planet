using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using UndeadEarth.Dal.Database;

namespace UndeadEarth.Dal
{
    public class UserRepository : IUserRetriever, IUserSaver
    {
        private string _connectionString;
        
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region "IUserRetriever Implementation"

        IUser IUserRetriever.GetUserById(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.GetUserById(userId);
            }
        }

        Tuple<int, DateTime> IUserRetriever.GetLastSavedEnergy(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();
                var dto = dataContext.UserDtos.SingleOrDefault(s => s.Id == userId);
                if (dto != null)
                {
                    if (dto.LastEnergy == null)
                    {
                        return null;
                    }

                    if (dto.LastEnergyDate == null)
                    {
                        dto.LastEnergyDate = DateTime.Now;
                    }

                    return new Tuple<int, DateTime>(dto.LastEnergy.Value, dto.LastEnergyDate.Value);
                }
            }

            return null;
        }

        bool IUserRetriever.UserExists(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                return dataContext.UserExists(userId);
            }
        }

        int IUserRetriever.GetCurrentMoney(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                IUser user = dataContext.GetUserById(userId);
                return user.Money;
            }
        }

        Guid? IUserRetriever.GetLastVisitedHotZone(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserDto user = dataContext.GetUserById(userId);
                return user.LastVisitedHotZoneId;
            }
        }

        int IUserRetriever.GetAttackPowerForDifficultyCalculation(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserDto user = dataContext.GetUserById(userId);
                return user.BaseLineAttackPower;
            }
        }

        int IUserRetriever.GetEnergyForDifficultyCalculation(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserDto user = dataContext.GetUserById(userId);
                return user.BaseLineEnergy;
            }
        }

        Tuple<int, DateTime> IUserRetriever.GetLastSavedSightRadius(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserDto dto = dataContext.GetUserById(userId);
                if (dto != null)
                {
                    if (dto.LastSightRadius == null)
                    {
                        return null;
                    }

                    if (dto.LastSightRadiusDate == null)
                    {
                        dto.LastSightRadiusDate = DateTime.Now;
                    }

                    return new Tuple<int, DateTime>(dto.LastSightRadius.Value, dto.LastSightRadiusDate.Value);
                }
            }

            return null;
        }

        int? IUserRetriever.GetCurrentBaseSightRadius(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserDto dto = dataContext.GetUserById(userId);

                if (dto != null)
                {
                    return dto.BaseSightRadius;
                }
            }

            return null;
        }

        int IUserRetriever.GetCurrentLevel(Guid userId)
        {
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                undeadEarthDataContext.ReadUncommited();

                UserDto userDto = undeadEarthDataContext.GetUserById(userId);
                return userDto.Level;
            }
        }

        int IUserRetriever.GetCurrentBaseSlots(Guid userId)
        {
            using (UndeadEarthDataContext undeadEarthDataContext = new UndeadEarthDataContext(_connectionString))
            {
                undeadEarthDataContext.ReadUncommited();

                UserDto userDto = undeadEarthDataContext.GetUserById(userId);
                return userDto.PossibleItemAmount;
            }
        }

        #endregion

        #region "IUserSaver Implementation"

        void IUserSaver.UpdateUserLocation(Guid userId, double lattitude, double longitude)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserDto userDto = dataContext.GetUserById(userId);

                HotZoneDto hotZone = dataContext.HotZoneDtos.SingleOrDefault(c => c.Latitude == (decimal)lattitude && c.Longitude == (decimal)longitude);

                if (userDto != null)
                {
                    userDto.Latitude = (decimal)lattitude;
                    userDto.Longitude = (decimal)longitude;
                    userDto.ZoneId = hotZone != null ? hotZone.Id : userDto.ZoneId;
                    dataContext.SubmitChanges();
                }
            }
        }

        void IUserSaver.UpdateZone(Guid userId, Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                dataContext.ReadUncommited();

                UserDto userDto = dataContext.GetUserById(userId);

                if (userDto != null)
                {
                    userDto.ZoneId = hotZoneId;

                    dataContext.SubmitChanges();
                }
            }
        }

        void IUserSaver.SaveLastEnergy(Guid userId, int energy, DateTime time)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.LastEnergy = energy;
                userDto.LastEnergyDate = time;
                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.AddMoney(Guid userId, int amount)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.Money = userDto.Money + amount;
                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.UpdateLastVisitedHotZone(Guid userId, Guid hotZoneId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.LastVisitedHotZoneId = hotZoneId;
                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.UpdateBaseLine(Guid userId, int maxAttackPower, int maxEnergy)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.BaseLineAttackPower = maxAttackPower;
                userDto.BaseLineEnergy = maxEnergy;
                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.SetCurrentBaseSightRadius(Guid userId, int userBaseSightRadius)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.BaseSightRadius = userBaseSightRadius;
                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.SaveLastSightRadius(Guid userId, int sightRadius, DateTime time)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.LastSightRadius = sightRadius;
                userDto.LastSightRadiusDate = time;
                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.SetUserLevel(Guid userId, int level)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                if (userDto == null)
                {
                    throw new InvalidOperationException("User does not exist.");
                }

                userDto.Level = level;

                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.UpdateEnergyForDifficultyCalculation(Guid userId, int newBaseEnergy)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                if (userDto == null)
                {
                    throw new InvalidOperationException("User does not exist.");
                }

                userDto.BaseLineEnergy = newBaseEnergy;

                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.UpdateAttackForDifficultyCalculation(Guid userId, int newBaseAttack)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                if (userDto == null)
                {
                    throw new InvalidOperationException("User does not exist.");
                }

                userDto.BaseLineAttackPower = newBaseAttack;

                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.UpdateUserInventorySlot(Guid userId, int newInventorySlotAmount)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                if (userDto == null)
                {
                    throw new InvalidOperationException("User does not exist.");
                }

                userDto.PossibleItemAmount = newInventorySlotAmount;

                dataContext.SubmitChanges();
            }
        }

        #endregion

        void IUserSaver.UpdateCurrentBaseEnergy(Guid userId, int energy)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.CurrentBaseEnergy = energy;
                dataContext.SubmitChanges();
            }
        }

        void IUserSaver.UpdateCurrentBaseAttack(Guid userId, int attackpower)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);

                userDto.CurrentBaseAttack = attackpower;
                dataContext.SubmitChanges();
            }
        }


        int IUserRetriever.GetCurrentBaseEnergy(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);
                return userDto.CurrentBaseEnergy;
            }
        }

        int IUserRetriever.GetCurrentBaseAttackPower(Guid userId)
        {
            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserById(userId);
                return userDto.CurrentBaseAttack;
            }
        }


        IUser IUserRetriever.GetUserByFacebookId(long facebookUserId)
        {
            if(facebookUserId == 0)
            {
                throw new InvalidOperationException("Facebook UserId cannot be 0.");
            }

            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserByFacebookUserId(facebookUserId);
                return userDto;
            }
        }

        bool IUserRetriever.FacebookUserExists(long facebookUserId)
        {
            if (facebookUserId == 0)
            {
                throw new InvalidOperationException("Facebook UserId cannot be 0.");
            }

            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserByFacebookUserId(facebookUserId);  
                return userDto != null;
            }
        }

        void IUserSaver.InsertUser(Guid userId, long facebookUserId, string name, Guid startingLocation, int baseAttackPower, int baseEnergy)
        {
            if(facebookUserId == 0)
            {
                throw new InvalidOperationException("Facebook UserId cannot be 0.");
            }

            using (UndeadEarthDataContext dataContext = new UndeadEarthDataContext(_connectionString))
            {
                UserDto userDto = dataContext.GetUserByFacebookUserId(facebookUserId);
                if(userDto != null)
                {
                    throw new InvalidOperationException("User already exists.");
                }

                userDto = new UserDto
                {
                    Id = userId,
                    FacebookUserId = facebookUserId,
                    ZoneId = startingLocation,
                    CurrentBaseAttack = baseAttackPower,
                    CurrentBaseEnergy = baseEnergy,
                    LastVisitedHotZoneId = startingLocation,
                    BaseLineAttackPower = baseAttackPower,
                    BaseLineEnergy = baseEnergy,
                    DisplayName = name,
                    Level = 1,
                    Email = string.Empty,
                    LocationId = startingLocation,
                    Money = 0,
                    PossibleItemAmount = 5
                };

                dataContext.UserDtos.InsertOnSubmit(userDto);
                dataContext.SubmitChanges();
            }
        }
    }
}