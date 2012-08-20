using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace UndeadEarth.Controllers
{
    public class BaseController : Controller
    {
        //inherit from Controller and add this override.  All of your controllers that are
        //associated with your facebook app must derive from the controller you created
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.AddHeader("p3p", "CP=\"IDC DSP COR ADM DEVi TAIi PSA PSD IVAi IVDi CONi HIS OUR IND CNT\"");
            base.OnActionExecuting(filterContext);
        }
    }
}
