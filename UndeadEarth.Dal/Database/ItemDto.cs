using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;
using System.Data.Linq.Mapping;
using System.Diagnostics;

namespace UndeadEarth.Dal.Database
{
    [Table(Name = "dbo.Items")]
    internal class ItemDto : IItem
    {
        private Guid _id;
        [Column(Storage = "_id", UpdateCheck = UpdateCheck.Never, IsPrimaryKey = true)]
        public Guid Id
        {
            [DebuggerNonUserCode]
            get { return _id; }
        }

        private string _name;
        [Column(Storage = "_name", UpdateCheck = UpdateCheck.Never)]
        public string Name
        {
            [DebuggerNonUserCode]
            get { return _name; }
        }

        private string _description;
        [Column(Storage = "_description", UpdateCheck = UpdateCheck.Never)]
        public string Description
        {
            [DebuggerNonUserCode]
            get { return _description; }
        }

        private int _price;
        [Column(Storage = "_price", UpdateCheck = UpdateCheck.Never)]
        public int Price
        {
            [DebuggerNonUserCode]
            get { return _price; }
        }

        private int? _energy;
        [Column(Storage = "_energy", UpdateCheck = UpdateCheck.Never, CanBeNull = true)]
        public int? Energy
        {
            [DebuggerNonUserCode]
            get { return _energy; }
        }

        private int? _distance;
        [Column(Storage = "_distance", UpdateCheck = UpdateCheck.Never, CanBeNull = true)]
        public int? Distance
        {
            [DebuggerNonUserCode]
            get { return _distance; }
        }

        private int? _attack;
        [Column(Storage = "_attack", UpdateCheck = UpdateCheck.Never, CanBeNull = true)]
        public int? Attack
        {
            [DebuggerNonUserCode]
            get { return _attack; }
        }

        private bool _isOneTimeUse;
        [Column(Storage = "_isOneTimeUse", UpdateCheck = UpdateCheck.Never)]
        public bool IsOneTimeUse
        {
            [DebuggerNonUserCode]
            get { return _isOneTimeUse; }
        }

        Guid IItem.Id
        {
            get { return _id; }
        }

        string IItem.Name
        {
            get { return _name; }
        }

        string IItem.Description
        {
            get { return _description; }
        }

        int IItem.Price
        {
            get { return _price; }
        }

        int IItem.Energy
        {
            get { return _energy ?? 0; }
        }

        int IItem.Distance
        {
            get { return _distance ?? 0; }
        }

        int IItem.Attack
        {
            get { return _attack ?? 0; }
        }

        bool IItem.IsOneTimeUse
        {
            get { return _isOneTimeUse; }
        }
    }
}
