using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "dbo.SafeHouses")]
    internal class SafeHouseDto : ISafeHouse
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

        private Guid? _hotZoneId;
        [Column(Storage = "_hotZoneId", UpdateCheck = UpdateCheck.Never, CanBeNull = true)]
        public Guid? HotZoneId
        {
            [DebuggerNonUserCode]
            get { return _hotZoneId; }
            [DebuggerNonUserCode]
            set { _hotZoneId = value; }
        }

        Guid ISafeHouse.Id
        {
            get { return _id; }
        }

        double ISafeHouse.Latitude
        {
            get { return Convert.ToDouble(Math.Round(_latitude, 4)); }
        }

        double ISafeHouse.Longitude
        {
            get { return Convert.ToDouble(Math.Round(_longitude, 4)); }
        }
    }
}
