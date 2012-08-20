using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Linq;

namespace UndeadEarth.Web.Models.Database
{
    public class UndeadEarthDataContext : DataContext
    {
        /// <summary>
        /// Initializes a new instance of the UndeadEarthDataContext class.
        /// </summary>
        public UndeadEarthDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public Table<InfoNodeDto> InfoNodeDtos
        {
            get { return GetTable<InfoNodeDto>(); }
        }

        public Table<HotZoneDto> HotZoneDtos
        {
            get { return GetTable<HotZoneDto>(); }
        }

        public Table<UserDto> UserDtos
        {
            get { return GetTable<UserDto>(); }
        }

        public Table<UserHotZoneProgressDto> UserHotZoneProgressDtos
        {
            get { return GetTable<UserHotZoneProgressDto>(); }
        }
    }
}
