using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Contract
{
    public interface IUserSaver
    {
        /// <summary>
        /// Updates users location
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="lattitude"></param>
        /// <param name="longitude"></param>
        void UpdateUserLocation(Guid userId, double lattitude, double longitude);

        /// <summary>
        /// Updates users hot zone id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hotZoneId"></param>
        void UpdateZone(Guid userId, Guid hotZoneId);

        /// <summary>
        /// Logs the users last energy reading.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="p"></param>
        /// <param name="time"></param>
        void SaveLastEnergy(Guid userId, int energy, DateTime time);

        /// <summary>
        /// Adds the number passed in to the users current money amount.
        /// </summary>
        /// <param name="moneyForHunt"></param>
        void AddMoney(Guid userId, int amount);

        /// <summary>
        /// Updates the last visited hotzone for a user
        /// </summary>
        /// <param name="hotzoneAssociatedWithZombiePack"></param>
        void UpdateLastVisitedHotZone(Guid userId, Guid hotzoneId);

        /// <summary>
        /// Updates the baseline attack power and energy for user.
        /// </summary>
        /// <param name="maxAttackPower"></param>
        /// <param name="maxEnergy"></param>
        void UpdateBaseLine(Guid userId, int maxAttackPower, int maxEnergy);

        /// <summary>
        /// Sets the users current sight radius.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sightRadius"></param>
        void SetCurrentBaseSightRadius(Guid userId, int sightRadius);

        /// <summary>
        /// Saves the sight radius for a user for a given time.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sightRadius"></param>
        /// <param name="time"></param>
        void SaveLastSightRadius(Guid userId, int sightRadius, DateTime time);

        /// <summary>
        /// Sets the user level
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="level"></param>
        void SetUserLevel(Guid userId, int level);

        /// <summary>
        /// Updates users base energy level.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newBaseEnergy"></param>
        void UpdateEnergyForDifficultyCalculation(Guid userId, int newBaseEnergy);

        /// <summary>
        /// Updates users base attack power.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newBaseAttack"></param>
        void UpdateAttackForDifficultyCalculation(Guid userId, int newBaseAttack);

        /// <summary>
        /// Updates users amount for inventory slots
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="newInventorySlotAmount"></param>
        void UpdateUserInventorySlot(Guid userId, int newInventorySlotAmount);

        /// <summary>
        /// Updates users base energy
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updatedUserEnergy"></param>
        void UpdateCurrentBaseEnergy(Guid userId, int updatedUserEnergy);

        /// <summary>
        /// Updates user base attack power
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="updatedUserAttackPower"></param>
        void UpdateCurrentBaseAttack(Guid userId, int updatedUserAttackPower);

        /// <summary>
        /// Inserts a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="facebookUserId"></param>
        /// <param name="startingLocation"></param>
        /// <param name="baseAttackPower"></param>
        /// <param name="baseEnergy"></param>
        void InsertUser(Guid userId, long facebookUserId, string name, Guid startingLocation, int baseAttackPower, int baseEnergy);
    }
}
