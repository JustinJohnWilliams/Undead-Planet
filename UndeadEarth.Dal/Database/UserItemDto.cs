using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "dbo.UserItems")]
    internal class UserItemDto
    {
        private Guid _userItemId;
        [Column(Storage = "_userItemId", UpdateCheck = UpdateCheck.Never, IsPrimaryKey = true)]
        public Guid UserItemId
        {
            [DebuggerNonUserCode]
            get { return _userItemId; }
            [DebuggerNonUserCode]
            set { _userItemId = value; }
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

        private Guid _itemId;
        [Column(Storage = "_itemId", UpdateCheck = UpdateCheck.Never)]
        public Guid ItemId
        {
            [DebuggerNonUserCode]
            get { return _itemId; }
            [DebuggerNonUserCode]
            set { _itemId = value; }
        }
    }
}