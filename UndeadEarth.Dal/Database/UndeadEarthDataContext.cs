using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Diagnostics;
using UndeadEarth.Contract;

namespace UndeadEarth.Dal.Database
{
    public class UndeadEarthDataContext : DataContext
    {
        private static MappingSource _mappingSource = new AttributeMappingSource();

        /// <summary>
        /// Initializes a new instance of the UndeadEarthDataContext class.
        /// </summary>
        public UndeadEarthDataContext(string connectionString)
            : base(connectionString, _mappingSource) { }

        #region Tables/Views

        [DebuggerNonUserCode]
        internal Table<HotZoneDto> HotZoneDtos { get { return GetTable<HotZoneDto>(); } }
        [DebuggerNonUserCode]
        internal Table<ZombiePackDto> ZombiePackDtos { get { return GetTable<ZombiePackDto>(); } }
        [DebuggerNonUserCode]
        internal Table<UserDto> UserDtos { get { return GetTable<UserDto>(); } }
        [DebuggerNonUserCode]
        internal Table<UserZombiePackProgressDto> UserZombiePackProgressDtos { get { return GetTable<UserZombiePackProgressDto>(); } }
        [DebuggerNonUserCode]
        internal Table<StoreDto> StoreDtos { get { return GetTable<StoreDto>(); } }
        [DebuggerNonUserCode]
        internal Table<ItemDto> ItemDtos { get { return GetTable<ItemDto>(); } }
        [DebuggerNonUserCode]
        internal Table<UserItemDto> UserItemDtos { get { return GetTable<UserItemDto>(); } }
        [DebuggerNonUserCode]
        internal Table<SafeHouseDto> SafeHouseDtos { get { return GetTable<SafeHouseDto>(); } }
        [DebuggerNonUserCode]
        internal Table<SafeHouseItemDto> SafeHouseItemDtos { get { return GetTable<SafeHouseItemDto>(); } }
        [DebuggerNonUserCode]
        public Table<UserCountDto> UserCountsDtos { get { return GetTable<UserCountDto>(); } }

        #endregion

        #region Compiled Queries

        private static Func<UndeadEarthDataContext, Guid, UserDto> _getUserByUserIdQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, UserDto>
                ((dataContext, userId) =>
                    dataContext.UserDtos.FirstOrDefault(c => c.Id == userId)
                );

        private static Func<UndeadEarthDataContext, Guid, bool> _doesUserExistQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, bool>
                ((dataContext, userId) =>
                    dataContext.UserDtos.Any(c => c.Id == userId)
                    );

        private static Func<UndeadEarthDataContext, Guid, ItemDto> _getItemByItemIdQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, ItemDto>
                ((dataContex, itemId) =>
                    dataContex.ItemDtos.FirstOrDefault(c => c.Id == itemId)
                    );

        private static Func<UndeadEarthDataContext, Guid, bool> _doesItemExistQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, bool>
                ((dataContext, itemId) =>
                    dataContext.ItemDtos.Any(c => c.Id == itemId)
                    );

        private static Func<UndeadEarthDataContext, Guid, Guid, UserItemDto> _getUserItemByItemIdQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, Guid, UserItemDto>
                ((dataContex, userId, itemId) =>
                    dataContex.UserItemDtos.FirstOrDefault(c => c.UserId == userId && c.ItemId == itemId)
                    );

        private static Func<UndeadEarthDataContext, Guid, Guid, bool> _doesUserItemExistQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, Guid, bool>
                ((dataContext, userId, itemId) =>
                    dataContext.UserItemDtos.Any(c => c.UserId == userId && c.ItemId == itemId)
                    );

        private static Func<UndeadEarthDataContext, Guid, UserCountDto> _getUserCountQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, UserCountDto>
                ((dataContext, userId) =>
                    dataContext.UserCountsDtos.FirstOrDefault(c => c.UserId == userId)
                    );

        private static Func<UndeadEarthDataContext, Guid, bool> _doesUserCountExistQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, bool>
                ((dataContext, userId) =>
                    dataContext.UserCountsDtos.Any(c => c.Id == userId)
                    );

        private static Func<UndeadEarthDataContext, Guid, Guid, UserZombiePackProgressDto> _getUserZombiePackProgressQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, Guid, Guid, UserZombiePackProgressDto>
            ((dataContext, userId, zombiePackId) =>
                dataContext.UserZombiePackProgressDtos.FirstOrDefault(c => c.UserId == userId && c.ZombiePackId == zombiePackId)
                );

        private static Func<UndeadEarthDataContext, long, UserDto> _getUserByFacebookUserIdQuery =
            CompiledQuery.Compile<UndeadEarthDataContext, long, UserDto>
                ((dataContext, userId) =>
                    dataContext.UserDtos.FirstOrDefault(c => c.FacebookUserId == userId)
                );

        #endregion

        #region Internal Methods

        internal UserDto GetUserById(Guid userId)
        {
            return _getUserByUserIdQuery.Invoke(this, userId);
        }

        internal UserDto GetUserByFacebookUserId(long facebookUserId)
        {
            return _getUserByFacebookUserIdQuery.Invoke(this, facebookUserId);
        }

        internal bool UserExists(Guid userId)
        {
            return _doesUserExistQuery.Invoke(this, userId);
        }

        internal ItemDto GetItemById(Guid itemId)
        {
            return _getItemByItemIdQuery.Invoke(this, itemId);
        }

        internal bool ItemExists(Guid itemId)
        {
            return _doesItemExistQuery.Invoke(this, itemId);
        }

        internal UserItemDto GetUserItemByUserIdAndItemId(Guid userId, Guid itemId)
        {
            return _getUserItemByItemIdQuery.Invoke(this, userId, itemId);
        }

        internal bool UserItemExists(Guid userId, Guid itemId)
        {
            return _doesUserItemExistQuery.Invoke(this, userId, itemId);
        }

        internal UserCountDto GetUserCountByUserId(Guid userId)
        {
            return _getUserCountQuery.Invoke(this, userId);
        }

        //internal bool UserCountExists(Guid userId)
        //{
        //    return _doesUserCountExistQuery.Invoke(this, userId);
        //}

        internal UserZombiePackProgressDto GetUserZombiePackProgressByUserIdAndZombiePackId(Guid userId, Guid zombiePackId)
        {
            return _getUserZombiePackProgressQuery(this, userId, zombiePackId);
        }

        [DebuggerNonUserCode]
        internal void ReadUncommited()
        {
            ExecuteCommand("SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED");
        }

        [DebuggerNonUserCode]
        internal DateTime GetDate()
        {
            return ExecuteQuery<DateTime>("select getdate()").First();
        }

        #endregion
    }
}
