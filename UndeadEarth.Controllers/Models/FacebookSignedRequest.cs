using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UndeadEarth.Controllers.Models
{
    public class FacebookSignedRequest
    {
        public string algorithm { get; set; }
        public string expires { get; set; }
        public string issued_at { get; set; }
        public string oauth_token { get; set; }
        public string user_id { get; set; }
    }
}
