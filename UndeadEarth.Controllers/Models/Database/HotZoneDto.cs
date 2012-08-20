using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq.Mapping;

namespace UndeadEarth.Web.Models.Database
{
    [Table(Name = "HotZones")]
    public class HotZoneDto
    {
        private Guid _id;
        [Column(Storage = "_id", IsPrimaryKey= true, UpdateCheck = UpdateCheck.Never)]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
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

        private string _name;
        [Column(Storage = "_name", UpdateCheck = UpdateCheck.Never)]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
