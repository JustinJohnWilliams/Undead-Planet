using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using UndeadEarth.Contract;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "dbo.ZombiePacks")]
    internal class ZombiePackDto : IZombiePack
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

        private string _name;
        [Column(Storage = "_name", UpdateCheck = UpdateCheck.Never)]
        public string Name
        {
            [DebuggerNonUserCode]
            get { return _name; }
            [DebuggerNonUserCode]
            set { _name = value; }
        }

        private Guid _hotZoneId;
        [Column(Storage = "_hotZoneId", UpdateCheck = UpdateCheck.Never)]
        public Guid HotZoneId
        {
            [DebuggerNonUserCode]
            get { return _hotZoneId; }
            [DebuggerNonUserCode]
            set { _hotZoneId = value; }
        }

        Guid IZombiePack.Id
        {
            get { return _id; }
        }

        double IZombiePack.Latitude
        {
            get { return Convert.ToDouble(Math.Round(_latitude, 4)); }
        }

        double IZombiePack.Longitude
        {
            get { return Convert.ToDouble(Math.Round(_longitude, 4)); }
        }

        string IZombiePack.Name
        {
            get { return _name; }
        }

        Guid IZombiePack.HotZoneId
        {
            get { return _hotZoneId; }
        }
    }
}
