using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserRetriever
    {
        /// <summary>
        /// Retrieves user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IUser GetUserById(Guid userId);
        
        /// <summary>
        /// Retrieves the user's current energy.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Tuple<int, DateTime> GetLastSavedEnergy(Guid userId);

        /// <summary>
        /// Returns true if user exists.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool UserExists(Guid userId);

        /// <summary>
        /// Returns the users base line energy for determining zombie pack difficulty.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetEnergyForDifficultyCalculation(Guid userId);

        /// <summary>
        /// Returns the users current money.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetCurrentMoney(Guid userId);

        /// <summary>
        /// Gets the last visited hot zone for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Guid? GetLastVisitedHotZone(Guid userId);

        /// <summary>
        /// Gets the baseline attack power for a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetAttackPowerForDifficultyCalculation(Guid userId);

        /// <summary>
        /// Returns the last saved sight radius and the time it was saved.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Tuple<int, DateTime> GetLastSavedSightRadius(Guid userId);

        /// <summary>
        /// Returns the current base sight radius.
        /// </summary>
        int? GetCurrentBaseSightRadius(Guid userId);

        /// <summary>
        /// Returns the current level for the user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetCurrentLevel(Guid userId);

        /// <summary>
        /// Returns the user's current slots
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetCurrentBaseSlots(Guid userId);

        /// <summary>
        /// Returns the user's current base energy.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetCurrentBaseEnergy(Guid userId);

        /// <summary>
        /// Returns the user's current base attack power.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        int GetCurrentBaseAttackPower(Guid userId);

        /// <summary>
        /// Returns user based on facebook user id.
        /// </summary>
        /// <param name="FacebookUserId"></param>
        /// <returns></returns>
        IUser GetUserByFacebookId(long facebookUserId);

        /// <summary>
        /// Returns true if user exists given facebook user id.
        /// </summary>
        /// <param name="facebookUserId"></param>
        /// <returns></returns>
        bool FacebookUserExists(long facebookUserId);
    }
}
