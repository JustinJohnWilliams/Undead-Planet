using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using UndeadEarth.Contract;
using System.Diagnostics;
using System.ComponentModel;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "Users")]
    internal class UserDto : IUser
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

        private string _email;
        [Column(Storage = "_email", UpdateCheck = UpdateCheck.Never)]
        public string Email
        {
            [DebuggerNonUserCode]
            get { return _email; }
            [DebuggerNonUserCode]
            set { _email = value; }
        }

        private string _displayName;
        [Column(Storage = "_displayName", UpdateCheck = UpdateCheck.Never)]
        public string DisplayName
        {
            [DebuggerNonUserCode]
            get { return _displayName; }
            [DebuggerNonUserCode]
            set { _displayName = value; }
        }

        private Guid _zoneId;
        [Column(Storage = "_zoneId", UpdateCheck = UpdateCheck.Never)]
        public Guid ZoneId
        {
            [DebuggerNonUserCode]
            get { return _zoneId; }
            [DebuggerNonUserCode]
            set { _zoneId = value; }
        }

        private Guid _locationId;
        [Column(Storage = "_locationId", UpdateCheck = UpdateCheck.Never)]
        public Guid LocationId
        {
            [DebuggerNonUserCode]
            get { return _locationId; }
            [DebuggerNonUserCode]
            set { _locationId = value; }
        }

        private decimal _latitude;
        [Column(Storage = "_latitude", UpdateCheck = UpdateCheck.Never)]
        public decimal Latitude
        {
            [DebuggerNonUserCode]
            get { return _latitude; }
            [DebuggerNonUserCode]
            set { _latitude = value; }
        }

        private decimal _longitude;
        [Column(Storage = "_longitude", UpdateCheck = UpdateCheck.Never)]
        public decimal Longitude
        {
            [DebuggerNonUserCode]
            get { return _longitude; }
            [DebuggerNonUserCode]
            set { _longitude = value; }
        }

        private int? _lastEnergy;
        [Column(Storage = "_lastEnergy", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public int? LastEnergy
        {
            [DebuggerNonUserCode]
            get { return _lastEnergy; }
            [DebuggerNonUserCode]
            set { _lastEnergy = value; }
        }

        private DateTime? _lastEnergyDate;
        [Column(Storage = "_lastEnergyDate", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public DateTime? LastEnergyDate
        {
            [DebuggerNonUserCode]
            get { return _lastEnergyDate; }
            [DebuggerNonUserCode]
            set { _lastEnergyDate = value; }
        }

        private int? _lastSightRadius;
        [Column(Storage = "_lastSightRadius", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public int? LastSightRadius
        {
            [DebuggerNonUserCode]
            get { return _lastSightRadius; }
            [DebuggerNonUserCode]
            set { _lastSightRadius = value; }
        }

        private DateTime? _lastSightRadiusDate;
        [Column(Storage = "_lastSightRadiusDate", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public DateTime? LastSightRadiusDate
        {
            [DebuggerNonUserCode]
            get { return _lastSightRadiusDate; }
            [DebuggerNonUserCode]
            set { _lastSightRadiusDate = value; }
        }

        private int? _baseSightRadius;
        [Column(Storage = "_baseSightRadius", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public int? BaseSightRadius
        {
            [DebuggerNonUserCode]
            get { return _baseSightRadius; }
            [DebuggerNonUserCode]
            set { _baseSightRadius = value; }
        }

        private int _money;
        [Column(Storage = "_money", UpdateCheck = UpdateCheck.Never)]
        public int Money
        {
            [DebuggerNonUserCode]
            get { return _money; }
            [DebuggerNonUserCode]
            set { _money = value; }
        }

        private Guid? _lastVisitedHotZoneId;
        [Column(Storage = "_lastVisitedHotZoneId", UpdateCheck = UpdateCheck.Never, CanBeNull = true)]
        public Guid? LastVisitedHotZoneId
        {
            [DebuggerNonUserCode]
            get { return _lastVisitedHotZoneId; }
            [DebuggerNonUserCode]
            set { _lastVisitedHotZoneId = value; }
        }

        private int _baseLineAttackPower;
        [Column(Storage = "_baseLineAttackPower", UpdateCheck = UpdateCheck.Never)]
        public int BaseLineAttackPower
        {
            [DebuggerNonUserCode]
            get { return _baseLineAttackPower; }
            [DebuggerNonUserCode]
            set { _baseLineAttackPower = value; }
        }

        private int _baseLineEnergy;
        [Column(Storage = "_baseLineEnergy", UpdateCheck = UpdateCheck.Never)]
        public int BaseLineEnergy
        {
            [DebuggerNonUserCode]
            get { return _baseLineEnergy; }
            [DebuggerNonUserCode]
            set { _baseLineEnergy = value; }
        }

        private int _level;
        [Column(Storage = "_level", UpdateCheck = UpdateCheck.Never, CanBeNull = false)]
        public int Level
        {
            [DebuggerNonUserCode]
            get { return _level; }
            [DebuggerNonUserCode]
            set { _level = value; }
        }

        private int _possibleItemAmount;
        [Column(Storage = "_possibleItemAmount", UpdateCheck = UpdateCheck.Never)]
        public int PossibleItemAmount
        {
            [DebuggerNonUserCode]
            get { return _possibleItemAmount; }
            [DebuggerNonUserCode]
            set { _possibleItemAmount = value; }
        }

        private int _currentBaseEnergy;
        [Column(Storage = "_currentBaseEnergy", UpdateCheck = UpdateCheck.Never, CanBeNull = false)]
        public int CurrentBaseEnergy
        {
            [DebuggerNonUserCode]
            get { return _currentBaseEnergy; }
            [DebuggerNonUserCode]
            set { _currentBaseEnergy = value; }
        }

        private int _currentBaseAttack;
        [Column(Storage = "_currentBaseAttack", UpdateCheck = UpdateCheck.Never, CanBeNull = false)]
        public int CurrentBaseAttack
        {
            [DebuggerNonUserCode]
            get { return _currentBaseAttack; }
            [DebuggerNonUserCode]
            set { _currentBaseAttack = value; }
        }

        private long _facebookUserId;
        [Column(Storage = "_facebookUserId", UpdateCheck = UpdateCheck.Never, CanBeNull = false)]
        public long FacebookUserId
        {
            [DebuggerNonUserCode]
            get { return _facebookUserId; }
            [DebuggerNonUserCode]
            set { _facebookUserId = value; }
        }

        Guid IUser.Id
        {
            get { return _id; }
        }

        string IUser.Email
        {
            get { return _email; }
        }

        string IUser.DisplayName
        {
            get { return _displayName; }
        }

        Guid IUser.ZoneId
        {
            get { return _zoneId; }
        }

        Guid IUser.LocationId
        {
            get { return _locationId; }
        }

        double IUser.Latitude
        {
            get { return Convert.ToDouble(Math.Round(_latitude, 4)); }
        }

        double IUser.Longitude
        {
            get { return Convert.ToDouble(Math.Round(_longitude, 4)); }
        }

        int IUser.Money
        {
            get { return _money; }
        }

        int IUser.PossibleItemAmount
        {
            get { return _possibleItemAmount; }
        }
    }
}
