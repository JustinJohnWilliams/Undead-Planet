using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UndeadEarth.Web.Models
{
    public static class ResponseHelper
    {
        public static void ThrowException(this HttpContextBase context, Exception e)
        {
            context.Response.StatusCode = 502;
            context.Response.StatusDescription = e.Message;
        }
    }
}
