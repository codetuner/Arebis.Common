using System;
using System.Web.Mvc;
using System.Web;

namespace Arebis.Web.Mvc
{
    /// <summary>
    /// Marks an action as to be not cached.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class NoCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.Controller.DisablePageCaching();
        }
    }
}
