using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using UndeadEarth.Contract;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "UserZombiePackProgress")]
    internal class UserZombiePackProgressDto : IUserZombiePackProgress
    {
        private Guid _id;
        [Column(Storage = "_id", IsPrimaryKey = true, UpdateCheck = UpdateCheck.Never)]
        public Guid Id
        {
            [DebuggerNonUserCode]
            get { return _id; }
            [DebuggerNonUserCode]
            set { _id = value; }
        }

        private Guid _userId;
        [Column(Storage = "_userId", UpdateCheck = UpdateCheck.Never)]
        public Guid UserId
        {
            [DebuggerNonUserCode]
            get { return _userId; }
            [DebuggerNonUserCode]
            set { _userId = value; }
        }

        private Guid _zombiePackId;
        [Column(Storage = "_zombiePackId", UpdateCheck = UpdateCheck.Never)]
        public Guid ZombiePackId
        {
            [DebuggerNonUserCode]
            get { return _zombiePackId; }
            [DebuggerNonUserCode]
            set { _zombiePackId = value; }
        }

        private bool _isDestroyed;
        [Column(Storage = "_isDestroyed", UpdateCheck = UpdateCheck.Never)]
        public bool IsDestroyed
        {
            [DebuggerNonUserCode]
            get { return _isDestroyed; }
            [DebuggerNonUserCode]
            set { _isDestroyed = value; }
        }

        private DateTime? _lastHuntDate;
        [Column(Storage = "_lastHuntDate", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public DateTime? LastHuntDate
        {
            [DebuggerNonUserCode]
            get { return _lastHuntDate; }
            [DebuggerNonUserCode]
            set { _lastHuntDate = value; }
        }

        private int _maxZombies;
        [Column(Storage = "_maxZombies", UpdateCheck = UpdateCheck.Never)]
        public int MaxZombies
        {
            [DebuggerNonUserCode]
            get { return _maxZombies; }
            [DebuggerNonUserCode]
            set { _maxZombies = value; }
        }

        private DateTime _lastRegen;
        [Column(Storage = "_lastRegen", UpdateCheck = UpdateCheck.Never)]
        public DateTime LastRegen
        {
            [DebuggerNonUserCode]
            get { return _lastRegen; }
            [DebuggerNonUserCode]
            set { _lastRegen = value; }
        }

        private int _regenMinuteTicks;
        [Column(Storage = "_regenMinuteTicks", UpdateCheck = UpdateCheck.Never)]
        public int RegenMinuteTicks
        {
            [DebuggerNonUserCode]
            get { return _regenMinuteTicks; }
            [DebuggerNonUserCode]
            set { _regenMinuteTicks = value; }
        }

        private int _regenZombieRate;
        [Column(Storage = "_regenZombieRate", UpdateCheck = UpdateCheck.Never)]
        public int RegenZombieRate
        {
            [DebuggerNonUserCode]
            get { return _regenZombieRate; }
            [DebuggerNonUserCode]
            set { _regenZombieRate = value; }
        }

        private int _zombiesLeft;
        [Column(Storage = "_zombiesLeft", UpdateCheck = UpdateCheck.Never)]
        public int ZombiesLeft
        {
            [DebuggerNonUserCode]
            get { return _zombiesLeft; }
            [DebuggerNonUserCode]
            set { _zombiesLeft = value; }
        }

        bool IUserZombiePackProgress.IsDestroyed
        {
            get { return _isDestroyed; }
        }

        int IUserZombiePackProgress.MaxZombies
        {
            get { return _maxZombies; }
        }

        int IUserZombiePackProgress.ZombiesLeft
        {
            get { return _zombiesLeft; }
        }
    }
}
