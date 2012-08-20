using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UndeadEarth.Contract;

namespace UndeadEarth.Controllers.Models
{
    public class FacebookUser : IFacebookUser
    {
        public long id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string picture { get; set; }

        #region IFacebookUser Members

        long IFacebookUser.UserId
        {
            get { return id; }
        }

        string IFacebookUser.Name
        {
            get { return name; }
        }

        string IFacebookUser.Email
        {
            get { return email; }
        }

        #endregion
    }
}
