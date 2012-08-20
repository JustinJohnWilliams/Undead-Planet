using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UndeadEarth.Web.Models
{
    public class Node
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
