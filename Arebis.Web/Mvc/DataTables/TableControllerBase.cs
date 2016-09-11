using Arebis.Web.Mvc;
using System;
using System.Net;
using System.Web.Mvc;

namespace Arebis.Web.Mvc.DataTables
{
    public abstract class TableControllerBase<TModel> : Controller
    {
        [HttpGet]
        [NoCache]
        public abstract ActionResult Edit(int id);

        [HttpPost]
        public abstract ActionResult Edit(TModel model);

        /// <summary>
        /// If the ModelState is valid, executes the action and returns status 202 Accepted.
        /// If the model state is not valid, or if action fails, returns the default view on the model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="action">Action to take when ModelState is valid.</param>
        /// <returns></returns>
        [NonAction]
        protected ActionResult ModelAction(TModel model, Action action)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    action();
                    return new HttpStatusCodeResult(HttpStatusCode.Accepted); // 202
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex);
            }

            return View(model);
        }
    }
}