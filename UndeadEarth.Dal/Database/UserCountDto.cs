using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name="UserCounts")]
    public class UserCountDto
    {
        private Guid _id;
        [Column(Storage="_id", AutoSync = AutoSync.Never, UpdateCheck = UpdateCheck.Never, IsPrimaryKey = true)]
        public Guid Id
        {
            [DebuggerNonUserCode]
            get { return _id; }
            [DebuggerNonUserCode]
            set { _id = value; }
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

        private long _zombiesKilled;
        [Column(Storage = "_zombiesKilled", UpdateCheck = UpdateCheck.Never)]
        public long ZombiesKilled
        {
            [DebuggerNonUserCode]
            get { return _zombiesKilled; }
            [DebuggerNonUserCode]
            set { _zombiesKilled = value; }
        }

        private long _miles;
        [Column(Storage = "_miles", UpdateCheck = UpdateCheck.Never)]
        public long Miles
        {
            [DebuggerNonUserCode]
            get { return _miles; }
            [DebuggerNonUserCode]
            set { _miles = value; }
        }

        private long _hotZonesDestroyed;
        [Column(Storage = "_hotZonesDestroyed", UpdateCheck = UpdateCheck.Never)]
        public long HotZonesDestroyed
        {
            [DebuggerNonUserCode]
            get { return _hotZonesDestroyed; }
            [DebuggerNonUserCode]
            set { _hotZonesDestroyed = value; }
        }

        private long _peakAttack;
        [Column(Storage = "_peakAttack", UpdateCheck = UpdateCheck.Never)]
        public long PeakAttack
        {
            [DebuggerNonUserCode]
            get { return _peakAttack; }
            [DebuggerNonUserCode]
            set { _peakAttack = value; }
        }

        private long _zombiePacksDestroyed;
        [Column(Storage = "_zombiePacksDestroyed", UpdateCheck = UpdateCheck.Never)]
        public long ZombiePacksDestroyed
        {
            [DebuggerNonUserCode]
            get { return _zombiePacksDestroyed; }
            [DebuggerNonUserCode]
            set { _zombiePacksDestroyed = value; }
        }

        private long _accumulatedMoney;
        [Column(Storage = "_accumulatedMoney", UpdateCheck = UpdateCheck.Never, CanBeNull = false)]
        public long AccumulatedMoney
        {
            [DebuggerNonUserCode]
            get { return _accumulatedMoney; }
            [DebuggerNonUserCode]
            set { _accumulatedMoney = value; }
        }
    }
}
