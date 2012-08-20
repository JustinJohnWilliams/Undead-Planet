using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "dbo.Stores")]
    internal class StoreDto : IStore
    {
        private Guid _id;
        [Column(Storage = "_id", UpdateCheck = UpdateCheck.Never, IsPrimaryKey = true)]
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

        private Guid? _hotZoneId;
        [Column(Storage = "_hotZoneId", UpdateCheck = UpdateCheck.Never, CanBeNull = true)]
        public Guid? HotZoneId
        {
            [DebuggerNonUserCode]
            get { return _hotZoneId; }
            [DebuggerNonUserCode]
            set { _hotZoneId = value; }
        }

        Guid IStore.Id
        {
            get { return _id; }
        }

        double IStore.Latitude
        {
            get { return Convert.ToDouble(Math.Round(_latitude, 4)); }
        }

        double IStore.Longitude
        {
            get { return Convert.ToDouble(Math.Round(_longitude, 4)); }
        }

        string IStore.Name
        {
            get { return _name; }
        }
    }
}
