using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;

namespace UndeadEarth.Web.Models.Database
{
    [Table(Name = "Users")]
    public class UserDto
    {
        private Guid _id;
        [Column(Storage = "_id", IsPrimaryKey = true, UpdateCheck = UpdateCheck.Never)]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _email;
        [Column(Storage = "_email", UpdateCheck = UpdateCheck.Never)]
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _displayName;
        [Column(Storage = "_displayName", UpdateCheck = UpdateCheck.Never)]
        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        private Guid _zoneId;
        [Column(Storage = "_zoneId", UpdateCheck = UpdateCheck.Never)]
        public Guid ZoneId
        {
            get { return _zoneId; }
            set { _zoneId = value; }
        }

        private Guid _locationId;
        [Column(Storage = "_locationId", UpdateCheck = UpdateCheck.Never)]
        public Guid LocationId
        {
            get { return _locationId; }
            set { _locationId = value; }
        }

        private decimal _latitude;
        [Column(Storage = "_latitude", UpdateCheck = UpdateCheck.Never)]
        public decimal Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        private decimal _longitude;
        [Column(Storage = "_longitude", UpdateCheck = UpdateCheck.Never)]
        public decimal Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }

        private decimal? _tempLatitude;
        [Column(Storage = "_tempLatitude", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public decimal? TempLatitude
        {
            get { return _tempLatitude; }
            set { _tempLatitude = value; }
        }

        private decimal? _tempLongitude;
        [Column(Storage = "_tempLongitude", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public decimal? TempLongitude
        {
            get { return _tempLongitude; }
            set { _tempLongitude = value; }
        }

        private Guid? _nextLocationId;
        [Column(Storage = "_nextLocationId", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public Guid? NextLocationId
        {
            get { return _nextLocationId; }
            set { _nextLocationId = value; }
        }

        private decimal? _nextLatitude;
        [Column(Storage = "_nextLatitude", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public decimal? NextLatitude
        {
            get { return _nextLatitude; }
            set { _nextLatitude = value; }
        }

        private decimal? _nextLongitude;
        [Column(Storage = "_nextLongitude", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public decimal? NextLongitude
        {
            get { return _nextLongitude; }
            set { _nextLongitude = value; }
        }

        private DateTime? _moveStartTime;
        [Column(Storage = "_moveStartTime", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public DateTime? MoveStartTime
        {
            get { return _moveStartTime; }
            set { _moveStartTime = value; }
        }

        private DateTime? _moveEndTime;
        [Column(Storage = "_moveEndTime", CanBeNull = true, UpdateCheck = UpdateCheck.Never)]
        public DateTime? MoveEndTime
        {
            get { return _moveEndTime; }
            set { _moveEndTime = value; }
        }
    }
}
