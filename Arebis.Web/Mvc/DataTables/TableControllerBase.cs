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