using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Controllers.ViewModels
{
    public class HomePageViewModel
    {
        public bool IsRegistered { get; set; }
        public List<UserStats> UserRank { get; set; }

    }
}
