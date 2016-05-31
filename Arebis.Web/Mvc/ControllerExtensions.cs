using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Arebis.Web.Mvc
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Disables caching of the current response.
        /// </summary>
        public static void DisablePageCaching(this ControllerBase controller)
        {
            controller.ControllerContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            controller.ControllerContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            controller.ControllerContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            controller.ControllerContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            controller.ControllerContext.HttpContext.Response.Cache.SetNoStore();
        }

        /// <summary>
        /// Clears model state errors from the viewdata.
        /// </summary>
        public static void ClearModelStateErrors(this ControllerBase controller)
        {
            ClearErrors(controller.ViewData.ModelState);
        }

        /// <summary>
        /// Clears the error information keeping the values of the modelstate.
        /// </summary>
        /// <param name="self"></param>
        public static void ClearErrors(this ModelStateDictionary self)
        {
            foreach (ModelState item in self.Values)
            {
                item.Errors.Clear();
            }
        }

        /// <summary>
        /// Clears the values from the modelstate but keeps the error information.
        /// </summary>
        public static void ClearValues(this ModelStateDictionary self)
        {
            foreach (ModelState item in self.Values)
            {
                item.Value = null;
            }
        }

        public static ActionResult Ok(this ControllerBase controller, string message = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK, message);
        }

        public static ActionResult Forbidden(this ControllerBase controller, string message = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden, message);
        }

        public static ActionResult Unauthorized(this ControllerBase controller, string message = null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, message);
        }
    }
}
