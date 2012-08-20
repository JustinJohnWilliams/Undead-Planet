using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;

namespace UndeadEarth.Web.Models.Database
{
    [Table(Name = "UserHotZoneProgress")]
    public class UserHotZoneProgressDto
    {
        private Guid _id;
        [Column(Storage = "_id", IsPrimaryKey = true)]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private Guid _userId;
        [Column(Storage = "_userId")]
        public Guid UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        private Guid _hotZoneId;
        [Column(Storage = "_hotZoneId")]
        public Guid HotZoneId
        {
            get { return _hotZoneId; }
            set { _hotZoneId = value; }
        }

        private bool _isDestroyed;
        [Column(Storage = "_isDestroyed")]
        public bool IsDestroyed
        {
            get { return _isDestroyed; }
            set { _isDestroyed = value; }
        }

        private DateTime? _lastHuntDate;
        [Column(Storage = "_lastHuntDate", CanBeNull = true)]
        public DateTime? LastHuntDate
        {
            get { return _lastHuntDate; }
            set { _lastHuntDate = value; }
        }

        private int _maxZombies;
        [Column(Storage = "_maxZombies")]
        public int MaxZombies
        {
            get { return _maxZombies; }
            set { _maxZombies = value; }
        }

        private DateTime _lastRegen;
        [Column(Storage = "_lastRegen")]
        public DateTime LastRegen
        {
            get { return _lastRegen; }
            set { _lastRegen = value; }
        }

        private int _regenMinuteTicks;
        [Column(Storage = "_regenMinuteTicks")]
        public int RegenMinuteTicks
        {
            get { return _regenMinuteTicks; }
            set { _regenMinuteTicks = value; }
        }

        private int _regenZombieRate;
        [Column(Storage = "_regenZombieRate")]
        public int RegenZombieRate
        {
            get { return _regenZombieRate; }
            set { _regenZombieRate = value; }
        }

        private int _zombiesLeft;
        [Column(Storage = "_zombiesLeft")]
        public int ZombiesLeft
        {
            get { return _zombiesLeft; }
            set { _zombiesLeft = value; }
        }
    }
}
