using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Model.Proxy
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public bool IsOneTimeUse { get; set; }
    }
}
