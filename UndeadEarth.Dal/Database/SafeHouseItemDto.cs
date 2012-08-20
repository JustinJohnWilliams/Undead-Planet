using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "dbo.SafeHouseItems")]
    internal class SafeHouseItemDto
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

        private Guid _safeHouseId;
        [Column(Storage = "_safeHouseId", UpdateCheck = UpdateCheck.Never)]
        public Guid SafeHouseId
        {
            [DebuggerNonUserCode]
            get { return _safeHouseId; }
            [DebuggerNonUserCode]
            set { _safeHouseId = value; }
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
