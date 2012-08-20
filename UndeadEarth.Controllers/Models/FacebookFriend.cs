using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Controllers.Models
{

    public class FacebookFriendContainer
    {
        public List<FacebookFriend> data { get; set; }
    }

    public class FacebookFriend : IFacebookFriend
    {
        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private long _id;
        public long id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        long IFacebookFriend.Id
        {
            get { return _id; }
        }

        string IFacebookFriend.Name
        {
            get { return _name; }
        }
    }
}
