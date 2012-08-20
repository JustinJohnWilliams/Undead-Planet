using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using UndeadEarth.Model.Proxy;
using System.ComponentModel;

namespace UndeadEarth.Silverlight.Model
{
    public class GameContext
    {
        public GameContext()
        {
            UserInventory = new List<Item>();
        }

        /// <summary>
        /// User Id for the current user that is playing the game.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Base Uri for all MVC controller communication.
        /// </summary>
        public string BaseUri { get; set; }

        /// <summary>
        /// Represents the current user's user node.
        /// </summary>
        public UserNode UserNode { get; set; }

        /// <summary>
        /// Gets or sets user attack power
        /// </summary>
        public int UserAttackPower
        {
            get;
            private set;
        }

        /// <summary>
        /// Represents the eyesight radius for user.
        /// </summary>
        public int SightRadius
        {
            get;
            private set;
        }

        /// <summary>
        /// HotZoneId user is currently in.
        /// </summary>
        public Guid HotZoneId
        {
            get;
            private set;
        }

        /// <summary>
        /// Represents the user's energy;
        /// </summary>
        public EnergyResult UserEnergy
        {
            get;
            private set;
        }

        /// <summary>
        /// Represents the user's level
        /// </summary>
        public LevelResult UserLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// Represents the users current money
        /// </summary>
        public int Money
        {
            get;
            private set;
        }

        public int ZoomLevel
        {
            get;
            private set;
        }

        public List<Item> UserInventory { get; private set; }

        /// <summary>
        /// Sets the users energy in the game context.  Notifcation that the energy was updated will be sent out from the shared game context.
        /// </summary>
        /// <param name="energyResult"></param>
        public void SetUserEnergy(EnergyResult energyResult)
        {
            UserEnergy = energyResult;
            OnUserEnergyChanged(UserEnergy);
        }

        /// <summary>
        /// Sets the users money in the game context. Notificatino that the money was updated will be sent out form the shared game context.
        /// </summary>
        /// <param name="money"></param>
        public void SetMoney(int money)
        {
            Money = money;
            OnUserMoneyChanged(money);
        }

        /// <summary>
        /// Sets the users level in the game context. Notification that the level was updated will be sent out from the shared game context.
        /// </summary>
        /// <param name="levelResult"></param>
        public void SetUserLevel(LevelResult levelResult)
        {
            UserLevel = levelResult;
            OnUserLevelChanged(UserLevel);
        }

        /// <summary>
        /// Sets the user node.  Notification that the user node was changed will be sent out from the shared game context.
        /// </summary>
        public void NotifyMoveComplete()
        {
            OnUserMoved();
        }

        public event EventHandler UserMoved;
        private void OnUserMoved()
        {
            if (UserMoved != null)
            {
                UserMoved(this, EventArgs.Empty);
            }
        }

        public event EventHandler<UserEnergyChangedEventArgs> UserEnergyChanged;
        private void OnUserEnergyChanged(EnergyResult energyResult)
        {
            if (UserEnergyChanged != null)
            {
                UserEnergyChanged(this, new UserEnergyChangedEventArgs { Energy = energyResult });
            }
        }

        public event EventHandler<UserLevelChangedEventArgs> UserLevelChanged;
        private void OnUserLevelChanged(LevelResult levelResult)
        {
            if (UserLevelChanged != null)
            {
                UserLevelChanged(this, new UserLevelChangedEventArgs { Level = levelResult });
            }
        }

        public event EventHandler<UserMoneyChangedEventArgs> UserMoneyChanged;
        private void OnUserMoneyChanged(int money)
        {
            if (UserMoneyChanged != null)
            {
                UserMoneyChanged(this, new UserMoneyChangedEventArgs { Money = money });
            }
        }

        public event EventHandler<ZombiePackDestroyedEventArgs> ZombiePackDestroyed;
        public void NotifyZombiePackDestroyed(Guid zombiePackId)
        {
            if (ZombiePackDestroyed != null)
            {
                ZombiePackDestroyed(this, new ZombiePackDestroyedEventArgs { ZombePackId = zombiePackId });
            }
        }

        internal void SetZoomLevel(int zoomLevel)
        {
            ZoomLevel = zoomLevel;
            OnZoomLevelChanged(zoomLevel);
        }

        public event EventHandler<ZoomLevelChangedEventArgs> ZoomLevelChanged;
        private void OnZoomLevelChanged(int zoomLevel)
        {
            if (ZoomLevelChanged != null)
            {
                ZoomLevelChanged(this, new ZoomLevelChangedEventArgs { ZoomLevel = zoomLevel });
            }
        }

        public event EventHandler UserInitalized;
        public void OnUserInitialized()
        {
            if (UserInitalized != null)
            {
                UserInitalized(this, EventArgs.Empty);
            }
        }

        public event EventHandler<UserInventoryChangedEventArgs> UserInventoryChanged;
        private void OnUserInventoryChanged(List<Item> items)
        {
            if (UserInventoryChanged != null)
            {
                UserInventoryChanged(this, new UserInventoryChangedEventArgs { Items = items });
            }
        }

        public void SetUserInventory(List<Item> inventory)
        {
            UserInventory.Clear();
            UserInventory.AddRange(inventory);
            OnUserInventoryChanged(inventory);
        }

        public event EventHandler UserAttackPowerChanged;
        private void OnUserAttackPowerChanged()
        {
            if (UserAttackPowerChanged != null)
            {
                UserAttackPowerChanged(this, EventArgs.Empty);
            }
        }
        public void SetUserAttackPower(int attackPower)
        {
            if (UserAttackPower != attackPower)
            {
                UserAttackPower = attackPower;
                OnUserAttackPowerChanged();
            }
        }

        public void SetUserSightRadius(int sightRadius)
        {
            if (SightRadius != sightRadius)
            {
                SightRadius = sightRadius;
                OnUserSightRadiusChanged();
            }
        }

        public event EventHandler UserSightRadiusChanged;
        private void OnUserSightRadiusChanged()
        {
            if (UserSightRadiusChanged != null)
            {
                UserSightRadiusChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler HotZoneIdChanged;

        private int _userMaxItems;
        public int UserMaxItems
        {
            get { return _userMaxItems; }
        }

        private void OnHotZoneIdChanged()
        {
            if (HotZoneIdChanged != null)
            {
                HotZoneIdChanged(this, EventArgs.Empty);
            }
        }

        public void SetHotZoneId(Guid hotZoneId)
        {
            if (HotZoneId != hotZoneId)
            {
                HotZoneId = hotZoneId;
                OnHotZoneIdChanged();
            }
        }

        public void SetUserMaxItems(int maxItems)
        {
            _userMaxItems = maxItems;
        }
    }
}
